using LouveSystems.CommandLineInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace iw4_zone_asset_finder.Commands
{
    public class Update : LouveSystems.CommandLineInterface.Command
    {
        public override string HelpMessage => "Updates the depencency map by loading all map zones and checking their assets. WARNING: SLOW";

        public override string HelpfulArguments => string.Empty;

        readonly string[] GAME_EXECUTABLES = new string[] { "iw4x.exe", "iw4_direct.exe" };

        readonly string ZONE_FOLDER = "zone";

        public override bool Execute(CommandLineInterface cli, string arguments, out string remainder)
        {
            remainder = arguments;

            if (!File.Exists(GAME_EXECUTABLES[0]) && !File.Exists(GAME_EXECUTABLES[1])){
                cli.Err($"Could not find a game executable ({string.Join(" or ", GAME_EXECUTABLES)})");
                return false;
            }

            if (!Directory.Exists(ZONE_FOLDER))
            {
                cli.Err($"Could not find the zone folder ({ZONE_FOLDER}), are you running this program from the game folder?");
                return false;
            }

            DateTime startTime = DateTime.Now;

            DependencyGraph.Current = new DependencyGraph();
            List<string> files = new List<string>();

            string[] validPrefixes = new string[] { "so", "mp" };

            foreach(var prefix in validPrefixes)
            {
                files.AddRange(Directory.EnumerateFiles("zone", $"{prefix}_*.ff", SearchOption.AllDirectories));
            }

            files = files.OrderByDescending(o => Path.GetDirectoryName(o)).Where(o => o.Count(c => c == '\\') == 2).ToList();


            List<string> zonesDone = new List<string>();

            foreach(var file in files)
            {
                var nameAlone = Path.GetFileNameWithoutExtension(file);

                if (zonesDone.Contains(nameAlone))
                {
                    continue;
                }

                zonesDone.Add(nameAlone);
                //var zonePath = Path.Combine(Path.GetFileNameWithoutExtension(Path.GetDirectoryName(file)), nameAlone);

                if (nameAlone.EndsWith("_load"))
                {
                    continue;
                }

                cli.Write($"Working on {nameAlone}...");
                string output = string.Empty;
                int result = -1;

                foreach (var exe in GAME_EXECUTABLES)
                {
                    if (File.Exists(exe))
                    {
                        output = Helper.GetProgramOutput(exe, $"-nosteam -zonebuilder -stdout +verifyzone {nameAlone} +quit", out result);
                        break;
                    }
                }

                var reg = new Regex(" ([0-9]{1,4}: )([A-z]{1,15}): (.*)");
                var matches = reg.Matches(output);

                foreach(Match match in matches)
                {
                    // Group #1 is nothing
                    // Group #2 is asset type
                    // Group #3 is asset name

                    if (match.Groups.Count == 4)
                    {
                        string type = match.Groups[2].Value;
                        string name = match.Groups[3].Value;

                        DependencyGraph.Current.AddAsset(nameAlone, $"{type.Trim()}:{name.Trim()}");
                    }
                }


                if (result != 0)
                {
                    cli.WriteLine();
                    cli.Warn($"Zonebuilder exited with code {result} for this zone, the dependency graph will probably be missing entries for this zone.");
                }
                else 
                {
                    cli.WriteLine($"Registered {matches.Count} assets into the graph");
                    DependencyGraph.Current.Save();
                }
            }

            //DependencyGraph.Current.Save();
            cli.WriteLine($"Dependency graph updated in {(DateTime.Now - startTime).ToString(@"hh\:mm\:ss")}, and saved to disk as {DependencyGraph.CurrentName}.");

            return true;
        }
    }
}
