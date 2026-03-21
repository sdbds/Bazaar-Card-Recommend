using System;
using UnityEngine;

namespace BazaarPlusPlus
{
	// Token: 0x02000016 RID: 22
	internal sealed class BoardPose
	{
		// Token: 0x17000022 RID: 34
		// (get) Token: 0x060000D3 RID: 211 RVA: 0x00006D89 File Offset: 0x00004F89
		// (set) Token: 0x060000D4 RID: 212 RVA: 0x00006D91 File Offset: 0x00004F91
		public Vector3 Position { get; set; } = Vector3.zero;

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x060000D5 RID: 213 RVA: 0x00006D9A File Offset: 0x00004F9A
		// (set) Token: 0x060000D6 RID: 214 RVA: 0x00006DA2 File Offset: 0x00004FA2
		public Quaternion Rotation { get; set; } = Quaternion.identity;
	}
}
