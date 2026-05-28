using System;
using BazaarGameClient.Domain.Models.Cards;
using HarmonyLib;
using UnityEngine;

namespace BazaarRecommend
{
    [HarmonyPatch(typeof(ItemController), "ShowCard")]
    public static class TierIndicatorPatch
    {
        private const string BadgeName = "BazaarRecommendItemTierBadge";
        private const string LegacyBadgeName = "JulesTierBadge";

        [HarmonyPostfix]
        static void Postfix(ItemController __instance, bool show)
        {
            try
            {
                // 清除旧 badge
                TierBadgeRenderer.RemoveBadge(__instance.transform, BadgeName);
                TierBadgeRenderer.RemoveBadge(__instance.transform, LegacyBadgeName);

                if (!show) return;

                var itemCard = __instance.CardData as ItemCard;
                if (itemCard == null) return;

                string cardId = itemCard.Template?.Id.ToString();
                string hero = null;
                var heroes = itemCard.Template?.Heroes;
                if (heroes != null && heroes.Count > 0)
                    foreach (var h in heroes) { hero = h.ToString(); break; }
                string tier = CardTierList.GetItemTier(cardId, hero);
                if (string.IsNullOrEmpty(tier)) return;

                var col = __instance.GetComponent<BoxCollider>();
                if (col == null) return;

                TierBadgeRenderer.CreateBadge(__instance.gameObject, col, BadgeName, tier);
                Console.WriteLine($"[TierBadge] {itemCard.Template?.InternalName} ({itemCard.Template?.Size}) = {tier}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TierBadge] ERROR: {ex}");
            }
        }
    }
}
