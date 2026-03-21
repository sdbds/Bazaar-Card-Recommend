using System;
using System.Collections.Generic;
using System.Linq;
using BazaarGameClient.Domain.Models;
using BazaarGameClient.Domain.Models.Cards;
using BazaarGameShared.Domain.Cards;
using BazaarGameShared.Domain.Core.Types;
using BazaarGameShared.Domain.Players;
using TheBazaar;
using UnityEngine;

namespace BazaarPlusPlus
{
	// Token: 0x02000028 RID: 40
	internal sealed class MonsterPreviewDebugController : MonoBehaviour
	{
		// Token: 0x06000160 RID: 352 RVA: 0x000083A8 File Offset: 0x000065A8
		private void Awake()
		{
			this._overlayController = base.GetComponent<MonsterPreviewController>();
			this._showcaseRuntime = base.GetComponent<MonsterLockShowcaseRuntime>();
			this._anchorStrategy = new FixedAnchorStrategy();
			this._presentation = MonsterPreviewDefaults.CreateDebugPresentation();
			this._tuner = new MonsterPreviewDebugTuner(this._anchorStrategy, this._presentation);
			if (this._overlayController != null)
			{
				this._overlayController.SetAnchorStrategy(this._anchorStrategy);
				this._overlayController.SetPresentation(MonsterPreviewDebugController.ClonePresentation(this._presentation));
				this._overlayController.SetDebugOptions(this._debugOptions);
				this._overlayController.SetVisible(false);
			}
			this.SeedAnchorToDefaultPose();
		}

		// Token: 0x06000161 RID: 353 RVA: 0x00008454 File Offset: 0x00006654
		private void Update()
		{
			if (this._overlayController == null || !this._overlayController.Visible)
			{
				return;
			}
			if (Time.unscaledTime >= this._nextRefreshTime)
			{
				this._nextRefreshTime = Time.unscaledTime + 0.2f;
				this.SyncPreviewData();
			}
		}

		// Token: 0x06000162 RID: 354 RVA: 0x000084A4 File Offset: 0x000066A4
		private void OnGUI()
		{
			if (this._overlayController == null)
			{
				return;
			}
			MonsterPreviewDebugController.InitStyles();
			Rect rect = new Rect((float)Screen.width - 92f - 16f, 16f, 92f, 30f);
			string label = this._overlayController.Visible ? "Preview ON" : "Preview";
			if (MonsterPreviewDebugController.DrawTintedButton(rect, label, MonsterPreviewDebugController.WidgetButtonStyle, this._overlayController.Visible ? MonsterPreviewDebugController.WidgetButtonActiveColor : MonsterPreviewDebugController.WidgetButtonColor))
			{
				this._widgetExpanded = !this._widgetExpanded;
			}
			if (!this._widgetExpanded)
			{
				return;
			}
			Rect rect2 = new Rect((float)Screen.width - 260f - 16f, rect.yMax + 8f, 260f, 376f);
			MonsterPreviewDebugController.DrawTintedBox(rect2, MonsterPreviewDebugController.PanelStyle, MonsterPreviewDebugController.PanelColor);
			GUILayout.BeginArea(new Rect(rect2.x + 12f, rect2.y + 12f, rect2.width - 24f, rect2.height - 24f));
			this.DrawWidgetHeader();
			GUILayout.Space(10f);
			this.DrawActionButtons();
			GUILayout.Space(10f);
			this.DrawAnchorSection();
			GUILayout.Space(10f);
			this.DrawLayoutSection();
			GUILayout.EndArea();
		}

		// Token: 0x06000163 RID: 355 RVA: 0x00008600 File Offset: 0x00006800
		public bool TryGetDebugState(out MonsterPreviewDebugController.DebugState state)
		{
			if (this._overlayController == null || this._anchorStrategy == null || this._presentation == null)
			{
				state = default(MonsterPreviewDebugController.DebugState);
				return false;
			}
			state = new MonsterPreviewDebugController.DebugState
			{
				DataSource = (this.UsingShowcaseTargets ? "locked_showcase" : (this._useMonsterDatabase ? "monster_db" : "player_hand")),
				EncounterId = (string.IsNullOrEmpty(this._activeEncounterId) ? "-" : this._activeEncounterId),
				MonsterTitle = (string.IsNullOrEmpty(this._activeMonsterTitle) ? "-" : this._activeMonsterTitle),
				Visible = this._overlayController.Visible,
				AnchorPosition = this.ActiveAnchorStrategy.Position,
				AnchorRotationEuler = this.ActiveAnchorStrategy.Rotation.eulerAngles,
				LocalOffset = this.ActivePresentation.LocalOffset,
				BoardSize = this.ActivePresentation.BoardSize,
				CardSpacingX = this.ActivePresentation.CardSpacing.x,
				CardScale = this.ActivePresentation.CardScale.x,
				BoardThickness = this.ActivePresentation.BoardThickness,
				BorderThickness = this.ActivePresentation.BorderThickness,
				BorderHeight = this.ActivePresentation.BorderHeight
			};
			return true;
		}

