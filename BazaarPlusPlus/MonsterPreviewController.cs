using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BazaarPlusPlus
{
	// Token: 0x02000032 RID: 50
	internal sealed class MonsterPreviewController : MonoBehaviour
	{
		// Token: 0x17000054 RID: 84
		// (get) Token: 0x06000217 RID: 535 RVA: 0x0000DCAC File Offset: 0x0000BEAC
		public bool Visible
		{
			get
			{
				return this._visible;
			}
		}

		// Token: 0x06000218 RID: 536 RVA: 0x0000DCB4 File Offset: 0x0000BEB4
		private void Awake()
		{
			this._presentation = new PreviewBoardPresentation();
			this._renderTarget = new MonsterPreviewBoardRenderTarget();
			this._coordinator = new MonsterPreviewOverlayCoordinator(this._renderTarget);
			this._coordinator.SetPresentation(this._presentation);
			BppLog.Info("MonsterPreviewController", "Awake completed; render target and coordinator created");
		}

		// Token: 0x06000219 RID: 537 RVA: 0x0000DD08 File Offset: 0x0000BF08
		private void LateUpdate()
		{
			if (!this._visible)
			{
				MonsterPreviewOverlayCoordinator coordinator = this._coordinator;
				if (coordinator == null)
				{
					return;
				}
				coordinator.SetVisible(false);
				return;
			}
			else
			{
				MonsterPreviewOverlayCoordinator coordinator2 = this._coordinator;
				if (coordinator2 == null)
				{
					return;
				}
				coordinator2.Tick();
				return;
			}
		}

		// Token: 0x0600021A RID: 538 RVA: 0x0000DD34 File Offset: 0x0000BF34
		public void SetAnchorStrategy(IBoardAnchorStrategy anchorStrategy)
		{
			this._anchorStrategy = anchorStrategy;
			MonsterPreviewOverlayCoordinator coordinator = this._coordinator;
			if (coordinator != null)
			{
				coordinator.SetAnchorStrategy(anchorStrategy);
			}
			BppLog.Debug("MonsterPreviewController", "Anchor strategy set: " + (((anchorStrategy != null) ? anchorStrategy.GetType().Name : null) ?? "null"));
		}

		// Token: 0x0600021B RID: 539 RVA: 0x0000DD88 File Offset: 0x0000BF88
		public void SetPresentation(PreviewBoardPresentation presentation)
		{
			this._presentation = (presentation ?? new PreviewBoardPresentation());
			MonsterPreviewOverlayCoordinator coordinator = this._coordinator;
			if (coordinator != null)
			{
				coordinator.SetPresentation(this._presentation);
			}
			BppLog.Debug("MonsterPreviewController", string.Format("Presentation updated: size={0}, offset={1}, spacing={2}, scale={3}", new object[]
			{
				this._presentation.BoardSize,
				this._presentation.LocalOffset,
				this._presentation.CardSpacing,
				this._presentation.CardScale
			}));
		}

		// Token: 0x0600021C RID: 540 RVA: 0x0000DE24 File Offset: 0x0000C024
		public void SetCards(IReadOnlyList<PreviewCardSpec> cards)
		{
			this._cards.Clear();
			if (cards != null)
			{
				this._cards.AddRange(MonsterPreviewController.CloneCards(cards));
			}
			BppLog.Debug("MonsterPreviewController", string.Format("SetCards count={0}, visible={1}", this._cards.Count, this._visible));
			MonsterPreviewOverlayCoordinator coordinator = this._coordinator;
			if (coordinator == null)
			{
				return;
			}
			coordinator.SetCards(this._cards);
		}

		// Token: 0x0600021D RID: 541 RVA: 0x0000DE98 File Offset: 0x0000C098
		public void SetSkillCards(IReadOnlyList<PreviewCardSpec> cards)
		{
			this._skillCards.Clear();
			if (cards != null)
			{
				this._skillCards.AddRange(MonsterPreviewController.CloneCards(cards));
			}
			BppLog.Debug("MonsterPreviewController", string.Format("SetSkillCards count={0}, visible={1}", this._skillCards.Count, this._visible));
			MonsterPreviewOverlayCoordinator coordinator = this._coordinator;
			if (coordinator == null)
			{
				return;
			}
			coordinator.SetSkillCards(this._skillCards);
		}

		// Token: 0x0600021E RID: 542 RVA: 0x0000DF09 File Offset: 0x0000C109
		public void SetDebugOptions(PreviewBoardDebugOptions debugOptions)
		{
			MonsterPreviewOverlayCoordinator coordinator = this._coordinator;
			if (coordinator == null)
			{
				return;
			}
			coordinator.SetDebugOptions(debugOptions);
		}

		// Token: 0x0600021F RID: 543 RVA: 0x0000DF1C File Offset: 0x0000C11C
		public void ShowRequest(PreviewBoardRequest request)
		{
			bool? flag;
			if (request == null)
			{
				flag = null;
			}
			else
			{
				PreviewBoardPresentation presentation = request.Presentation;
				flag = ((presentation != null) ? new bool?(presentation.Visible) : null);
			}
			bool? flag2 = flag;
			this._visible = flag2.GetValueOrDefault();
			string component = "MonsterPreviewController";
			string format = "ShowRequest visible={0} items={1} skills={2} hasDataSource={3} hasAnchor={4}";
			object[] array = new object[5];
			array[0] = this._visible;
			int num = 1;
			int? num2;
			if (request == null)
			{
				num2 = null;
			}
			else
			{
				PreviewBoardModel initialModel = request.InitialModel;
				if (initialModel == null)
				{
					num2 = null;
				}
				else
				{
					IReadOnlyList<PreviewCardSpec> itemCards = initialModel.ItemCards;
					num2 = ((itemCards != null) ? new int?(itemCards.Count) : null);
				}
			}
			int? num3 = num2;
			array[num] = num3.GetValueOrDefault(-1);
			int num4 = 2;
			int? num5;
			if (request == null)
			{
				num5 = null;
			}
			else
			{
				PreviewBoardModel initialModel2 = request.InitialModel;
				if (initialModel2 == null)
				{
					num5 = null;
				}
				else
				{
					IReadOnlyList<PreviewCardSpec> skillCards = initialModel2.SkillCards;
					num5 = ((skillCards != null) ? new int?(skillCards.Count) : null);
				}
			}
			num3 = num5;
			array[num4] = num3.GetValueOrDefault(-1);
			array[3] = (((request != null) ? request.DataSource : null) != null);
			array[4] = (((request != null) ? request.AnchorStrategy : null) != null);
			BppLog.Info(component, string.Format(format, array));
			MonsterPreviewOverlayCoordinator coordinator = this._coordinator;
			if (coordinator == null)
			{
				return;
			}
			coordinator.ShowRequest(request);
		}

		// Token: 0x06000220 RID: 544 RVA: 0x0000E06F File Offset: 0x0000C26F
		public void HidePreview()
		{
			BppLog.Info("MonsterPreviewController", "HidePreview called");
			this.SetVisible(false);
		}

		// Token: 0x06000221 RID: 545 RVA: 0x0000E087 File Offset: 0x0000C287
		public void ClearCards()
		{
			this._cards.Clear();
			this._skillCards.Clear();
			BppLog.Debug("MonsterPreviewController", "ClearCards");
			MonsterPreviewOverlayCoordinator coordinator = this._coordinator;
			if (coordinator == null)
			{
				return;
			}
			coordinator.ClearCards();
		}

		// Token: 0x06000222 RID: 546 RVA: 0x0000E0C0 File Offset: 0x0000C2C0
		public void SetVisible(bool visible)
		{
			if (this._visible == visible)
			{
				BppLog.Debug("MonsterPreviewController", string.Format("SetVisible ignored: already {0}", visible));
				return;
			}
			this._visible = visible;
			BppLog.Info("MonsterPreviewController", string.Format("SetVisible visible={0}", this._visible));
			if (!this._visible)
			{
				MonsterPreviewOverlayCoordinator coordinator = this._coordinator;
				if (coordinator == null)
				{
					return;
				}
				coordinator.SetVisible(false);
				return;
			}
			else
			{
				MonsterPreviewOverlayCoordinator coordinator2 = this._coordinator;
				if (coordinator2 != null)
				{
					coordinator2.SetVisible(true);
				}
				if (this._anchorStrategy != null)
				{
					MonsterPreviewOverlayCoordinator coordinator3 = this._coordinator;
					if (coordinator3 != null)
					{
						coordinator3.SetAnchorStrategy(this._anchorStrategy);
					}
				}
				MonsterPreviewOverlayCoordinator coordinator4 = this._coordinator;
				if (coordinator4 != null)
				{
					coordinator4.SetPresentation(this._presentation);
				}
				MonsterPreviewOverlayCoordinator coordinator5 = this._coordinator;
				if (coordinator5 != null)
				{
					coordinator5.SetCards(this._cards);
				}
				MonsterPreviewOverlayCoordinator coordinator6 = this._coordinator;
				if (coordinator6 == null)
				{
					return;
				}
				coordinator6.SetSkillCards(this._skillCards);
				return;
			}
		}

		// Token: 0x06000223 RID: 547 RVA: 0x0000E1A6 File Offset: 0x0000C3A6
		public void Refresh()
		{
			MonsterPreviewOverlayCoordinator coordinator = this._coordinator;
			if (coordinator == null)
			{
				return;
			}
			coordinator.Refresh();
		}

		// Token: 0x06000224 RID: 548 RVA: 0x0000E1B8 File Offset: 0x0000C3B8
		private void OnDestroy()
		{
			MonsterPreviewBoardRenderTarget renderTarget = this._renderTarget;
			if (renderTarget != null)
			{
				renderTarget.Dispose();
			}
			this._renderTarget = null;
			this._coordinator = null;
		}

		// Token: 0x06000225 RID: 549 RVA: 0x0000E1D9 File Offset: 0x0000C3D9
		private static List<PreviewCardSpec> CloneCards(IReadOnlyList<PreviewCardSpec> cards)
		{
			return (from card in cards
			select new PreviewCardSpec
			{
				TemplateId = card.TemplateId,
				Tier = card.Tier,
				SourceName = card.SourceName,
				Enchant = card.Enchant,
				Size = card.Size,
				Attributes = ((card.Attributes != null) ? new Dictionary<int, int>(card.Attributes) : new Dictionary<int, int>())
			}).ToList<PreviewCardSpec>();
		}

		// Token: 0x0400012C RID: 300
		private readonly List<PreviewCardSpec> _cards = new List<PreviewCardSpec>();

		// Token: 0x0400012D RID: 301
		private readonly List<PreviewCardSpec> _skillCards = new List<PreviewCardSpec>();

		// Token: 0x0400012E RID: 302
		private MonsterPreviewOverlayCoordinator _coordinator;

		// Token: 0x0400012F RID: 303
		private MonsterPreviewBoardRenderTarget _renderTarget;

		// Token: 0x04000130 RID: 304
		private IBoardAnchorStrategy _anchorStrategy;

		// Token: 0x04000131 RID: 305
		private PreviewBoardPresentation _presentation;

		// Token: 0x04000132 RID: 306
		private bool _visible;
	}
}
