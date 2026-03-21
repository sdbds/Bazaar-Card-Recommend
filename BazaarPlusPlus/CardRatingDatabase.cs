using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BazaarGameClient.Domain.Models.Cards;
using Newtonsoft.Json;

namespace BazaarPlusPlus
{
	public class CardRatingDatabase
	{
		// Token: 0x02000060 RID: 96
		private static CardRatingDatabase _instance;

		public static CardRatingDatabase Instance => _instance ?? (_instance = new CardRatingDatabase());

		// 以 img_guid (小写) 为键：可能与游戏内 TemplateId 对应
		private readonly Dictionary<string, CardRatingInfo> _byGuid = new Dictionary<string, CardRatingInfo>();

		// 以规范化名称为键（小写、去空格、去撇号）：用于兜底匹配
		private readonly Dictionary<string, CardRatingInfo> _byName = new Dictionary<string, CardRatingInfo>();

		private CardRatingDatabase()
		{
			Load();
		}

		private void Load()
		{
			try
			{
				Assembly assembly = Assembly.GetExecutingAssembly();
				const string resourceName = "BazaarPlusPlus.Data.jules_tierlist.json";
				Stream stream = assembly.GetManifestResourceStream(resourceName);
				if (stream == null)
				{
					BppLog.Warn("CardRatingDB", "Embedded resource not found: " + resourceName);
					return;
				}
				string json;
				using (StreamReader reader = new StreamReader(stream))
				{
					json = reader.ReadToEnd();
				}
				TierListData data = JsonConvert.DeserializeObject<TierListData>(json);
				if (data == null || data.Cards == null)
				{
					BppLog.Warn("CardRatingDB", "Failed to parse tier list JSON");
					return;
				}
				foreach (CardRatingInfo card in data.Cards)
				{
					if (!string.IsNullOrEmpty(card.ImgGuid))
					{
						_byGuid[card.ImgGuid.ToLowerInvariant()] = card;
					}
					if (!string.IsNullOrEmpty(card.Name))
					{
						_byName[NormalizeName(card.Name)] = card;
					}
				}
				BppLog.Info("CardRatingDB", string.Format("Loaded {0} card ratings for Jules", data.Cards.Count));
			}
			catch (Exception ex)
			{
				BppLog.Error("CardRatingDB", "Failed to load card ratings", ex);
			}
		}

		public bool TryGet(Card card, out CardRatingInfo info)
		{
			if (card == null)
			{
				info = null;
				return false;
			}

			// 优先用 TemplateId（GUID）匹配 img_guid
			string guidStr = card.TemplateId.ToString().ToLowerInvariant();
			if (_byGuid.TryGetValue(guidStr, out info))
			{
				return true;
			}

			// 兜底：用内部名称匹配（去掉空格和撇号后比较）
			string internalName = (card.Template != null) ? card.Template.InternalName : null;
			if (!string.IsNullOrEmpty(internalName))
			{
				string normalizedInternal = NormalizeName(internalName);
				if (_byName.TryGetValue(normalizedInternal, out info))
				{
					BppLog.Debug("CardRatingDB", string.Format("Matched by name: '{0}' -> {1}", internalName, info.Tier));
					return true;
				}
				// 调试：首次未匹配时记录，方便排查命名差异
				BppLog.Debug("CardRatingDB", string.Format("No rating match for TemplateId={0}, InternalName='{1}' (normalized='{2}')", guidStr, internalName, normalizedInternal));
			}

			info = null;
			return false;
		}

		public static string GetTierColor(string tier)
		{
			switch (tier)
			{
				case "S Tier": return "FFD700"; // 金色
				case "A Tier": return "7FEB59"; // 绿色
				case "B Tier": return "4FC3F7"; // 蓝色
				case "C Tier": return "AAAAAA"; // 灰色
				case "D Tier": return "FF9800"; // 橙色
				case "F Tier": return "F44336"; // 红色
				default:       return "FFFFFF";
			}
		}

		private static string NormalizeName(string name)
		{
			return name.ToLowerInvariant()
				.Replace(" ", "")
				.Replace("'", "")
				.Replace("-", "")
				.Replace("_", "");
		}

		private class TierListData
		{
			[JsonProperty("cards")]
			public List<CardRatingInfo> Cards { get; set; }
		}
	}
}
