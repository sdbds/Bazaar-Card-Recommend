using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BazaarGameClient.Domain.Models;
using BazaarGameShared.Domain.Core.Types;
using BepInEx.Configuration;
using BepInEx.Logging;
using TheBazaar;

namespace BazaarPlusPlus
{
	// Token: 0x0200003B RID: 59
	internal static class ModState
	{
		// Token: 0x0600024B RID: 587 RVA: 0x0000E964 File Offset: 0x0000CB64
		public static void Initialize(ConfigFile config)
		{
			ModState.IsInGameRun = false;
			ModState.EnableNameOverrideConfig = config.Bind<bool>("StreamerMode", "EnableNameOverride", false, "Whether to set the in-game display name to Anonymous");
			ModState.EnchantPreviewAlwaysShowConfig = config.Bind<bool>("EnchantPreview", "AlwaysShow", true, "Whether to always show enchant preview text in item tooltips. If disabled, hold Ctrl to show it.");
			BppLog.Info("ModState", string.Format("Configuration initialized: enableNameOverride={0}, enchantPreviewAlwaysShow={1}", ModState.EnableNameOverrideConfig.Value, ModState.EnchantPreviewAlwaysShowConfig.Value));
			ModState.CardsJsonPath = CardJsonPathResolver.GetCardsJsonPath();
			if (string.IsNullOrWhiteSpace(ModState.CardsJsonPath))
			{
				BppLog.Error("ModState", "Failed to resolve cards.json path from BepInEx game root; cards.json will be unavailable");
				return;
			}
			BppLog.Info("ModState", "cards.json path initialized: " + ModState.CardsJsonPath);
		}

		// Token: 0x0600024C RID: 588 RVA: 0x0000EA20 File Offset: 0x0000CC20
		public static void Subscribe()
		{
			Events.Event runStarted = Events.RunStarted;
			Action action;
			if ((action = ModState.<>O.<0>__OnRunStarted) == null)
			{
				action = (ModState.<>O.<0>__OnRunStarted = new Action(ModState.OnRunStarted));
			}
			runStarted.AddListener(action, null);
			Events.Event runEnded = Events.RunEnded;
			Action action2;
			if ((action2 = ModState.<>O.<1>__OnRunEnded) == null)
			{
				action2 = (ModState.<>O.<1>__OnRunEnded = new Action(ModState.OnRunEnded));
			}
			runEnded.AddListener(action2, null);
			Events.Event runInterrupted = Events.RunInterrupted;
			Action action3;
			if ((action3 = ModState.<>O.<2>__OnRunInterrupted) == null)
			{
				action3 = (ModState.<>O.<2>__OnRunInterrupted = new Action(ModState.OnRunInterrupted));
			}
			runInterrupted.AddListener(action3, null);
			BppLog.Info("ModState", "Subscribed to run lifecycle events");
		}

		// Token: 0x0600024D RID: 589 RVA: 0x0000EAAE File Offset: 0x0000CCAE
		public static void RefreshRunStateFromCurrentState()
		{
			ModState.SetInGameRun(ModState.ComputeIsInGameRun(), "Live run-state reconciliation");
		}

		// Token: 0x0600024E RID: 590 RVA: 0x0000EABF File Offset: 0x0000CCBF
		private static void OnRunStarted()
		{
			EncounterTracker.ResetEncounterState("Run started");
			ModState.SetInGameRun(true, "Run started");
		}

		// Token: 0x0600024F RID: 591 RVA: 0x0000EAD6 File Offset: 0x0000CCD6
		private static void OnRunEnded()
		{
			ModState.SetInGameRun(false, "Run ended");
		}

		// Token: 0x06000250 RID: 592 RVA: 0x0000EAE3 File Offset: 0x0000CCE3
		private static void OnRunInterrupted()
		{
			ModState.SetInGameRun(false, "Run interrupted");
		}

		// Token: 0x06000251 RID: 593 RVA: 0x0000EAF0 File Offset: 0x0000CCF0
		private static void SetInGameRun(bool inGameRun, string reason)
		{
			if (ModState.IsInGameRun == inGameRun)
			{
				return;
			}
			ModState.IsInGameRun = inGameRun;
			if (!inGameRun)
			{
				EncounterTracker.ResetEncounterState(reason);
			}
			string component = "ModState";
			string format = "{0}; IsInGameRun={1}, appState={2}, runState={3}, hasActiveRun={4}";
			object[] array = new object[5];
			array[0] = reason;
			array[1] = ModState.IsInGameRun;
			int num = 2;
			AppState currentState = AppState.CurrentState;
			array[num] = (((currentState != null) ? currentState.GetType().Name : null) ?? "null");
			int num2 = 3;
			RunState currentState2 = Data.CurrentState;
			array[num2] = (((currentState2 != null) ? currentState2.StateName.ToString() : null) ?? "null");
			array[4] = Data.HasActiveRun;
			BppLog.Debug(component, string.Format(format, array));
		}

		// Token: 0x06000252 RID: 594 RVA: 0x0000EB98 File Offset: 0x0000CD98
		private static bool ComputeIsInGameRun()
		{
			if (Data.IsInCombat)
			{
				return true;
			}
			AppState currentState = AppState.CurrentState;
			if (currentState is RunAppState)
			{
				return !currentState.IsEndOfRunState();
			}
			return currentState is ReplayState || currentState is StartRunAppState;
		}

		// Token: 0x0400013E RID: 318
		public static readonly bool IsDebug = false;

		// Token: 0x0400013F RID: 319
		public static ManualLogSource Logger;

		// Token: 0x04000140 RID: 320
		public static ConfigEntry<bool> EnableNameOverrideConfig;

		// Token: 0x04000141 RID: 321
		public static ConfigEntry<bool> EnchantPreviewAlwaysShowConfig;

		// Token: 0x04000142 RID: 322
		public static bool IsInGameRun;

		// Token: 0x04000143 RID: 323
		public static EVictoryCondition LastVictoryCondition;

		// Token: 0x04000144 RID: 324
		public static string LastMessageId = "";

		// Token: 0x04000145 RID: 325
		public static DateTime LastSentTime = DateTime.MinValue;

		// Token: 0x04000146 RID: 326
		public static readonly TimeSpan SendInterval = TimeSpan.FromSeconds(2.0);

		// Token: 0x04000147 RID: 327
		public static List<RunInfo.CardInfo> AvailableEncounters;

		// Token: 0x04000148 RID: 328
		public static List<RunInfo.CardInfo> CurrentEncounterChoices;

		// Token: 0x04000149 RID: 329
		public static List<RunInfo.MonsterPreview> EncounterMonsterPreviews;

		// Token: 0x0400014A RID: 330
		public static List<EEnchantmentType> AvailableEnchantments = new List<EEnchantmentType>();

		// Token: 0x0400014B RID: 331
		public static string CardsJsonPath;

		// Token: 0x02000087 RID: 135
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x0400023F RID: 575
			public static Action <0>__OnRunStarted;

			// Token: 0x04000240 RID: 576
			public static Action <1>__OnRunEnded;

			// Token: 0x04000241 RID: 577
			public static Action <2>__OnRunInterrupted;
		}
	}
}
