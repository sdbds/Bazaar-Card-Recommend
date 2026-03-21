using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

namespace BazaarPlusPlus
{
	// Token: 0x02000033 RID: 51
	internal sealed class MonsterPreviewWarmupController : MonoBehaviour
	{
		// Token: 0x06000227 RID: 551 RVA: 0x0000E224 File Offset: 0x0000C424
		private void Start()
		{
			MonsterPreviewWarmupController.<Start>d__1 <Start>d__;
			<Start>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Start>d__.<>4__this = this;
			<Start>d__.<>1__state = -1;
			<Start>d__.<>t__builder.Start<MonsterPreviewWarmupController.<Start>d__1>(ref <Start>d__);
		}

		// Token: 0x06000228 RID: 552 RVA: 0x0000E25C File Offset: 0x0000C45C
		private static Task<bool> WarmStaticDataAsync()
		{
			MonsterPreviewWarmupController.<WarmStaticDataAsync>d__2 <WarmStaticDataAsync>d__;
			<WarmStaticDataAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<WarmStaticDataAsync>d__.<>1__state = -1;
			<WarmStaticDataAsync>d__.<>t__builder.Start<MonsterPreviewWarmupController.<WarmStaticDataAsync>d__2>(ref <WarmStaticDataAsync>d__);
			return <WarmStaticDataAsync>d__.<>t__builder.Task;
		}

		// Token: 0x04000133 RID: 307
		private bool _started;
	}
}
