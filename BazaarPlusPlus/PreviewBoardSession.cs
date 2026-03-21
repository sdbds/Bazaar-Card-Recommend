using System;
using System.Collections.Generic;

namespace BazaarPlusPlus
{
	// Token: 0x02000022 RID: 34
	internal sealed class PreviewBoardSession
	{
		// Token: 0x06000137 RID: 311 RVA: 0x000075CE File Offset: 0x000057CE
		public PreviewBoardSession(IBoardRenderTarget renderTarget)
		{
			this._renderTarget = renderTarget;
		}

		// Token: 0x06000138 RID: 312 RVA: 0x000075F4 File Offset: 0x000057F4
		public void Show(PreviewBoardRequest request)
		{
			this._request = request;
			string component = "PreviewBoardSession";
			string format = "Show request received hasDataSource={0} hasAnchor={1} visible={2}";
			object arg = ((request != null) ? request.DataSource : null) != null;
			object arg2 = ((request != null) ? request.AnchorStrategy : null) != null;
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
			BppLog.Info(component, string.Format(format, arg, arg2, flag2.GetValueOrDefault()));
		}

		// Token: 0x06000139 RID: 313 RVA: 0x00007680 File Offset: 0x00005880
		public void Hide()
		{
			this._renderTarget.SetVisible(false);
			this._request = null;
			this._lastSignature = string.Empty;
			this._lastPresentationSignature = string.Empty;
			this._lastPose = null;
			BppLog.Info("PreviewBoardSession", "Hide called; cleared cached signature and pose");
		}

		// Token: 0x0600013A RID: 314 RVA: 0x000076CC File Offset: 0x000058CC
		public void Tick()
		{
			if (this._request == null)
			{
				return;
			}
			PreviewBoardPresentation presentation = this._request.Presentation;
			bool flag = presentation != null && presentation.Visible;
			this._renderTarget.SetVisible(flag);
			if (!flag)
			{
				BppLog.Info("PreviewBoardSession", "Tick skipped because request is hidden");
				return;
			}
			PreviewBoardModel previewBoardModel = PreviewBoardSession.ResolveModel(this._request);
			BoardPose boardPose = PreviewBoardSession.ResolvePose(this._request);
			if (previewBoardModel == null || boardPose == null)
			{
				BppLog.Info("PreviewBoardSession", string.Format("Tick skipped modelNull={0} poseNull={1}", previewBoardModel == null, boardPose == null));
				return;
			}
			string text = string.IsNullOrWhiteSpace(previewBoardModel.Signature) ? PreviewBoardSignature.Build(previewBoardModel) : previewBoardModel.Signature;
			string text2 = PreviewBoardSession.BuildPresentationSignature(this._request.Presentation);
			if (!this.ShouldRender(text, text2, boardPose))
			{
				BppLog.Info("PreviewBoardSession", "Tick skipped because signature and pose are unchanged");
				return;
			}
			previewBoardModel.Signature = text;
			BoardRenderModel renderModel = new BoardRenderModel
			{
				Data = previewBoardModel,
				Debug = this._request.Debug,
				Pose = boardPose,
				Presentation = this._request.Presentation
			};
			this._renderTarget.Render(renderModel);
			this._lastSignature = text;
			this._lastPresentationSignature = text2;
			this._lastPose = PreviewBoardSession.ClonePose(boardPose);
			string component = "PreviewBoardSession";
			string format = "Rendered signature={0} pose={1} items={2} skills={3}";
			object[] array = new object[4];
			array[0] = text;
			array[1] = boardPose.Position;
			int num = 2;
			IReadOnlyList<PreviewCardSpec> itemCards = previewBoardModel.ItemCards;
			array[num] = ((itemCards != null) ? itemCards.Count : 0);
			int num2 = 3;
			IReadOnlyList<PreviewCardSpec> skillCards = previewBoardModel.SkillCards;
			array[num2] = ((skillCards != null) ? skillCards.Count : 0);
			BppLog.Info(component, string.Format(format, array));
		}

		// Token: 0x0600013B RID: 315 RVA: 0x00007870 File Offset: 0x00005A70
		private static PreviewBoardModel ResolveModel(PreviewBoardRequest request)
		{
			PreviewBoardModel previewBoardModel;
			if (request.DataSource != null && request.DataSource.TryBuild(out previewBoardModel) && previewBoardModel != null)
			{
				return previewBoardModel;
			}
			return request.InitialModel;
		}

		// Token: 0x0600013C RID: 316 RVA: 0x000078A0 File Offset: 0x00005AA0
		private static BoardPose ResolvePose(PreviewBoardRequest request)
		{
			BoardPose boardPose;
			if (request.AnchorStrategy != null && request.AnchorStrategy.TryResolve(out boardPose) && boardPose != null)
			{
				return boardPose;
			}
			return request.Pose;
		}

		// Token: 0x0600013D RID: 317 RVA: 0x000078CF File Offset: 0x00005ACF
		private bool ShouldRender(string signature, string presentationSignature, BoardPose pose)
		{
			return signature != this._lastSignature || presentationSignature != this._lastPresentationSignature || !PreviewBoardSession.SamePose(this._lastPose, pose);
		}

		// Token: 0x0600013E RID: 318 RVA: 0x000078FE File Offset: 0x00005AFE
		private static bool SamePose(BoardPose left, BoardPose right)
		{
			return left != null && left.Position == right.Position && left.Rotation == right.Rotation;
		}

		// Token: 0x0600013F RID: 319 RVA: 0x0000792B File Offset: 0x00005B2B
		private static BoardPose ClonePose(BoardPose pose)
		{
			return new BoardPose
			{
				Position = pose.Position,
				Rotation = pose.Rotation
			};
		}

		// Token: 0x06000140 RID: 320 RVA: 0x0000794C File Offset: 0x00005B4C
		private static string BuildPresentationSignature(PreviewBoardPresentation presentation)
		{
			if (presentation == null)
			{
				presentation = new PreviewBoardPresentation();
			}
			return string.Join("|", new object[]
			{
				presentation.Visible,
				presentation.DebugEnabled,
				presentation.LocalOffset.x,
				presentation.LocalOffset.y,
				presentation.LocalOffset.z,
				presentation.CardScale.x,
				presentation.CardScale.y,
				presentation.CardScale.z,
				presentation.CardSpacing.x,
				presentation.CardSpacing.y,
				presentation.CardSpacing.z,
				presentation.BoardSize.x,
				presentation.BoardSize.y,
				presentation.SkillBoardWidth,
				presentation.BoardThickness,
				presentation.BorderThickness,
				presentation.BorderHeight
			});
		}

		// Token: 0x04000099 RID: 153
		private readonly IBoardRenderTarget _renderTarget;

		// Token: 0x0400009A RID: 154
		private PreviewBoardRequest _request;

		// Token: 0x0400009B RID: 155
		private string _lastSignature = string.Empty;

		// Token: 0x0400009C RID: 156
		private string _lastPresentationSignature = string.Empty;

		// Token: 0x0400009D RID: 157
		private BoardPose _lastPose;
	}
}
