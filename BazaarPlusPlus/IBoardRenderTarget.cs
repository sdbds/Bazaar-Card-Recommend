using System;

namespace BazaarPlusPlus
{
	// Token: 0x02000019 RID: 25
	internal interface IBoardRenderTarget
	{
		// Token: 0x060000E2 RID: 226
		void Render(BoardRenderModel renderModel);

		// Token: 0x060000E3 RID: 227
		void SetVisible(bool visible);
	}
}
