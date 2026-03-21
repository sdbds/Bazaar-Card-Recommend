using System;
using System.Collections.Generic;

namespace BazaarPlusPlus
{
	// Token: 0x0200000A RID: 10
	internal sealed class MonsterInfo
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000024 RID: 36 RVA: 0x00002E71 File Offset: 0x00001071
		// (set) Token: 0x06000025 RID: 37 RVA: 0x00002E79 File Offset: 0x00001079
		public Guid EncounterId { get; set; }

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000026 RID: 38 RVA: 0x00002E82 File Offset: 0x00001082
		// (set) Token: 0x06000027 RID: 39 RVA: 0x00002E8A File Offset: 0x0000108A
		public string EncounterKey { get; set; } = string.Empty;

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000028 RID: 40 RVA: 0x00002E93 File Offset: 0x00001093
		// (set) Token: 0x06000029 RID: 41 RVA: 0x00002E9B File Offset: 0x0000109B
		public string EncounterShortId { get; set; } = string.Empty;

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600002A RID: 42 RVA: 0x00002EA4 File Offset: 0x000010A4
		// (set) Token: 0x0600002B RID: 43 RVA: 0x00002EAC File Offset: 0x000010AC
		public string Title { get; set; } = string.Empty;

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600002C RID: 44 RVA: 0x00002EB5 File Offset: 0x000010B5
		// (set) Token: 0x0600002D RID: 45 RVA: 0x00002EBD File Offset: 0x000010BD
		public string BaseTier { get; set; } = string.Empty;

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600002E RID: 46 RVA: 0x00002EC6 File Offset: 0x000010C6
		// (set) Token: 0x0600002F RID: 47 RVA: 0x00002ECE File Offset: 0x000010CE
		public int? CombatLevel { get; set; }

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000030 RID: 48 RVA: 0x00002ED7 File Offset: 0x000010D7
		// (set) Token: 0x06000031 RID: 49 RVA: 0x00002EDF File Offset: 0x000010DF
		public int? Health { get; set; }

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000032 RID: 50 RVA: 0x00002EE8 File Offset: 0x000010E8
		// (set) Token: 0x06000033 RID: 51 RVA: 0x00002EF0 File Offset: 0x000010F0
		public int? RewardGold { get; set; }

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000034 RID: 52 RVA: 0x00002EF9 File Offset: 0x000010F9
		// (set) Token: 0x06000035 RID: 53 RVA: 0x00002F01 File Offset: 0x00001101
		public int? RewardXp { get; set; }

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000036 RID: 54 RVA: 0x00002F0A File Offset: 0x0000110A
		// (set) Token: 0x06000037 RID: 55 RVA: 0x00002F12 File Offset: 0x00001112
		public IReadOnlyList<MonsterBoardCardInfo> BoardCards { get; set; } = Array.Empty<MonsterBoardCardInfo>();

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000038 RID: 56 RVA: 0x00002F1B File Offset: 0x0000111B
		// (set) Token: 0x06000039 RID: 57 RVA: 0x00002F23 File Offset: 0x00001123
		public IReadOnlyList<MonsterSkillInfo> Skills { get; set; } = Array.Empty<MonsterSkillInfo>();
	}
}
