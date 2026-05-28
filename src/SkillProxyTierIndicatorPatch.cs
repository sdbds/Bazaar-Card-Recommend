using System;
using System.Threading.Tasks;
using BazaarGameClient.Domain.Models.Cards;
using HarmonyLib;
using TheBazaar;

namespace BazaarRecommend
{
    [HarmonyPatch(typeof(SkillProxyRenderer), "Initialize")]
    public static class SkillProxyInitializeTierIndicatorPatch
    {
        [HarmonyPostfix]
        static void Postfix(SkillProxyRenderer __instance, Card card)
        {
            SkillProxyTierIndicator.Apply(__instance, card);
        }
    }

    [HarmonyPatch(typeof(SkillProxyRenderer), "UpdateCard")]
    public static class SkillProxyUpdateTierIndicatorPatch
    {
        [HarmonyPostfix]
        static void Postfix(SkillProxyRenderer __instance, Card cardData, ref Task __result)
        {
            __result = ApplyAfterUpdate(__result, __instance, cardData);
        }

        private static async Task ApplyAfterUpdate(Task original, SkillProxyRenderer renderer, Card cardData)
        {
            await original;
            SkillProxyTierIndicator.Apply(renderer, cardData);
        }
    }

    internal static class SkillProxyTierIndicator
    {
        private const string BadgeName = "BazaarRecommendSkillBarTierBadge";

        public static void Apply(SkillProxyRenderer renderer, Card card)
        {
            if (renderer == null)
                return;

            try
            {
                TierBadgeRenderer.RemoveBadge(renderer.transform, BadgeName);

                var skillCard = (card ?? renderer.Card) as SkillCard;
                if (skillCard == null)
                    return;

                string cardId = skillCard.Template?.Id.ToString();
                if (string.IsNullOrWhiteSpace(cardId) && skillCard.TemplateId != Guid.Empty)
                    cardId = skillCard.TemplateId.ToString();

                string tier = CardTierList.GetSkillTier(cardId);
                if (string.IsNullOrEmpty(tier))
                    return;

                if (!TierBadgeRenderer.TryCreateCompactBadge(renderer.gameObject, BadgeName, tier))
                    return;

                Console.WriteLine($"[TierBadge] SkillProxy {skillCard.Name} = {tier}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TierBadge] SkillProxy ERROR: {ex}");
            }
        }
    }
}
