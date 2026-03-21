using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace BazaarPlusPlus
{
	// Token: 0x02000025 RID: 37
	internal static class PreviewCardSpecFilter
	{
		// Token: 0x06000150 RID: 336 RVA: 0x00007D20 File Offset: 0x00005F20
		public static List<PreviewCardSpec> FilterLocallyRenderable(IEnumerable<PreviewCardSpec> specs)
		{
			Func<Guid, bool> hasTemplate;
			if ((hasTemplate = PreviewCardSpecFilter.<>O.<0>__Contains) == null)
			{
				hasTemplate = (PreviewCardSpecFilter.<>O.<0>__Contains = new Func<Guid, bool>(LocalCardTemplateCatalog.Contains));
			}
			return PreviewCardSpecFilter.Filter(specs, hasTemplate);
		}

		// Token: 0x06000151 RID: 337 RVA: 0x00007D44 File Offset: 0x00005F44
		internal static List<PreviewCardSpec> Filter(IEnumerable<PreviewCardSpec> specs, Func<Guid, bool> hasTemplate)
		{
			List<PreviewCardSpec> list = new List<PreviewCardSpec>();
			if (specs == null || hasTemplate == null)
			{
				return list;
			}
			foreach (PreviewCardSpec previewCardSpec in specs)
			{
				Guid arg;
				if (previewCardSpec != null && !string.IsNullOrWhiteSpace(previewCardSpec.TemplateId) && Guid.TryParse(previewCardSpec.TemplateId, out arg) && hasTemplate(arg))
				{
					list.Add(new PreviewCardSpec
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

		// Token: 0x02000073 RID: 115
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x040001C6 RID: 454
			public static Func<Guid, bool> <0>__Contains;
		}
	}
}
