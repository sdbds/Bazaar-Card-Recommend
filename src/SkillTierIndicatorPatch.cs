using System;
using BazaarGameClient.Domain.Models.Cards;
using HarmonyLib;
using UnityEngine;

namespace BazaarRecommend
{
    [HarmonyPatch(typeof(SkillController), "ShowCard")]
    public static class SkillTierIndicatorPatch
    {
        private const string BadgeName = "BazaarRecommendSkillTierBadge";

        [HarmonyPostfix]
        static void Postfix(SkillController __instance, bool show)
        {
            try
            {
                TierBadgeRenderer.RemoveBadge(__instance.transform, BadgeName);

                if (!show) return;

                var skillCard = __instance.CardData as SkillCard;
                if (skillCard == null) return;

                string cardId = skillCard.Template?.Id.ToString();
                if (string.IsNullOrWhiteSpace(cardId) && skillCard.TemplateId != Guid.Empty)
                    cardId = skillCard.TemplateId.ToString();

                string tier = CardTierList.GetSkillTier(cardId);
                if (string.IsNullOrEmpty(tier)) return;

                var col = __instance.GetComponent<BoxCollider>();
                if (col != null)
                {
                    TierBadgeRenderer.CreateSkillSelectionBadge(__instance.gameObject, col, BadgeName, tier);
                }
                else if (!TierBadgeRenderer.TryCreateSkillSelectionBadge(__instance.gameObject, BadgeName, tier))
                {
                    return;
                }

                Console.WriteLine($"[TierBadge] Skill {skillCard.Name} = {tier}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TierBadge] Skill ERROR: {ex}");
            }
        }
    }
}
