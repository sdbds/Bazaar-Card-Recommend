using System;
using HarmonyLib;

namespace BazaarPlusPlus
{
	// Token: 0x02000045 RID: 69
	[HarmonyPatch(typeof(CardController), "ShowTooltips")]
	internal static class ShowcaseCardShowTooltipsPatch
	{
		// Token: 0x06000268 RID: 616 RVA: 0x0000EEB1 File Offset: 0x0000D0B1
		[HarmonyPrefix]
		private static void Prefix(CardController __instance)
		{
			if (__instance != null && __instance.GetComponent<ShowcaseCardMarker>() != null)
			{
				ShowcaseTooltipBypass.Active = true;
			}
		}

		// Token: 0x06000269 RID: 617 RVA: 0x0000EED0 File Offset: 0x0000D0D0
		[HarmonyPostfix]
		private static void Postfix()
		{
			ShowcaseTooltipBypass.Active = false;
		}
	}
}