		// Token: 0x06000164 RID: 356 RVA: 0x00008778 File Offset: 0x00006978
		private void DrawWidgetHeader()
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label("Monster Preview", MonsterPreviewDebugController.TitleStyle, Array.Empty<GUILayoutOption>());
			GUILayout.FlexibleSpace();
			if (MonsterPreviewDebugController.DrawTintedButton("x", MonsterPreviewDebugController.StepperButtonStyle, MonsterPreviewDebugController.StepperButtonColor, new GUILayoutOption[]
			{
				GUILayout.Width(26f),
				GUILayout.Height(22f)
			}))
			{
				this._widgetExpanded = false;
			}
			GUILayout.EndHorizontal();
			GUILayout.Label(this.UsingShowcaseTargets ? "Visible  |  Locked Showcase Target" : ((this._overlayController.Visible ? "Visible" : "Hidden") + "  |  " + (this._useMonsterDatabase ? "Monster DB" : "Player Hand")), MonsterPreviewDebugController.SubtitleStyle, Array.Empty<GUILayoutOption>());
			GUILayout.Label(this.UsingShowcaseTargets ? "Adjusting right-click preview" : ((string.IsNullOrEmpty(this._activeMonsterTitle) ? "No target" : this._activeMonsterTitle) ?? ""), MonsterPreviewDebugController.SubtitleStyle, Array.Empty<GUILayoutOption>());
		}

