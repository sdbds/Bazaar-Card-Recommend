using System;

namespace BazaarPlusPlus
{
	// Token: 0x0200000C RID: 12
	internal sealed class MonsterSkillInfo
	{
		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000046 RID: 70 RVA: 0x0000300A File Offset: 0x0000120A
		// (set) Token: 0x06000047 RID: 71 RVA: 0x00003012 File Offset: 0x00001212
		public Guid SkillId { get; set; }

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000048 RID: 72 RVA: 0x0000301B File Offset: 0x0000121B
		// (set) Token: 0x06000049 RID: 73 RVA: 0x00003023 File Offset: 0x00001223
		public string Title { get; set; } = string.Empty;

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600004A RID: 74 RVA: 0x0000302C File Offset: 0x0000122C
		// (set) Token: 0x0600004B RID: 75 RVA: 0x00003034 File Offset: 0x00001234
		public string Tier { get; set; } = string.Empty;

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600004C RID: 76 RVA: 0x0000303D File Offset: 0x0000123D
		// (set) Token: 0x0600004D RID: 77 RVA: 0x00003045 File Offset: 0x00001245
		public string Type { get; set; } = string.Empty;
	}
}
