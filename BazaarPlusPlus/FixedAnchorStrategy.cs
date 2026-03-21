using System;
using UnityEngine;

namespace BazaarPlusPlus
{
	// Token: 0x02000015 RID: 21
	internal sealed class FixedAnchorStrategy : IBoardAnchorStrategy
	{
		// Token: 0x17000020 RID: 32
		// (get) Token: 0x060000CB RID: 203 RVA: 0x00006CD0 File Offset: 0x00004ED0
		// (set) Token: 0x060000CC RID: 204 RVA: 0x00006CD8 File Offset: 0x00004ED8
		public Vector3 Position { get; set; } = Vector3.zero;

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x060000CD RID: 205 RVA: 0x00006CE1 File Offset: 0x00004EE1
		// (set) Token: 0x060000CE RID: 206 RVA: 0x00006CE9 File Offset: 0x00004EE9
		public Quaternion Rotation { get; set; } = Quaternion.identity;

		// Token: 0x060000CF RID: 207 RVA: 0x00006CF2 File Offset: 0x00004EF2
		public FixedAnchorStrategy(BoardPose pose)
		{
			if (pose != null)
			{
				this.Position = pose.Position;
				this.Rotation = pose.Rotation;
			}
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x00006D2B File Offset: 0x00004F2B
		public FixedAnchorStrategy()
		{
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x00006D49 File Offset: 0x00004F49
		public void SetPose(BoardPose pose)
		{
			if (pose == null)
			{
				return;
			}
			this.Position = pose.Position;
			this.Rotation = pose.Rotation;
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x00006D67 File Offset: 0x00004F67
		public bool TryResolve(out BoardPose pose)
		{
			pose = new BoardPose
			{
				Position = this.Position,
				Rotation = this.Rotation
			};
			return true;
		}
	}
}