		// Token: 0x06000165 RID: 357 RVA: 0x00008880 File Offset: 0x00006A80
		private void DrawActionButtons()
		{
			if (this.UsingShowcaseTargets)
			{
				GUILayout.Label("Locked showcase is using live tuning targets.", MonsterPreviewDebugController.SubtitleStyle, Array.Empty<GUILayoutOption>());
				GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
				if (MonsterPreviewDebugController.DrawTintedButton("Reset Anchor", MonsterPreviewDebugController.ActionButtonStyle, MonsterPreviewDebugController.ActionButtonColor, Array.Empty<GUILayoutOption>()))
				{
					this.ResetAnchor();
				}
				GUILayout.EndHorizontal();
				return;
			}
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			if (MonsterPreviewDebugController.DrawTintedButton(this._overlayController.Visible ? "Hide" : "Show", MonsterPreviewDebugController.ActionButtonStyle, MonsterPreviewDebugController.ActionButtonColor, Array.Empty<GUILayoutOption>()))
			{
				this.TogglePreview();
			}
			if (MonsterPreviewDebugController.DrawTintedButton("Source", MonsterPreviewDebugController.ActionButtonStyle, MonsterPreviewDebugController.ActionButtonColor, Array.Empty<GUILayoutOption>()))
			{
				this.ToggleDataSource();
			}
			if (MonsterPreviewDebugController.DrawTintedButton("Sync", MonsterPreviewDebugController.ActionButtonStyle, MonsterPreviewDebugController.ActionButtonColor, Array.Empty<GUILayoutOption>()))
			{
				this.SyncPreviewData();
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			if (MonsterPreviewDebugController.DrawTintedButton("Reset Anchor", MonsterPreviewDebugController.ActionButtonStyle, MonsterPreviewDebugController.ActionButtonColor, Array.Empty<GUILayoutOption>()))
			{
				this.ResetAnchor();
			}
			GUILayout.EndHorizontal();
		}

		// Token: 0x06000166 RID: 358 RVA: 0x00008990 File Offset: 0x00006B90
		private void DrawAnchorSection()
		{
			GUILayout.Label("ANCHOR", MonsterPreviewDebugController.SectionStyle, Array.Empty<GUILayoutOption>());
			string label = "X";
			Vector3 vector = this.ActiveAnchorStrategy.Position;
			this.DrawStepperRow(label, vector.x.ToString("F1"), delegate
			{
				this.MoveAnchor(new Vector3(-0.5f, 0f, 0f));
			}, delegate
			{
				this.MoveAnchor(new Vector3(0.5f, 0f, 0f));
			});
			string label2 = "Y";
			vector = this.ActiveAnchorStrategy.Position;
			this.DrawStepperRow(label2, vector.y.ToString("F1"), delegate
			{
				this.MoveAnchor(new Vector3(0f, -0.5f, 0f));
			}, delegate
			{
				this.MoveAnchor(new Vector3(0f, 0.5f, 0f));
			});
			string label3 = "Z";
			vector = this.ActiveAnchorStrategy.Position;
			this.DrawStepperRow(label3, vector.z.ToString("F1"), delegate
			{
				this.MoveAnchor(new Vector3(0f, 0f, -0.5f));
			}, delegate
			{
				this.MoveAnchor(new Vector3(0f, 0f, 0.5f));
			});
			string label4 = "Yaw";
			vector = this.ActiveAnchorStrategy.Rotation.eulerAngles;
			this.DrawStepperRow(label4, vector.y.ToString("F0"), delegate
			{
				this.RotateAnchor(-5f);
			}, delegate
			{
				this.RotateAnchor(5f);
			});
		}

		// Token: 0x06000167 RID: 359 RVA: 0x00008ABC File Offset: 0x00006CBC
		private void DrawLayoutSection()
		{
			GUILayout.Label("LAYOUT", MonsterPreviewDebugController.SectionStyle, Array.Empty<GUILayoutOption>());
			string label = "Width";
			Vector2 boardSize = this.ActivePresentation.BoardSize;
			this.DrawStepperRow(label, boardSize.x.ToString("F2"), delegate
			{
				this.AdjustLayout(delegate
				{
					this.ActiveTuner.AdjustBoardWidth(-0.25f);
				});
			}, delegate
			{
				this.AdjustLayout(delegate
				{
					this.ActiveTuner.AdjustBoardWidth(0.25f);
				});
			});
			string label2 = "Height";
			boardSize = this.ActivePresentation.BoardSize;
			this.DrawStepperRow(label2, boardSize.y.ToString("F2"), delegate
			{
				this.AdjustLayout(delegate
				{
					this.ActiveTuner.AdjustBoardHeight(-0.25f);
				});
			}, delegate
			{
				this.AdjustLayout(delegate
				{
					this.ActiveTuner.AdjustBoardHeight(0.25f);
				});
			});
			string label3 = "Gap";
			Vector3 vector = this.ActivePresentation.CardSpacing;
			this.DrawStepperRow(label3, vector.x.ToString("F2"), delegate
			{
				this.AdjustLayout(delegate
				{
					this.ActiveTuner.AdjustSpacingX(-0.25f);
				});
			}, delegate
			{
				this.AdjustLayout(delegate
				{
					this.ActiveTuner.AdjustSpacingX(0.25f);
				});
			});
			string label4 = "Scale";
			vector = this.ActivePresentation.CardScale;
			this.DrawStepperRow(label4, vector.x.ToString("F2"), delegate
			{
				this.AdjustLayout(delegate
				{
					this.ActiveTuner.AdjustCardScale(-0.05f);
				});
			}, delegate
			{
				this.AdjustLayout(delegate
				{
					this.ActiveTuner.AdjustCardScale(0.05f);
				});
			});
			this.DrawStepperRow("Plate", this.ActivePresentation.BoardThickness.ToString("F2"), delegate
			{
				this.AdjustLayout(delegate
				{
					this.ActiveTuner.AdjustBoardThickness(-0.02f);
				});
			}, delegate
			{
				this.AdjustLayout(delegate
				{
					this.ActiveTuner.AdjustBoardThickness(0.02f);
				});
			});
			this.DrawStepperRow("Border", this.ActivePresentation.BorderThickness.ToString("F2"), delegate
			{
				this.AdjustLayout(delegate
				{
					this.ActiveTuner.AdjustBorderThickness(-0.02f);
				});
			}, delegate
			{
				this.AdjustLayout(delegate
				{
					this.ActiveTuner.AdjustBorderThickness(0.02f);
				});
			});
			this.DrawStepperRow("Lip", this.ActivePresentation.BorderHeight.ToString("F2"), delegate
			{
				this.AdjustLayout(delegate
				{
					this.ActiveTuner.AdjustBorderHeight(-0.02f);
				});
			}, delegate
			{
				this.AdjustLayout(delegate
				{
					this.ActiveTuner.AdjustBorderHeight(0.02f);
				});
			});
		}

		// Token: 0x06000168 RID: 360 RVA: 0x00008C90 File Offset: 0x00006E90
		private void DrawStepperRow(string label, string value, Action decrement, Action increment)
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label(label, MonsterPreviewDebugController.RowLabelStyle, new GUILayoutOption[]
			{
				GUILayout.Width(50f)
			});
			if (MonsterPreviewDebugController.DrawTintedButton("-", MonsterPreviewDebugController.StepperButtonStyle, MonsterPreviewDebugController.StepperButtonColor, new GUILayoutOption[]
			{
				GUILayout.Width(28f),
				GUILayout.Height(24f)
			}))
			{
				decrement();
			}
			GUILayout.Label(value, MonsterPreviewDebugController.ValueStyle, new GUILayoutOption[]
			{
				GUILayout.Width(58f)
			});
			if (MonsterPreviewDebugController.DrawTintedButton("+", MonsterPreviewDebugController.StepperButtonStyle, MonsterPreviewDebugController.StepperButtonColor, new GUILayoutOption[]
			{
				GUILayout.Width(28f),
				GUILayout.Height(24f)
			}))
			{
				increment();
			}
			GUILayout.EndHorizontal();
		}

		// Token: 0x06000169 RID: 361 RVA: 0x00008D64 File Offset: 0x00006F64
		private void TogglePreview()
		{
			if (!this._anchorSeeded)
			{
				this.SeedAnchorToDefaultPose();
			}
			this._overlayController.SetVisible(!this._overlayController.Visible);
			if (this._overlayController.Visible)
			{
				this._nextRefreshTime = Time.unscaledTime + 0.2f;
				this.SyncPreviewData();
			}
			BppLog.Debug("MonsterPreviewDebugController", string.Format("Preview visible={0}", this._overlayController.Visible));
		}

