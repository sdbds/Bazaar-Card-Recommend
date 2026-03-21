using System;
using System.Collections;
using System.Collections.Generic;
using BazaarGameClient.Domain.Models.Cards;
using HarmonyLib;
using TheBazaar;
using TheBazaar.Tooltips;
using UnityEngine.InputSystem;

namespace BazaarPlusPlus
{
	// Token: 0x0200004C RID: 76
	[HarmonyPatch(typeof(CardController), "ShowTooltips")]
	internal static class UpgradePreviewTooltipPatch
	{
		// Token: 0x06000271 RID: 625 RVA: 0x0000F170 File Offset: 0x0000D370
		[HarmonyPostfix]
		private static void Postfix(CardController __instance)
		{
			UpgradePreviewTooltipPatch.TryScheduleUpgradeTooltip(__instance, null);
		}

		// Token: 0x06000272 RID: 626 RVA: 0x0000F17C File Offset: 0x0000D37C
		internal static bool TryScheduleUpgradeTooltip(CardController controller, CardTooltipData tooltipData = null)
		{
			if (controller == null)
			{
				return false;
			}
			if (!KeyBindings.Modifiers.IsShiftPressed(Keyboard.current))
			{
				return false;
			}
			Card cardData = controller.CardData;
			if (!(cardData is ItemCard))
			{
				return false;
			}
			if (!cardData.CanCardUpgrade())
			{
				return false;
			}
			CardTooltipData cardTooltipData = tooltipData ?? (controller.GetTooltipData() as CardTooltipData);
			if (cardTooltipData == null)
			{
				return false;
			}
			if (Data.TooltipParentComponent == null)
			{
				return false;
			}
			if (!UpgradePreviewTooltipPatch.PendingControllers.Add(controller))
			{
				return false;
			}
			controller.StartCoroutine(UpgradePreviewTooltipPatch.ShowUpgradeTooltipWhenReady(controller, cardData, cardTooltipData));
			return true;
		}

		// Token: 0x06000273 RID: 627 RVA: 0x0000F201 File Offset: 0x0000D401
		private static IEnumerator ShowUpgradeTooltipWhenReady(CardController controller, Card card, CardTooltipData tooltipData)
		{
			UpgradePreviewTooltipPatch.<ShowUpgradeTooltipWhenReady>d__3 <ShowUpgradeTooltipWhenReady>d__ = new UpgradePreviewTooltipPatch.<ShowUpgradeTooltipWhenReady>d__3(0);
			<ShowUpgradeTooltipWhenReady>d__.controller = controller;
			<ShowUpgradeTooltipWhenReady>d__.card = card;
			<ShowUpgradeTooltipWhenReady>d__.tooltipData = tooltipData;
			return <ShowUpgradeTooltipWhenReady>d__;
		}

		// Token: 0x0400016E RID: 366
		private static readonly HashSet<CardController> PendingControllers = new HashSet<CardController>();
	}
}
