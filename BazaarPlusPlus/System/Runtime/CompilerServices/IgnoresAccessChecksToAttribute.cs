using System;

namespace System.Runtime.CompilerServices
{
	// Token: 0x02000058 RID: 88
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	internal sealed class IgnoresAccessChecksToAttribute : Attribute
	{
		// Token: 0x0600029C RID: 668 RVA: 0x0000FEB4 File Offset: 0x0000E0B4
		internal IgnoresAccessChecksToAttribute(string assemblyName)
		{
		}
	}
}