		// Token: 0x0600016A RID: 362 RVA: 0x00008DE0 File Offset: 0x00006FE0
		private void ToggleDataSource()
		{
			this._useMonsterDatabase = !this._useMonsterDatabase;
			this._lastCardSignature = string.Empty;
			this._lastSkillSignature = string.Empty;
			if (this._overlayController.Visible)
			{
				this.SyncPreviewData();
			}
			BppLog.Debug("MonsterPreviewDebugController", "Preview data source=" + (this._useMonsterDatabase ? "monster_db" : "player_hand"));
		}

		// Token: 0x0600016B RID: 363 RVA: 0x00008E50 File Offset: 0x00007050
		private void ResetAnchor()
		{
			if (this.UsingShowcaseTargets)
			{
				this.ActiveTuner.ResetAnchor(MonsterPreviewDefaults.DefaultAnchorPose);
			}
			else
			{
				this.SeedAnchorToDefaultPose();
			}
			this.ApplyLayout();
			BppLog.Debug("MonsterPreviewDebugController", string.Format("Anchor pos={0} rot={1}", this.ActiveAnchorStrategy.Position, this.ActiveAnchorStrategy.Rotation.eulerAngles));
		}

		// Token: 0x0600016C RID: 364 RVA: 0x00008EC0 File Offset: 0x000070C0
		private void MoveAnchor(Vector3 delta)
		{
			this.ActiveTuner.MoveAnchor(delta);
			this.ApplyLayout();
			BppLog.Debug("MonsterPreviewDebugController", string.Format("Anchor pos={0} rot={1}", this.ActiveAnchorStrategy.Position, this.ActiveAnchorStrategy.Rotation.eulerAngles));
		}

		// Token: 0x0600016D RID: 365 RVA: 0x00008F1C File Offset: 0x0000711C
		private void RotateAnchor(float delta)
		{
			this.ActiveTuner.RotateAnchorY(delta);
			this.ApplyLayout();
			BppLog.Debug("MonsterPreviewDebugController", string.Format("Anchor pos={0} rot={1}", this.ActiveAnchorStrategy.Position, this.ActiveAnchorStrategy.Rotation.eulerAngles));
		}

		// Token: 0x0600016E RID: 366 RVA: 0x00008F78 File Offset: 0x00007178
		private void AdjustLayout(Action adjustment)
		{
			adjustment();
			this.ApplyLayout();
			BppLog.Debug("MonsterPreviewDebugController", string.Format("Layout size={0} offset={1} spacingX={2:F2} scale={3:F2} boardT={4:F2} borderT={5:F2} borderH={6:F2}", new object[]
			{
				this.ActivePresentation.BoardSize,
				this.ActivePresentation.LocalOffset,
				this.ActivePresentation.CardSpacing.x,
				this.ActivePresentation.CardScale.x,
				this.ActivePresentation.BoardThickness,
				this.ActivePresentation.BorderThickness,
				this.ActivePresentation.BorderHeight
			}));
		}

		// Token: 0x0600016F RID: 367 RVA: 0x0000903A File Offset: 0x0000723A
		private void SyncPreviewData()
		{
			if (this._useMonsterDatabase)
			{
				this.SyncCardsFromMonsterDatabase();
				return;
			}
			this.SyncCardsFromHand();
		}

		// Token: 0x06000170 RID: 368 RVA: 0x00009054 File Offset: 0x00007254
		private void SyncCardsFromMonsterDatabase()
		{
			this._activeEncounterId = "4a4542cd";
			MonsterInfo monsterInfo;
			if (!MonsterDatabase.TryGetByEncounterId("4a4542cd", out monsterInfo))
			{
				this._activeMonsterTitle = string.Empty;
				BppLog.Warn("MonsterPreviewDebugController", "Monster DB miss encounterId=4a4542cd");
				this._overlayController.SetCards(new List<PreviewCardSpec>());
				this._overlayController.SetSkillCards(new List<PreviewCardSpec>());
				return;
			}
			this._activeMonsterTitle = monsterInfo.Title;
			PreviewBoardModel previewBoardModel = MonsterDatabasePreviewDataSource.BuildModel(monsterInfo, "monster_db");
			List<PreviewCardSpec> list = previewBoardModel.ItemCards.ToList<PreviewCardSpec>();
			List<PreviewCardSpec> list2 = previewBoardModel.SkillCards.ToList<PreviewCardSpec>();
			BppLog.Debug("MonsterPreviewDebugController", string.Format("Monster DB hit encounterId={0} key={1} shortId={2} title={3} boardCards={4} skillCards={5} previewCards={6}", new object[]
			{
				"4a4542cd",
				monsterInfo.EncounterKey,
				monsterInfo.EncounterShortId,
				monsterInfo.Title,
				monsterInfo.BoardCards.Count,
				monsterInfo.Skills.Count,
				list.Count
			}));
			string text = MonsterPreviewDebugController.BuildSignature(list);
			string text2 = MonsterPreviewDebugController.BuildSignature(list2);
			if (text == this._lastCardSignature && text2 == this._lastSkillSignature)
			{
				BppLog.Debug("MonsterPreviewDebugController", "Monster preview signature unchanged encounterId=4a4542cd");
				return;
			}
			this._lastCardSignature = text;
			this._lastSkillSignature = text2;
			this._overlayController.SetCards(list);
			this._overlayController.SetSkillCards(list2);
		}

