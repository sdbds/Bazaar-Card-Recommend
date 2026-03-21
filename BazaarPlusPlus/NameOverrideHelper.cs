using System;
using TheBazaar;
using TheBazaar.ProfileData;

namespace BazaarPlusPlus
{
	// Token: 0x02000040 RID: 64
	internal static class NameOverrideHelper
	{
		// Token: 0x06000263 RID: 611 RVA: 0x0000ED20 File Offset: 0x0000CF20
		public static bool TryGetReplacementName(string originalName, out string replacementName)
		{
			replacementName = null;
			if (!ModState.EnableNameOverrideConfig.Value)
			{
				return false;
			}
			IProfile profile = Data.Profile;
			string text = (profile != null) ? profile.Username : null;
			if (string.IsNullOrEmpty(text))
			{
				BppLog.Debug("NameOverride", "Skipping replacement because profile username is unavailable");
				return false;
			}
			if (originalName != text)
			{
				return false;
			}
			replacementName = "Anonymous";
			return true;
		}

		// Token: 0x0400016B RID: 363
		private const string ReplacementName = "Anonymous";
	}
}
