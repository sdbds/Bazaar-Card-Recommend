using System;
using System.IO;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

namespace BazaarPlusPlus
{
	// Token: 0x0200004D RID: 77
	[BepInPlugin("BazaarPlusPlus", "BazaarPlusPlus", "1.1.0")]
	public class Plugin : BaseUnityPlugin
	{
		// Token: 0x06000275 RID: 629 RVA: 0x0000F22C File Offset: 0x0000D42C
		protected virtual void Awake()
		{
			ModState.Logger = base.Logger;
			BppLog.Info("Plugin", "Plugin BazaarPlusPlus loaded");
			this._harmony.PatchAll();
			ConfigFile config = new ConfigFile(Path.Combine(Paths.ConfigPath, "BazaarPlusPlus.cfg"), true);
			ModState.Initialize(config);
			ModState.Subscribe();
			CombatStatusBar.InitializeConfig(config);
			MonsterDatabase.Load();
			EncounterTracker.Subscribe();
			base.gameObject.AddComponent<RunStateSyncController>();
			base.gameObject.AddComponent<CombatStatusBar>();
			base.gameObject.AddComponent<MonsterPreviewController>();
			base.gameObject.AddComponent<MonsterPreviewWarmupController>();
			base.gameObject.AddComponent<MonsterLockShowcaseRuntime>();
			base.gameObject.AddComponent<TooltipModifierRefreshController>();
			if (ModState.IsDebug)
			{
				base.gameObject.AddComponent<DebugPanel>();
				base.gameObject.AddComponent<MonsterPreviewDebugController>();
			}
			BppLog.Info("Plugin", "MonsterPreview components attached");
		}

		// Token: 0x06000276 RID: 630 RVA: 0x0000F303 File Offset: 0x0000D503
		protected virtual void OnDestroy()
		{
			BppLog.Flush();
		}

		// Token: 0x0400016F RID: 367
		private readonly Harmony _harmony = new Harmony("BazaarPlusPlus");
	}
}
