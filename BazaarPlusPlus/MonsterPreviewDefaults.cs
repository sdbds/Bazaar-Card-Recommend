using System;
using UnityEngine;

namespace BazaarPlusPlus
{
	// Token: 0x0200001C RID: 28
	internal static class MonsterPreviewDefaults
	{
		// Token: 0x060000EA RID: 234 RVA: 0x00006F51 File Offset: 0x00005151
		public static PreviewBoardPresentation CreateDebugPresentation()
		{
			return MonsterPreviewDefaults.CreateShowcasePresentation();
		}

		// Token: 0x060000EB RID: 235 RVA: 0x00006F58 File Offset: 0x00005158
		public static PreviewBoardPresentation CreateShowcasePresentation()
		{
			return new PreviewBoardPresentation
			{
				Visible = true,
				LocalOffset = new Vector3(0f, 0.1f, 0f),
				CardSpacing = new Vector3(1.1f, 0f, 0f),
				CardScale = Vector3.one * 0.9f,
				BoardSize = new Vector2(14.5f, 3f),
				SkillBoardWidth = 1.3f,
				BoardThickness = 0.01f,
				BorderThickness = 0.02f,
				BorderHeight = 0.04f
			};
		}

		// Token: 0x04000075 RID: 117
		public static readonly BoardPose DefaultAnchorPose = new BoardPose
		{
			Position = new Vector3(0f, 6f, -1.5f),
			Rotation = Quaternion.identity
		};
	}
}
