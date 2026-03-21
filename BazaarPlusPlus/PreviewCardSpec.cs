using System;
using System.Collections.Generic;

namespace BazaarPlusPlus
{
	// Token: 0x02000024 RID: 36
	internal sealed class PreviewCardSpec
	{
		// Token: 0x17000044 RID: 68
		// (get) Token: 0x06000143 RID: 323 RVA: 0x00007C7F File Offset: 0x00005E7F
		// (set) Token: 0x06000144 RID: 324 RVA: 0x00007C87 File Offset: 0x00005E87
		public string TemplateId { get; set; } = string.Empty;

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x06000145 RID: 325 RVA: 0x00007C90 File Offset: 0x00005E90
		// (set) Token: 0x06000146 RID: 326 RVA: 0x00007C98 File Offset: 0x00005E98
		public string SourceName { get; set; } = string.Empty;

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x06000147 RID: 327 RVA: 0x00007CA1 File Offset: 0x00005EA1
		// (set) Token: 0x06000148 RID: 328 RVA: 0x00007CA9 File Offset: 0x00005EA9
		public int Tier { get; set; }

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x06000149 RID: 329 RVA: 0x00007CB2 File Offset: 0x00005EB2
		// (set) Token: 0x0600014A RID: 330 RVA: 0x00007CBA File Offset: 0x00005EBA
		public int Size { get; set; } = 1;

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x0600014B RID: 331 RVA: 0x00007CC3 File Offset: 0x00005EC3
		// (set) Token: 0x0600014C RID: 332 RVA: 0x00007CCB File Offset: 0x00005ECB
		public string Enchant { get; set; } = "None";

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x0600014D RID: 333 RVA: 0x00007CD4 File Offset: 0x00005ED4
		// (set) Token: 0x0600014E RID: 334 RVA: 0x00007CDC File Offset: 0x00005EDC
		public Dictionary<int, int> Attributes { get; set; } = new Dictionary<int, int>();
	}
}
