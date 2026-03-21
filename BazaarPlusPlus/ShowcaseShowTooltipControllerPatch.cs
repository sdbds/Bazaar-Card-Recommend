using System;
using HarmonyLib;
using TheBazaar.UI.Tooltips;

namespace BazaarPlusPlus
{
	// Token: 0x02000047 RID: 71
	[HarmonyPatch(typeof(CardTooltipController), "ShowTooltipController")]
	internal static class ShowcaseShowTooltipControllerPatch
	{
		// Token: 0x0600026B RID: 619 RVA: 0x0000EEE7 File Offset: 0x0000D0E7
		[HarmonyPrefix]
		private static void Prefix()
		{
			if (ShowcaseTooltipBypass.Active)
			{
				ShowcaseTooltipBypass.ShowingTooltip = true;
			}
		}

		// Token: 0x0600026C RID: 620 RVA: 0x0000EEF6 File Offset: 0x0000D0F6
		[HarmonyPostfix]
		private static void Postfix()
		{
			ShowcaseTooltipBypass.ShowingTooltip = false;
		}
	}
}
