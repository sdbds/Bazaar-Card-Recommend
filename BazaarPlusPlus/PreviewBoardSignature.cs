using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace BazaarPlusPlus
{
	// Token: 0x02000023 RID: 35
	internal static class PreviewBoardSignature
	{
		// Token: 0x06000141 RID: 321 RVA: 0x00007AA4 File Offset: 0x00005CA4
		public static string Build(PreviewBoardModel model)
		{
			if (model == null)
			{
				return string.Empty;
			}
			string separator = "|";
			IEnumerable<PreviewCardSpec> itemCards = model.ItemCards;
			Func<PreviewCardSpec, string> selector;
			if ((selector = PreviewBoardSignature.<>O.<0>__BuildCardSignature) == null)
			{
				selector = (PreviewBoardSignature.<>O.<0>__BuildCardSignature = new Func<PreviewCardSpec, string>(PreviewBoardSignature.BuildCardSignature));
			}
			string text = string.Join(separator, itemCards.Select(selector));
			string separator2 = "|";
			IEnumerable<PreviewCardSpec> skillCards = model.SkillCards;
			Func<PreviewCardSpec, string> selector2;
			if ((selector2 = PreviewBoardSignature.<>O.<0>__BuildCardSignature) == null)
			{
				selector2 = (PreviewBoardSignature.<>O.<0>__BuildCardSignature = new Func<PreviewCardSpec, string>(PreviewBoardSignature.BuildCardSignature));
			}
			string text2 = string.Join(separator2, skillCards.Select(selector2));
			string text3 = string.Join("|", from entry in model.Metadata
			orderby entry.Key
			select entry.Key + "=" + entry.Value);
			return string.Join("::", new string[]
			{
				model.Title ?? string.Empty,
				text,
				text2,
				text3
			});
		}

		// Token: 0x06000142 RID: 322 RVA: 0x00007BA4 File Offset: 0x00005DA4
		private static string BuildCardSignature(PreviewCardSpec card)
		{
			if (card == null)
			{
				return string.Empty;
			}
			string text = string.Join(",", from entry in card.Attributes
			orderby entry.Key
			select string.Format("{0}:{1}", entry.Key, entry.Value));
			return string.Join(";", new string[]
			{
				card.TemplateId ?? string.Empty,
				card.SourceName ?? string.Empty,
				card.Tier.ToString(),
				card.Size.ToString(),
				card.Enchant ?? "None",
				text
			});
		}

		// Token: 0x02000071 RID: 113
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x040001C0 RID: 448
			public static Func<PreviewCardSpec, string> <0>__BuildCardSignature;
		}
	}
}
