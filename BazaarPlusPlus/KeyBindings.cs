using System;
using UnityEngine.InputSystem;

namespace BazaarPlusPlus
{
	// Token: 0x02000014 RID: 20
	internal static class KeyBindings
	{
		// Token: 0x0200006D RID: 109
		internal static class Modifiers
		{
			// Token: 0x060002EF RID: 751 RVA: 0x0001025F File Offset: 0x0000E45F
			public static bool IsCtrlPressed(Keyboard keyboard)
			{
				return keyboard != null && (keyboard.leftCtrlKey.isPressed || keyboard.rightCtrlKey.isPressed);
			}

			// Token: 0x060002F0 RID: 752 RVA: 0x00010280 File Offset: 0x0000E480
			public static bool IsShiftPressed(Keyboard keyboard)
			{
				return keyboard != null && (keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed);
			}
		}

		// Token: 0x0200006E RID: 110
		internal static class Toggle
		{
			// Token: 0x1700007A RID: 122
			// (get) Token: 0x060002F1 RID: 753 RVA: 0x000102A1 File Offset: 0x0000E4A1
			public static Key DebugPanel
			{
				get
				{
					return Key.F2;
				}
			}

			// Token: 0x1700007B RID: 123
			// (get) Token: 0x060002F2 RID: 754 RVA: 0x000102A5 File Offset: 0x0000E4A5
			public static Key CombatStatusBar
			{
				get
				{
					return Key.F6;
				}
			}
		}

		// Token: 0x0200006F RID: 111
		internal static class DebugPanel
		{
			// Token: 0x1700007C RID: 124
			// (get) Token: 0x060002F3 RID: 755 RVA: 0x000102A9 File Offset: 0x0000E4A9
			public static Key SelectSummary
			{
				get
				{
					return Key.Digit1;
				}
			}

			// Token: 0x1700007D RID: 125
			// (get) Token: 0x060002F4 RID: 756 RVA: 0x000102AD File Offset: 0x0000E4AD
			public static Key SelectPreview
			{
				get
				{
					return Key.Digit2;
				}
			}

			// Token: 0x1700007E RID: 126
			// (get) Token: 0x060002F5 RID: 757 RVA: 0x000102B1 File Offset: 0x0000E4B1
			public static Key SelectRun
			{
				get
				{
					return Key.Digit3;
				}
			}

			// Token: 0x1700007F RID: 127
			// (get) Token: 0x060002F6 RID: 758 RVA: 0x000102B5 File Offset: 0x0000E4B5
			public static Key SelectEncounters
			{
				get
				{
					return Key.Digit4;
				}
			}

			// Token: 0x17000080 RID: 128
			// (get) Token: 0x060002F7 RID: 759 RVA: 0x000102B9 File Offset: 0x0000E4B9
			public static Key ToggleViewMode
			{
				get
				{
					return Key.Tab;
				}
			}
		}
	}
}
