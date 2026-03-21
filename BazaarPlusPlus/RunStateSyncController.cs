using System;
using UnityEngine;

namespace BazaarPlusPlus
{
	// Token: 0x02000037 RID: 55
	internal sealed class RunStateSyncController : MonoBehaviour
	{
		// Token: 0x06000230 RID: 560 RVA: 0x0000E2E5 File Offset: 0x0000C4E5
		private void OnEnable()
		{
			ModState.RefreshRunStateFromCurrentState();
		}

		// Token: 0x06000231 RID: 561 RVA: 0x0000E2EC File Offset: 0x0000C4EC
		private void Update()
		{
			if (Time.unscaledTime < this._nextRefreshAt)
			{
				return;
			}
			this._nextRefreshAt = Time.unscaledTime + 0.25f;
			ModState.RefreshRunStateFromCurrentState();
		}

		// Token: 0x04000134 RID: 308
		private const float RefreshIntervalSeconds = 0.25f;

		// Token: 0x04000135 RID: 309
		private float _nextRefreshAt;
	}
}
