using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using BepInEx;
using Newtonsoft.Json;

namespace BazaarPlusPlus
{
	// Token: 0x02000009 RID: 9
	internal static class MonsterDatabase
	{
		// Token: 0x06000018 RID: 24 RVA: 0x000027D0 File Offset: 0x000009D0
		public static void Load()
		{
			try
			{
				string text;
				string value = MonsterDatabase.ReadDatabaseJson(out text);
				if (string.IsNullOrWhiteSpace(value))
				{
					BppLog.Warn("MonsterDatabase", "Monster database JSON was empty");
					MonsterDatabase._db.Clear();
					MonsterDatabase._dbByShortEncounterId.Clear();
				}
				else
				{
					Dictionary<string, MonsterDatabase.MonsterRecordDto> dictionary = JsonConvert.DeserializeObject<Dictionary<string, MonsterDatabase.MonsterRecordDto>>(value) ?? new Dictionary<string, MonsterDatabase.MonsterRecordDto>();
					MonsterDatabase._db.Clear();
					MonsterDatabase._dbByShortEncounterId.Clear();
					int num = 0;
					foreach (KeyValuePair<string, MonsterDatabase.MonsterRecordDto> keyValuePair in dictionary)
					{
						Guid guid;
						if (!Guid.TryParse(keyValuePair.Key, out guid))
						{
							num++;
						}
						else
						{
							MonsterInfo monsterInfo = MonsterDatabase.MapMonster(guid, keyValuePair.Value);
							if (monsterInfo != null)
							{
								MonsterDatabase._db[guid] = monsterInfo;
								MonsterDatabase._dbByShortEncounterId[MonsterDatabase.GetShortEncounterId(keyValuePair.Key)] = monsterInfo;
							}
						}
					}
					BppLog.Info("MonsterDatabase", string.Format("Loaded {0} entries from {1} invalidEncounterIds={2} shortKeys={3}", new object[]
					{
						MonsterDatabase._db.Count,
						text,
						num,
						MonsterDatabase._dbByShortEncounterId.Count
					}));
				}
			}
			catch (Exception ex)
			{
				BppLog.Error("MonsterDatabase", "Failed to load monster database", ex);
				MonsterDatabase._db.Clear();
				MonsterDatabase._dbByShortEncounterId.Clear();
			}
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002960 File Offset: 0x00000B60
		public static bool TryGetByEncounterId(Guid encounterId, out MonsterInfo monster)
		{
			bool flag = MonsterDatabase._db.TryGetValue(encounterId, out monster);
			BppLog.Debug("MonsterDatabase", string.Format("Lookup encounterId={0} found={1}", encounterId, flag));
			return flag;
		}

		// Token: 0x0600001A RID: 26 RVA: 0x0000299C File Offset: 0x00000B9C
		public static bool TryGetByEncounterId(string encounterId, out MonsterInfo monster)
		{
			monster = null;
			if (string.IsNullOrWhiteSpace(encounterId))
			{
				BppLog.Debug("MonsterDatabase", "Lookup encounterId=<empty> found=False");
				return false;
			}
			Guid encounterId2;
			return (Guid.TryParse(encounterId, out encounterId2) && MonsterDatabase.TryGetByEncounterId(encounterId2, out monster)) || MonsterDatabase.TryGetByEncounterIdPrefix(encounterId, out monster);
		}

		// Token: 0x0600001B RID: 27 RVA: 0x000029E4 File Offset: 0x00000BE4
		public static bool TryGetByEncounterIdPrefix(string encounterIdPrefix, out MonsterInfo monster)
		{
			monster = null;
			string shortEncounterId = MonsterDatabase.GetShortEncounterId(encounterIdPrefix);
			bool flag = !string.IsNullOrWhiteSpace(shortEncounterId) && MonsterDatabase._dbByShortEncounterId.TryGetValue(shortEncounterId, out monster);
			BppLog.Debug("MonsterDatabase", string.Format("Lookup encounterIdPrefix={0} normalized={1} found={2}", encounterIdPrefix, shortEncounterId, flag));
			return flag;
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00002A30 File Offset: 0x00000C30
		public static MonsterDatabase.LegacyMonsterEntry TryGet(string encounterInternalName)
		{
			return null;
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002A33 File Offset: 0x00000C33
		public static IReadOnlyCollection<MonsterInfo> GetAll()
		{
			return MonsterDatabase._db.Values.ToList<MonsterInfo>();
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00002A44 File Offset: 0x00000C44
		private static string ReadDatabaseJson(out string source)
		{
			using (Stream manifestResourceStream = typeof(MonsterDatabase).Assembly.GetManifestResourceStream("BazaarPlusPlus.Data.monsters_bazaardb.json"))
			{
				if (manifestResourceStream != null)
				{
					using (StreamReader streamReader = new StreamReader(manifestResourceStream))
					{
						source = "embedded:BazaarPlusPlus.Data.monsters_bazaardb.json";
						return streamReader.ReadToEnd();
					}
				}
			}
			string text = Path.Combine(Paths.PluginPath, "monsters_bazaardb.json");
			if (File.Exists(text))
			{
				source = text;
				return File.ReadAllText(text);
			}
			throw new FileNotFoundException("Monster database not found in embedded resource 'BazaarPlusPlus.Data.monsters_bazaardb.json' or plugin path '" + text + "'");
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00002AF0 File Offset: 0x00000CF0
		private static MonsterInfo MapMonster(Guid encounterId, MonsterDatabase.MonsterRecordDto dto)
		{
			if (dto == null)
			{
				return null;
			}
			MonsterDatabase.MonsterMetadataDto monsterMetadata = dto.MonsterMetadata;
			List<MonsterBoardCardInfo> list;
			if (monsterMetadata == null)
			{
				list = null;
			}
			else
			{
				List<MonsterDatabase.MonsterBoardCardDto> board = monsterMetadata.Board;
				if (board == null)
				{
					list = null;
				}
				else
				{
					Func<MonsterDatabase.MonsterBoardCardDto, MonsterBoardCardInfo> selector;
					if ((selector = MonsterDatabase.<>O.<0>__MapBoardCard) == null)
					{
						selector = (MonsterDatabase.<>O.<0>__MapBoardCard = new Func<MonsterDatabase.MonsterBoardCardDto, MonsterBoardCardInfo>(MonsterDatabase.MapBoardCard));
					}
					list = (from card in board.Select(selector)
					where card != null
					select card).ToList<MonsterBoardCardInfo>();
				}
			}
			List<MonsterBoardCardInfo> boardCards = list ?? new List<MonsterBoardCardInfo>();
			MonsterDatabase.MonsterMetadataDto monsterMetadata2 = dto.MonsterMetadata;
			List<MonsterSkillInfo> list2;
			if (monsterMetadata2 == null)
			{
				list2 = null;
			}
			else
			{
				List<MonsterDatabase.MonsterSkillDto> skills = monsterMetadata2.Skills;
				if (skills == null)
				{
					list2 = null;
				}
				else
				{
					Func<MonsterDatabase.MonsterSkillDto, MonsterSkillInfo> selector2;
					if ((selector2 = MonsterDatabase.<>O.<1>__MapSkill) == null)
					{
						selector2 = (MonsterDatabase.<>O.<1>__MapSkill = new Func<MonsterDatabase.MonsterSkillDto, MonsterSkillInfo>(MonsterDatabase.MapSkill));
					}
					list2 = (from skill in skills.Select(selector2)
					where skill != null
					select skill).ToList<MonsterSkillInfo>();
				}
			}
			List<MonsterSkillInfo> skills2 = list2 ?? new List<MonsterSkillInfo>();
			MonsterInfo monsterInfo = new MonsterInfo();
			monsterInfo.EncounterId = encounterId;
			monsterInfo.EncounterKey = (dto.EncounterId ?? encounterId.ToString());
			monsterInfo.EncounterShortId = MonsterDatabase.GetShortEncounterId(dto.EncounterId ?? encounterId.ToString());
			monsterInfo.Title = (dto.Title ?? string.Empty);
			monsterInfo.BaseTier = (dto.BaseTier ?? string.Empty);
			MonsterDatabase.MonsterCombatantDto combatant = dto.Combatant;
			monsterInfo.CombatLevel = ((combatant != null) ? combatant.Level : null);
			MonsterDatabase.MonsterMetadataDto monsterMetadata3 = dto.MonsterMetadata;
			monsterInfo.Health = ((monsterMetadata3 != null) ? monsterMetadata3.Health : null);
			MonsterDatabase.MonsterRewardsDto rewards = dto.Rewards;
			monsterInfo.RewardGold = ((rewards != null) ? rewards.Gold : null);
			MonsterDatabase.MonsterRewardsDto rewards2 = dto.Rewards;
			monsterInfo.RewardXp = ((rewards2 != null) ? rewards2.Xp : null);
			monsterInfo.BoardCards = boardCards;
			monsterInfo.Skills = skills2;
			return monsterInfo;
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002CE0 File Offset: 0x00000EE0
		private static MonsterBoardCardInfo MapBoardCard(MonsterDatabase.MonsterBoardCardDto dto)
		{
			Guid cardId;
			if (dto == null || !Guid.TryParse(dto.CardId, out cardId))
			{
				BppLog.Debug("MonsterDatabase", "Dropping invalid board card id=" + (((dto != null) ? dto.CardId : null) ?? "null"));
				return null;
			}
			return new MonsterBoardCardInfo
			{
				CardId = cardId,
				Title = (dto.Title ?? string.Empty),
				Tier = (dto.Tier ?? string.Empty),
				Size = (dto.Size ?? string.Empty),
				Type = (dto.Type ?? string.Empty)
			};
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00002D8C File Offset: 0x00000F8C
		private static MonsterSkillInfo MapSkill(MonsterDatabase.MonsterSkillDto dto)
		{
			Guid skillId;
			if (dto == null || !Guid.TryParse(dto.SkillId, out skillId))
			{
				BppLog.Debug("MonsterDatabase", "Dropping invalid skill id=" + (((dto != null) ? dto.SkillId : null) ?? "null"));
				return null;
			}
			return new MonsterSkillInfo
			{
				SkillId = skillId,
				Title = (dto.Title ?? string.Empty),
				Tier = (dto.Tier ?? string.Empty),
				Type = (dto.Type ?? string.Empty)
			};
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00002E24 File Offset: 0x00001024
		private static string GetShortEncounterId(string encounterId)
		{
			if (string.IsNullOrWhiteSpace(encounterId))
			{
				return string.Empty;
			}
			int num = encounterId.IndexOf('-');
			if (num <= 0)
			{
				return encounterId;
			}
			return encounterId.Substring(0, num);
		}

		// Token: 0x0400000B RID: 11
		private const string EmbeddedResourceName = "BazaarPlusPlus.Data.monsters_bazaardb.json";

		// Token: 0x0400000C RID: 12
		private static readonly Dictionary<Guid, MonsterInfo> _db = new Dictionary<Guid, MonsterInfo>();

		// Token: 0x0400000D RID: 13
		private static readonly Dictionary<string, MonsterInfo> _dbByShortEncounterId = new Dictionary<string, MonsterInfo>(StringComparer.OrdinalIgnoreCase);

		// Token: 0x0200005C RID: 92
		private sealed class MonsterRecordDto
		{
			// Token: 0x17000060 RID: 96
			// (get) Token: 0x060002A2 RID: 674 RVA: 0x0000FF01 File Offset: 0x0000E101
			// (set) Token: 0x060002A3 RID: 675 RVA: 0x0000FF09 File Offset: 0x0000E109
			[JsonProperty("encounter_id")]
			public string EncounterId { get; set; } = string.Empty;

			// Token: 0x17000061 RID: 97
			// (get) Token: 0x060002A4 RID: 676 RVA: 0x0000FF12 File Offset: 0x0000E112
			// (set) Token: 0x060002A5 RID: 677 RVA: 0x0000FF1A File Offset: 0x0000E11A
			[JsonProperty("title")]
			public string Title { get; set; } = string.Empty;

			// Token: 0x17000062 RID: 98
			// (get) Token: 0x060002A6 RID: 678 RVA: 0x0000FF23 File Offset: 0x0000E123
			// (set) Token: 0x060002A7 RID: 679 RVA: 0x0000FF2B File Offset: 0x0000E12B
			[JsonProperty("base_tier")]
			public string BaseTier { get; set; } = string.Empty;

			// Token: 0x17000063 RID: 99
			// (get) Token: 0x060002A8 RID: 680 RVA: 0x0000FF34 File Offset: 0x0000E134
			// (set) Token: 0x060002A9 RID: 681 RVA: 0x0000FF3C File Offset: 0x0000E13C
			[JsonProperty("rewards")]
			public MonsterDatabase.MonsterRewardsDto Rewards { get; set; }

			// Token: 0x17000064 RID: 100
			// (get) Token: 0x060002AA RID: 682 RVA: 0x0000FF45 File Offset: 0x0000E145
			// (set) Token: 0x060002AB RID: 683 RVA: 0x0000FF4D File Offset: 0x0000E14D
			[JsonProperty("combatant")]
			public MonsterDatabase.MonsterCombatantDto Combatant { get; set; }

			// Token: 0x17000065 RID: 101
			// (get) Token: 0x060002AC RID: 684 RVA: 0x0000FF56 File Offset: 0x0000E156
			// (set) Token: 0x060002AD RID: 685 RVA: 0x0000FF5E File Offset: 0x0000E15E
			[JsonProperty("monster_metadata")]
			public MonsterDatabase.MonsterMetadataDto MonsterMetadata { get; set; }
		}

		// Token: 0x0200005D RID: 93
		private sealed class MonsterRewardsDto
		{
			// Token: 0x17000066 RID: 102
			// (get) Token: 0x060002AF RID: 687 RVA: 0x0000FF90 File Offset: 0x0000E190
			// (set) Token: 0x060002B0 RID: 688 RVA: 0x0000FF98 File Offset: 0x0000E198
			[JsonProperty("gold")]
			public int? Gold { get; set; }

			// Token: 0x17000067 RID: 103
			// (get) Token: 0x060002B1 RID: 689 RVA: 0x0000FFA1 File Offset: 0x0000E1A1
			// (set) Token: 0x060002B2 RID: 690 RVA: 0x0000FFA9 File Offset: 0x0000E1A9
			[JsonProperty("xp")]
			public int? Xp { get; set; }
		}

		// Token: 0x0200005E RID: 94
		private sealed class MonsterCombatantDto
		{
			// Token: 0x17000068 RID: 104
			// (get) Token: 0x060002B4 RID: 692 RVA: 0x0000FFBA File Offset: 0x0000E1BA
			// (set) Token: 0x060002B5 RID: 693 RVA: 0x0000FFC2 File Offset: 0x0000E1C2
			[JsonProperty("type")]
			public string Type { get; set; } = string.Empty;

			// Token: 0x17000069 RID: 105
			// (get) Token: 0x060002B6 RID: 694 RVA: 0x0000FFCB File Offset: 0x0000E1CB
			// (set) Token: 0x060002B7 RID: 695 RVA: 0x0000FFD3 File Offset: 0x0000E1D3
			[JsonProperty("level")]
			public int? Level { get; set; }
		}

		// Token: 0x0200005F RID: 95
		private sealed class MonsterMetadataDto
		{
			// Token: 0x1700006A RID: 106
			// (get) Token: 0x060002B9 RID: 697 RVA: 0x0000FFEF File Offset: 0x0000E1EF
			// (set) Token: 0x060002BA RID: 698 RVA: 0x0000FFF7 File Offset: 0x0000E1F7
			[JsonProperty("available")]
			public string Available { get; set; } = string.Empty;

			// Token: 0x1700006B RID: 107
			// (get) Token: 0x060002BB RID: 699 RVA: 0x00010000 File Offset: 0x0000E200
			// (set) Token: 0x060002BC RID: 700 RVA: 0x00010008 File Offset: 0x0000E208
			[JsonProperty("day")]
			public int? Day { get; set; }

			// Token: 0x1700006C RID: 108
			// (get) Token: 0x060002BD RID: 701 RVA: 0x00010011 File Offset: 0x0000E211
			// (set) Token: 0x060002BE RID: 702 RVA: 0x00010019 File Offset: 0x0000E219
			[JsonProperty("health")]
			public int? Health { get; set; }

			// Token: 0x1700006D RID: 109
			// (get) Token: 0x060002BF RID: 703 RVA: 0x00010022 File Offset: 0x0000E222
			// (set) Token: 0x060002C0 RID: 704 RVA: 0x0001002A File Offset: 0x0000E22A
			[JsonProperty("board")]
			public List<MonsterDatabase.MonsterBoardCardDto> Board { get; set; } = new List<MonsterDatabase.MonsterBoardCardDto>();

			// Token: 0x1700006E RID: 110
			// (get) Token: 0x060002C1 RID: 705 RVA: 0x00010033 File Offset: 0x0000E233
			// (set) Token: 0x060002C2 RID: 706 RVA: 0x0001003B File Offset: 0x0000E23B
			[JsonProperty("skills")]
			public List<MonsterDatabase.MonsterSkillDto> Skills { get; set; } = new List<MonsterDatabase.MonsterSkillDto>();
		}

		// Token: 0x02000060 RID: 96
		private sealed class MonsterBoardCardDto
		{
			// Token: 0x1700006F RID: 111
			// (get) Token: 0x060002C4 RID: 708 RVA: 0x0001006D File Offset: 0x0000E26D
			// (set) Token: 0x060002C5 RID: 709 RVA: 0x00010075 File Offset: 0x0000E275
			[JsonProperty("cardid")]
			public string CardId { get; set; } = string.Empty;

			// Token: 0x17000070 RID: 112
			// (get) Token: 0x060002C6 RID: 710 RVA: 0x0001007E File Offset: 0x0000E27E
			// (set) Token: 0x060002C7 RID: 711 RVA: 0x00010086 File Offset: 0x0000E286
			[JsonProperty("title")]
			public string Title { get; set; } = string.Empty;

			// Token: 0x17000071 RID: 113
			// (get) Token: 0x060002C8 RID: 712 RVA: 0x0001008F File Offset: 0x0000E28F
			// (set) Token: 0x060002C9 RID: 713 RVA: 0x00010097 File Offset: 0x0000E297
			[JsonProperty("tier")]
			public string Tier { get; set; } = string.Empty;

			// Token: 0x17000072 RID: 114
			// (get) Token: 0x060002CA RID: 714 RVA: 0x000100A0 File Offset: 0x0000E2A0
			// (set) Token: 0x060002CB RID: 715 RVA: 0x000100A8 File Offset: 0x0000E2A8
			[JsonProperty("size")]
			public string Size { get; set; } = string.Empty;

			// Token: 0x17000073 RID: 115
			// (get) Token: 0x060002CC RID: 716 RVA: 0x000100B1 File Offset: 0x0000E2B1
			// (set) Token: 0x060002CD RID: 717 RVA: 0x000100B9 File Offset: 0x0000E2B9
			[JsonProperty("type")]
			public string Type { get; set; } = string.Empty;
		}

		// Token: 0x02000061 RID: 97
		private sealed class MonsterSkillDto
		{
			// Token: 0x17000074 RID: 116
			// (get) Token: 0x060002CF RID: 719 RVA: 0x00010101 File Offset: 0x0000E301
			// (set) Token: 0x060002D0 RID: 720 RVA: 0x00010109 File Offset: 0x0000E309
			[JsonProperty("skill_id")]
			public string SkillId { get; set; } = string.Empty;

			// Token: 0x17000075 RID: 117
			// (get) Token: 0x060002D1 RID: 721 RVA: 0x00010112 File Offset: 0x0000E312
			// (set) Token: 0x060002D2 RID: 722 RVA: 0x0001011A File Offset: 0x0000E31A
			[JsonProperty("title")]
			public string Title { get; set; } = string.Empty;

			// Token: 0x17000076 RID: 118
			// (get) Token: 0x060002D3 RID: 723 RVA: 0x00010123 File Offset: 0x0000E323
			// (set) Token: 0x060002D4 RID: 724 RVA: 0x0001012B File Offset: 0x0000E32B
			[JsonProperty("tier")]
			public string Tier { get; set; } = string.Empty;

			// Token: 0x17000077 RID: 119
			// (get) Token: 0x060002D5 RID: 725 RVA: 0x00010134 File Offset: 0x0000E334
			// (set) Token: 0x060002D6 RID: 726 RVA: 0x0001013C File Offset: 0x0000E33C
			[JsonProperty("type")]
			public string Type { get; set; } = string.Empty;
		}

		// Token: 0x02000062 RID: 98
		internal sealed class LegacyMonsterEntry
		{
			// Token: 0x17000078 RID: 120
			// (get) Token: 0x060002D8 RID: 728 RVA: 0x00010179 File Offset: 0x0000E379
			// (set) Token: 0x060002D9 RID: 729 RVA: 0x00010181 File Offset: 0x0000E381
			public List<string> Items { get; set; }

			// Token: 0x17000079 RID: 121
			// (get) Token: 0x060002DA RID: 730 RVA: 0x0001018A File Offset: 0x0000E38A
			// (set) Token: 0x060002DB RID: 731 RVA: 0x00010192 File Offset: 0x0000E392
			public List<string> Skills { get; set; }
		}

		// Token: 0x02000063 RID: 99
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x0400019C RID: 412
			public static Func<MonsterDatabase.MonsterBoardCardDto, MonsterBoardCardInfo> <0>__MapBoardCard;

			// Token: 0x0400019D RID: 413
			public static Func<MonsterDatabase.MonsterSkillDto, MonsterSkillInfo> <1>__MapSkill;
		}
	}
}
