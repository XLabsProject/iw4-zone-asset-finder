using LouveSystems.CommandLineInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iw4_zone_asset_finder.Commands
{
    public class Ignore : LouveSystems.CommandLineInterface.Command
    {
        public override string HelpMessage => "Removes a zone from the dependency graph until it is reloaded again";

        public override string HelpfulArguments => $"<zone name>";

        public override bool Execute(CommandLineInterface cli, string arguments, out string remainder)
        {
            if (DependencyGraph.Current == null)
            {
                remainder = string.Empty;
                cli.Err("No dependency graph loaded! Please generate a dependency graph with UPDATE, or LOAD/RELOAD an existing one");
                return false;
            }

            var zoneName = cli.GetFirstString(arguments, out remainder);

            DependencyGraph.Current.RemoveZone(zoneName);
            cli.WriteLine($"Zone {zoneName} will be ignored from dependencies until the graph is loaded or reloaded.");

            return true;
        }
    }
}
