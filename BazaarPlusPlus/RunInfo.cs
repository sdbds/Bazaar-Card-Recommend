using System;
using System.Collections.Generic;
using BazaarGameShared.Domain.Core;
using BazaarGameShared.Domain.Core.Types;

namespace BazaarPlusPlus
{
	// Token: 0x0200003C RID: 60
	public class RunInfo
	{
		// Token: 0x17000055 RID: 85
		// (get) Token: 0x06000254 RID: 596 RVA: 0x0000EC12 File Offset: 0x0000CE12
		// (set) Token: 0x06000255 RID: 597 RVA: 0x0000EC1A File Offset: 0x0000CE1A
		public int? Regen { get; set; }

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x06000256 RID: 598 RVA: 0x0000EC23 File Offset: 0x0000CE23
		// (set) Token: 0x06000257 RID: 599 RVA: 0x0000EC2B File Offset: 0x0000CE2B
		public int? Level { get; set; }

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x06000258 RID: 600 RVA: 0x0000EC34 File Offset: 0x0000CE34
		// (set) Token: 0x06000259 RID: 601 RVA: 0x0000EC3C File Offset: 0x0000CE3C
		public int? Gold { get; set; }

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x0600025A RID: 602 RVA: 0x0000EC45 File Offset: 0x0000CE45
		// (set) Token: 0x0600025B RID: 603 RVA: 0x0000EC4D File Offset: 0x0000CE4D
		public int? Income { get; set; }

		// Token: 0x0400014C RID: 332
		public string Hero;

		// Token: 0x0400014D RID: 333
		public List<RunInfo.SkillInfo> Skills;

		// Token: 0x0400014E RID: 334
		public List<RunInfo.SkillInfo> OppSkills;

		// Token: 0x0400014F RID: 335
		public List<RunInfo.CardInfo> Cards;

		// Token: 0x04000150 RID: 336
		public List<RunInfo.CardInfo> Stash;

		// Token: 0x04000151 RID: 337
		public List<RunInfo.CardInfo> OppCards;

		// Token: 0x04000152 RID: 338
		public List<RunInfo.CardInfo> OppStash;

		// Token: 0x04000153 RID: 339
		public int? OppHealth;

		// Token: 0x04000154 RID: 340
		public int? OppRegen;

		// Token: 0x04000155 RID: 341
		public int? OppGold;

		// Token: 0x04000156 RID: 342
		public int? OppIncome;

		// Token: 0x04000157 RID: 343
		public int? OppShield;

		// Token: 0x04000158 RID: 344
		public int? OppLevel;

		// Token: 0x04000159 RID: 345
		public string OppName;

		// Token: 0x0400015A RID: 346
		public uint Wins;

		// Token: 0x0400015B RID: 347
		public uint Losses;

		// Token: 0x0400015C RID: 348
		public int Day;

		// Token: 0x0400015D RID: 349
		public string Version = "0.0.1";

		// Token: 0x0400015E RID: 350
		public int? Health;

		// Token: 0x0400015F RID: 351
		public int? Shield;

		// Token: 0x04000164 RID: 356
		public string Name;

		// Token: 0x04000165 RID: 357
		public string OppHero;

		// Token: 0x04000166 RID: 358
		public int? Prestige;

		// Token: 0x04000167 RID: 359
		public int? OppPrestige;

		// Token: 0x04000168 RID: 360
		public bool PlayMode;

		// Token: 0x04000169 RID: 361
		public List<RunInfo.CardInfo> AvailableEncounters;

		// Token: 0x0400016A RID: 362
		public List<RunInfo.CardInfo> CurrentEncounterChoices;

		// Token: 0x02000088 RID: 136
		public class SkillInfo
		{
			// Token: 0x17000083 RID: 131
			// (get) Token: 0x0600032F RID: 815 RVA: 0x00011CA0 File Offset: 0x0000FEA0
			// (set) Token: 0x06000330 RID: 816 RVA: 0x00011CA8 File Offset: 0x0000FEA8
			public Dictionary<ECardAttributeType, int> Attributes { get; set; } = new Dictionary<ECardAttributeType, int>();

			// Token: 0x04000242 RID: 578
			public ETier Tier;

			// Token: 0x04000243 RID: 579
			public Guid TemplateId;

			// Token: 0x04000244 RID: 580
			public string Name;
		}

