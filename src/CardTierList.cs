using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace BazaarRecommend
{
    public static class CardTierList
    {
        // key: 卡片 Id (GUID)，value: { hero -> tier }
        private static Dictionary<string, Dictionary<string, string>> _tierMap;

        private static Dictionary<string, Dictionary<string, string>> TierMap
        {
            get
            {
                if (_tierMap == null)
                    _tierMap = Load();
                return _tierMap;
            }
        }

        // 返回该卡片在指定英雄下的 tier，hero 为 null 则返回任意一个
        public static string GetTier(string cardId, string hero = null)
        {
            if (string.IsNullOrWhiteSpace(cardId))
                return null;
            if (!TierMap.TryGetValue(cardId, out var heroTiers))
                return null;
            if (hero != null && heroTiers.TryGetValue(hero, out string t))
                return t;
            // 无指定英雄时取第一个
            foreach (var v in heroTiers.Values)
                return v;
            return null;
        }

        private static Dictionary<string, Dictionary<string, string>> Load()
        {
            var map = new Dictionary<string, Dictionary<string, string>>();
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                using (var stream = assembly.GetManifestResourceStream("BazaarRecommend.Data.tierlist.json"))
                using (var reader = new StreamReader(stream))
                {
                    var arr = JArray.Parse(reader.ReadToEnd());
                    foreach (var entry in arr)
                    {
                        string id   = entry["id"]?.ToString();
                        string hero = entry["hero"]?.ToString();
                        string tier = entry["tier"]?.ToString();
                        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(tier))
                            continue;
                        if (!map.ContainsKey(id))
                            map[id] = new Dictionary<string, string>();
                        map[id][hero ?? ""] = tier;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BazaarRecommend] Failed to load tierlist: {ex.Message}");
            }
            return map;
        }
    }
}
