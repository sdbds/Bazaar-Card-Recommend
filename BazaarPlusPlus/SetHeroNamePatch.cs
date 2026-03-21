using System;
using HarmonyLib;
using TheBazaar;

namespace BazaarPlusPlus
{
	// Token: 0x02000042 RID: 66
	[HarmonyPatch(typeof(HeroBannerController), "SetHeroName")]
	public static class SetHeroNamePatch
	{
		// Token: 0x06000265 RID: 613 RVA: 0x0000EDB4 File Offset: 0x0000CFB4
		[HarmonyPrefix]
		private static bool Prefix(ref string newName, ref int usernameId)
		{
			string text;
			if (!NameOverrideHelper.TryGetReplacementName(newName, out text))
			{
				return true;
			}
			newName = text;
			usernameId = 0;
			BppLog.Debug("NameOverride", "SetHeroName replaced username with " + text);
			return true;
		}
	}
}
