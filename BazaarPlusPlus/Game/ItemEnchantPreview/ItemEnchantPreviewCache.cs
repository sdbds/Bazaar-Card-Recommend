using System;
using System.Collections.Generic;
using System.Linq;
using BazaarGameShared.Domain.Core.Types;
using BazaarPlusPlus.Game.ItemEnchantPreview.Preview;
using TheBazaar.Tooltips;

namespace BazaarPlusPlus.Game.ItemEnchantPreview
{
	// Token: 0x0200004F RID: 79
	public static class ItemEnchantPreviewCache
	{
		// Token: 0x06000278 RID: 632 RVA: 0x0000F324 File Offset: 0x0000D524
		public static string CreateKey(ItemEnchantPreviewSnapshot snapshot)
		{
			string text = string.Join(",", from pair in snapshot.PreviewAttributes
			orderby pair.Key
			select string.Format("{0}:{1}", pair.Key, pair.Value));
			return string.Format("{0}|{1}|{2}|{3}|{4}|{5}", new object[]
			{
				snapshot.InstanceId,
				snapshot.TemplateId,
				snapshot.Section,
				snapshot.CurrentEnchantment,
				snapshot.PreviewEnchantment,
				text
			});
		}

		// Token: 0x06000279 RID: 633 RVA: 0x0000F3DC File Offset: 0x0000D5DC
		public static bool TryGet(ItemEnchantPreviewSnapshot snapshot, out List<TooltipSegment> segments)
		{
			string key = ItemEnchantPreviewCache.CreateKey(snapshot);
			Dictionary<string, ItemEnchantPreviewCache.CacheEntry> cache = ItemEnchantPreviewCache.Cache;
			lock (cache)
			{
				ItemEnchantPreviewCache.CacheEntry cacheEntry;
				if (ItemEnchantPreviewCache.Cache.TryGetValue(key, out cacheEntry))
				{
					if (DateTime.UtcNow - cacheEntry.CachedAtUtc <= ItemEnchantPreviewCache.CacheDuration)
					{
						segments = new List<TooltipSegment>(cacheEntry.Segments);
						return true;
					}
					ItemEnchantPreviewCache.Cache.Remove(key);
				}
			}
			segments = null;
			return false;
		}

		// Token: 0x0600027A RID: 634 RVA: 0x0000F46C File Offset: 0x0000D66C
		public static void Save(ItemEnchantPreviewSnapshot snapshot, List<TooltipSegment> segments)
		{
			string key = ItemEnchantPreviewCache.CreateKey(snapshot);
			Dictionary<string, ItemEnchantPreviewCache.CacheEntry> cache = ItemEnchantPreviewCache.Cache;
			lock (cache)
			{
				ItemEnchantPreviewCache.Cache[key] = new ItemEnchantPreviewCache.CacheEntry
				{
					CachedAtUtc = DateTime.UtcNow,
					Segments = new List<TooltipSegment>(segments)
				};
				if (ItemEnchantPreviewCache.Cache.Count > 256)
				{
					ItemEnchantPreviewCache.Cache.Clear();
				}
			}
		}

		// Token: 0x04000173 RID: 371
		private static readonly Dictionary<string, ItemEnchantPreviewCache.CacheEntry> Cache = new Dictionary<string, ItemEnchantPreviewCache.CacheEntry>();

		// Token: 0x04000174 RID: 372
		private static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(2.0);

		// Token: 0x0200008E RID: 142
		private sealed class CacheEntry
		{
			// Token: 0x04000269 RID: 617
			public DateTime CachedAtUtc;

			// Token: 0x0400026A RID: 618
			public List<TooltipSegment> Segments = new List<TooltipSegment>();
		}
	}
}
