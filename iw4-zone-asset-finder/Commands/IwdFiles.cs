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
    public class IwdFiles : LouveSystems.CommandLineInterface.Command
    {
        public override string HelpMessage => "Lists IWD files used by a map";

        public override string HelpfulArguments => "<zone name>";

        readonly string[] GAME_EXECUTABLES = new string[] { "iw4x.exe", "iw4_direct.exe" };

        public override bool Execute(CommandLineInterface cli, string arguments, out string remainder)
        {

            string zoneName = cli.GetFirstString(arguments, out remainder);

            if (!File.Exists(GAME_EXECUTABLES[0]) && !File.Exists(GAME_EXECUTABLES[1]))
            {
                cli.Err($"Could not find a game executable ({string.Join(" or ", GAME_EXECUTABLES)})");
                return false;
            }

            string output = string.Empty;
            int result = -1;

            foreach (var exe in GAME_EXECUTABLES)
            {
                if (File.Exists(exe))
                {
                    cli.WriteLine($"Loading {zoneName}...");
                    output = Helper.GetProgramOutput(exe, $"-nosteam -zonebuilder -stdout +loadzone {zoneName} +listRawImages +listStreamedSounds 9 +quit", out result);
                    break;
                }
            }

            if (result == 0)
            {
                var dumpFile = $"{zoneName}_iwd.txt";

                using (FileStream fs = new FileStream(dumpFile, FileMode.Create))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        var reg = new Regex(@"(\([a-z]+ [0-9]\)) (.*)");
                        var matches = reg.Matches(output);

                        foreach (Match match in matches)
                        {
                            // Group #1 is nothing
                            // Group #2 is asset name

                            if (match.Groups.Count == 3)
                            {
                                string assetName = match.Groups[2].Value.Trim();

                                cli.WriteLine(assetName);
                                sw.WriteLine(assetName);

                            }
                        }
                    }
                }

                cli.Color(ConsoleColor.Green);
                cli.Write($"\nSuccessfully wrote all referenced iwd-files in ");
                cli.Color(ConsoleColor.Cyan);
                cli.WriteLine(dumpFile);
                return true;
            }
            else
            {
                cli.WriteLine();
                cli.Err($"Zonebuilder exited with code {result} for this zone. Aborting.");
                return false;
            }
        }
    }
}