		// Token: 0x02000089 RID: 137
		public class MonsterPreview
		{
			// Token: 0x04000246 RID: 582
			public Guid EncounterTemplateId;

			// Token: 0x04000247 RID: 583
			public Guid EncounterId;

			// Token: 0x04000248 RID: 584
			public string EncounterShortId;

			// Token: 0x04000249 RID: 585
			public string EncounterName;

			// Token: 0x0400024A RID: 586
			public string Title;

			// Token: 0x0400024B RID: 587
			public string MonsterTemplateId;

			// Token: 0x0400024C RID: 588
			public int? CombatLevel;

			// Token: 0x0400024D RID: 589
			public int? Health;

			// Token: 0x0400024E RID: 590
			public int? RewardGold;

			// Token: 0x0400024F RID: 591
			public int? RewardXp;

			// Token: 0x04000250 RID: 592
			public bool? SandstormEnabled;

			// Token: 0x04000251 RID: 593
			public List<RunInfo.MonsterPreviewCard> BoardCards;

			// Token: 0x04000252 RID: 594
			public List<RunInfo.MonsterPreviewCard> Skills;
		}

		// Token: 0x0200008A RID: 138
		public class MonsterPreviewCard
		{
			// Token: 0x17000084 RID: 132
			// (get) Token: 0x06000333 RID: 819 RVA: 0x00011CCC File Offset: 0x0000FECC
			// (set) Token: 0x06000334 RID: 820 RVA: 0x00011CD4 File Offset: 0x0000FED4
			public string TemplateId { get; set; } = string.Empty;

			// Token: 0x17000085 RID: 133
			// (get) Token: 0x06000335 RID: 821 RVA: 0x00011CDD File Offset: 0x0000FEDD
			// (set) Token: 0x06000336 RID: 822 RVA: 0x00011CE5 File Offset: 0x0000FEE5
			public string SourceName { get; set; } = string.Empty;

			// Token: 0x17000086 RID: 134
			// (get) Token: 0x06000337 RID: 823 RVA: 0x00011CEE File Offset: 0x0000FEEE
			// (set) Token: 0x06000338 RID: 824 RVA: 0x00011CF6 File Offset: 0x0000FEF6
			public int Tier { get; set; }

			// Token: 0x17000087 RID: 135
			// (get) Token: 0x06000339 RID: 825 RVA: 0x00011CFF File Offset: 0x0000FEFF
			// (set) Token: 0x0600033A RID: 826 RVA: 0x00011D07 File Offset: 0x0000FF07
			public int Size { get; set; } = 1;

			// Token: 0x17000088 RID: 136
			// (get) Token: 0x0600033B RID: 827 RVA: 0x00011D10 File Offset: 0x0000FF10
			// (set) Token: 0x0600033C RID: 828 RVA: 0x00011D18 File Offset: 0x0000FF18
			public string Enchant { get; set; } = "None";

			// Token: 0x17000089 RID: 137
			// (get) Token: 0x0600033D RID: 829 RVA: 0x00011D21 File Offset: 0x0000FF21
			// (set) Token: 0x0600033E RID: 830 RVA: 0x00011D29 File Offset: 0x0000FF29
			public Dictionary<int, int> Attributes { get; set; } = new Dictionary<int, int>();
		}

		// Token: 0x0200008B RID: 139
		public class CardInfo
		{
			// Token: 0x1700008A RID: 138
			// (get) Token: 0x06000340 RID: 832 RVA: 0x00011D6D File Offset: 0x0000FF6D
			// (set) Token: 0x06000341 RID: 833 RVA: 0x00011D75 File Offset: 0x0000FF75
			public Dictionary<ECardAttributeType, int> Attributes { get; set; } = new Dictionary<ECardAttributeType, int>();

			// Token: 0x04000259 RID: 601
			public ETier Tier;

			// Token: 0x0400025A RID: 602
			public string Name;

			// Token: 0x0400025B RID: 603
			public Guid TemplateId;

			// Token: 0x0400025C RID: 604
			public EContainerSocketId? Left;

			// Token: 0x0400025D RID: 605
			public InstanceId Instance;

			// Token: 0x0400025E RID: 606
			public HashSet<ECardTag> Tags;

			// Token: 0x04000260 RID: 608
			public string Enchant;
		}
	}
}