		// Token: 0x06000171 RID: 369 RVA: 0x000091B8 File Offset: 0x000073B8
		private void SyncCardsFromHand()
		{
			this._activeEncounterId = string.Empty;
			this._activeMonsterTitle = string.Empty;
			Run run = Data.Run;
			IPlayerInventory container;
			if (run == null)
			{
				container = null;
			}
			else
			{
				Player player = run.Player;
				container = ((player != null) ? player.Hand : null);
			}
			List<PreviewCardSpec> list = MonsterPreviewDebugController.BuildCardSpecs(GameDataReader.GetItemsAsCards(container));
			Run run2 = Data.Run;
			IEnumerable<SkillCard> skills;
			if (run2 == null)
			{
				skills = null;
			}
			else
			{
				Player player2 = run2.Player;
				skills = ((player2 != null) ? player2.Skills : null);
			}
			List<PreviewCardSpec> list2 = MonsterPreviewDebugController.BuildSkillSpecs(skills);
			string text = MonsterPreviewDebugController.BuildSignature(list);
			string text2 = MonsterPreviewDebugController.BuildSignature(list2);
			if (text == this._lastCardSignature && text2 == this._lastSkillSignature)
			{
				BppLog.Debug("MonsterPreviewDebugController", "Player hand preview signature unchanged");
				return;
			}
			this._lastCardSignature = text;
			this._lastSkillSignature = text2;
			BppLog.Debug("MonsterPreviewDebugController", string.Format("Player hand preview cards={0} skills={1}", list.Count, list2.Count));
			this._overlayController.SetCards(list);
			this._overlayController.SetSkillCards(list2);
		}

		// Token: 0x06000172 RID: 370 RVA: 0x000092B0 File Offset: 0x000074B0
		private void SeedAnchorToDefaultPose()
		{
			this._anchorStrategy.Position = MonsterPreviewDefaults.DefaultAnchorPose.Position;
			this._anchorStrategy.Rotation = MonsterPreviewDefaults.DefaultAnchorPose.Rotation;
			this._anchorSeeded = true;
			BppLog.Debug("MonsterPreviewDebugController", string.Format("Seeded anchor to fixed preview pose: {0}", this._anchorStrategy.Position));
		}

		// Token: 0x06000173 RID: 371 RVA: 0x00009312 File Offset: 0x00007512
		private void ApplyLayout()
		{
			if (this.UsingShowcaseTargets)
			{
				MonsterPreviewController overlayController = this._overlayController;
				if (overlayController == null)
				{
					return;
				}
				overlayController.Refresh();
				return;
			}
			else
			{
				MonsterPreviewController overlayController2 = this._overlayController;
				if (overlayController2 == null)
				{
					return;
				}
				overlayController2.SetPresentation(MonsterPreviewDebugController.ClonePresentation(this._presentation));
				return;
			}
		}

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x06000174 RID: 372 RVA: 0x00009348 File Offset: 0x00007548
		private bool UsingShowcaseTargets
		{
			get
			{
				return this._showcaseRuntime != null && this._showcaseRuntime.IsPreviewActive;
			}
		}

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x06000175 RID: 373 RVA: 0x00009365 File Offset: 0x00007565
		private FixedAnchorStrategy ActiveAnchorStrategy
		{
			get
			{
				if (!this.UsingShowcaseTargets)
				{
					return this._anchorStrategy;
				}
				return this._showcaseRuntime.AnchorStrategy;
			}
		}

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x06000176 RID: 374 RVA: 0x00009381 File Offset: 0x00007581
		private PreviewBoardPresentation ActivePresentation
		{
			get
			{
				if (!this.UsingShowcaseTargets)
				{
					return this._presentation;
				}
				return this._showcaseRuntime.Presentation;
			}
		}

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x06000177 RID: 375 RVA: 0x0000939D File Offset: 0x0000759D
		private MonsterPreviewDebugTuner ActiveTuner
		{
			get
			{
				if (!this.UsingShowcaseTargets)
				{
					return this._tuner;
				}
				return this._showcaseRuntime.DebugTuner;
			}
		}

