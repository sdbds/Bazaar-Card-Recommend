using System;
using UnityEngine;

namespace BazaarPlusPlus
{
	// Token: 0x02000029 RID: 41
	internal sealed class MonsterPreviewDebugTuner
	{
		// Token: 0x060001A6 RID: 422 RVA: 0x00009EAF File Offset: 0x000080AF
		public MonsterPreviewDebugTuner(FixedAnchorStrategy anchor, PreviewBoardPresentation presentation)
		{
			this._anchor = anchor;
			this._presentation = presentation;
		}

		// Token: 0x060001A7 RID: 423 RVA: 0x00009EC5 File Offset: 0x000080C5
		public void MoveAnchor(Vector3 delta)
		{
			this._anchor.Position += delta;
		}

		// Token: 0x060001A8 RID: 424 RVA: 0x00009EDE File Offset: 0x000080DE
		public void RotateAnchorY(float degrees)
		{
			this._anchor.Rotation = Quaternion.Euler(0f, degrees, 0f) * this._anchor.Rotation;
		}

		// Token: 0x060001A9 RID: 425 RVA: 0x00009F0B File Offset: 0x0000810B
		public void ResetAnchor(BoardPose pose)
		{
			if (pose == null)
			{
				return;
			}
			this._anchor.Position = pose.Position;
			this._anchor.Rotation = pose.Rotation;
		}

		// Token: 0x060001AA RID: 426 RVA: 0x00009F33 File Offset: 0x00008133
		public void AdjustBoardWidth(float delta)
		{
			this._presentation.BoardSize = new Vector2(Math.Max(1f, this._presentation.BoardSize.x + delta), this._presentation.BoardSize.y);
		}

		// Token: 0x060001AB RID: 427 RVA: 0x00009F71 File Offset: 0x00008171
		public void AdjustBoardHeight(float delta)
		{
			this._presentation.BoardSize = new Vector2(this._presentation.BoardSize.x, Math.Max(1f, this._presentation.BoardSize.y + delta));
		}

		// Token: 0x060001AC RID: 428 RVA: 0x00009FB0 File Offset: 0x000081B0
		public void AdjustSpacingX(float delta)
		{
			this._presentation.CardSpacing = new Vector3(Math.Max(0.2f, this._presentation.CardSpacing.x + delta), this._presentation.CardSpacing.y, this._presentation.CardSpacing.z);
		}

		// Token: 0x060001AD RID: 429 RVA: 0x0000A009 File Offset: 0x00008209
		public void AdjustCardScale(float delta)
		{
			this._presentation.CardScale = Vector3.one * Math.Max(0.1f, this._presentation.CardScale.x + delta);
		}

		// Token: 0x060001AE RID: 430 RVA: 0x0000A03C File Offset: 0x0000823C
		public void AdjustBoardThickness(float delta)
		{
			this._presentation.BoardThickness = Math.Max(0.01f, this._presentation.BoardThickness + delta);
		}

		// Token: 0x060001AF RID: 431 RVA: 0x0000A060 File Offset: 0x00008260
		public void AdjustBorderThickness(float delta)
		{
			this._presentation.BorderThickness = Math.Max(0.01f, this._presentation.BorderThickness + delta);
		}

		// Token: 0x060001B0 RID: 432 RVA: 0x0000A084 File Offset: 0x00008284
		public void AdjustBorderHeight(float delta)
		{
			this._presentation.BorderHeight = Math.Max(0.01f, this._presentation.BorderHeight + delta);
		}

		// Token: 0x040000D0 RID: 208
		private const float MinBoardSize = 1f;

		// Token: 0x040000D1 RID: 209
		private const float MinSpacing = 0.2f;

		// Token: 0x040000D2 RID: 210
		private const float MinScale = 0.1f;

		// Token: 0x040000D3 RID: 211
		private const float MinThickness = 0.01f;

		// Token: 0x040000D4 RID: 212
		private readonly FixedAnchorStrategy _anchor;

		// Token: 0x040000D5 RID: 213
		private readonly PreviewBoardPresentation _presentation;
	}
}
