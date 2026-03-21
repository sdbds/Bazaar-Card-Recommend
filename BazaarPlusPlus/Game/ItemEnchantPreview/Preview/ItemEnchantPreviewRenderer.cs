using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using BazaarGameClient.Domain.Models.Cards;
using BazaarGameClient.Domain.Tooltips;
using BazaarGameShared.Domain.Cards.Enchantments;
using BazaarGameShared.Domain.Core;
using BazaarGameShared.Domain.Core.Types;
using BazaarGameShared.Domain.Tooltips;
using BazaarGameShared.Domain.Values;
using TheBazaar;
using TheBazaar.Tooltips;

namespace BazaarPlusPlus.Game.ItemEnchantPreview.Preview
{
	// Token: 0x02000055 RID: 85
	public static class ItemEnchantPreviewRenderer
	{
		// Token: 0x06000288 RID: 648 RVA: 0x0000FA18 File Offset: 0x0000DC18
		public static List<TooltipSegment> Render(ItemCard previewCard, TEnchantment enchantment)
		{
			List<TooltipSegment> list = new List<TooltipSegment>();
			TEnchantmentLocalization localization = enchantment.Localization;
			if (((localization != null) ? localization.Tooltips : null) == null || enchantment.Localization.Tooltips.Count == 0)
			{
				return list;
			}
			foreach (TTooltip ttooltip in enchantment.Localization.Tooltips)
			{
				TLocalizableText tlocalizableText = (ttooltip != null) ? ttooltip.Content : null;
				if (tlocalizableText != null)
				{
					string text = ItemEnchantPreviewRenderer.RenderTooltipText(previewCard, tlocalizableText);
					if (!string.IsNullOrWhiteSpace(text))
					{
						list.Add(ItemEnchantPreviewFormatting.CreateSegment(previewCard.Enchantment.GetValueOrDefault(EEnchantmentType.Heavy), text));
					}
				}
			}
			return list;
		}

		// Token: 0x06000289 RID: 649 RVA: 0x0000FAD4 File Offset: 0x0000DCD4
		private static string RenderTooltipText(ItemCard previewCard, TLocalizableText content)
		{
			string localizedText = ItemEnchantPreviewRenderer.GetLocalizedText(content);
			if (string.IsNullOrWhiteSpace(localizedText))
			{
				return string.Empty;
			}
			string result;
			try
			{
				result = ItemEnchantPreviewRenderer.RenderWithCardTooltipData(previewCard, localizedText).TrimEnd();
			}
			catch
			{
				try
				{
					result = ItemEnchantPreviewRenderer.RenderTooltipBuilder(TooltipBuilder.Create(new TooltipContext
					{
						Instance = previewCard,
						Template = previewCard.Template,
						ValueContext = new ValueContext(Data.Run, previewCard, null)
					}, localizedText)).TrimEnd();
				}
				catch
				{
					result = localizedText;
				}
			}
			return result;
		}

		// Token: 0x0600028A RID: 650 RVA: 0x0000FB70 File Offset: 0x0000DD70
		private static string RenderWithCardTooltipData(ItemCard previewCard, string localized)
		{
			TooltipBuilder tooltipBuilder = TooltipBuilder.Create(new TooltipContext
			{
				Instance = previewCard,
				Template = previewCard.Template,
				ValueContext = new ValueContext(Data.Run, previewCard, null)
			}, localized);
			if (ItemEnchantPreviewRenderer.CardTooltipRenderMethod == null)
			{
				return ItemEnchantPreviewRenderer.RenderTooltipBuilder(tooltipBuilder);
			}
			CardTooltipData obj = new CardTooltipData(previewCard, previewCard.Template);
			string text = ItemEnchantPreviewRenderer.CardTooltipRenderMethod.Invoke(obj, new object[]
			{
				tooltipBuilder
			}) as string;
			if (!string.IsNullOrWhiteSpace(text))
			{
				return text;
			}
			return ItemEnchantPreviewRenderer.RenderTooltipBuilder(tooltipBuilder);
		}

		// Token: 0x0600028B RID: 651 RVA: 0x0000FC04 File Offset: 0x0000DE04
		private static string GetLocalizedText(TLocalizableText content)
		{
			string result;
			try
			{
				result = content.GetLocalizedText();
			}
			catch
			{
				result = (content.Text ?? string.Empty);
			}
			return result;
		}

		// Token: 0x0600028C RID: 652 RVA: 0x0000FC40 File Offset: 0x0000DE40
		private static string RenderTooltipBuilder(TooltipBuilder builder)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (ITooltipComponent tooltipComponent in builder.Components)
			{
				ITooltipToken tooltipToken = tooltipComponent as ITooltipToken;
				if (tooltipToken != null && tooltipToken.ReferencedAttribute != null && tooltipToken.ReferencedAttribute.Value.RequiresConversionToSecondsForTooltips())
				{
					float value = TooltipExtensions.MillisecondsToSeconds(tooltipToken.Resolve().GetValueOrDefault());
					stringBuilder.Append(value.IsDecimal() ? value.GetDecimalValueString() : value.ToString());
				}
				else
				{
					stringBuilder.Append(tooltipComponent.Render());
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x04000178 RID: 376
		private static readonly MethodInfo CardTooltipRenderMethod = typeof(CardTooltipData).GetMethod("RenderTooltip", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[]
		{
			typeof(TooltipBuilder)
		}, null);
	}
}
