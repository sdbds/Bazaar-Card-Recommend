using System;

namespace BazaarPlusPlus
{
	// Token: 0x02000034 RID: 52
	internal sealed class MonsterLockShowcaseController
	{
		// Token: 0x0600022A RID: 554 RVA: 0x0000E29F File Offset: 0x0000C49F
		public bool ShouldConsumeNextClickToClosePreview(bool isPreviewActive, bool closeOnNextClickArmed, bool isLeftClick, bool isRightClick)
		{
			return isPreviewActive && closeOnNextClickArmed && (isLeftClick || isRightClick);
		}

		// Token: 0x0600022B RID: 555 RVA: 0x0000E2AD File Offset: 0x0000C4AD
		public bool ShouldInterceptLockToggle(bool isPreviewActive, bool hasCurrentCard, bool isShowcaseCard, bool isMonsterCard)
		{
			if (!hasCurrentCard)
			{
				return false;
			}
			if (isShowcaseCard)
			{
				return isPreviewActive;
			}
			return isMonsterCard;
		}

		// Token: 0x0600022C RID: 556 RVA: 0x0000E2BB File Offset: 0x0000C4BB
		public bool ShouldShowForLock(Guid? lockedCardId, bool isShowcaseCard, bool isMonsterCard)
		{
			return lockedCardId != null && !isShowcaseCard && isMonsterCard;
		}
	}
}
