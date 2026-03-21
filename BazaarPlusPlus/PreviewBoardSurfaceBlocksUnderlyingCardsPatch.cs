using System;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace BazaarPlusPlus
{
	// Token: 0x02000043 RID: 67
	[HarmonyPatch(typeof(CardController), "IsPointerOverThis")]
	internal static class PreviewBoardSurfaceBlocksUnderlyingCardsPatch
	{
		// Token: 0x06000266 RID: 614 RVA: 0x0000EDEC File Offset: 0x0000CFEC
		[HarmonyPostfix]
		private static void Postfix(CardController __instance, Vector2 screenPos, ref bool __result)
		{
			if (!__result || __instance == null || Camera.main == null)
			{
				return;
			}
			if (__instance.GetComponent<ShowcaseCardMarker>() != null)
			{
				return;
			}
			RaycastHit[] array = (from hit in Physics.RaycastAll(Camera.main.ScreenPointToRay(screenPos), float.MaxValue)
			orderby hit.distance
			select hit).ToArray<RaycastHit>();
			if (array.Length == 0)
			{
				return;
			}
			Transform transform = array[0].transform;
			if (transform == null)
			{
				return;
			}
			if (transform.GetComponentInParent<PreviewBoardSurfaceMarker>() == null)
			{
				return;
			}
			__result = false;
		}
	}
}
