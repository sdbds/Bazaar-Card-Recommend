using System;
using System.Collections.Generic;
using System.Text;
using BazaarGameClient.Domain.Models.Cards;
using BazaarPlusPlus.Game.ItemEnchantPreview;
using BepInEx.Configuration;
using HarmonyLib;
using TheBazaar;
using TheBazaar.Tooltips;
using UnityEngine.InputSystem;

namespace BazaarPlusPlus
{
	// Token: 0x0200004B RID: 75
	[HarmonyPatch(typeof(CardTooltipData), "GetPassiveTooltipBlock")]
	public static class CardTooltipDataPassivePatch
	{
		// Token: 0x0600026F RID: 623 RVA: 0x0000EFD4 File Offset: 0x0000D1D4
		private static void AppendTooltipText(StringBuilder builder, string text)
		{
			if (builder == null || string.IsNullOrWhiteSpace(text))
			{
				return;
			}
			string text2 = text.Replace("\r\n", "\n").Replace('\r', '\n').TrimEnd('\n');
			if (text2.Length == 0)
			{
				return;
			}
			foreach (string value in text2.Split('\n', StringSplitOptions.None))
			{
				if (!string.IsNullOrWhiteSpace(value))
				{
					builder.Append(value);
					builder.Append('\n');
				}
			}
		}

		// Token: 0x06000270 RID: 624 RVA: 0x0000F04C File Offset: 0x0000D24C
		[HarmonyPostfix]
		private static void Postfix(CardTooltipData __instance, ref ValueTuple<StringBuilder, TooltipSegment?> __result)
		{
			try
			{
				if (__result.Item1 == null || Data.IsInCombat)
				{
					return;
				}

				if (KeyBindings.Modifiers.IsShiftPressed(Keyboard.current))
				{
					return;
				}

				Card card = __instance.CardInstance;
				StringBuilder item = __result.Item1;
				bool headerWritten = false;

				// 1. 卡牌评级（任意 ItemCard 均显示，不受 Ctrl 限制）
				CardRatingInfo ratingInfo;
				if (card is ItemCard && CardRatingDatabase.Instance.TryGet(card, out ratingInfo))
				{
					if (item.Length > 0 && item[item.Length - 1] != '\n')
					{
						item.Append('\n');
					}
					item.Append("Bazaar++\n");
					headerWritten = true;
					string tierColor = CardRatingDatabase.GetTierColor(ratingInfo.Tier);
					item.Append(string.Format("<size=60%>  评级: <color=#{0}>{1}</color></size>\n", tierColor, ratingInfo.Tier));
				}

				// 2. 附魔预览（原有逻辑，受 AlwaysShow / Ctrl 控制）
				ConfigEntry<bool> enchantPreviewAlwaysShowConfig = ModState.EnchantPreviewAlwaysShowConfig;
				if (enchantPreviewAlwaysShowConfig == null || enchantPreviewAlwaysShowConfig.Value || KeyBindings.Modifiers.IsCtrlPressed(Keyboard.current))
				{
					List<TooltipSegment> list = ItemEnchantPreviewService.BuildPreviewSegments(card);
					if (list.Count != 0)
					{
						if (!headerWritten)
						{
							if (item.Length > 0 && item[item.Length - 1] != '\n')
							{
								item.Append('\n');
							}
							item.Append("Bazaar++\n");
						}
						foreach (TooltipSegment tooltipSegment in list)
						{
							if (!string.IsNullOrWhiteSpace(tooltipSegment.Text))
							{
								CardTooltipDataPassivePatch.AppendTooltipText(item, tooltipSegment.Text);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				BppLog.Error("ItemEnchantPreview", "Failed to append passive tooltip previews", ex);
			}
		}
	}
}
