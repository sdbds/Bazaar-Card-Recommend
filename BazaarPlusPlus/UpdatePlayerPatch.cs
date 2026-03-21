using System;
using HarmonyLib;
using TheBazaar;
using TheBazaar.ProfileData;

namespace BazaarPlusPlus
{
	// Token: 0x02000041 RID: 65
	[HarmonyPatch(typeof(HeroBannerController), "UpdatePlayer")]
	public static class UpdatePlayerPatch
	{
		// Token: 0x06000264 RID: 612 RVA: 0x0000ED7C File Offset: 0x0000CF7C
		[HarmonyPrefix]
		private static bool Prefix(HeroBannerController __instance, ref string userName, ref int nameId, ref string titlePrefix, ISeasonRank currentSeasonRank, int? leaderboardPosition)
		{
			string text;
			if (!NameOverrideHelper.TryGetReplacementName(userName, out text))
			{
				return true;
			}
			userName = text;
			nameId = 0;
			BppLog.Debug("NameOverride", "UpdatePlayer replaced username with " + text);
			return true;
		}
	}
}
