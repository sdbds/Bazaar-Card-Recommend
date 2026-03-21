using System;
using System.Collections.Generic;
using System.Linq;

namespace BazaarPlusPlus
{
	// Token: 0x0200001A RID: 26
	internal sealed class InMemoryPreviewDataSource : IPreviewDataSource
	{
		// Token: 0x060000E4 RID: 228 RVA: 0x00006E41 File Offset: 0x00005041
		public void SetCards(IReadOnlyList<PreviewCardSpec> itemCards, IReadOnlyList<PreviewCardSpec> skillCards)
		{
			this._itemCards = InMemoryPreviewDataSource.CloneCards(itemCards);
			this._skillCards = InMemoryPreviewDataSource.CloneCards(skillCards);
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x00006E5B File Offset: 0x0000505B
		public void SetMetadata(string title, IReadOnlyDictionary<string, string> metadata = null)
		{
			this._title = (title ?? string.Empty);
			this._metadata = (metadata ?? new Dictionary<string, string>());
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x00006E80 File Offset: 0x00005080
		public bool TryBuild(out PreviewBoardModel model)
		{
			model = new PreviewBoardModel
			{
				Title = this._title,
				ItemCards = InMemoryPreviewDataSource.CloneCards(this._itemCards),
				SkillCards = InMemoryPreviewDataSource.CloneCards(this._skillCards),
				Metadata = new Dictionary<string, string>(this._metadata)
			};
			model.Signature = PreviewBoardSignature.Build(model);
			return true;
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x00006EE2 File Offset: 0x000050E2
		private static IReadOnlyList<PreviewCardSpec> CloneCards(IReadOnlyList<PreviewCardSpec> cards)
		{
			IReadOnlyList<PreviewCardSpec> readOnlyList;
			if (cards == null)
			{
				readOnlyList = null;
			}
			else
			{
				readOnlyList = (from card in cards
				select new PreviewCardSpec
				{
					TemplateId = card.TemplateId,
					Tier = card.Tier,
					SourceName = card.SourceName,
					Enchant = card.Enchant,
					Size = card.Size,
					Attributes = ((card.Attributes != null) ? new Dictionary<int, int>(card.Attributes) : new Dictionary<int, int>())
				}).ToList<PreviewCardSpec>();
			}
			return readOnlyList ?? new List<PreviewCardSpec>();
		}

		// Token: 0x04000071 RID: 113
		private IReadOnlyList<PreviewCardSpec> _itemCards = new List<PreviewCardSpec>();

		// Token: 0x04000072 RID: 114
		private IReadOnlyList<PreviewCardSpec> _skillCards = new List<PreviewCardSpec>();

		// Token: 0x04000073 RID: 115
		private IReadOnlyDictionary<string, string> _metadata = new Dictionary<string, string>();

		// Token: 0x04000074 RID: 116
		private string _title = string.Empty;
	}
}
