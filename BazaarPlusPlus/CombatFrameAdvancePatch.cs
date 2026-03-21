using System;
using HarmonyLib;
using TheBazaar;

namespace BazaarPlusPlus
{
	// Token: 0x0200003E RID: 62
	[HarmonyPatch(typeof(FinalBlowSlowDownController), "Process")]
	internal class CombatFrameAdvancePatch
	{
		// Token: 0x0600025F RID: 607 RVA: 0x0000ECF5 File Offset: 0x0000CEF5
		[HarmonyPostfix]
		private static void Postfix()
		{
			CombatStatusBar.AdvanceCombatFrame();
		}
	}
}
