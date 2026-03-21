using System;
using System.Collections.Generic;
using System.Threading;
using BazaarGameShared.Domain.Core.Types;
using BazaarGameShared.Infra.Messages;
using BazaarGameShared.Infra.Messages.CombatSimEvents;
using HarmonyLib;
using TheBazaar;

namespace BazaarPlusPlus
{
	// Token: 0x0200003D RID: 61
	[HarmonyPatch(typeof(CombatSimHandler), "Simulate")]
	internal class CombatSimPatch
	{
		// Token: 0x0600025D RID: 605 RVA: 0x0000EC6C File Offset: 0x0000CE6C
		[HarmonyPrefix]
		private static void Prefix(NetMessageCombatSim message, CancellationTokenSource cancellationToken)
		{
			if (ModState.LastMessageId == message.MessageId)
			{
				return;
			}
			ModState.LastMessageId = message.MessageId;
			CombatSim data = message.Data;
			int? num;
			if (data == null)
			{
				num = null;
			}
			else
			{
				List<CombatSimFrame> frames = data.Frames;
				num = ((frames != null) ? new int?(frames.Count) : null);
			}
			int? num2 = num;
			CombatStatusBar.SetCombatFrameTotal(num2.GetValueOrDefault());
			ModState.LastVictoryCondition = ((message.Data.Winner == ECombatantId.Player) ? EVictoryCondition.Win : EVictoryCondition.Lose);
		}
	}
}