		// Token: 0x06000178 RID: 376 RVA: 0x000093BC File Offset: 0x000075BC
		private static List<PreviewCardSpec> BuildCardSpecs(List<Card> cards)
		{
			List<PreviewCardSpec> list = new List<PreviewCardSpec>();
			if (cards == null)
			{
				return list;
			}
			foreach (Card card in cards)
			{
				if (card != null && card.Type == ECardType.Item)
				{
					List<PreviewCardSpec> list2 = list;
					PreviewCardSpec previewCardSpec = new PreviewCardSpec();
					previewCardSpec.TemplateId = card.TemplateId.ToString();
					previewCardSpec.Tier = (int)card.Tier;
					ITCard template = card.Template;
					previewCardSpec.SourceName = (((template != null) ? template.InternalName : null) ?? string.Empty);
					ItemCard itemCard = card as ItemCard;
					EEnchantmentType? eenchantmentType;
					previewCardSpec.Enchant = (((itemCard != null) ? ((itemCard.Enchantment != null) ? eenchantmentType.GetValueOrDefault().ToString() : null) : null) ?? "None");
					previewCardSpec.Size = Math.Max(1, (int)card.Size);
					Dictionary<ECardAttributeType, int> attributes = card.Attributes;
					Dictionary<int, int> dictionary;
					if (attributes == null)
					{
						dictionary = null;
					}
					else
					{
						dictionary = attributes.ToDictionary((KeyValuePair<ECardAttributeType, int> kv) => (int)kv.Key, (KeyValuePair<ECardAttributeType, int> kv) => kv.Value);
					}
					previewCardSpec.Attributes = (dictionary ?? new Dictionary<int, int>());
					list2.Add(previewCardSpec);
				}
			}
			return list;
		}

		// Token: 0x06000179 RID: 377 RVA: 0x0000953C File Offset: 0x0000773C
		private static List<PreviewCardSpec> BuildSkillSpecs(IEnumerable<SkillCard> skills)
		{
			List<PreviewCardSpec> list = new List<PreviewCardSpec>();
			if (skills == null)
			{
				return list;
			}
			foreach (SkillCard skillCard in skills)
			{
				if (skillCard != null && skillCard.Type == ECardType.Skill)
				{
					List<PreviewCardSpec> list2 = list;
					PreviewCardSpec previewCardSpec = new PreviewCardSpec();
					previewCardSpec.TemplateId = skillCard.TemplateId.ToString();
					previewCardSpec.Tier = (int)skillCard.Tier;
					ITCard template = skillCard.Template;
					previewCardSpec.SourceName = (((template != null) ? template.InternalName : null) ?? string.Empty);
					previewCardSpec.Size = 1;
					previewCardSpec.Enchant = "None";
					Dictionary<ECardAttributeType, int> attributes = skillCard.Attributes;
					Dictionary<int, int> dictionary;
					if (attributes == null)
					{
						dictionary = null;
					}
					else
					{
						dictionary = attributes.ToDictionary((KeyValuePair<ECardAttributeType, int> kv) => (int)kv.Key, (KeyValuePair<ECardAttributeType, int> kv) => kv.Value);
					}
					previewCardSpec.Attributes = (dictionary ?? new Dictionary<int, int>());
					list2.Add(previewCardSpec);
				}
			}
			return list;
		}

		// Token: 0x0600017A RID: 378 RVA: 0x00009664 File Offset: 0x00007864
		private static string BuildSignature(IReadOnlyList<PreviewCardSpec> cards)
		{
			return string.Join("|", cards.Select(delegate(PreviewCardSpec card)
			{
				string separator = ";";
				string[] array = new string[6];
				array[0] = card.TemplateId;
				array[1] = (card.SourceName ?? string.Empty);
				array[2] = card.Tier.ToString();
				array[3] = card.Size.ToString();
				array[4] = (card.Enchant ?? "None");
				array[5] = string.Join(",", from kv in card.Attributes
				orderby kv.Key
				select string.Format("{0}:{1}", kv.Key, kv.Value));
				return string.Join(separator, array);
			}));
		}

		// Token: 0x0600017B RID: 379 RVA: 0x00009698 File Offset: 0x00007898
		private static PreviewBoardPresentation ClonePresentation(PreviewBoardPresentation presentation)
		{
			return new PreviewBoardPresentation
			{
				Visible = presentation.Visible,
				DebugEnabled = presentation.DebugEnabled,
				LocalOffset = presentation.LocalOffset,
				CardSpacing = presentation.CardSpacing,
				CardScale = presentation.CardScale,
				BoardSize = presentation.BoardSize,
				SkillBoardWidth = presentation.SkillBoardWidth,
				BoardThickness = presentation.BoardThickness,
				BorderThickness = presentation.BorderThickness,
				BorderHeight = presentation.BorderHeight
			};
		}

