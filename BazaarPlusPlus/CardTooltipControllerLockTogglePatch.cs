using System;
using BazaarGameClient.Domain.Models.Cards;
using BazaarGameShared.Domain.Cards;
using HarmonyLib;
using TheBazaar;
using TheBazaar.Tooltips;
using TheBazaar.UI.Tooltips;
using UnityEngine.EventSystems;

namespace BazaarPlusPlus
{
	// Token: 0x02000049 RID: 73
	[HarmonyPatch(typeof(CardTooltipController), "LockTooltipToggle")]
	public static class CardTooltipControllerLockTogglePatch
	{
		// Token: 0x0600026E RID: 622 RVA: 0x0000EF08 File Offset: 0x0000D108
		[HarmonyPrefix]
		private static bool Prefix(CardTooltipController __instance)
		{
			Card card = (__instance != null) ? __instance.CurrentCard : null;
			MonsterLockShowcaseRuntime instance = MonsterLockShowcaseRuntime.Instance;
			if (instance != null && instance.TryConsumeNextClickToClosePreview(new PointerEventData.InputButton?(PointerEventData.InputButton.Right), "next right click"))
			{
				return false;
			}
			if (instance != null && instance.ShouldInterceptLockToggle(card) && instance.HandleLockToggle(card))
			{
				return false;
			}
			if (card == null)
			{
				return true;
			}
			CardAndSkillLookup cardAndSkillLookup = Data.CardAndSkillLookup;
			CardController cardController = (cardAndSkillLookup != null) ? cardAndSkillLookup.GetCardController(card) : null;
			if (cardController == null || cardController.GetComponent<ShowcaseCardMarker>() == null)
			{
				return true;
			}
			string component = "EncounterTooltipPreview";
			string str = "Suppressed lock toggle for showcase card ";
			ITCard template = card.Template;
			BppLog.Debug(component, str + (((template != null) ? template.InternalName : null) ?? card.TemplateId.ToString()));
			return false;
		}
	}
}
