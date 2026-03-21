using System;
using UnityEngine;

namespace BazaarPlusPlus
{
	// Token: 0x02000020 RID: 32
	internal sealed class PreviewBoardPresentation
	{
		// Token: 0x17000034 RID: 52
		// (get) Token: 0x06000115 RID: 277 RVA: 0x00007406 File Offset: 0x00005606
		// (set) Token: 0x06000116 RID: 278 RVA: 0x0000740E File Offset: 0x0000560E
		public bool Visible { get; set; }

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x06000117 RID: 279 RVA: 0x00007417 File Offset: 0x00005617
		// (set) Token: 0x06000118 RID: 280 RVA: 0x0000741F File Offset: 0x0000561F
		public bool DebugEnabled { get; set; }

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x06000119 RID: 281 RVA: 0x00007428 File Offset: 0x00005628
		// (set) Token: 0x0600011A RID: 282 RVA: 0x00007430 File Offset: 0x00005630
		public Vector3 LocalOffset { get; set; } = Vector3.zero;

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x0600011B RID: 283 RVA: 0x00007439 File Offset: 0x00005639
		// (set) Token: 0x0600011C RID: 284 RVA: 0x00007441 File Offset: 0x00005641
		public Vector3 CardScale { get; set; } = Vector3.one;

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x0600011D RID: 285 RVA: 0x0000744A File Offset: 0x0000564A
		// (set) Token: 0x0600011E RID: 286 RVA: 0x00007452 File Offset: 0x00005652
		public Vector3 CardSpacing { get; set; } = new Vector3(1.1f, 0f, 0f);

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x0600011F RID: 287 RVA: 0x0000745B File Offset: 0x0000565B
		// (set) Token: 0x06000120 RID: 288 RVA: 0x00007463 File Offset: 0x00005663
		public Vector2 BoardSize { get; set; } = new Vector2(8.25f, 2.75f);

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x06000121 RID: 289 RVA: 0x0000746C File Offset: 0x0000566C
		// (set) Token: 0x06000122 RID: 290 RVA: 0x00007474 File Offset: 0x00005674
		public float SkillBoardWidth { get; set; } = 2.4f;

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x06000123 RID: 291 RVA: 0x0000747D File Offset: 0x0000567D
		// (set) Token: 0x06000124 RID: 292 RVA: 0x00007485 File Offset: 0x00005685
		public float BoardThickness { get; set; } = 0.04f;

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x06000125 RID: 293 RVA: 0x0000748E File Offset: 0x0000568E
		// (set) Token: 0x06000126 RID: 294 RVA: 0x00007496 File Offset: 0x00005696
		public float BorderThickness { get; set; } = 0.08f;

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x06000127 RID: 295 RVA: 0x0000749F File Offset: 0x0000569F
		// (set) Token: 0x06000128 RID: 296 RVA: 0x000074A7 File Offset: 0x000056A7
		public float BorderHeight { get; set; } = 0.06f;
	}
}
