using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iw4_zone_asset_finder
{
    [Serializable]
    public class DependencyGraph
    {
        public class SearchResult
        {
            public string Asset;
            public List<string> Zones = new List<string>();

            public SearchResult(string asset, List<string> zones)
            {
                Asset = asset;
                Zones = zones;
            }
        }

        public static DependencyGraph Current;
        public static string CurrentName = "DEPENDENCY_GRAPH.JSON";

        [Newtonsoft.Json.JsonRequired]
        private Dictionary<string, List<string>> assetsLocation = new Dictionary<string, List<string>>();

        public int Count => assetsLocation.Count;

        public void RemoveZone(string zoneName)
        {
            assetsLocation = assetsLocation.Where(o => !o.Value.Contains(zoneName)).ToDictionary(p=>p.Key, p=>p.Value);
        }

        public List<SearchResult> FindAssetByName(string assetName)
        {
            var keys = assetsLocation.Keys.Where(o => AreStringsRoughlySimilar(o, assetName));

            return keys.Select(o => new SearchResult(o, assetsLocation[o])).ToList();
        }

        public SearchResult FindAssetByPreciseName(string assetName)
        {
            var keys = assetsLocation.Keys.Where(o => o == assetName);

            if (keys.Count() == 0)
            {
                return null;
            }

            return keys.Select(o => new SearchResult(o, assetsLocation[o])).First();
        }

        public void AddAsset(string zone, string assetPath)
        {
            if (!assetsLocation.ContainsKey(assetPath))
            {
                assetsLocation[assetPath] = new List<string>();
            }

            if (assetsLocation[assetPath].Contains(zone))
            {
                return;
            }

            assetsLocation[assetPath].Add(zone);
        }

        public void Save()
        {
            File.WriteAllText(CurrentName, Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented));
        }

        public static void Load()
        {
            Current = null;

            if (File.Exists(CurrentName))
            {
                try
                {
                    Current = Newtonsoft.Json.JsonConvert.DeserializeObject<DependencyGraph>(File.ReadAllText("DEPENDENCY_GRAPH.JSON"));
                }
                catch
                {
                }
            }

        }

        public List<string> GetTypes()
        {
            return assetsLocation.Keys.Select(o => o.Substring(0, o.IndexOf(':')).Trim()).Distinct().ToList();
        }

        private static bool AreStringsRoughlySimilar(string master, string element)
        {
            master = Path.GetFileName(master.ToUpper());
            element = Path.GetFileName(element.ToUpper());

            int i;
            if ((i = master.IndexOf(':')) > -1)
            {
                master = master.Substring(i + 1);
            }
            if ((i = element.IndexOf(':')) > -1)
            {
                element = element.Substring(i + 1);
            }

            if (master == element)
            {
                return true;
            }

            if (master.Contains(element))
            {
                return true;
            }

            return false;
        }
    }
}
