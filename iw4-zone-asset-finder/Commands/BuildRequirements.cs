using LouveSystems.CommandLineInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iw4_zone_asset_finder.Commands
{
    public class BuildRequirements : LouveSystems.CommandLineInterface.Command
    {
        public override string HelpMessage => "Given multiple space-separated asset names and types enclosed by a \", outputs a minimal set of zones as requirement for building a patch zone containing these assets";

        public override string HelpfulArguments => $"\"<type>:<name> <type>:<name>...\"";

        public override bool Execute(CommandLineInterface cli, string arguments, out string remainder)
        {
            if (DependencyGraph.Current == null)
            {
                remainder = string.Empty;
                cli.Err("No dependency graph loaded! Please generate a dependency graph with UPDATE, or RELOAD an existing one");
                return false;
            }

            var assets = cli.GetFirstString(arguments, out remainder);

            var assetsToFind = new List<string>(assets.Split(' '));

            List<string> finalAssets = new List<string>();
            List<string> zonesToInclude = new List<string>();

            List<DependencyGraph.SearchResult> assetInZones = new List<DependencyGraph.SearchResult>();

            foreach (var asset in assetsToFind)
            {
                var assetSearchResult = DependencyGraph.Current.FindAssetByPreciseName(asset);
                if (assetSearchResult == null)
                {
                    var multiSearch = DependencyGraph.Current.FindAssetByName(asset);

                    if (multiSearch.Count > 0) {
                        assetSearchResult = multiSearch[0];
                        cli.Warn($"Warning: Could not find asset named {asset}, performed lose search instead and replaced it with asset {assetSearchResult.Asset}");
                    }
                    else
                    {
                        cli.Err($"Asset {asset} could not be find in the dependency graph. This asset will be missing");
                        continue;
                    }
                }

                var aWithType = asset.Split(':');
                finalAssets.Add($"{aWithType[0]},{aWithType[1]}");
                assetInZones.Add(assetSearchResult);

            }

            Dictionary<string, int> finalZoneScore = new Dictionary<string, int>();
            while (assetInZones.Count > 0)
            {
                Dictionary<string, int> zoneScore = new Dictionary<string, int>();

                foreach(var searchResult in assetInZones)
                {
                    foreach (var zone in searchResult.Zones)
                    {
                        if (!zoneScore.ContainsKey(zone))
                        {
                            zoneScore[zone] = 0;
                        }

                        zoneScore[zone]++;
                    }
                }

                var nextZone = zoneScore.OrderByDescending(o => o.Value).First().Key;

                zonesToInclude.Add(nextZone);
                assetInZones.RemoveAll(o => o.Zones.Contains(nextZone));
                finalZoneScore[nextZone] = zoneScore[nextZone];
            }

            cli.Color(ConsoleColor.Gray);
            cli.Write($"The following dependencies are suggested: ");

            foreach(var zoneScore in finalZoneScore)
            {
                cli.Color(ConsoleColor.White);
                cli.Write(zoneScore.Key);
                cli.Color(ConsoleColor.Gray);
                cli.Write($"({zoneScore.Value} assets), ");
            }

            cli.WriteLine("\n\nSource requirements:");

            cli.Color(ConsoleColor.Cyan);
            cli.WriteLine($"{string.Join("\n", zonesToInclude.Select(o=>$"require,{o}"))}\n");
            cli.Color(ConsoleColor.Gray);
            
            cli.WriteLine($"{string.Join("\n", finalAssets)}");

            return true;
        }
    }
}
