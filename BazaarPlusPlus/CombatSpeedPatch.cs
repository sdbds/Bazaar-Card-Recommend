using System;
using HarmonyLib;
using TheBazaar;

namespace BazaarPlusPlus
{
	// Token: 0x0200003F RID: 63
	[HarmonyPatch(typeof(CombatSimHandler), "SetSpeed")]
	internal class CombatSpeedPatch
	{
		// Token: 0x06000261 RID: 609 RVA: 0x0000ED04 File Offset: 0x0000CF04
		[HarmonyPrefix]
		private static void Prefix(ref float speed)
		{
			if (!CombatStatusBar.IsCombatPlaybackActive)
			{
				return;
			}
			speed = CombatStatusBar.CombatSpeedMultiplier;
		}
	}
}
