using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Linq;

namespace BazaarPlusPlus
{
	// Token: 0x02000007 RID: 7
	[NullableContext(1)]
	[Nullable(0)]
	internal static class ItemAttr
	{
		// Token: 0x06000009 RID: 9 RVA: 0x00002170 File Offset: 0x00000370
		public static IReadOnlyDictionary<string, int> GetAttributes(Guid templateId, string tier)
		{
			if (!ItemAttr.EnsureLoaded())
			{
				return new Dictionary<string, int>();
			}
			ItemAttr.CardAttributes cardAttributes;
			if (!ItemAttr._cardsByTemplateId.TryGetValue(templateId, out cardAttributes))
			{
				return new Dictionary<string, int>();
			}
			string text = ItemAttr.NormalizeTier(tier);
			if (text == null)
			{
				return new Dictionary<string, int>();
			}
			Dictionary<string, int> dictionary = new Dictionary<string, int>(StringComparer.Ordinal);
			foreach (string text2 in ItemAttr.TierOrder)
			{
				if (ItemAttr.CompareTier(text2, text) > 0)
				{
					break;
				}
				IReadOnlyDictionary<string, int> readOnlyDictionary;
				if (cardAttributes.AttributesByTier.TryGetValue(text2, out readOnlyDictionary))
				{
					foreach (KeyValuePair<string, int> keyValuePair in readOnlyDictionary)
					{
						dictionary[keyValuePair.Key] = keyValuePair.Value;
					}
				}
			}
			return dictionary;
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002244 File Offset: 0x00000444
		internal static bool Warm()
		{
			return ItemAttr.EnsureLoaded();
		}

		// Token: 0x0600000B RID: 11 RVA: 0x0000224C File Offset: 0x0000044C
		internal static void ResetForTests()
		{
			object syncRoot = ItemAttr.SyncRoot;
			lock (syncRoot)
			{
				ItemAttr._cardsByTemplateId = new Dictionary<Guid, ItemAttr.CardAttributes>();
				ItemAttr._loadedPath = null;
			}
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00002298 File Offset: 0x00000498
		private static bool EnsureLoaded()
		{
			string cardsJsonPath = ModState.CardsJsonPath;
			if (string.IsNullOrWhiteSpace(cardsJsonPath) || !File.Exists(cardsJsonPath))
			{
				return false;
			}
			if (string.Equals(ItemAttr._loadedPath, cardsJsonPath, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			object syncRoot = ItemAttr.SyncRoot;
			bool result;
			lock (syncRoot)
			{
				if (string.Equals(ItemAttr._loadedPath, cardsJsonPath, StringComparison.OrdinalIgnoreCase))
				{
					result = true;
				}
				else
				{
					try
					{
						ItemAttr._cardsByTemplateId = ItemAttr.LoadCards(cardsJsonPath);
						ItemAttr._loadedPath = cardsJsonPath;
						result = true;
					}
					catch (Exception ex)
					{
						BppLog.Error("ItemAttr", "Failed to load card attributes from '" + cardsJsonPath + "'", ex);
						result = false;
					}
				}
			}
			return result;
		}

		// Token: 0x0600000D RID: 13 RVA: 0x00002350 File Offset: 0x00000550
		private static IReadOnlyDictionary<Guid, ItemAttr.CardAttributes> LoadCards(string path)
		{
			JObject jobject = JObject.Parse(File.ReadAllText(path));
			JArray jarray;
			if ((jarray = (jobject["5.0.0"] as JArray)) == null)
			{
				JProperty jproperty = jobject.Properties().FirstOrDefault<JProperty>();
				jarray = (((jproperty != null) ? jproperty.Value : null) as JArray);
			}
			JArray jarray2 = jarray;
			if (jarray2 == null)
			{
				return new Dictionary<Guid, ItemAttr.CardAttributes>();
			}
			Dictionary<Guid, ItemAttr.CardAttributes> dictionary = new Dictionary<Guid, ItemAttr.CardAttributes>();
			foreach (JObject jobject2 in jarray2.OfType<JObject>())
			{
				Guid key;
				if (Guid.TryParse(jobject2.Value<string>("Id"), out key))
				{
					Dictionary<string, IReadOnlyDictionary<string, int>> dictionary2 = new Dictionary<string, IReadOnlyDictionary<string, int>>(StringComparer.Ordinal);
					JObject jobject3 = jobject2["Tiers"] as JObject;
					if (jobject3 != null)
					{
						foreach (JProperty jproperty2 in jobject3.Properties())
						{
							string text = ItemAttr.NormalizeTier(jproperty2.Name);
							if (text != null)
							{
								JObject attributesObject = jproperty2.Value["Attributes"] as JObject;
								dictionary2[text] = ItemAttr.ParseAttributes(attributesObject);
							}
						}
					}
					dictionary[key] = new ItemAttr.CardAttributes
					{
						AttributesByTier = dictionary2
					};
				}
			}
			return dictionary;
		}

		// Token: 0x0600000E RID: 14 RVA: 0x000024B0 File Offset: 0x000006B0
		private static IReadOnlyDictionary<string, int> ParseAttributes([Nullable(2)] JObject attributesObject)
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>(StringComparer.Ordinal);
			if (attributesObject == null)
			{
				return dictionary;
			}
			foreach (JProperty jproperty in attributesObject.Properties())
			{
				if (jproperty.Value.Type == JTokenType.Integer)
				{
					dictionary[jproperty.Name] = jproperty.Value.Value<int>();
				}
			}
			return dictionary;
		}

		// Token: 0x0600000F RID: 15 RVA: 0x0000252C File Offset: 0x0000072C
		[NullableContext(2)]
		private static string NormalizeTier(string tier)
		{
			if (string.IsNullOrWhiteSpace(tier))
			{
				return null;
			}
			return ItemAttr.TierOrder.FirstOrDefault((string candidate) => string.Equals(candidate, tier.Trim(), StringComparison.OrdinalIgnoreCase));
		}

		// Token: 0x06000010 RID: 16 RVA: 0x0000256B File Offset: 0x0000076B
		private static int CompareTier(string left, string right)
		{
			return Array.IndexOf<string>(ItemAttr.TierOrder, left) - Array.IndexOf<string>(ItemAttr.TierOrder, right);
		}

		// Token: 0x04000004 RID: 4
		private static readonly string[] TierOrder = new string[]
		{
			"Bronze",
			"Silver",
			"Gold",
			"Diamond",
			"Legendary"
		};

		// Token: 0x04000005 RID: 5
		private static readonly object SyncRoot = new object();

		// Token: 0x04000006 RID: 6
		private static IReadOnlyDictionary<Guid, ItemAttr.CardAttributes> _cardsByTemplateId = new Dictionary<Guid, ItemAttr.CardAttributes>();

		// Token: 0x04000007 RID: 7
		[Nullable(2)]
		private static string _loadedPath;

		// Token: 0x0200005A RID: 90
		[Nullable(0)]
		private sealed class CardAttributes
		{
			// Token: 0x1700005F RID: 95
			// (get) Token: 0x0600029D RID: 669 RVA: 0x0000FEBC File Offset: 0x0000E0BC
			// (set) Token: 0x0600029E RID: 670 RVA: 0x0000FEC4 File Offset: 0x0000E0C4
			public IReadOnlyDictionary<string, IReadOnlyDictionary<string, int>> AttributesByTier { get; set; } = new Dictionary<string, IReadOnlyDictionary<string, int>>(StringComparer.Ordinal);
		}
	}
}
