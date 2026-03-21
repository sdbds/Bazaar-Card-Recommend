using System;
using BazaarGameClient.Domain.Models.Cards;
using BepInEx.Configuration;
using TheBazaar;
using TheBazaar.Tooltips;
using TheBazaar.UI.Tooltips;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BazaarPlusPlus
{
	// Token: 0x02000039 RID: 57
	internal sealed class TooltipModifierRefreshController : MonoBehaviour
	{
		// Token: 0x06000234 RID: 564 RVA: 0x0000E324 File Offset: 0x0000C524
		private void Update()
		{
			TooltipModifierRefreshController.TooltipModifierMode currentMode = TooltipModifierRefreshController.GetCurrentMode();
			if (currentMode == this._lastMode)
			{
				return;
			}
			this._lastMode = currentMode;
			TooltipModifierRefreshController.TryRefreshHoveredCardTooltip();
		}

		// Token: 0x06000235 RID: 565 RVA: 0x0000E350 File Offset: 0x0000C550
		private static TooltipModifierRefreshController.TooltipModifierMode GetCurrentMode()
		{
			Keyboard current = Keyboard.current;
			if (KeyBindings.Modifiers.IsShiftPressed(current))
			{
				return TooltipModifierRefreshController.TooltipModifierMode.Upgrade;
			}
			ConfigEntry<bool> enchantPreviewAlwaysShowConfig = ModState.EnchantPreviewAlwaysShowConfig;
			if (enchantPreviewAlwaysShowConfig == null || enchantPreviewAlwaysShowConfig.Value || KeyBindings.Modifiers.IsCtrlPressed(current))
			{
				return TooltipModifierRefreshController.TooltipModifierMode.Enchant;
			}
			return TooltipModifierRefreshController.TooltipModifierMode.Normal;
		}

		// Token: 0x06000236 RID: 566 RVA: 0x0000E38C File Offset: 0x0000C58C
		private static void TryRefreshHoveredCardTooltip()
		{
			TooltipParentComponent tooltipParentComponent = Data.TooltipParentComponent;
			CardAndSkillLookup cardAndSkillLookup = Data.CardAndSkillLookup;
			if (tooltipParentComponent == null || cardAndSkillLookup == null)
			{
				return;
			}
			if (tooltipParentComponent.HasAnyLockedTooltipControllers())
			{
				return;
			}
			foreach (CardController cardController in cardAndSkillLookup.CardControllerDictionary.Values)
			{
				if (!(cardController == null) && cardController.CardData != null)
				{
					Card cardData = cardController.CardData;
					if (cardData is ItemCard && (cardController.IsCursorOverCard || cardController.IsHovering) && !(tooltipParentComponent.GetCardTooltipController(cardData) == null))
					{
						CardTooltipData cardTooltipData = cardController.GetTooltipData() as CardTooltipData;
						if (cardTooltipData != null)
						{
							tooltipParentComponent.HideCardTooltipController();
							tooltipParentComponent.ShowCardTooltipController(cardController.transform, cardController.TooltipOffset, cardTooltipData, false);
							UpgradePreviewTooltipPatch.TryScheduleUpgradeTooltip(cardController, cardTooltipData);
							break;
						}
					}
				}
			}
		}

		// Token: 0x04000136 RID: 310
		private TooltipModifierRefreshController.TooltipModifierMode _lastMode;

		// Token: 0x02000085 RID: 133
		private enum TooltipModifierMode
		{
			// Token: 0x0400023A RID: 570
			Normal,
			// Token: 0x0400023B RID: 571
			Enchant,
			// Token: 0x0400023C RID: 572
			Upgrade
		}
	}
}
