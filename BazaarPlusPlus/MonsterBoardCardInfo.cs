using System;

namespace BazaarPlusPlus
{
	// Token: 0x0200000B RID: 11
	internal sealed class MonsterBoardCardInfo
	{
		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600003B RID: 59 RVA: 0x00002F81 File Offset: 0x00001181
		// (set) Token: 0x0600003C RID: 60 RVA: 0x00002F89 File Offset: 0x00001189
		public Guid CardId { get; set; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600003D RID: 61 RVA: 0x00002F92 File Offset: 0x00001192
		// (set) Token: 0x0600003E RID: 62 RVA: 0x00002F9A File Offset: 0x0000119A
		public string Title { get; set; } = string.Empty;

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600003F RID: 63 RVA: 0x00002FA3 File Offset: 0x000011A3
		// (set) Token: 0x06000040 RID: 64 RVA: 0x00002FAB File Offset: 0x000011AB
		public string Tier { get; set; } = string.Empty;

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000041 RID: 65 RVA: 0x00002FB4 File Offset: 0x000011B4
		// (set) Token: 0x06000042 RID: 66 RVA: 0x00002FBC File Offset: 0x000011BC
		public string Size { get; set; } = string.Empty;

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000043 RID: 67 RVA: 0x00002FC5 File Offset: 0x000011C5
		// (set) Token: 0x06000044 RID: 68 RVA: 0x00002FCD File Offset: 0x000011CD
		public string Type { get; set; } = string.Empty;
	}
}
