using LouveSystems.CommandLineInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iw4_zone_asset_finder.Commands
{
    public class FindPreciseAsset : LouveSystems.CommandLineInterface.Command
    {
        public override string HelpMessage => "Lists all zones referencing a precise asset";

        public override string HelpfulArguments => $"<asset type>:<asset name>";

        public override bool Execute(CommandLineInterface cli, string arguments, out string remainder)
        {
            if (DependencyGraph.Current == null)
            {
                remainder = string.Empty;
                cli.Err("No dependency graph loaded! Please generate a dependency graph with UPDATE, or RELOAD an existing one");
                return false;
            }

            var assetName = cli.GetFirstString(arguments, out remainder);

            var zone = DependencyGraph.Current.FindAssetByPreciseName(assetName);
            cli.Color(ConsoleColor.Gray);
            cli.Write($"For asset ");
            cli.Color(ConsoleColor.Cyan);
            cli.Write($"\"{assetName}\"");
            cli.Color(ConsoleColor.Gray);
            cli.WriteLine(" :");

            if (zone != null)
            {
                cli.Color(ConsoleColor.Green);
                cli.WriteLine($"Found in the following zones:\n\n{zone.Asset.PadRight(16)} \n    {string.Join("\n    ", zone.Zones)}");
            }
            else
            {
                cli.Color(ConsoleColor.Red);
                cli.WriteLine("No results. This asset is not present in the dependency graph.");
            }

            return true;
        }
    }
}
