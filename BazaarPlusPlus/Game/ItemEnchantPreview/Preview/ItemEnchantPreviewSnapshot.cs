using System;
using System.Collections.Generic;
using BazaarGameShared.Domain.Core.Types;

namespace BazaarPlusPlus.Game.ItemEnchantPreview.Preview
{
	// Token: 0x02000056 RID: 86
	public sealed class ItemEnchantPreviewSnapshot
	{
		// Token: 0x17000059 RID: 89
		// (get) Token: 0x0600028E RID: 654 RVA: 0x0000FD46 File Offset: 0x0000DF46
		// (set) Token: 0x0600028F RID: 655 RVA: 0x0000FD4E File Offset: 0x0000DF4E
		public string InstanceId { get; set; } = string.Empty;

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x06000290 RID: 656 RVA: 0x0000FD57 File Offset: 0x0000DF57
		// (set) Token: 0x06000291 RID: 657 RVA: 0x0000FD5F File Offset: 0x0000DF5F
		public string TemplateId { get; set; } = string.Empty;

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x06000292 RID: 658 RVA: 0x0000FD68 File Offset: 0x0000DF68
		// (set) Token: 0x06000293 RID: 659 RVA: 0x0000FD70 File Offset: 0x0000DF70
		public EInventorySection? Section { get; set; }

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x06000294 RID: 660 RVA: 0x0000FD79 File Offset: 0x0000DF79
		// (set) Token: 0x06000295 RID: 661 RVA: 0x0000FD81 File Offset: 0x0000DF81
		public EEnchantmentType? CurrentEnchantment { get; set; }

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x06000296 RID: 662 RVA: 0x0000FD8A File Offset: 0x0000DF8A
		// (set) Token: 0x06000297 RID: 663 RVA: 0x0000FD92 File Offset: 0x0000DF92
		public EEnchantmentType PreviewEnchantment { get; set; }

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x06000298 RID: 664 RVA: 0x0000FD9B File Offset: 0x0000DF9B
		// (set) Token: 0x06000299 RID: 665 RVA: 0x0000FDA3 File Offset: 0x0000DFA3
		public Dictionary<ECardAttributeType, int> PreviewAttributes { get; set; } = new Dictionary<ECardAttributeType, int>();
	}
}
