using System;
using System.Collections.Generic;

namespace BazaarPlusPlus
{
	// Token: 0x0200001F RID: 31
	internal sealed class PreviewBoardModel
	{
		// Token: 0x1700002F RID: 47
		// (get) Token: 0x0600010A RID: 266 RVA: 0x00007372 File Offset: 0x00005572
		// (set) Token: 0x0600010B RID: 267 RVA: 0x0000737A File Offset: 0x0000557A
		public IReadOnlyList<PreviewCardSpec> ItemCards { get; set; } = new List<PreviewCardSpec>();

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x0600010C RID: 268 RVA: 0x00007383 File Offset: 0x00005583
		// (set) Token: 0x0600010D RID: 269 RVA: 0x0000738B File Offset: 0x0000558B
		public IReadOnlyList<PreviewCardSpec> SkillCards { get; set; } = new List<PreviewCardSpec>();

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x0600010E RID: 270 RVA: 0x00007394 File Offset: 0x00005594
		// (set) Token: 0x0600010F RID: 271 RVA: 0x0000739C File Offset: 0x0000559C
		public string Title { get; set; } = string.Empty;

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x06000110 RID: 272 RVA: 0x000073A5 File Offset: 0x000055A5
		// (set) Token: 0x06000111 RID: 273 RVA: 0x000073AD File Offset: 0x000055AD
		public string Signature { get; set; } = string.Empty;

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x06000112 RID: 274 RVA: 0x000073B6 File Offset: 0x000055B6
		// (set) Token: 0x06000113 RID: 275 RVA: 0x000073BE File Offset: 0x000055BE
		public IReadOnlyDictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
	}
}
