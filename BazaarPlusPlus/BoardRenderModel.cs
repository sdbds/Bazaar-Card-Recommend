using System;

namespace BazaarPlusPlus
{
	// Token: 0x02000017 RID: 23
	internal sealed class BoardRenderModel
	{
		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060000D8 RID: 216 RVA: 0x00006DC9 File Offset: 0x00004FC9
		// (set) Token: 0x060000D9 RID: 217 RVA: 0x00006DD1 File Offset: 0x00004FD1
		public PreviewBoardModel Data { get; set; } = new PreviewBoardModel();

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060000DA RID: 218 RVA: 0x00006DDA File Offset: 0x00004FDA
		// (set) Token: 0x060000DB RID: 219 RVA: 0x00006DE2 File Offset: 0x00004FE2
		public PreviewBoardPresentation Presentation { get; set; } = new PreviewBoardPresentation();

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060000DC RID: 220 RVA: 0x00006DEB File Offset: 0x00004FEB
		// (set) Token: 0x060000DD RID: 221 RVA: 0x00006DF3 File Offset: 0x00004FF3
		public PreviewBoardDebugOptions Debug { get; set; } = new PreviewBoardDebugOptions();

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060000DE RID: 222 RVA: 0x00006DFC File Offset: 0x00004FFC
		// (set) Token: 0x060000DF RID: 223 RVA: 0x00006E04 File Offset: 0x00005004
		public BoardPose Pose { get; set; } = new BoardPose();
	}
}
