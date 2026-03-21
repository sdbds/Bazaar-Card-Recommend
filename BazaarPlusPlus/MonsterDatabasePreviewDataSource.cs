using System;
using System.Collections.Generic;
using BazaarGameShared.Domain.Core.Types;

namespace BazaarPlusPlus
{
	// Token: 0x02000027 RID: 39
	internal sealed class MonsterDatabasePreviewDataSource : IPreviewDataSource
	{
		// Token: 0x06000158 RID: 344 RVA: 0x00007EEC File Offset: 0x000060EC
		public MonsterDatabasePreviewDataSource(string encounterId)
		{
			this._encounterId = (encounterId ?? string.Empty);
		}

		// Token: 0x06000159 RID: 345 RVA: 0x00007F04 File Offset: 0x00006104
		public bool TryBuild(out PreviewBoardModel model)
		{
			model = null;
			MonsterInfo monster;
			if (!MonsterDatabase.TryGetByEncounterId(this._encounterId, out monster))
			{
				return false;
			}
			model = MonsterDatabasePreviewDataSource.BuildModel(monster, "monster_db");
			return model.ItemCards.Count > 0 || model.SkillCards.Count > 0;
		}

		// Token: 0x0600015A RID: 346 RVA: 0x00007F54 File Offset: 0x00006154
		public static PreviewBoardModel BuildModel(MonsterInfo monster, string source)
		{
			List<PreviewCardSpec> itemCards = MonsterDatabasePreviewDataSource.BuildItemCards(monster);
			List<PreviewCardSpec> skillCards = MonsterDatabasePreviewDataSource.BuildSkillCards(monster);
			PreviewBoardModel previewBoardModel = new PreviewBoardModel();
			previewBoardModel.Title = (((monster != null) ? monster.Title : null) ?? string.Empty);
			previewBoardModel.ItemCards = itemCards;
			previewBoardModel.SkillCards = skillCards;
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["source"] = (source ?? "monster_db");
			dictionary["encounter"] = (((monster != null) ? monster.EncounterShortId : null) ?? string.Empty);
			string key = "health";
			string text;
			if (monster == null)
			{
				text = null;
			}
			else
			{
				int? num = monster.Health;
				text = ((num != null) ? num.GetValueOrDefault().ToString() : null);
			}
			dictionary[key] = (text ?? string.Empty);
			string key2 = "reward_gold";
			string text2;
			if (monster == null)
			{
				text2 = null;
			}
			else
			{
				int? num = monster.RewardGold;
				text2 = ((num != null) ? num.GetValueOrDefault().ToString() : null);
			}
			dictionary[key2] = (text2 ?? string.Empty);
			string key3 = "reward_xp";
			string text3;
			if (monster == null)
			{
				text3 = null;
			}
			else
			{
				int? num = monster.RewardXp;
				text3 = ((num != null) ? num.GetValueOrDefault().ToString() : null);
			}
			dictionary[key3] = (text3 ?? string.Empty);
			previewBoardModel.Metadata = dictionary;
			previewBoardModel.Signature = PreviewBoardSignature.Build(previewBoardModel);
			return previewBoardModel;
		}

		// Token: 0x0600015B RID: 347 RVA: 0x000080A0 File Offset: 0x000062A0
		private static List<PreviewCardSpec> BuildItemCards(MonsterInfo monster)
		{
			List<PreviewCardSpec> list = new List<PreviewCardSpec>();
			if (((monster != null) ? monster.BoardCards : null) == null)
			{
				return list;
			}
			foreach (MonsterBoardCardInfo monsterBoardCardInfo in monster.BoardCards)
			{
				if (monsterBoardCardInfo != null && !(monsterBoardCardInfo.CardId == Guid.Empty))
				{
					list.Add(new PreviewCardSpec
					{
						TemplateId = monsterBoardCardInfo.CardId.ToString(),
						Tier = MonsterDatabasePreviewDataSource.ParseTier(monsterBoardCardInfo.Tier),
						Size = MonsterDatabasePreviewDataSource.ParseSize(monsterBoardCardInfo.Size),
						Enchant = "None",
						Attributes = MonsterDatabasePreviewDataSource.BuildAttributes(monsterBoardCardInfo.CardId, monsterBoardCardInfo.Tier)
					});
				}
			}
			return list;
		}

		// Token: 0x0600015C RID: 348 RVA: 0x00008184 File Offset: 0x00006384
		private static List<PreviewCardSpec> BuildSkillCards(MonsterInfo monster)
		{
			List<PreviewCardSpec> list = new List<PreviewCardSpec>();
			if (((monster != null) ? monster.Skills : null) == null)
			{
				return list;
			}
			foreach (MonsterSkillInfo monsterSkillInfo in monster.Skills)
			{
				if (monsterSkillInfo != null && !(monsterSkillInfo.SkillId == Guid.Empty))
				{
					list.Add(new PreviewCardSpec
					{
						TemplateId = monsterSkillInfo.SkillId.ToString(),
						Tier = MonsterDatabasePreviewDataSource.ParseTier(monsterSkillInfo.Tier),
						SourceName = (monsterSkillInfo.Title ?? string.Empty),
						Size = 1,
						Enchant = "None",
						Attributes = MonsterDatabasePreviewDataSource.BuildAttributes(monsterSkillInfo.SkillId, monsterSkillInfo.Tier)
					});
				}
			}
			return list;
		}

		// Token: 0x0600015D RID: 349 RVA: 0x00008274 File Offset: 0x00006474
		private static int ParseTier(string tier)
		{
			if (string.IsNullOrWhiteSpace(tier))
			{
				return 0;
			}
			string a = tier.Trim().ToLowerInvariant();
			if (a == "bronze")
			{
				return 0;
			}
			if (a == "silver")
			{
				return 1;
			}
			if (a == "gold")
			{
				return 2;
			}
			if (a == "diamond")
			{
				return 3;
			}
			if (!(a == "legendary"))
			{
				return 0;
			}
			return 4;
		}

		// Token: 0x0600015E RID: 350 RVA: 0x000082E8 File Offset: 0x000064E8
		private static int ParseSize(string size)
		{
			if (string.IsNullOrWhiteSpace(size))
			{
				return 1;
			}
			string a = size.Trim().ToLowerInvariant();
			if (a == "small")
			{
				return 1;
			}
			if (a == "medium")
			{
				return 2;
			}
			if (!(a == "large"))
			{
				return 1;
			}
			return 3;
		}

		// Token: 0x0600015F RID: 351 RVA: 0x0000833C File Offset: 0x0000653C
		private static Dictionary<int, int> BuildAttributes(Guid templateId, string tier)
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			foreach (KeyValuePair<string, int> keyValuePair in ItemAttr.GetAttributes(templateId, tier))
			{
				ECardAttributeType key;
				if (Enum.TryParse<ECardAttributeType>(keyValuePair.Key, out key))
				{
					dictionary[(int)key] = keyValuePair.Value;
				}
			}
			return dictionary;
		}

		// Token: 0x040000A7 RID: 167
		private readonly string _encounterId;
	}
}
