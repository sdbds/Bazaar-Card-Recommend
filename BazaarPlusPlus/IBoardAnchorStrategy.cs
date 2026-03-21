using System;

namespace BazaarPlusPlus
{
	// Token: 0x02000018 RID: 24
	internal interface IBoardAnchorStrategy
	{
		// Token: 0x060000E1 RID: 225
		bool TryResolve(out BoardPose pose);
	}
}
