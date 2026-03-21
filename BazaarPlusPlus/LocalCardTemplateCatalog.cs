using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Linq;

namespace BazaarPlusPlus
{
	// Token: 0x02000008 RID: 8
	[NullableContext(1)]
	[Nullable(0)]
	internal static class LocalCardTemplateCatalog
	{
		// Token: 0x06000012 RID: 18 RVA: 0x000025D8 File Offset: 0x000007D8
		public static bool Contains(Guid templateId)
		{
			return templateId != Guid.Empty && LocalCardTemplateCatalog.EnsureLoaded() && LocalCardTemplateCatalog._templateIds.Contains(templateId);
		}

		// Token: 0x06000013 RID: 19 RVA: 0x000025FB File Offset: 0x000007FB
		internal static bool Warm()
		{
			return LocalCardTemplateCatalog.EnsureLoaded();
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002604 File Offset: 0x00000804
		internal static void ResetForTests()
		{
			object syncRoot = LocalCardTemplateCatalog.SyncRoot;
			lock (syncRoot)
			{
				LocalCardTemplateCatalog._templateIds = new HashSet<Guid>();
				LocalCardTemplateCatalog._loadedPath = null;
			}
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00002650 File Offset: 0x00000850
		private static bool EnsureLoaded()
		{
			string cardsJsonPath = ModState.CardsJsonPath;
			if (string.IsNullOrWhiteSpace(cardsJsonPath) || !File.Exists(cardsJsonPath))
			{
				return false;
			}
			if (string.Equals(LocalCardTemplateCatalog._loadedPath, cardsJsonPath, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			object syncRoot = LocalCardTemplateCatalog.SyncRoot;
			bool result;
			lock (syncRoot)
			{
				if (string.Equals(LocalCardTemplateCatalog._loadedPath, cardsJsonPath, StringComparison.OrdinalIgnoreCase))
				{
					result = true;
				}
				else
				{
					try
					{
						LocalCardTemplateCatalog._templateIds = LocalCardTemplateCatalog.LoadTemplateIds(cardsJsonPath);
						LocalCardTemplateCatalog._loadedPath = cardsJsonPath;
						result = true;
					}
					catch (Exception ex)
					{
						BppLog.Error("LocalCardTemplateCatalog", "Failed to load local card template catalog from '" + cardsJsonPath + "'", ex);
						result = false;
					}
				}
			}
			return result;
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00002708 File Offset: 0x00000908
		private static HashSet<Guid> LoadTemplateIds(string path)
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
				return new HashSet<Guid>();
			}
			HashSet<Guid> hashSet = new HashSet<Guid>();
			using (IEnumerator<JObject> enumerator = jarray2.OfType<JObject>().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Guid item;
					if (Guid.TryParse(enumerator.Current.Value<string>("Id"), out item))
					{
						hashSet.Add(item);
					}
				}
			}
			return hashSet;
		}

		// Token: 0x04000008 RID: 8
		private static readonly object SyncRoot = new object();

		// Token: 0x04000009 RID: 9
		private static HashSet<Guid> _templateIds = new HashSet<Guid>();

		// Token: 0x0400000A RID: 10
		[Nullable(2)]
		private static string _loadedPath;
	}
}
