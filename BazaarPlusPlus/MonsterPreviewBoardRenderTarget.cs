using System;
using System.Collections.Generic;
using UnityEngine;

namespace BazaarPlusPlus
{
	// Token: 0x0200002C RID: 44
	internal sealed class MonsterPreviewBoardRenderTarget : IBoardRenderTarget
	{
		// Token: 0x060001E0 RID: 480 RVA: 0x0000C319 File Offset: 0x0000A519
		public MonsterPreviewBoardRenderTarget() : this(MonsterPreviewBoardRenderTarget.CreateBoard())
		{
		}

		// Token: 0x060001E1 RID: 481 RVA: 0x0000C328 File Offset: 0x0000A528
		internal MonsterPreviewBoardRenderTarget(MonsterPreviewBoard board)
		{
			this._board = board;
			string component = "MonsterPreviewBoardRenderTarget";
			string format = "Constructed boardExists={0} boardAlive={1}";
			object arg = this._board != null;
			MonsterPreviewBoard board2 = this._board;
			BppLog.Info(component, string.Format(format, arg, board2 != null && board2.IsAlive));
		}

		// Token: 0x060001E2 RID: 482 RVA: 0x0000C386 File Offset: 0x0000A586
		public void Dispose()
		{
			this._renderGate.MarkDisposed();
			MonsterPreviewBoard board = this._board;
			if (board != null)
			{
				board.Dispose();
			}
			this._board = null;
		}

		// Token: 0x060001E3 RID: 483 RVA: 0x0000C3AC File Offset: 0x0000A5AC
		public void Render(BoardRenderModel renderModel)
		{
			if (!this.EnsureBoard())
			{
				BppLog.Info("MonsterPreviewBoardRenderTarget", "Render skipped because board could not be created");
				return;
			}
			if (renderModel == null)
			{
				renderModel = new BoardRenderModel();
			}
			PreviewBoardPresentation presentation = renderModel.Presentation;
			bool flag = presentation != null && presentation.Visible;
			int generation = this._renderGate.BeginRender(flag);
			string component = "MonsterPreviewBoardRenderTarget";
			string format = "Render visible={0} generation={1} items={2} skills={3} pose={4}";
			object[] array = new object[5];
			array[0] = flag;
			array[1] = generation;
			int num = 2;
			PreviewBoardModel data = renderModel.Data;
			int? num2;
			if (data == null)
			{
				num2 = null;
			}
			else
			{
				IReadOnlyList<PreviewCardSpec> itemCards = data.ItemCards;
				num2 = ((itemCards != null) ? new int?(itemCards.Count) : null);
			}
			int? num3 = num2;
			array[num] = num3.GetValueOrDefault();
			int num4 = 3;
			PreviewBoardModel data2 = renderModel.Data;
			int? num5;
			if (data2 == null)
			{
				num5 = null;
			}
			else
			{
				IReadOnlyList<PreviewCardSpec> skillCards = data2.SkillCards;
				num5 = ((skillCards != null) ? new int?(skillCards.Count) : null);
			}
			num3 = num5;
			array[num4] = num3.GetValueOrDefault();
			int num6 = 4;
			BoardPose pose = renderModel.Pose;
			array[num6] = ((pose != null) ? new Vector3?(pose.Position) : null);
			BppLog.Info(component, string.Format(format, array));
			this._board.SetMonsterInfo(renderModel.Data ?? new PreviewBoardModel());
			this._board.SetPresentation(renderModel.Presentation ?? new PreviewBoardPresentation());
			this._board.SetDebugOptions(renderModel.Debug ?? new PreviewBoardDebugOptions());
			this._board.UpdateAnchor(renderModel.Pose.Position, renderModel.Pose.Rotation);
			this._board.SetVisible(flag);
			MonsterPreviewBoard board = this._board;
			PreviewBoardModel data3 = renderModel.Data;
			IReadOnlyList<PreviewCardSpec> cards = ((data3 != null) ? data3.ItemCards : null) ?? new List<PreviewCardSpec>();
			PreviewBoardModel data4 = renderModel.Data;
			board.RebuildAsync(cards, ((data4 != null) ? data4.SkillCards : null) ?? new List<PreviewCardSpec>(), () => this._renderGate.ShouldCancel(generation));
		}

		// Token: 0x060001E4 RID: 484 RVA: 0x0000C5B4 File Offset: 0x0000A7B4
		public void SetVisible(bool visible)
		{
			if (!this.EnsureBoard())
			{
				BppLog.Info("MonsterPreviewBoardRenderTarget", string.Format("SetVisible({0}) skipped because board could not be created", visible));
				return;
			}
			if (!visible)
			{
				this._renderGate.InvalidateForHide();
				this._board.Clear();
			}
			else
			{
				this._renderGate.MarkVisible();
			}
			this._board.SetVisible(visible);
			BppLog.Info("MonsterPreviewBoardRenderTarget", string.Format("SetVisible visible={0}", visible));
		}

		// Token: 0x060001E5 RID: 485 RVA: 0x0000C634 File Offset: 0x0000A834
		private bool EnsureBoard()
		{
			if (this._board != null && this._board.IsAlive)
			{
				return true;
			}
			string component = "MonsterPreviewBoardRenderTarget";
			string format = "Board missing or dead; recreating boardExists={0} boardAlive={1}";
			object arg = this._board != null;
			MonsterPreviewBoard board = this._board;
			BppLog.Warn(component, string.Format(format, arg, board != null && board.IsAlive));
			this._board = MonsterPreviewBoardRenderTarget.CreateBoard();
			return this._board != null && this._board.IsAlive;
		}

		// Token: 0x060001E6 RID: 486 RVA: 0x0000C6B4 File Offset: 0x0000A8B4
		private static MonsterPreviewBoard CreateBoard()
		{
			MonsterPreviewBoard monsterPreviewBoard = new MonsterPreviewBoard("MonsterPreviewBoard", new MonsterPreviewItemCardFactory(), new MonsterPreviewSkillCardFactory());
			BppLog.Info("MonsterPreviewBoardRenderTarget", string.Format("CreateBoard created boardAlive={0}", monsterPreviewBoard != null && monsterPreviewBoard.IsAlive));
			return monsterPreviewBoard;
		}

		// Token: 0x04000118 RID: 280
		private MonsterPreviewBoard _board;

		// Token: 0x04000119 RID: 281
		private readonly PreviewRenderGenerationGate _renderGate = new PreviewRenderGenerationGate();
	}
}
