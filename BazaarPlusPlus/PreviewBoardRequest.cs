using System;

namespace BazaarPlusPlus
{
	// Token: 0x02000021 RID: 33
	internal sealed class PreviewBoardRequest
	{
		// Token: 0x1700003E RID: 62
		// (get) Token: 0x0600012A RID: 298 RVA: 0x00007534 File Offset: 0x00005734
		// (set) Token: 0x0600012B RID: 299 RVA: 0x0000753C File Offset: 0x0000573C
		public IPreviewDataSource DataSource { get; set; }

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x0600012C RID: 300 RVA: 0x00007545 File Offset: 0x00005745
		// (set) Token: 0x0600012D RID: 301 RVA: 0x0000754D File Offset: 0x0000574D
		public IBoardAnchorStrategy AnchorStrategy { get; set; }

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x0600012E RID: 302 RVA: 0x00007556 File Offset: 0x00005756
		// (set) Token: 0x0600012F RID: 303 RVA: 0x0000755E File Offset: 0x0000575E
		public PreviewBoardModel InitialModel { get; set; } = new PreviewBoardModel();

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x06000130 RID: 304 RVA: 0x00007567 File Offset: 0x00005767
		// (set) Token: 0x06000131 RID: 305 RVA: 0x0000756F File Offset: 0x0000576F
		public PreviewBoardPresentation Presentation { get; set; } = new PreviewBoardPresentation();

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x06000132 RID: 306 RVA: 0x00007578 File Offset: 0x00005778
		// (set) Token: 0x06000133 RID: 307 RVA: 0x00007580 File Offset: 0x00005780
		public PreviewBoardDebugOptions Debug { get; set; } = new PreviewBoardDebugOptions();

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x06000134 RID: 308 RVA: 0x00007589 File Offset: 0x00005789
		// (set) Token: 0x06000135 RID: 309 RVA: 0x00007591 File Offset: 0x00005791
		public BoardPose Pose { get; set; } = new BoardPose();
	}
}
