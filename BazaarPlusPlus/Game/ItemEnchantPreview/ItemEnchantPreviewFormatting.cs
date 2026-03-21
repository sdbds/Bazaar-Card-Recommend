using System;
using System.Text.RegularExpressions;
using BazaarGameShared.Domain.Core.Types;
using TheBazaar.Tooltips;
using TheBazaar.Utilities;

namespace BazaarPlusPlus.Game.ItemEnchantPreview
{
	// Token: 0x02000052 RID: 82
	public static class ItemEnchantPreviewFormatting
	{
		// Token: 0x06000280 RID: 640 RVA: 0x0000F640 File Offset: 0x0000D840
		public static TooltipSegment CreateSegment(EEnchantmentType enchantmentType, string renderedText)
		{
			string enchantmentLabel = ItemEnchantPreviewFormatting.GetEnchantmentLabel(enchantmentType);
			string enchantmentColorHex = ItemEnchantPreviewFormatting.GetEnchantmentColorHex(enchantmentType);
			string text = ItemEnchantPreviewFormatting.ScaleInlineSizes(renderedText, 0.55f);
			return new TooltipSegment(string.Format("<size={0}%>\u00a0\u00a0· <color=#{1}>{2}</color>: </size><size={3}%>{4}</size>", new object[]
			{
				60,
				enchantmentColorHex,
				enchantmentLabel,
				55,
				text
			}), null, null, -1, null);
		}

		// Token: 0x06000281 RID: 641 RVA: 0x0000F6B4 File Offset: 0x0000D8B4
		private static string ScaleInlineSizes(string text, float scale)
		{
			if (string.IsNullOrEmpty(text))
			{
				return text;
			}
			return ItemEnchantPreviewFormatting.SizeTagRegex.Replace(text, delegate(Match match)
			{
				int num;
				if (!int.TryParse(match.Groups[1].Value, out num))
				{
					return match.Value;
				}
				int num2 = Math.Max(1, (int)Math.Round((double)((float)num * scale)));
				return string.Format("<size={0}%>", num2);
			});
		}

		// Token: 0x06000282 RID: 642 RVA: 0x0000F6F0 File Offset: 0x0000D8F0
		public static string GetEnchantmentLabel(EEnchantmentType enchantmentType)
		{
			string result;
			try
			{
				result = new LocalizableText(enchantmentType.ToString()).GetLocalizedText();
			}
			catch
			{
				result = enchantmentType.ToString();
			}
			return result;
		}

		// Token: 0x06000283 RID: 643 RVA: 0x0000F73C File Offset: 0x0000D93C
		public static string GetEnchantmentColorHex(EEnchantmentType enchantmentType)
		{
			switch (enchantmentType)
			{
			case EEnchantmentType.Deadly:
				return "F5503D";
			case EEnchantmentType.Fiery:
				return "FF9F45";
			case EEnchantmentType.Golden:
				return "FFCD19";
			case EEnchantmentType.Heavy:
				return "CB9F6E";
			case EEnchantmentType.Icy:
				return "3FC8F7";
			case EEnchantmentType.Obsidian:
				return "9D4A6F";
			case EEnchantmentType.Radiant:
				return "98A8FE";
			case EEnchantmentType.Restorative:
				return "8EEA31";
			case EEnchantmentType.Shielded:
				return "F4CF20";
			case EEnchantmentType.Shiny:
				return "98A8FE";
			case EEnchantmentType.Toxic:
				return "0EBE4F";
			case EEnchantmentType.Turbo:
				return "00ECC3";
			}
			return "FFFFFF";
		}

		// Token: 0x04000175 RID: 373
		private const int PrefixSizePercent = 60;

		// Token: 0x04000176 RID: 374
		private const int EffectSizePercent = 55;

		// Token: 0x04000177 RID: 375
		private static readonly Regex SizeTagRegex = new Regex("<size=(\\d+)%>", RegexOptions.Compiled | RegexOptions.CultureInvariant);
	}
}
