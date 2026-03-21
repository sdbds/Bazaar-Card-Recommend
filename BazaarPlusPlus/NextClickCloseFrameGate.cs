using System;

namespace BazaarPlusPlus
{
	// Token: 0x02000035 RID: 53
	internal static class NextClickCloseFrameGate
	{
		// Token: 0x0600022E RID: 558 RVA: 0x0000E2D7 File Offset: 0x0000C4D7
		public static bool CanConsume(int armedFrame, int currentFrame)
		{
			return currentFrame > armedFrame;
		}
	}
}