		// Token: 0x0600017C RID: 380 RVA: 0x00009724 File Offset: 0x00007924
		private static void InitStyles()
		{
			if (MonsterPreviewDebugController._stylesInitialized)
			{
				return;
			}
			MonsterPreviewDebugController.WidgetButtonStyle.normal.background = Texture2D.whiteTexture;
			MonsterPreviewDebugController.WidgetButtonStyle.normal.textColor = new Color(0.95f, 0.97f, 1f);
			MonsterPreviewDebugController.WidgetButtonStyle.fontSize = 12;
			MonsterPreviewDebugController.WidgetButtonStyle.fontStyle = 1;
			MonsterPreviewDebugController.WidgetButtonStyle.alignment = 4;
			MonsterPreviewDebugController.WidgetButtonStyle.padding = new RectOffset(10, 10, 6, 6);
			MonsterPreviewDebugController.WidgetButtonStyle.border = new RectOffset(10, 10, 10, 10);
			MonsterPreviewDebugController.PanelStyle.normal.background = Texture2D.whiteTexture;
			MonsterPreviewDebugController.PanelStyle.border = new RectOffset(14, 14, 14, 14);
			MonsterPreviewDebugController.TitleStyle.normal.textColor = new Color(0.96f, 0.97f, 0.99f);
			MonsterPreviewDebugController.TitleStyle.fontSize = 14;
			MonsterPreviewDebugController.TitleStyle.fontStyle = 1;
			MonsterPreviewDebugController.SubtitleStyle.normal.textColor = new Color(0.7f, 0.76f, 0.84f);
			MonsterPreviewDebugController.SubtitleStyle.fontSize = 11;
			MonsterPreviewDebugController.SectionStyle.normal.textColor = new Color(0.82f, 0.87f, 0.94f);
			MonsterPreviewDebugController.SectionStyle.fontSize = 11;
			MonsterPreviewDebugController.SectionStyle.fontStyle = 1;
			MonsterPreviewDebugController.StepperButtonStyle.normal.background = Texture2D.whiteTexture;
			MonsterPreviewDebugController.StepperButtonStyle.normal.textColor = new Color(0.94f, 0.96f, 0.99f);
			MonsterPreviewDebugController.StepperButtonStyle.fontSize = 12;
			MonsterPreviewDebugController.StepperButtonStyle.fontStyle = 1;
			MonsterPreviewDebugController.StepperButtonStyle.alignment = 4;
			MonsterPreviewDebugController.StepperButtonStyle.margin = new RectOffset(0, 4, 2, 2);
			MonsterPreviewDebugController.ActionButtonStyle.normal.background = Texture2D.whiteTexture;
			MonsterPreviewDebugController.ActionButtonStyle.normal.textColor = new Color(0.94f, 0.96f, 0.99f);
			MonsterPreviewDebugController.ActionButtonStyle.fontSize = 11;
			MonsterPreviewDebugController.ActionButtonStyle.fontStyle = 1;
			MonsterPreviewDebugController.ActionButtonStyle.alignment = 4;
			MonsterPreviewDebugController.ActionButtonStyle.padding = new RectOffset(8, 8, 6, 6);
			MonsterPreviewDebugController.ActionButtonStyle.margin = new RectOffset(0, 6, 0, 4);
			MonsterPreviewDebugController.RowLabelStyle.normal.textColor = new Color(0.84f, 0.89f, 0.96f);
			MonsterPreviewDebugController.RowLabelStyle.fontSize = 11;
			MonsterPreviewDebugController.RowLabelStyle.alignment = 3;
			MonsterPreviewDebugController.ValueStyle.normal.textColor = new Color(0.98f, 0.98f, 0.99f);
			MonsterPreviewDebugController.ValueStyle.fontSize = 11;
			MonsterPreviewDebugController.ValueStyle.alignment = 4;
			MonsterPreviewDebugController._stylesInitialized = true;
		}

		// Token: 0x0600017D RID: 381 RVA: 0x000099F1 File Offset: 0x00007BF1
		private static void DrawTintedBox(Rect rect, GUIStyle style, Color color)
		{
			Color color2 = GUI.color;
			GUI.color = color;
			GUI.Box(rect, GUIContent.none, style);
			GUI.color = color2;
		}

		// Token: 0x0600017E RID: 382 RVA: 0x00009A10 File Offset: 0x00007C10
		private static bool DrawTintedButton(Rect rect, string label, GUIStyle style, Color color)
		{
			Color backgroundColor = GUI.backgroundColor;
			GUI.backgroundColor = color;
			bool result = GUI.Button(rect, label, style);
			GUI.backgroundColor = backgroundColor;
			return result;
		}

		// Token: 0x0600017F RID: 383 RVA: 0x00009A38 File Offset: 0x00007C38
		private static bool DrawTintedButton(string label, GUIStyle style, Color color, params GUILayoutOption[] options)
		{
			Color backgroundColor = GUI.backgroundColor;
			GUI.backgroundColor = color;
			bool result = GUILayout.Button(label, style, options);
			GUI.backgroundColor = backgroundColor;
			return result;
		}

		// Token: 0x040000A8 RID: 168
		private const float RefreshInterval = 0.2f;

		// Token: 0x040000A9 RID: 169
		private const float MoveStep = 0.5f;

		// Token: 0x040000AA RID: 170
		private const float RotationStep = 5f;

		// Token: 0x040000AB RID: 171
		private const float SizeStep = 0.25f;

		// Token: 0x040000AC RID: 172
		private const float ThicknessStep = 0.02f;

		// Token: 0x040000AD RID: 173
		private const float ScaleStep = 0.05f;

		// Token: 0x040000AE RID: 174
		private const float WidgetWidth = 92f;

