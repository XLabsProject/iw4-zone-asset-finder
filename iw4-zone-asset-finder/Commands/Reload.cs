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
    public class Reload : LouveSystems.CommandLineInterface.Command
    {
        public override string HelpMessage => "Reloads the dependency map from disk";

        public override string HelpfulArguments => string.Empty;

        public override bool Execute(CommandLineInterface cli, string arguments, out string remainder)
        {
            remainder = arguments;
            try
            {
                DependencyGraph.Load();
                if (DependencyGraph.Current == null)
                {
                    cli.Warn($"No dependency graph file to reload!");
                }
                else
                {
                    cli.WriteLine($"Successfully loaded the dependency graph ({DependencyGraph.Current.Count} zones)");
                }

                return true;
            }
            catch(Exception e)
            {
                cli.Err($"Error while trying to load the dep graph: {e}");
            }

            return false;
        }
    }
}
