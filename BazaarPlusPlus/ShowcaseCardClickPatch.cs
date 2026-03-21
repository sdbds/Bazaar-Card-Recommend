using System;
using HarmonyLib;
using UnityEngine.EventSystems;

namespace BazaarPlusPlus
{
	// Token: 0x02000044 RID: 68
	[HarmonyPatch(typeof(CardController), "ProceedClick")]
	internal static class ShowcaseCardClickPatch
	{
		// Token: 0x06000267 RID: 615 RVA: 0x0000EE95 File Offset: 0x0000D095
		[HarmonyPrefix]
		private static bool Prefix(CardController __instance, PointerEventData eventData)
		{
			return __instance == null || __instance.GetComponent<ShowcaseCardMarker>() == null;
		}
	}
}
