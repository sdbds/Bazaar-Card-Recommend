using System;
using System.Collections.Generic;
using BazaarGameClient.Domain.Models;
using TheBazaar;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BazaarPlusPlus
{
	// Token: 0x0200000E RID: 14
	internal sealed class DebugPanel : MonoBehaviour
	{
		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600009A RID: 154 RVA: 0x00004A1E File Offset: 0x00002C1E
		// (set) Token: 0x0600009B RID: 155 RVA: 0x00004A25 File Offset: 0x00002C25
		public static bool IsVisible { get; private set; }

		// Token: 0x0600009C RID: 156 RVA: 0x00004A2D File Offset: 0x00002C2D
		private void OnDisable()
		{
			DebugPanel.IsVisible = false;
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00004A38 File Offset: 0x00002C38
		private void Update()
		{
			Keyboard current = Keyboard.current;
			if (current == null)
			{
				return;
			}
			if (current[KeyBindings.Toggle.DebugPanel].wasPressedThisFrame)
			{
				DebugPanel.IsVisible = !DebugPanel.IsVisible;
				if (DebugPanel.IsVisible)
				{
					this.RefreshSnapshot(true);
				}
			}
			if (!DebugPanel.IsVisible)
			{
				return;
			}
			if (current[KeyBindings.DebugPanel.SelectSummary].wasPressedThisFrame)
			{
				this.SelectSection(DebugPanelSection.Summary);
			}
			else if (current[KeyBindings.DebugPanel.SelectPreview].wasPressedThisFrame)
			{
				this.SelectSection(DebugPanelSection.Preview);
			}
			else if (current[KeyBindings.DebugPanel.SelectRun].wasPressedThisFrame)
			{
				this.SelectSection(DebugPanelSection.Run);
			}
			else if (current[KeyBindings.DebugPanel.SelectEncounters].wasPressedThisFrame)
			{
				this.SelectSection(DebugPanelSection.Encounters);
			}
			if (current[KeyBindings.DebugPanel.ToggleViewMode].wasPressedThisFrame)
			{
				this._panelState.ToggleViewMode();
			}
			if (Time.unscaledTime >= this._nextRefreshTime)
			{
				this.RefreshSnapshot(false);
			}
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00004B20 File Offset: 0x00002D20
		private void OnGUI()
		{
			if (!DebugPanel.IsVisible)
			{
				return;
			}
			DebugPanel.InitStyles();
			Rect rect = new Rect(10f, 10f, 520f, (float)(Screen.height - 20));
			GUI.Box(rect, "");
			GUILayout.BeginArea(new Rect(rect.x + 8f, rect.y + 8f, rect.width - 16f, rect.height - 16f));
			this.DrawToolbar();
			GUILayout.Space(8f);
			this._scroll = GUILayout.BeginScrollView(this._scroll, Array.Empty<GUILayoutOption>());
			if (this._panelState.ShowAllSections)
			{
				this.DrawAllSections();
			}
			else
			{
				this.DrawSection(this._panelState.ActiveSection);
			}
			GUILayout.EndScrollView();
			GUILayout.EndArea();
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00004BF8 File Offset: 0x00002DF8
		private void DrawToolbar()
		{
			GUILayout.Label("Bazaar++ Debug Panel", DebugPanel.HeaderStyle, Array.Empty<GUILayoutOption>());
			GUILayout.Label("Mode: " + (this._panelState.ShowAllSections ? "All Sections" : this._panelState.ActiveSection.ToString()), DebugPanel.StatusStyle, Array.Empty<GUILayoutOption>());
			GUILayout.Label("[F2] Toggle  [1-4] Sections  [Tab] " + (this._panelState.ShowAllSections ? "Single" : "All"), DebugPanel.MutedStyle, Array.Empty<GUILayoutOption>());
			GUILayout.Space(8f);
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			this.DrawSectionButton("1 Summary", DebugPanelSection.Summary);
			this.DrawSectionButton("2 Preview", DebugPanelSection.Preview);
			this.DrawSectionButton("3 Run", DebugPanelSection.Run);
			this.DrawSectionButton("4 Encounters", DebugPanelSection.Encounters);
			GUILayout.EndHorizontal();
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00004CDC File Offset: 0x00002EDC
		private void DrawSectionButton(string label, DebugPanelSection section)
		{
			bool enabled = GUI.enabled;
			GUI.enabled = (this._panelState.ActiveSection != section || this._panelState.ShowAllSections);
			if (GUILayout.Button(label, DebugPanel.ToolbarButtonStyle, Array.Empty<GUILayoutOption>()))
			{
				this.SelectSection(section);
			}
			GUI.enabled = enabled;
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x00004D2D File Offset: 0x00002F2D
		private void DrawAllSections()
		{
			this.DrawSection(DebugPanelSection.Summary);
			this.DrawSection(DebugPanelSection.Preview);
			this.DrawSection(DebugPanelSection.Run);
			this.DrawSection(DebugPanelSection.Encounters);
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x00004D4B File Offset: 0x00002F4B
		private void DrawSection(DebugPanelSection section)
		{
			switch (section)
			{
			case DebugPanelSection.Summary:
				this.DrawSummarySection();
				return;
			case DebugPanelSection.Preview:
				this.DrawPreviewSection();
				return;
			case DebugPanelSection.Run:
				this.DrawRunSection();
				return;
			case DebugPanelSection.Encounters:
				this.DrawEncounterSections();
				return;
			default:
				return;
			}
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00004D80 File Offset: 0x00002F80
		private void DrawSummarySection()
		{
			this.DrawSectionHeader("SUMMARY");
			MonsterPreviewDebugController.DebugState? preview = this._snapshot.Preview;
			this.DrawRow("Preview Visible", (preview != null && preview.GetValueOrDefault().Visible) ? "on" : "off");
			this.DrawRow("Data Source", ((preview != null) ? preview.GetValueOrDefault().DataSource : null) ?? "-");
			string key = "Hero";
			DebugPanel.RunSummary run = this._snapshot.Run;
			this.DrawRow(key, ((run != null) ? run.Hero : null) ?? "-");
			string key2 = "Day";
			DebugPanel.RunSummary run2 = this._snapshot.Run;
			this.DrawRow(key2, ((run2 != null) ? run2.Day : null) ?? "-");
			string key3 = "W/L";
			DebugPanel.RunSummary run3 = this._snapshot.Run;
			this.DrawRow(key3, ((run3 != null) ? run3.WinLoss : null) ?? "-");
			string key4 = "State";
			DebugPanel.RunSummary run4 = this._snapshot.Run;
			this.DrawRow(key4, ((run4 != null) ? run4.State : null) ?? "-");
			string key5 = "Encounter ID";
			DebugPanel.RunSummary run5 = this._snapshot.Run;
			this.DrawRow(key5, ((run5 != null) ? run5.EncounterId : null) ?? "-");
			this.DrawRow("Monster Title", ((preview != null) ? preview.GetValueOrDefault().MonsterTitle : null) ?? "-");
			this.DrawRow("World Pos", (preview != null) ? DebugPanel.FormatVector3(preview.Value.AnchorPosition) : "-");
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00004F30 File Offset: 0x00003130
		private void DrawPreviewSection()
		{
			this.DrawSectionHeader("PREVIEW");
			if (this._snapshot.Preview == null)
			{
				GUILayout.Label("Preview debug state unavailable.", DebugPanel.MutedStyle, Array.Empty<GUILayoutOption>());
				return;
			}
			MonsterPreviewDebugController.DebugState value = this._snapshot.Preview.Value;
			this.DrawRow("Data Source", value.DataSource);
			this.DrawRow("Encounter ID", value.EncounterId);
			this.DrawRow("Monster Title", value.MonsterTitle);
			this.DrawRow("Visible", value.Visible ? "on" : "off");
			this.DrawRow("World Pos", DebugPanel.FormatVector3(value.AnchorPosition));
			this.DrawRow("World Rot", DebugPanel.FormatVector3(value.AnchorRotationEuler));
			this.DrawRow("Local Offset", DebugPanel.FormatVector3(value.LocalOffset));
			this.DrawRow("Board Size", string.Format("{0:F2} x {1:F2}", value.BoardSize.x, value.BoardSize.y));
			this.DrawRow("Card Spacing X", value.CardSpacingX.ToString("F2"));
			this.DrawRow("Card Scale", value.CardScale.ToString("F2"));
			this.DrawRow("Plate Thickness", value.BoardThickness.ToString("F2"));
			this.DrawRow("Border Thickness", value.BorderThickness.ToString("F2"));
			this.DrawRow("Border Height", value.BorderHeight.ToString("F2"));
			GUILayout.Space(8f);
			GUILayout.Label("Preview controls moved to the compact Preview widget in the top-right corner.", DebugPanel.MutedStyle, Array.Empty<GUILayoutOption>());
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x000050F4 File Offset: 0x000032F4
		private void DrawRunSection()
		{
			this.DrawSectionHeader("RUN");
			string key = "Hero";
			DebugPanel.RunSummary run = this._snapshot.Run;
			this.DrawRow(key, ((run != null) ? run.Hero : null) ?? "-");
			string key2 = "Day";
			DebugPanel.RunSummary run2 = this._snapshot.Run;
			this.DrawRow(key2, ((run2 != null) ? run2.Day : null) ?? "-");
			string key3 = "W/L";
			DebugPanel.RunSummary run3 = this._snapshot.Run;
			this.DrawRow(key3, ((run3 != null) ? run3.WinLoss : null) ?? "-");
			string key4 = "State";
			DebugPanel.RunSummary run4 = this._snapshot.Run;
			this.DrawRow(key4, ((run4 != null) ? run4.State : null) ?? "-");
			string key5 = "Encounter";
			DebugPanel.RunSummary run5 = this._snapshot.Run;
			this.DrawRow(key5, ((run5 != null) ? run5.EncounterId : null) ?? "-");
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x000051E4 File Offset: 0x000033E4
		private void DrawEncounterSections()
		{
			foreach (DebugPanel.EncounterSection encounterSection in this._snapshot.EncounterSections)
			{
				this.DrawSectionHeader(encounterSection.Title.ToUpperInvariant());
				if (encounterSection.Entries.Count == 0)
				{
					GUILayout.Label("(none)", DebugPanel.MutedStyle, Array.Empty<GUILayoutOption>());
				}
				else
				{
					foreach (DebugPanel.EncounterEntry entry in encounterSection.Entries)
					{
						this.DrawEncounterEntry(entry);
					}
					if (encounterSection.MatchedMonsterRows > 0)
					{
						GUILayout.Label(string.Format("Matched monster preview: {0}/{1}", encounterSection.MatchedMonsterRows, encounterSection.Entries.Count), DebugPanel.MutedStyle, Array.Empty<GUILayoutOption>());
					}
				}
			}
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x000052F0 File Offset: 0x000034F0
		private void DrawEncounterEntry(DebugPanel.EncounterEntry entry)
		{
			bool flag = this._panelState.IsEncounterExpanded(entry.Key);
			string text = (entry.Preview != null) ? "matched" : "unmatched";
			string text2 = flag ? "[-]" : "[+]";
			if (GUILayout.Button(string.Concat(new string[]
			{
				text2,
				" ",
				entry.Name,
				"  T",
				entry.Tier,
				"  ",
				entry.Enchant,
				"  ",
				entry.CardId,
				"  ",
				text
			}), DebugPanel.EntryStyle, Array.Empty<GUILayoutOption>()))
			{
				this._panelState.ToggleEncounter(entry.Key);
			}
			if (!flag)
			{
				return;
			}
			GUILayout.BeginVertical("box", Array.Empty<GUILayoutOption>());
			GUILayout.Space(2f);
			this.DrawRow("Tier", entry.Tier);
			this.DrawRow("Enchant", entry.Enchant);
			this.DrawRow("CardID", entry.CardId);
			if (entry.Preview == null)
			{
				GUILayout.Label("Monster preview unavailable.", DebugPanel.MutedStyle, Array.Empty<GUILayoutOption>());
				GUILayout.EndVertical();
				return;
			}
			if (!string.IsNullOrEmpty(entry.Preview.EncounterName))
			{
				this.DrawRow("Encounter Name", entry.Preview.EncounterName);
			}
			if (!string.IsNullOrEmpty(entry.Preview.Title))
			{
				this.DrawRow("Title", entry.Preview.Title);
			}
			if (entry.Preview.EncounterId != Guid.Empty)
			{
				this.DrawRow("Encounter Id", entry.Preview.EncounterShortId);
			}
			string text3 = (entry.Preview.CombatLevel != null) ? entry.Preview.CombatLevel.Value.ToString() : "?";
			string text4 = (entry.Preview.RewardGold != null) ? entry.Preview.RewardGold.Value.ToString() : "?";
			string text5 = (entry.Preview.RewardXp != null) ? entry.Preview.RewardXp.Value.ToString() : "?";
			string text6 = (entry.Preview.SandstormEnabled == null) ? "?" : (entry.Preview.SandstormEnabled.Value ? "on" : "off");
			this.DrawRow("Combat", string.Concat(new string[]
			{
				"lvl=",
				text3,
				", reward=",
				text4,
				"g/",
				text5,
				"xp, sand=",
				text6
			}));
			if (!string.IsNullOrEmpty(entry.Preview.MonsterTemplateId))
			{
				this.DrawRow("Monster Tpl", entry.Preview.MonsterTemplateId);
			}
			this.DrawPreviewCardList("Board", entry.Preview.BoardCards);
			this.DrawPreviewCardList("Skills", entry.Preview.Skills);
			GUILayout.Space(2f);
			GUILayout.EndVertical();
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x0000562C File Offset: 0x0000382C
		private void DrawPreviewCardList(string title, List<RunInfo.MonsterPreviewCard> values)
		{
			if (values == null)
			{
				GUILayout.Label(title + ": unavailable", DebugPanel.MutedStyle, Array.Empty<GUILayoutOption>());
				return;
			}
			GUILayout.Label(string.Format("{0} ({1})", title, values.Count), DebugPanel.LabelStyle, Array.Empty<GUILayoutOption>());
			if (values.Count == 0)
			{
				GUILayout.Label("    - (none)", DebugPanel.MutedStyle, Array.Empty<GUILayoutOption>());
				return;
			}
			foreach (RunInfo.MonsterPreviewCard monsterPreviewCard in values)
			{
				if (monsterPreviewCard != null)
				{
					string text = string.IsNullOrWhiteSpace(monsterPreviewCard.SourceName) ? monsterPreviewCard.TemplateId : (monsterPreviewCard.SourceName + " (" + monsterPreviewCard.TemplateId + ")");
					GUILayout.Label(string.Format("    - {0}  T{1}  size={2}  {3}", new object[]
					{
						text,
						monsterPreviewCard.Tier,
						monsterPreviewCard.Size,
						monsterPreviewCard.Enchant
					}), DebugPanel.ValueStyle, Array.Empty<GUILayoutOption>());
				}
			}
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00005758 File Offset: 0x00003958
		private void DrawSectionHeader(string title)
		{
			GUILayout.Space(10f);
			GUILayout.Label(title, DebugPanel.SectionStyle, Array.Empty<GUILayoutOption>());
			GUILayout.Space(6f);
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00005780 File Offset: 0x00003980
		private void DrawRow(string key, string value)
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label(key, DebugPanel.KeyStyle, new GUILayoutOption[]
			{
				GUILayout.Width(148f)
			});
			GUILayout.Label(value ?? "-", DebugPanel.ValueStyle, Array.Empty<GUILayoutOption>());
			GUILayout.EndHorizontal();
			GUILayout.Space(2f);
		}

		// Token: 0x060000AB RID: 171 RVA: 0x000057DD File Offset: 0x000039DD
		private void SelectSection(DebugPanelSection section)
		{
			this._panelState.SelectSection(section);
			this._panelState.ShowOnlySelectedSection();
			this._scroll = Vector2.zero;
		}

		// Token: 0x060000AC RID: 172 RVA: 0x00005804 File Offset: 0x00003A04
		private void RefreshSnapshot(bool force)
		{
			if (!force && Time.unscaledTime < this._nextRefreshTime)
			{
				return;
			}
			this._nextRefreshTime = Time.unscaledTime + 0.1f;
			try
			{
				this._snapshot = this.BuildSnapshot();
			}
			catch (Exception ex)
			{
				BppLog.Warn("DebugPanel", "Rebuild failed: " + ex.Message);
			}
		}

		// Token: 0x060000AD RID: 173 RVA: 0x00005870 File Offset: 0x00003A70
		private DebugPanel.PanelSnapshot BuildSnapshot()
		{
			return new DebugPanel.PanelSnapshot
			{
				Run = this.BuildRunSummary(),
				Preview = this.BuildPreviewState(),
				EncounterSections = 
				{
					this.BuildEncounterSection("map", "Available Encounters (map)", ModState.AvailableEncounters, ModState.EncounterMonsterPreviews),
					this.BuildEncounterSection("choice", "Current Encounter Choices", ModState.CurrentEncounterChoices, ModState.EncounterMonsterPreviews)
				}
			};
		}

		// Token: 0x060000AE RID: 174 RVA: 0x000058E4 File Offset: 0x00003AE4
		private DebugPanel.RunSummary BuildRunSummary()
		{
			Run run = Data.Run;
			RunState currentState = Data.CurrentState;
			DebugPanel.RunSummary runSummary = new DebugPanel.RunSummary();
			string text;
			if (run == null)
			{
				text = null;
			}
			else
			{
				Player player = run.Player;
				text = ((player != null) ? player.Hero.ToString() : null);
			}
			runSummary.Hero = (text ?? "-");
			runSummary.Day = (((run != null) ? run.Day.ToString() : null) ?? "-");
			runSummary.WinLoss = ((run != null) ? string.Format("{0} / {1}", run.Victories, run.Losses) : "-");
			runSummary.State = (((currentState != null) ? currentState.StateName.ToString() : null) ?? "-");
			Guid? guid;
			runSummary.EncounterId = (((Data.CurrentEncounterId != null) ? guid.GetValueOrDefault().ToString() : null) ?? "-");
			return runSummary;
		}

		// Token: 0x060000AF RID: 175 RVA: 0x000059E8 File Offset: 0x00003BE8
		private MonsterPreviewDebugController.DebugState? BuildPreviewState()
		{
			MonsterPreviewDebugController component = base.GetComponent<MonsterPreviewDebugController>();
			MonsterPreviewDebugController.DebugState value;
			if (component == null || !component.TryGetDebugState(out value))
			{
				return null;
			}
			return new MonsterPreviewDebugController.DebugState?(value);
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x00005A20 File Offset: 0x00003C20
		private DebugPanel.EncounterSection BuildEncounterSection(string sectionKey, string title, List<RunInfo.CardInfo> cards, List<RunInfo.MonsterPreview> monsterPreviews)
		{
			DebugPanel.EncounterSection encounterSection = new DebugPanel.EncounterSection
			{
				Key = sectionKey,
				Title = title
			};
			if (cards == null || cards.Count == 0)
			{
				return encounterSection;
			}
			Dictionary<Guid, RunInfo.MonsterPreview> dictionary = new Dictionary<Guid, RunInfo.MonsterPreview>();
			Dictionary<string, RunInfo.MonsterPreview> dictionary2 = new Dictionary<string, RunInfo.MonsterPreview>();
			if (monsterPreviews != null)
			{
				foreach (RunInfo.MonsterPreview monsterPreview in monsterPreviews)
				{
					if (monsterPreview.EncounterTemplateId != Guid.Empty)
					{
						dictionary[monsterPreview.EncounterTemplateId] = monsterPreview;
					}
					if (!string.IsNullOrEmpty(monsterPreview.EncounterName))
					{
						dictionary2[monsterPreview.EncounterName] = monsterPreview;
					}
				}
			}
			foreach (RunInfo.CardInfo cardInfo in cards)
			{
				string text = cardInfo.Name ?? cardInfo.TemplateId.ToString("N").Substring(0, 8);
				DebugPanel.EncounterEntry encounterEntry = new DebugPanel.EncounterEntry
				{
					Key = string.Format("{0}:{1:N}", sectionKey, cardInfo.TemplateId),
					Name = text,
					Tier = cardInfo.Tier.ToString(),
					Enchant = ((string.IsNullOrEmpty(cardInfo.Enchant) || cardInfo.Enchant == "None") ? "-" : cardInfo.Enchant),
					CardId = cardInfo.TemplateId.ToString("N").Substring(0, 8)
				};
				if (!dictionary.TryGetValue(cardInfo.TemplateId, out encounterEntry.Preview))
				{
					dictionary2.TryGetValue(text, out encounterEntry.Preview);
				}
				if (encounterEntry.Preview != null)
				{
					encounterSection.MatchedMonsterRows++;
				}
				encounterSection.Entries.Add(encounterEntry);
			}
			return encounterSection;
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x00005C38 File Offset: 0x00003E38
		private static string FormatVector3(Vector3 value)
		{
			return string.Format("({0:F2}, {1:F2}, {2:F2})", value.x, value.y, value.z);
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x00005C68 File Offset: 0x00003E68
		private static void InitStyles()
		{
			if (DebugPanel._stylesInitialized)
			{
				return;
			}
			DebugPanel.HeaderStyle.normal.textColor = new Color(1f, 0.85f, 0.4f);
			DebugPanel.HeaderStyle.fontStyle = 1;
			DebugPanel.HeaderStyle.fontSize = 18;
			DebugPanel.StatusStyle.normal.textColor = new Color(0.92f, 0.92f, 0.92f);
			DebugPanel.StatusStyle.fontSize = 14;
			DebugPanel.SectionStyle.normal.textColor = new Color(0.75f, 0.9f, 1f);
			DebugPanel.SectionStyle.fontStyle = 1;
			DebugPanel.SectionStyle.fontSize = 15;
			DebugPanel.KeyStyle.normal.textColor = new Color(0.72f, 0.88f, 1f);
			DebugPanel.KeyStyle.fontSize = 14;
			DebugPanel.KeyStyle.fontStyle = 1;
			DebugPanel.LabelStyle.normal.textColor = Color.white;
			DebugPanel.LabelStyle.fontSize = 14;
			DebugPanel.ValueStyle.normal.textColor = Color.white;
			DebugPanel.ValueStyle.fontSize = 14;
			DebugPanel.ValueStyle.wordWrap = true;
			DebugPanel.MutedStyle.normal.textColor = new Color(0.75f, 0.75f, 0.75f);
			DebugPanel.MutedStyle.fontSize = 12;
			DebugPanel.MutedStyle.wordWrap = true;
			DebugPanel.CopyButtonStyle(DebugPanel.ToolbarButtonStyle);
			DebugPanel.ToolbarButtonStyle.alignment = 3;
			DebugPanel.ToolbarButtonStyle.fontSize = 13;
			DebugPanel.ToolbarButtonStyle.padding = new RectOffset(8, 8, 6, 6);
			DebugPanel.CopyButtonStyle(DebugPanel.EntryStyle);
			DebugPanel.EntryStyle.alignment = 3;
			DebugPanel.EntryStyle.fontSize = 14;
			DebugPanel.EntryStyle.padding = new RectOffset(10, 8, 8, 8);
			DebugPanel.EntryStyle.wordWrap = true;
			DebugPanel._stylesInitialized = true;
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x00005E5C File Offset: 0x0000405C
		private static void CopyButtonStyle(GUIStyle target)
		{
			GUIStyle button = GUI.skin.button;
			target.normal = button.normal;
			target.hover = button.hover;
			target.active = button.active;
			target.focused = button.focused;
			target.onNormal = button.onNormal;
			target.onHover = button.onHover;
			target.onActive = button.onActive;
			target.onFocused = button.onFocused;
			target.border = button.border;
			target.margin = button.margin;
			target.overflow = button.overflow;
			target.padding = button.padding;
		}

		// Token: 0x04000050 RID: 80
		private const float RefreshInterval = 0.1f;

		// Token: 0x04000051 RID: 81
		private const float WindowWidth = 520f;

		// Token: 0x04000052 RID: 82
		private static readonly GUIStyle HeaderStyle = new GUIStyle();

		// Token: 0x04000053 RID: 83
		private static readonly GUIStyle StatusStyle = new GUIStyle();

		// Token: 0x04000054 RID: 84
		private static readonly GUIStyle SectionStyle = new GUIStyle();

		// Token: 0x04000055 RID: 85
		private static readonly GUIStyle KeyStyle = new GUIStyle();

		// Token: 0x04000056 RID: 86
		private static readonly GUIStyle LabelStyle = new GUIStyle();

		// Token: 0x04000057 RID: 87
		private static readonly GUIStyle ValueStyle = new GUIStyle();

		// Token: 0x04000058 RID: 88
		private static readonly GUIStyle MutedStyle = new GUIStyle();

		// Token: 0x04000059 RID: 89
		private static readonly GUIStyle ToolbarButtonStyle = new GUIStyle();

		// Token: 0x0400005A RID: 90
		private static readonly GUIStyle EntryStyle = new GUIStyle();

		// Token: 0x0400005B RID: 91
		private static bool _stylesInitialized;

		// Token: 0x0400005C RID: 92
		private readonly DebugPanelState _panelState = new DebugPanelState();

		// Token: 0x0400005D RID: 93
		private Vector2 _scroll = Vector2.zero;

		// Token: 0x0400005E RID: 94
		private DebugPanel.PanelSnapshot _snapshot = DebugPanel.PanelSnapshot.Empty;

		// Token: 0x0400005F RID: 95
		private float _nextRefreshTime;

		// Token: 0x02000067 RID: 103
		private sealed class PanelSnapshot
		{
			// Token: 0x040001A7 RID: 423
			public static readonly DebugPanel.PanelSnapshot Empty = new DebugPanel.PanelSnapshot();

			// Token: 0x040001A8 RID: 424
			public MonsterPreviewDebugController.DebugState? Preview;

			// Token: 0x040001A9 RID: 425
			public DebugPanel.RunSummary Run;

			// Token: 0x040001AA RID: 426
			public List<DebugPanel.EncounterSection> EncounterSections = new List<DebugPanel.EncounterSection>();
		}

		// Token: 0x02000068 RID: 104
		private sealed class RunSummary
		{
			// Token: 0x040001AB RID: 427
			public string Hero;

			// Token: 0x040001AC RID: 428
			public string Day;

			// Token: 0x040001AD RID: 429
			public string WinLoss;

			// Token: 0x040001AE RID: 430
			public string State;

			// Token: 0x040001AF RID: 431
			public string EncounterId;
		}

		// Token: 0x02000069 RID: 105
		private sealed class EncounterSection
		{
			// Token: 0x040001B0 RID: 432
			public string Key;

			// Token: 0x040001B1 RID: 433
			public string Title;

			// Token: 0x040001B2 RID: 434
			public List<DebugPanel.EncounterEntry> Entries = new List<DebugPanel.EncounterEntry>();

			// Token: 0x040001B3 RID: 435
			public int MatchedMonsterRows;
		}

		// Token: 0x0200006A RID: 106
		private sealed class EncounterEntry
		{
			// Token: 0x040001B4 RID: 436
			public string Key;

			// Token: 0x040001B5 RID: 437
			public string Name;

			// Token: 0x040001B6 RID: 438
			public string Tier;

			// Token: 0x040001B7 RID: 439
			public string Enchant;

			// Token: 0x040001B8 RID: 440
			public string CardId;

			// Token: 0x040001B9 RID: 441
			public RunInfo.MonsterPreview Preview;
		}
	}
}
