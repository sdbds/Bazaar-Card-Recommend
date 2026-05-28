using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace BazaarRecommend
{
    public static class CardTierList
    {
        private const string ItemTierResourceName = "BazaarRecommend.Data.tierlist.json";
        private const string SkillTierResourceName = "BazaarRecommend.Data.skill_tierlist.json";

        // key: card Id (GUID), value: { hero -> tier }
        private static Dictionary<string, Dictionary<string, string>> _itemTierMap;
        private static Dictionary<string, string> _skillTierMap;

        private static Dictionary<string, Dictionary<string, string>> ItemTierMap
        {
            get
            {
                if (_itemTierMap == null)
                    _itemTierMap = LoadItemTiers();
                return _itemTierMap;
            }
        }

        private static Dictionary<string, string> SkillTierMap
        {
            get
            {
                if (_skillTierMap == null)
                    _skillTierMap = LoadSkillTiers();
                return _skillTierMap;
            }
        }

        // 返回该卡片在指定英雄下的 tier，hero 为 null 则返回任意一个
        public static string GetTier(string cardId, string hero = null)
        {
            return GetItemTier(cardId, hero);
        }

        public static string GetItemTier(string cardId, string hero = null)
        {
            if (string.IsNullOrWhiteSpace(cardId))
                return null;
            if (!ItemTierMap.TryGetValue(cardId, out var heroTiers))
                return null;
            if (hero != null && heroTiers.TryGetValue(hero, out string t))
                return t;
            // 无指定英雄时取第一个
            foreach (var v in heroTiers.Values)
                return v;
            return null;
        }

        public static string GetSkillTier(string cardId)
        {
            if (string.IsNullOrWhiteSpace(cardId))
                return null;
            if (SkillTierMap.TryGetValue(cardId, out string tier))
                return tier;
            return null;
        }

        private static Dictionary<string, Dictionary<string, string>> LoadItemTiers()
        {
            var map = new Dictionary<string, Dictionary<string, string>>();
            try
            {
                var arr = LoadJsonArrayResource(ItemTierResourceName);
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
            catch (Exception ex)
            {
                Console.WriteLine($"[BazaarRecommend] Failed to load item tierlist: {ex.Message}");
            }
            return map;
        }

        private static Dictionary<string, string> LoadSkillTiers()
        {
            var map = new Dictionary<string, string>();
            try
            {
                var arr = LoadJsonArrayResource(SkillTierResourceName);
                foreach (var entry in arr)
                {
                    string id   = entry["id"]?.ToString();
                    string tier = entry["tier"]?.ToString();
                    if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(tier))
                        continue;
                    map[id] = tier;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BazaarRecommend] Failed to load skill tierlist: {ex.Message}");
            }
            return map;
        }

        private static JArray LoadJsonArrayResource(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    throw new FileNotFoundException($"Embedded resource not found: {resourceName}");
                using (var reader = new StreamReader(stream))
                    return JArray.Parse(reader.ReadToEnd());
            }
        }
    }
}
