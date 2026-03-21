using System;
using HarmonyLib;
using TheBazaar.UI.Tooltips;

namespace BazaarPlusPlus
{
	// Token: 0x02000048 RID: 72
	[HarmonyPatch(typeof(CardTooltipController), "DisableLockModeCanvas")]
	internal static class ShowcaseDisableLockCanvasPatch
	{
		// Token: 0x0600026D RID: 621 RVA: 0x0000EEFE File Offset: 0x0000D0FE
		[HarmonyPrefix]
		private static bool Prefix()
		{
			return !ShowcaseTooltipBypass.ShowingTooltip;
		}
	}
}
