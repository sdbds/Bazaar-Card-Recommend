using System;
using System.Collections.Generic;

namespace BazaarPlusPlus
{
	// Token: 0x0200001D RID: 29
	internal sealed class MonsterPreviewOverlayCoordinator
	{
		// Token: 0x060000ED RID: 237 RVA: 0x0000702C File Offset: 0x0000522C
		public MonsterPreviewOverlayCoordinator(IBoardRenderTarget renderTarget)
		{
			this._renderTarget = renderTarget;
			this._session = new PreviewBoardSession(renderTarget);
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060000EE RID: 238 RVA: 0x00007068 File Offset: 0x00005268
		public bool Visible
		{
			get
			{
				return this._visible;
			}
		}

		// Token: 0x060000EF RID: 239 RVA: 0x00007070 File Offset: 0x00005270
		public void SetAnchorStrategy(IBoardAnchorStrategy anchorStrategy)
		{
			this._externalRequest = null;
			this._anchorStrategy = anchorStrategy;
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x00007080 File Offset: 0x00005280
		public void SetPresentation(PreviewBoardPresentation presentation)
		{
			this._externalRequest = null;
			this._presentation = (presentation ?? new PreviewBoardPresentation());
			this._presentation.Visible = this._visible;
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x000070AA File Offset: 0x000052AA
		public void SetCards(IReadOnlyList<PreviewCardSpec> cards)
		{
			this._externalRequest = null;
			this._dataSource.SetCards(cards ?? new List<PreviewCardSpec>(), this.GetSkillCardsSnapshot());
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x000070CE File Offset: 0x000052CE
		public void SetDebugOptions(PreviewBoardDebugOptions debugOptions)
		{
			this._externalRequest = null;
			this._debugOptions = (debugOptions ?? new PreviewBoardDebugOptions());
			this._presentation.DebugEnabled = this._debugOptions.Enabled;
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x000070FD File Offset: 0x000052FD
		public void SetSkillCards(IReadOnlyList<PreviewCardSpec> cards)
		{
			this._externalRequest = null;
			this._dataSource.SetCards(this.GetItemCardsSnapshot(), cards ?? new List<PreviewCardSpec>());
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x00007121 File Offset: 0x00005321
		public void ClearCards()
		{
			this._externalRequest = null;
			this._dataSource.SetCards(new List<PreviewCardSpec>(), new List<PreviewCardSpec>());
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x00007140 File Offset: 0x00005340
		public void ShowRequest(PreviewBoardRequest request)
		{
			this._externalRequest = request;
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
			if (this._visible)
			{
				this._session.Show(request);
			}
			else
			{
				this._session.Hide();
			}
			this._renderTarget.SetVisible(this._visible);
			BppLog.Info("MonsterPreviewOverlayCoordinator", string.Format("ShowRequest visible={0} hasExternalRequest={1}", this._visible, this._externalRequest != null));
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x000071EE File Offset: 0x000053EE
		public void SetVisible(bool visible)
		{
			this._visible = visible;
			this._presentation.Visible = visible;
			if (!visible)
			{
				this._session.Hide();
			}
		}

		// Token: 0x060000F7 RID: 247 RVA: 0x00007211 File Offset: 0x00005411
		public void Refresh()
		{
			if (!this._visible)
			{
				return;
			}
			this._session.Show(this.BuildRequestForCurrentMode());
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x0000722D File Offset: 0x0000542D
		public void Tick()
		{
			if (!this._visible)
			{
				return;
			}
			this._session.Show(this.BuildRequestForCurrentMode());
			this._session.Tick();
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x00007254 File Offset: 0x00005454
		private PreviewBoardRequest BuildRequestForCurrentMode()
		{
			if (this._externalRequest != null)
			{
				return this._externalRequest;
			}
			return this.BuildRequest();
		}

		// Token: 0x060000FA RID: 250 RVA: 0x0000726B File Offset: 0x0000546B
		private PreviewBoardRequest BuildRequest()
		{
			return new PreviewBoardRequest
			{
				DataSource = this._dataSource,
				AnchorStrategy = this._anchorStrategy,
				Presentation = this._presentation,
				Debug = this._debugOptions
			};
		}

		// Token: 0x060000FB RID: 251 RVA: 0x000072A4 File Offset: 0x000054A4
		private IReadOnlyList<PreviewCardSpec> GetItemCardsSnapshot()
		{
			PreviewBoardModel previewBoardModel;
			this._dataSource.TryBuild(out previewBoardModel);
			return ((previewBoardModel != null) ? previewBoardModel.ItemCards : null) ?? new List<PreviewCardSpec>();
		}

		// Token: 0x060000FC RID: 252 RVA: 0x000072D4 File Offset: 0x000054D4
		private IReadOnlyList<PreviewCardSpec> GetSkillCardsSnapshot()
		{
			PreviewBoardModel previewBoardModel;
			this._dataSource.TryBuild(out previewBoardModel);
			return ((previewBoardModel != null) ? previewBoardModel.SkillCards : null) ?? new List<PreviewCardSpec>();
		}

		// Token: 0x04000076 RID: 118
		private readonly IBoardRenderTarget _renderTarget;

		// Token: 0x04000077 RID: 119
		private readonly PreviewBoardSession _session;

		// Token: 0x04000078 RID: 120
		private readonly InMemoryPreviewDataSource _dataSource = new InMemoryPreviewDataSource();

		// Token: 0x04000079 RID: 121
		private IBoardAnchorStrategy _anchorStrategy;

		// Token: 0x0400007A RID: 122
		private PreviewBoardRequest _externalRequest;

		// Token: 0x0400007B RID: 123
		private PreviewBoardPresentation _presentation = new PreviewBoardPresentation();

		// Token: 0x0400007C RID: 124
		private PreviewBoardDebugOptions _debugOptions = new PreviewBoardDebugOptions();

		// Token: 0x0400007D RID: 125
		private bool _visible;
	}
}
