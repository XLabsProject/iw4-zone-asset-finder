using LouveSystems.CommandLineInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iw4_zone_asset_finder.Commands
{
    public class FindAsset : LouveSystems.CommandLineInterface.Command
    {
        public override string HelpMessage => "Searches an asset through all zones and lists references";

        public override string HelpfulArguments => $"<asset name>";

        public override bool Execute(CommandLineInterface cli, string arguments, out string remainder)
        {
            if (DependencyGraph.Current == null)
            {
                remainder = string.Empty;
                cli.Err("No dependency graph loaded! Please generate a dependency graph with UPDATE, or RELOAD an existing one");
                return false;
            }

            var assetName = cli.GetFirstString(arguments, out remainder);
            var type = string.Empty;
            List <DependencyGraph.SearchResult> zones;

            zones = DependencyGraph.Current.FindAssetByName(assetName);
            cli.Color(ConsoleColor.Gray);
            cli.Write($"For search term ");
            cli.Color(ConsoleColor.Cyan);
            cli.Write($"\"{assetName}\"");
            cli.Color(ConsoleColor.Gray);
            cli.WriteLine(" :");

            if (zones.Count > 0)
            {
                cli.Color(ConsoleColor.Green);
                cli.WriteLine($"Found in the following zones:\n\n{string.Join("\n", zones.Select(o=>$"{o.Asset.PadRight(16)} \n    {string.Join("\n    ", o.Zones)}\n"))}");
            }
            else
            {
                cli.Color(ConsoleColor.Red);
                cli.WriteLine("No results.");
            }

            return true;
        }
    }
}
