using System;

namespace BazaarPlusPlus
{
	// Token: 0x02000026 RID: 38
	internal sealed class PreviewRenderGenerationGate
	{
		// Token: 0x06000152 RID: 338 RVA: 0x00007E60 File Offset: 0x00006060
		public int BeginRender(bool visible)
		{
			this._hidden = !visible;
			this._generation++;
			return this._generation;
		}

		// Token: 0x06000153 RID: 339 RVA: 0x00007E80 File Offset: 0x00006080
		public int InvalidateForHide()
		{
			this._hidden = true;
			this._generation++;
			return this._generation;
		}

		// Token: 0x06000154 RID: 340 RVA: 0x00007E9D File Offset: 0x0000609D
		public void MarkVisible()
		{
			this._hidden = false;
		}

		// Token: 0x06000155 RID: 341 RVA: 0x00007EA6 File Offset: 0x000060A6
		public void MarkDisposed()
		{
			this._disposed = true;
			this._hidden = true;
			this._generation++;
		}

		// Token: 0x06000156 RID: 342 RVA: 0x00007EC4 File Offset: 0x000060C4
		public bool ShouldCancel(int generation)
		{
			return this._disposed || this._hidden || generation != this._generation;
		}

		// Token: 0x040000A4 RID: 164
		private int _generation;

		// Token: 0x040000A5 RID: 165
		private bool _hidden;

		// Token: 0x040000A6 RID: 166
		private bool _disposed;
	}
}
