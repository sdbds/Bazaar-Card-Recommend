using System;
using System.Collections.Generic;

namespace BazaarPlusPlus
{
	// Token: 0x02000010 RID: 16
	internal sealed class DebugPanelState
	{
		// Token: 0x1700001E RID: 30
		// (get) Token: 0x060000B6 RID: 182 RVA: 0x00005F97 File Offset: 0x00004197
		// (set) Token: 0x060000B7 RID: 183 RVA: 0x00005F9F File Offset: 0x0000419F
		public DebugPanelSection ActiveSection { get; private set; }

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x060000B8 RID: 184 RVA: 0x00005FA8 File Offset: 0x000041A8
		// (set) Token: 0x060000B9 RID: 185 RVA: 0x00005FB0 File Offset: 0x000041B0
		public bool ShowAllSections { get; private set; }

		// Token: 0x060000BA RID: 186 RVA: 0x00005FB9 File Offset: 0x000041B9
		public void SelectSection(DebugPanelSection section)
		{
			this.ActiveSection = section;
		}

		// Token: 0x060000BB RID: 187 RVA: 0x00005FC2 File Offset: 0x000041C2
		public void ToggleViewMode()
		{
			this.ShowAllSections = !this.ShowAllSections;
		}

		// Token: 0x060000BC RID: 188 RVA: 0x00005FD3 File Offset: 0x000041D3
		public void ShowOnlySelectedSection()
		{
			this.ShowAllSections = false;
		}

		// Token: 0x060000BD RID: 189 RVA: 0x00005FDC File Offset: 0x000041DC
		public void ToggleEncounter(string encounterKey)
		{
			if (string.IsNullOrEmpty(encounterKey))
			{
				return;
			}
			if (!this._expandedEncounterKeys.Add(encounterKey))
			{
				this._expandedEncounterKeys.Remove(encounterKey);
			}
		}

		// Token: 0x060000BE RID: 190 RVA: 0x00006002 File Offset: 0x00004202
		public bool IsEncounterExpanded(string encounterKey)
		{
			return !string.IsNullOrEmpty(encounterKey) && this._expandedEncounterKeys.Contains(encounterKey);
		}

		// Token: 0x04000066 RID: 102
		private readonly HashSet<string> _expandedEncounterKeys = new HashSet<string>();
	}
}
