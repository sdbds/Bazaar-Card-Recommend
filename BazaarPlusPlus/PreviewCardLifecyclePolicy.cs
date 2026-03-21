using System;

namespace BazaarPlusPlus
{
	// Token: 0x02000030 RID: 48
	internal static class PreviewCardLifecyclePolicy
	{
		// Token: 0x060001FC RID: 508 RVA: 0x0000D153 File Offset: 0x0000B353
		public static bool ShouldReturnToPool(PreviewCardKind kind)
		{
			return false;
		}

		// Token: 0x060001FD RID: 509 RVA: 0x0000D156 File Offset: 0x0000B356
		public static bool ShouldRefreshAfterInstantiate(PreviewCardKind kind)
		{
			return kind == PreviewCardKind.Item || kind == PreviewCardKind.Skill;
		}
	}
}