		// Token: 0x040000AF RID: 175
		private const float WidgetButtonHeight = 30f;

		// Token: 0x040000B0 RID: 176
		private const float PanelWidth = 260f;

		// Token: 0x040000B1 RID: 177
		private const float PanelPadding = 12f;

		// Token: 0x040000B2 RID: 178
		private static readonly Color WidgetButtonColor = new Color(0.1f, 0.13f, 0.18f, 0.96f);

		// Token: 0x040000B3 RID: 179
		private static readonly Color WidgetButtonActiveColor = new Color(0.16f, 0.34f, 0.29f, 0.98f);

		// Token: 0x040000B4 RID: 180
		private static readonly Color PanelColor = new Color(0.07f, 0.09f, 0.13f, 0.96f);

		// Token: 0x040000B5 RID: 181
		private static readonly Color ActionButtonColor = new Color(0.15f, 0.2f, 0.28f, 1f);

		// Token: 0x040000B6 RID: 182
		private static readonly Color StepperButtonColor = new Color(0.18f, 0.24f, 0.32f, 1f);

		// Token: 0x040000B7 RID: 183
		private static readonly GUIStyle WidgetButtonStyle = new GUIStyle();

		// Token: 0x040000B8 RID: 184
		private static readonly GUIStyle PanelStyle = new GUIStyle();

		// Token: 0x040000B9 RID: 185
		private static readonly GUIStyle TitleStyle = new GUIStyle();

		// Token: 0x040000BA RID: 186
		private static readonly GUIStyle SubtitleStyle = new GUIStyle();

		// Token: 0x040000BB RID: 187
		private static readonly GUIStyle SectionStyle = new GUIStyle();

		// Token: 0x040000BC RID: 188
		private static readonly GUIStyle StepperButtonStyle = new GUIStyle();

		// Token: 0x040000BD RID: 189
		private static readonly GUIStyle ActionButtonStyle = new GUIStyle();

		// Token: 0x040000BE RID: 190
		private static readonly GUIStyle RowLabelStyle = new GUIStyle();

		// Token: 0x040000BF RID: 191
		private static readonly GUIStyle ValueStyle = new GUIStyle();

		// Token: 0x040000C0 RID: 192
		private static bool _stylesInitialized;

		// Token: 0x040000C1 RID: 193
		private MonsterPreviewController _overlayController;

		// Token: 0x040000C2 RID: 194
		private MonsterLockShowcaseRuntime _showcaseRuntime;

		// Token: 0x040000C3 RID: 195
		private FixedAnchorStrategy _anchorStrategy;

		// Token: 0x040000C4 RID: 196
		private PreviewBoardPresentation _presentation;

		// Token: 0x040000C5 RID: 197
		private string _lastCardSignature = string.Empty;

		// Token: 0x040000C6 RID: 198
		private string _lastSkillSignature = string.Empty;

		// Token: 0x040000C7 RID: 199
		private float _nextRefreshTime;

		// Token: 0x040000C8 RID: 200
		private bool _anchorSeeded;

		// Token: 0x040000C9 RID: 201
		private bool _useMonsterDatabase = true;

		// Token: 0x040000CA RID: 202
		private const string DefaultEncounterId = "4a4542cd";

		// Token: 0x040000CB RID: 203
		private string _activeEncounterId = string.Empty;

		// Token: 0x040000CC RID: 204
		private string _activeMonsterTitle = string.Empty;

		// Token: 0x040000CD RID: 205
		private bool _widgetExpanded;

		// Token: 0x040000CE RID: 206
		private readonly PreviewBoardDebugOptions _debugOptions = new PreviewBoardDebugOptions
		{
			Enabled = true,
			ShowAnchorPoint = true,
			ShowItemSlots = true,
			ShowSkillSlots = true,
			ShowCardBounds = true,
			ShowLabels = true
		};

		// Token: 0x040000CF RID: 207
		private MonsterPreviewDebugTuner _tuner;

		// Token: 0x02000074 RID: 116
		internal struct DebugState
		{
			// Token: 0x040001C7 RID: 455
			public string DataSource;

			// Token: 0x040001C8 RID: 456
			public string EncounterId;

			// Token: 0x040001C9 RID: 457
			public string MonsterTitle;

			// Token: 0x040001CA RID: 458
			public bool Visible;

			// Token: 0x040001CB RID: 459
			public Vector3 AnchorPosition;

			// Token: 0x040001CC RID: 460
			public Vector3 AnchorRotationEuler;

			// Token: 0x040001CD RID: 461
			public Vector3 LocalOffset;

			// Token: 0x040001CE RID: 462
			public Vector2 BoardSize;

			// Token: 0x040001CF RID: 463
			public float CardSpacingX;

			// Token: 0x040001D0 RID: 464
			public float CardScale;

			// Token: 0x040001D1 RID: 465
			public float BoardThickness;

			// Token: 0x040001D2 RID: 466
			public float BorderThickness;

			// Token: 0x040001D3 RID: 467
			public float BorderHeight;
		}
	}
}
