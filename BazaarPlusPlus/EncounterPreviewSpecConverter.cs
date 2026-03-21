using System;
using System.Collections.Generic;

namespace BazaarPlusPlus
{
	// Token: 0x02000011 RID: 17
	internal static class EncounterPreviewSpecConverter
	{
		// Token: 0x060000C0 RID: 192 RVA: 0x00006030 File Offset: 0x00004230
		internal static List<PreviewCardSpec> BuildCachedSpecs(List<RunInfo.MonsterPreviewCard> cards)
		{
			List<PreviewCardSpec> list = new List<PreviewCardSpec>();
			if (cards == null)
			{
				return list;
			}
			foreach (RunInfo.MonsterPreviewCard monsterPreviewCard in cards)
			{
				if (monsterPreviewCard != null && !string.IsNullOrWhiteSpace(monsterPreviewCard.TemplateId))
				{
					list.Add(new PreviewCardSpec
					{
						TemplateId = monsterPreviewCard.TemplateId,
						SourceName = (monsterPreviewCard.SourceName ?? string.Empty),
						Tier = monsterPreviewCard.Tier,
						Size = ((monsterPreviewCard.Size <= 0) ? 1 : monsterPreviewCard.Size),
						Enchant = (string.IsNullOrWhiteSpace(monsterPreviewCard.Enchant) ? "None" : monsterPreviewCard.Enchant),
						Attributes = ((monsterPreviewCard.Attributes != null) ? new Dictionary<int, int>(monsterPreviewCard.Attributes) : new Dictionary<int, int>())
					});
				}
			}
			return list;
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x00006130 File Offset: 0x00004330
		internal static List<RunInfo.MonsterPreviewCard> ToCachedCards(IEnumerable<PreviewCardSpec> specs)
		{
			List<RunInfo.MonsterPreviewCard> list = new List<RunInfo.MonsterPreviewCard>();
			if (specs == null)
			{
				return list;
			}
			foreach (PreviewCardSpec previewCardSpec in specs)
			{
				if (previewCardSpec != null && !string.IsNullOrWhiteSpace(previewCardSpec.TemplateId))
				{
					list.Add(new RunInfo.MonsterPreviewCard
					{
						TemplateId = previewCardSpec.TemplateId,
						SourceName = (previewCardSpec.SourceName ?? string.Empty),
						Tier = previewCardSpec.Tier,
						Size = ((previewCardSpec.Size <= 0) ? 1 : previewCardSpec.Size),
						Enchant = (string.IsNullOrWhiteSpace(previewCardSpec.Enchant) ? "None" : previewCardSpec.Enchant),
						Attributes = ((previewCardSpec.Attributes != null) ? new Dictionary<int, int>(previewCardSpec.Attributes) : new Dictionary<int, int>())
					});
				}
			}
			return list;
		}
	}
}
