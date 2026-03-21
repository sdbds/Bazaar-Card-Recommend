using System;
using HarmonyLib;
using TheBazaar.UI.Tooltips;

namespace BazaarPlusPlus
{
	// Token: 0x02000046 RID: 70
	[HarmonyPatch(typeof(TooltipParentComponent), "IsCardTooltipControllerLocked")]
	internal static class ShowcaseTooltipLockBypassPatch
	{
		// Token: 0x0600026A RID: 618 RVA: 0x0000EED8 File Offset: 0x0000D0D8
		[HarmonyPrefix]
		private static bool Prefix(ref bool __result)
		{
			if (!ShowcaseTooltipBypass.Active)
			{
				return true;
			}
			__result = false;
			return false;
		}
	}
}
