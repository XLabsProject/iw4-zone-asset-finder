using System;

namespace iw4_zone_asset_finder.Commands
{
    class Types : LouveSystems.CommandLineInterface.Command
    {
        public override string HelpMessage => "List available asset types";
        public override string HelpfulArguments => string.Empty;

        public override bool Execute(LouveSystems.CommandLineInterface.CommandLineInterface cli, string arguments, out string remainder)
        {
            remainder = arguments;

            cli.WriteLine(string.Join("\n", DependencyGraph.Current.GetTypes()));

            return true;
        }
    }
}
