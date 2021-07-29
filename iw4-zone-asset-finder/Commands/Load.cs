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
    public class Load : LouveSystems.CommandLineInterface.Command
    {
        public override string HelpMessage => "Loads a dependency graph from disk";

        public override string HelpfulArguments => "<filename.json>";

        public override bool Execute(CommandLineInterface cli, string arguments, out string remainder)
        {

            string filename = cli.GetFirstString(arguments, out remainder);

            if (!File.Exists(filename))
            {
                cli.Err($"No such file as {filename}! Aborting.");
                return false;
            }

            try
            {
                DependencyGraph.CurrentName = filename;
                DependencyGraph.Load();
                if (DependencyGraph.Current == null)
                {
                    cli.Warn($"The provided file is invalid and could not be loaded.");
                }
                else
                {
                    cli.WriteLine($"Successfully loaded the dependency graph {filename} ({DependencyGraph.Current.Count} zones)");
                }

                return true;
            }
            catch (Exception e)
            {
                cli.Err($"Error while trying to load the dep graph: {e}");
            }

            return false;
        }
    }
}
