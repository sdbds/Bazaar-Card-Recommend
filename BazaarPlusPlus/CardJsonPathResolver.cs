using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BepInEx;

namespace BazaarPlusPlus
{
	// Token: 0x02000006 RID: 6
	[NullableContext(2)]
	[Nullable(0)]
	internal static class CardJsonPathResolver
	{
		// Token: 0x06000006 RID: 6 RVA: 0x0000209D File Offset: 0x0000029D
		public static string GetCardsJsonPath()
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				return CardJsonPathResolver.GetCardsJsonPath("mac", Paths.GameRootPath);
			}
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				return CardJsonPathResolver.GetCardsJsonPath("windows", Paths.GameRootPath);
			}
			return null;
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000020D8 File Offset: 0x000002D8
		[NullableContext(1)]
		[return: Nullable(2)]
		internal static string GetCardsJsonPath(string platform)
		{
			return CardJsonPathResolver.GetCardsJsonPath(platform, null);
		}

		// Token: 0x06000008 RID: 8 RVA: 0x000020E4 File Offset: 0x000002E4
		internal static string GetCardsJsonPath([Nullable(1)] string platform, string gameRootPath)
		{
			if (string.IsNullOrWhiteSpace(gameRootPath))
			{
				return null;
			}
			if (string.Equals(platform, "mac", StringComparison.OrdinalIgnoreCase))
			{
				return Path.Combine(new string[]
				{
					gameRootPath,
					"TheBazaar.app",
					"Contents",
					"Resources",
					"Data",
					"StreamingAssets",
					"cards.json"
				});
			}
			if (string.Equals(platform, "windows", StringComparison.OrdinalIgnoreCase))
			{
				return Path.Combine(gameRootPath, "TheBazaar_Data", "StreamingAssets", "cards.json");
			}
			return null;
		}
	}
}
