using System;
using System.Collections.Generic;
using BazaarGameClient.Domain.Models.Cards;
using BazaarGameShared.Domain.Cards;
using TheBazaar;
using TheBazaar.Tooltips;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace BazaarPlusPlus
{
	// Token: 0x02000031 RID: 49
	internal sealed class MonsterLockShowcaseRuntime : MonoBehaviour
	{
		// Token: 0x1700004F RID: 79
		// (get) Token: 0x060001FE RID: 510 RVA: 0x0000D161 File Offset: 0x0000B361
		// (set) Token: 0x060001FF RID: 511 RVA: 0x0000D168 File Offset: 0x0000B368
		public static MonsterLockShowcaseRuntime Instance { get; private set; }

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x06000200 RID: 512 RVA: 0x0000D170 File Offset: 0x0000B370
		public bool IsPreviewActive
		{
			get
			{
				return this._lockedCard != null;
			}
		}

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x06000201 RID: 513 RVA: 0x0000D17B File Offset: 0x0000B37B
		public FixedAnchorStrategy AnchorStrategy
		{
			get
			{
				return this._anchorStrategy;
			}
		}

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x06000202 RID: 514 RVA: 0x0000D183 File Offset: 0x0000B383
		public PreviewBoardPresentation Presentation
		{
			get
			{
				return this._presentation;
			}
		}

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x06000203 RID: 515 RVA: 0x0000D18B File Offset: 0x0000B38B
		public MonsterPreviewDebugTuner DebugTuner
		{
			get
			{
				return this._tuner;
			}
		}

		// Token: 0x06000204 RID: 516 RVA: 0x0000D194 File Offset: 0x0000B394
		public MonsterLockShowcaseRuntime()
		{
			this._tuner = new MonsterPreviewDebugTuner(this._anchorStrategy, this._presentation);
		}

		// Token: 0x06000205 RID: 517 RVA: 0x0000D1EB File Offset: 0x0000B3EB
		private void Awake()
		{
			MonsterLockShowcaseRuntime.Instance = this;
			this._overlayController = base.GetComponent<MonsterPreviewController>();
			BppLog.Info("MonsterLockShowcaseRuntime", string.Format("Awake overlayControllerFound={0}", this._overlayController != null));
		}

		// Token: 0x06000206 RID: 518 RVA: 0x0000D224 File Offset: 0x0000B424
		private void OnDestroy()
		{
			if (MonsterLockShowcaseRuntime.Instance == this)
			{
				MonsterLockShowcaseRuntime.Instance = null;
			}
		}

		// Token: 0x06000207 RID: 519 RVA: 0x0000D234 File Offset: 0x0000B434
		private void Update()
		{
			if (!this.IsPreviewActive || !this._closeOnNextClickArmed)
			{
				return;
			}
			Mouse current = Mouse.current;
			if (current == null)
			{
				return;
			}
			if (current.leftButton.wasPressedThisFrame)
			{
				this.TryConsumeNextClickToClosePreview(true, false, "next global left click");
				return;
			}
			if (current.rightButton.wasPressedThisFrame)
			{
				this.TryConsumeNextClickToClosePreview(false, true, "next global right click");
			}
		}

		// Token: 0x06000208 RID: 520 RVA: 0x0000D294 File Offset: 0x0000B494
		public bool HandleLockToggle(Card card)
		{
			if (this._overlayController == null)
			{
				return false;
			}
			if (this.TryConsumeNextClickToClosePreview(false, true, "next right click"))
			{
				return true;
			}
			if (this.IsPreviewActive)
			{
				this.HideOverlay("right click toggle");
				return true;
			}
			bool isShowcaseCard = card != null && MonsterLockShowcaseRuntime.IsShowcaseCard(card);
			bool isMonsterCard = card != null && MonsterLockShowcaseRuntime.IsMonsterSourceCard(card);
			if (!this._controller.ShouldShowForLock((card != null) ? new Guid?(card.TemplateId) : null, isShowcaseCard, isMonsterCard))
			{
				return false;
			}
			PreviewBoardModel previewBoardModel;
			string text;
			if (!MonsterLockShowcaseRuntime.TryBuildPreview(card, out previewBoardModel, out text))
			{
				return false;
			}
			this._lockedCard = card;
			this._anchorStrategy.SetPose(MonsterPreviewDefaults.DefaultAnchorPose);
			MonsterLockShowcaseRuntime.CopyPresentation(MonsterPreviewDefaults.CreateShowcasePresentation(), this._presentation);
			MonsterPreviewController overlayController = this._overlayController;
			PreviewBoardModel previewModel = previewBoardModel;
			string text2;
			if (card == null)
			{
				text2 = null;
			}
			else
			{
				ITCard template = card.Template;
				text2 = ((template != null) ? template.InternalName : null);
			}
			overlayController.ShowRequest(this.CreateShowcaseRequest(previewModel, text2 ?? text, text));
			this._closeOnNextClickArmed = true;
			this._closeOnNextClickArmedFrame = Time.frameCount;
			string component = "MonsterLockShowcaseRuntime";
			string format = "Activated BPP showcase mode source={0} card={1} templateId={2} items={3} skills={4} armedFrame={5}";
			object[] array = new object[6];
			array[0] = text;
			int num = 1;
			object obj;
			if (card == null)
			{
				obj = null;
			}
			else
			{
				ITCard template2 = card.Template;
				obj = ((template2 != null) ? template2.InternalName : null);
			}
			array[num] = (obj ?? "-");
			array[2] = ((card != null) ? new Guid?(card.TemplateId) : null);
			int num2 = 3;
			int? num3;
			if (previewBoardModel == null)
			{
				num3 = null;
			}
			else
			{
				IReadOnlyList<PreviewCardSpec> itemCards = previewBoardModel.ItemCards;
				num3 = ((itemCards != null) ? new int?(itemCards.Count) : null);
			}
			int? num4 = num3;
			array[num2] = num4.GetValueOrDefault();
			int num5 = 4;
			int? num6;
			if (previewBoardModel == null)
			{
				num6 = null;
			}
			else
			{
				IReadOnlyList<PreviewCardSpec> skillCards = previewBoardModel.SkillCards;
				num6 = ((skillCards != null) ? new int?(skillCards.Count) : null);
			}
			num4 = num6;
			array[num5] = num4.GetValueOrDefault();
			array[5] = this._closeOnNextClickArmedFrame;
			BppLog.Info(component, string.Format(format, array));
			return true;
		}

		// Token: 0x06000209 RID: 521 RVA: 0x0000D488 File Offset: 0x0000B688
		public bool TryConsumeNextClickToClosePreview(PointerEventData.InputButton? button, string reason)
		{
			PointerEventData.InputButton? inputButton = button;
			PointerEventData.InputButton inputButton2 = PointerEventData.InputButton.Left;
			return this.TryConsumeNextClickToClosePreview(inputButton.GetValueOrDefault() == inputButton2 & inputButton != null, button.GetValueOrDefault() == PointerEventData.InputButton.Right, reason);
		}

		// Token: 0x0600020A RID: 522 RVA: 0x0000D4BC File Offset: 0x0000B6BC
		private void HideOverlay(string reason)
		{
			this._lockedCard = null;
			this._closeOnNextClickArmed = false;
			this._closeOnNextClickArmedFrame = -1;
			if (this._overlayController == null)
			{
				return;
			}
			this._overlayController.ClearCards();
			this._overlayController.HidePreview();
			BppLog.Info("MonsterLockShowcaseRuntime", "Hiding preview: " + reason);
		}

		// Token: 0x0600020B RID: 523 RVA: 0x0000D518 File Offset: 0x0000B718
		private static bool IsShowcaseCard(Card card)
		{
			CardAndSkillLookup cardAndSkillLookup = Data.CardAndSkillLookup;
			CardController cardController = (cardAndSkillLookup != null) ? cardAndSkillLookup.GetCardController(card) : null;
			return cardController != null && cardController.GetComponent<ShowcaseCardMarker>() != null;
		}

		// Token: 0x0600020C RID: 524 RVA: 0x0000D550 File Offset: 0x0000B750
		public bool ShouldInterceptLockToggle(Card card)
		{
			if (this._overlayController == null)
			{
				return false;
			}
			bool isShowcaseCard = card != null && MonsterLockShowcaseRuntime.IsShowcaseCard(card);
			bool isMonsterCard = card != null && MonsterLockShowcaseRuntime.IsMonsterSourceCard(card);
			return this._controller.ShouldInterceptLockToggle(this.IsPreviewActive, card != null, isShowcaseCard, isMonsterCard);
		}

		// Token: 0x0600020D RID: 525 RVA: 0x0000D5A0 File Offset: 0x0000B7A0
		private static bool IsMonsterSourceCard(Card card)
		{
			MonsterInfo monsterInfo;
			return card != null && ModState.IsInGameRun && (MonsterDatabase.TryGetByEncounterId(card.TemplateId.ToString(), out monsterInfo) || MonsterLockShowcaseRuntime.FindEncounterPreview(card) != null);
		}

		// Token: 0x0600020E RID: 526 RVA: 0x0000D5E4 File Offset: 0x0000B7E4
		private static bool TryBuildPreview(Card card, out PreviewBoardModel previewModel, out string source)
		{
			previewModel = null;
			source = string.Empty;
			if (card == null || !ModState.IsInGameRun)
			{
				return false;
			}
			MonsterInfo monsterInfo;
			if (MonsterDatabase.TryGetByEncounterId(card.TemplateId.ToString(), out monsterInfo))
			{
				PreviewBoardModel previewBoardModel = MonsterDatabasePreviewDataSource.BuildModel(monsterInfo, "monster_db");
				List<PreviewCardSpec> list = PreviewCardSpecFilter.FilterLocallyRenderable(previewBoardModel.ItemCards);
				List<PreviewCardSpec> list2 = PreviewCardSpecFilter.FilterLocallyRenderable(previewBoardModel.SkillCards);
				source = "monster_db:" + monsterInfo.EncounterShortId;
				previewModel = MonsterLockShowcaseRuntime.CreateFilteredPreviewModel(previewBoardModel, list, list2);
				string source2 = source;
				IReadOnlyList<PreviewCardSpec> itemCards = previewBoardModel.ItemCards;
				int originalItemCount = (itemCards != null) ? itemCards.Count : 0;
				int count = list.Count;
				IReadOnlyList<PreviewCardSpec> skillCards = previewBoardModel.SkillCards;
				MonsterLockShowcaseRuntime.LogFilteredPreviewCounts(card, source2, originalItemCount, count, (skillCards != null) ? skillCards.Count : 0, list2.Count);
				return list.Count > 0 || list2.Count > 0;
			}
			RunInfo.MonsterPreview monsterPreview = MonsterLockShowcaseRuntime.FindEncounterPreview(card);
			if (monsterPreview == null)
			{
				return false;
			}
			List<PreviewCardSpec> list3 = EncounterPreviewSpecConverter.BuildCachedSpecs(monsterPreview.BoardCards);
			List<PreviewCardSpec> list4 = EncounterPreviewSpecConverter.BuildCachedSpecs(monsterPreview.Skills);
			List<PreviewCardSpec> list5 = PreviewCardSpecFilter.FilterLocallyRenderable(list3);
			List<PreviewCardSpec> list6 = PreviewCardSpecFilter.FilterLocallyRenderable(list4);
			source = "encounter_tracker_cache";
			PreviewBoardModel previewBoardModel2 = new PreviewBoardModel();
			string title;
			if (!string.IsNullOrWhiteSpace(monsterPreview.Title))
			{
				title = monsterPreview.Title;
			}
			else
			{
				ITCard template = card.Template;
				title = (((template != null) ? template.InternalName : null) ?? string.Empty);
			}
			previewBoardModel2.Title = title;
			previewBoardModel2.ItemCards = list5;
			previewBoardModel2.SkillCards = list6;
			previewBoardModel2.Metadata = MonsterLockShowcaseRuntime.BuildEncounterPreviewMetadata(monsterPreview, source);
			previewModel = previewBoardModel2;
			previewModel.Signature = PreviewBoardSignature.Build(previewModel);
			MonsterLockShowcaseRuntime.LogFilteredPreviewCounts(card, source, list3.Count, list5.Count, list4.Count, list6.Count);
			return list5.Count > 0 || list6.Count > 0;
		}

		// Token: 0x0600020F RID: 527 RVA: 0x0000D7A0 File Offset: 0x0000B9A0
		private static void LogFilteredPreviewCounts(Card card, string source, int originalItemCount, int filteredItemCount, int originalSkillCount, int filteredSkillCount)
		{
			string component = "MonsterLockShowcaseRuntime";
			string format = "Preview filter source={0} encounter={1} templateId={2} items={3}/{4} skills={5}/{6}";
			object[] array = new object[7];
			array[0] = source;
			int num = 1;
			object obj;
			if (card == null)
			{
				obj = null;
			}
			else
			{
				ITCard template = card.Template;
				obj = ((template != null) ? template.InternalName : null);
			}
			array[num] = (obj ?? "-");
			array[2] = ((card != null) ? new Guid?(card.TemplateId) : null);
			array[3] = filteredItemCount;
			array[4] = originalItemCount;
			array[5] = filteredSkillCount;
			array[6] = originalSkillCount;
			BppLog.Info(component, string.Format(format, array));
		}

		// Token: 0x06000210 RID: 528 RVA: 0x0000D838 File Offset: 0x0000BA38
		private static RunInfo.MonsterPreview FindEncounterPreview(Card card)
		{
			List<RunInfo.MonsterPreview> encounterMonsterPreviews = ModState.EncounterMonsterPreviews;
			if (encounterMonsterPreviews == null || encounterMonsterPreviews.Count == 0)
			{
				return null;
			}
			ITCard template = card.Template;
			string text = (template != null) ? template.InternalName : null;
			foreach (RunInfo.MonsterPreview monsterPreview in encounterMonsterPreviews)
			{
				if (monsterPreview != null)
				{
					if (monsterPreview.EncounterTemplateId == card.TemplateId)
					{
						return monsterPreview;
					}
					if (!string.IsNullOrWhiteSpace(text) && string.Equals(monsterPreview.EncounterName, text, StringComparison.OrdinalIgnoreCase))
					{
						return monsterPreview;
					}
				}
			}
			return null;
		}

		// Token: 0x06000211 RID: 529 RVA: 0x0000D8E0 File Offset: 0x0000BAE0
		private PreviewBoardRequest CreateShowcaseRequest(PreviewBoardModel previewModel, string title, string source)
		{
			InMemoryPreviewDataSource inMemoryPreviewDataSource = new InMemoryPreviewDataSource();
			if (previewModel == null)
			{
				previewModel = new PreviewBoardModel();
			}
			inMemoryPreviewDataSource.SetCards(previewModel.ItemCards, previewModel.SkillCards);
			Dictionary<string, string> dictionary = new Dictionary<string, string>(previewModel.Metadata ?? new Dictionary<string, string>());
			dictionary["source"] = source;
			Dictionary<string, string> metadata = dictionary;
			inMemoryPreviewDataSource.SetMetadata(string.IsNullOrWhiteSpace(previewModel.Title) ? title : previewModel.Title, metadata);
			PreviewBoardPresentation previewBoardPresentation = MonsterLockShowcaseRuntime.ClonePresentation(this._presentation);
			if (!previewBoardPresentation.Visible)
			{
				BppLog.Warn("MonsterLockShowcaseRuntime", "Showcase presentation was hidden before request creation; forcing visible source=" + source + " title=" + title);
				previewBoardPresentation.Visible = true;
			}
			return new PreviewBoardRequest
			{
				DataSource = inMemoryPreviewDataSource,
				AnchorStrategy = this._anchorStrategy,
				Presentation = previewBoardPresentation,
				Debug = new PreviewBoardDebugOptions()
			};
		}

		// Token: 0x06000212 RID: 530 RVA: 0x0000D9B0 File Offset: 0x0000BBB0
		private bool TryConsumeNextClickToClosePreview(bool isLeftClick, bool isRightClick, string reason)
		{
			if (!this._controller.ShouldConsumeNextClickToClosePreview(this.IsPreviewActive, this._closeOnNextClickArmed, isLeftClick, isRightClick))
			{
				return false;
			}
			if (!NextClickCloseFrameGate.CanConsume(this._closeOnNextClickArmedFrame, Time.frameCount))
			{
				BppLog.Debug("MonsterLockShowcaseRuntime", string.Format("Ignored close consume in armed frame reason={0} armedFrame={1} currentFrame={2}", reason, this._closeOnNextClickArmedFrame, Time.frameCount));
				return false;
			}
			this.HideOverlay(reason);
			return true;
		}

		// Token: 0x06000213 RID: 531 RVA: 0x0000DA20 File Offset: 0x0000BC20
		private static PreviewBoardModel CreateFilteredPreviewModel(PreviewBoardModel sourceModel, IReadOnlyList<PreviewCardSpec> cards, IReadOnlyList<PreviewCardSpec> skillCards)
		{
			PreviewBoardModel previewBoardModel = new PreviewBoardModel();
			previewBoardModel.Title = (((sourceModel != null) ? sourceModel.Title : null) ?? string.Empty);
			previewBoardModel.ItemCards = (cards ?? new List<PreviewCardSpec>());
			previewBoardModel.SkillCards = (skillCards ?? new List<PreviewCardSpec>());
			previewBoardModel.Metadata = new Dictionary<string, string>(((sourceModel != null) ? sourceModel.Metadata : null) ?? new Dictionary<string, string>());
			previewBoardModel.Signature = PreviewBoardSignature.Build(previewBoardModel);
			return previewBoardModel;
		}

		// Token: 0x06000214 RID: 532 RVA: 0x0000DA9C File Offset: 0x0000BC9C
		private static IReadOnlyDictionary<string, string> BuildEncounterPreviewMetadata(RunInfo.MonsterPreview preview, string source)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["source"] = (source ?? string.Empty);
			dictionary["encounter"] = (((preview != null) ? preview.EncounterShortId : null) ?? string.Empty);
			dictionary["health"] = (((preview != null) ? ((preview.Health != null) ? preview.Health.GetValueOrDefault().ToString() : null) : null) ?? string.Empty);
			dictionary["reward_gold"] = (((preview != null) ? ((preview.RewardGold != null) ? preview.RewardGold.GetValueOrDefault().ToString() : null) : null) ?? string.Empty);
			dictionary["reward_xp"] = (((preview != null) ? ((preview.RewardXp != null) ? preview.RewardXp.GetValueOrDefault().ToString() : null) : null) ?? string.Empty);
			return dictionary;
		}

		// Token: 0x06000215 RID: 533 RVA: 0x0000DB90 File Offset: 0x0000BD90
		private static void CopyPresentation(PreviewBoardPresentation source, PreviewBoardPresentation destination)
		{
			destination.Visible = source.Visible;
			destination.DebugEnabled = source.DebugEnabled;
			destination.LocalOffset = source.LocalOffset;
			destination.CardScale = source.CardScale;
			destination.CardSpacing = source.CardSpacing;
			destination.BoardSize = source.BoardSize;
			destination.SkillBoardWidth = source.SkillBoardWidth;
			destination.BoardThickness = source.BoardThickness;
			destination.BorderThickness = source.BorderThickness;
			destination.BorderHeight = source.BorderHeight;
		}

		// Token: 0x06000216 RID: 534 RVA: 0x0000DC18 File Offset: 0x0000BE18
		private static PreviewBoardPresentation ClonePresentation(PreviewBoardPresentation presentation)
		{
			if (presentation == null)
			{
				presentation = new PreviewBoardPresentation();
			}
			return new PreviewBoardPresentation
			{
				Visible = presentation.Visible,
				DebugEnabled = presentation.DebugEnabled,
				LocalOffset = presentation.LocalOffset,
				CardScale = presentation.CardScale,
				CardSpacing = presentation.CardSpacing,
				BoardSize = presentation.BoardSize,
				SkillBoardWidth = presentation.SkillBoardWidth,
				BoardThickness = presentation.BoardThickness,
				BorderThickness = presentation.BorderThickness,
				BorderHeight = presentation.BorderHeight
			};
		}

		// Token: 0x04000123 RID: 291
		private readonly MonsterLockShowcaseController _controller = new MonsterLockShowcaseController();

		// Token: 0x04000124 RID: 292
		private readonly FixedAnchorStrategy _anchorStrategy = new FixedAnchorStrategy(MonsterPreviewDefaults.DefaultAnchorPose);

		// Token: 0x04000125 RID: 293
		private readonly PreviewBoardPresentation _presentation = MonsterPreviewDefaults.CreateShowcasePresentation();

		// Token: 0x04000126 RID: 294
		private readonly MonsterPreviewDebugTuner _tuner;

		// Token: 0x04000127 RID: 295
		private MonsterPreviewController _overlayController;

		// Token: 0x04000128 RID: 296
		private Card _lockedCard;

		// Token: 0x04000129 RID: 297
		private bool _closeOnNextClickArmed;

		// Token: 0x0400012A RID: 298
		private int _closeOnNextClickArmedFrame = -1;
	}
}
