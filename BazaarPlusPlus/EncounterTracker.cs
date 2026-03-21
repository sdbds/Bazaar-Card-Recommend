using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BazaarGameClient.Domain.Models;
using BazaarGameClient.Domain.Models.Cards;
using BazaarGameShared.Domain.Cards.Encounter.Combat;
using BazaarGameShared.Domain.Core;
using BazaarGameShared.Domain.Runs;
using TheBazaar;

namespace BazaarPlusPlus
{
	// Token: 0x02000012 RID: 18
	internal static class EncounterTracker
	{
		// Token: 0x060000C2 RID: 194 RVA: 0x0000622C File Offset: 0x0000442C
		public static void Subscribe()
		{
			Events.Event<List<Card>> cardDealtSimEvent = Events.CardDealtSimEvent;
			Action<List<Card>> action;
			if ((action = EncounterTracker.<>O.<0>__OnCardDealt) == null)
			{
				action = (EncounterTracker.<>O.<0>__OnCardDealt = new Action<List<Card>>(EncounterTracker.OnCardDealt));
			}
			cardDealtSimEvent.AddListener(action, null);
			BppLog.Info("EncounterTracker", "Subscribed to CardDealtSimEvent");
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x00006264 File Offset: 0x00004464
		private static void OnCardDealt(List<Card> dealtCards)
		{
			if (!ModState.IsInGameRun)
			{
				EncounterTracker.ResetEncounterState("Ignoring card dealt outside of an active run");
				return;
			}
			RunState currentState = Data.CurrentState;
			if (currentState == null)
			{
				EncounterTracker.ResetEncounterState("CurrentState is null");
				return;
			}
			ERunState stateName = currentState.StateName;
			if (!EncounterTracker.IsSupportedSelectionState(stateName))
			{
				EncounterTracker.ResetEncounterState(string.Format("Ignoring unsupported state {0}", stateName));
				return;
			}
			List<string> selectionSet = currentState.SelectionSet;
			if (selectionSet == null || selectionSet.Count == 0)
			{
				EncounterTracker.ResetEncounterState(string.Format("SelectionSet empty in state {0}", stateName));
				return;
			}
			List<Card> list = (from id in selectionSet
			select Data.Entities.GetValueOrDefault(new InstanceId(id)) into c
			where c != null
			select c).ToList<Card>();
			if (list.Count == 0)
			{
				EncounterTracker.ResetEncounterState(string.Format("SelectionSet resolved to zero cards in state {0}", stateName));
				return;
			}
			List<RunInfo.CardInfo> cardInfo = GameDataReader.GetCardInfo(list);
			List<RunInfo.MonsterPreview> list2 = EncounterTracker.BuildMonsterPreviews(list);
			if (stateName == ERunState.Encounter)
			{
				ModState.AvailableEncounters = cardInfo;
				ModState.EncounterMonsterPreviews = ((list2.Count > 0) ? list2 : null);
				ModState.CurrentEncounterChoices = null;
				string component = "EncounterTracker";
				string format = "Updated map encounters: count={0}, monsterPreviews={1}";
				object arg = cardInfo.Count;
				List<RunInfo.MonsterPreview> encounterMonsterPreviews = ModState.EncounterMonsterPreviews;
				BppLog.Debug(component, string.Format(format, arg, (encounterMonsterPreviews != null) ? encounterMonsterPreviews.Count : 0));
			}
			else
			{
				ModState.CurrentEncounterChoices = cardInfo;
				ModState.AvailableEncounters = null;
				ModState.EncounterMonsterPreviews = ((list2.Count > 0) ? list2 : null);
				string component2 = "EncounterTracker";
				string format2 = "Updated encounter choices: state={0}, count={1}, monsterPreviews={2}";
				object arg2 = stateName;
				object arg3 = cardInfo.Count;
				List<RunInfo.MonsterPreview> encounterMonsterPreviews2 = ModState.EncounterMonsterPreviews;
				BppLog.Debug(component2, string.Format(format2, arg2, arg3, (encounterMonsterPreviews2 != null) ? encounterMonsterPreviews2.Count : 0));
			}
			BppLog.Debug("EncounterTracker", string.Format("State={0}, choiceCount={1}", stateName, list.Count));
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x00006440 File Offset: 0x00004640
		private static List<RunInfo.MonsterPreview> BuildMonsterPreviews(List<Card> cards)
		{
			List<RunInfo.MonsterPreview> list = new List<RunInfo.MonsterPreview>();
			foreach (Card card in cards)
			{
				TCardEncounterCombat tcardEncounterCombat = card.Template as TCardEncounterCombat;
				if (tcardEncounterCombat != null)
				{
					string internalName = card.Template.InternalName;
					TCombatantMonster tcombatantMonster = tcardEncounterCombat.CombatantType as TCombatantMonster;
					MonsterInfo monsterInfo;
					MonsterDatabase.TryGetByEncounterId(card.TemplateId, out monsterInfo);
					PreviewBoardModel previewBoardModel = (monsterInfo != null) ? MonsterDatabasePreviewDataSource.BuildModel(monsterInfo, "encounter_tracker_cache") : null;
					list.Add(new RunInfo.MonsterPreview
					{
						EncounterTemplateId = card.TemplateId,
						EncounterId = ((monsterInfo != null) ? monsterInfo.EncounterId : Guid.Empty),
						EncounterShortId = (((monsterInfo != null) ? monsterInfo.EncounterShortId : null) ?? string.Empty),
						EncounterName = internalName,
						Title = (((monsterInfo != null) ? monsterInfo.Title : null) ?? string.Empty),
						MonsterTemplateId = ((tcombatantMonster != null) ? tcombatantMonster.MonsterTemplateId.ToString() : null),
						CombatLevel = ((tcombatantMonster == null) ? null : new int?((int)tcombatantMonster.Level)),
						Health = ((monsterInfo != null) ? monsterInfo.Health : null),
						RewardGold = new int?(tcardEncounterCombat.RewardCombatGold),
						RewardXp = new int?(tcardEncounterCombat.RewardCombatXp),
						SandstormEnabled = new bool?(tcardEncounterCombat.SandstormEnabled),
						BoardCards = ((previewBoardModel != null) ? EncounterPreviewSpecConverter.ToCachedCards(previewBoardModel.ItemCards) : null),
						Skills = ((previewBoardModel != null) ? EncounterPreviewSpecConverter.ToCachedCards(previewBoardModel.SkillCards) : null)
					});
				}
			}
			return list;
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x00006630 File Offset: 0x00004830
		internal static bool IsSupportedSelectionState(ERunState stateName)
		{
			return stateName == ERunState.Encounter || stateName == ERunState.Choice || stateName == ERunState.Loot || stateName == ERunState.Pedestal;
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x00006644 File Offset: 0x00004844
		internal static void ResetEncounterState(string reason)
		{
			bool flag = ModState.AvailableEncounters != null || ModState.CurrentEncounterChoices != null || ModState.EncounterMonsterPreviews != null;
			ModState.AvailableEncounters = null;
			ModState.CurrentEncounterChoices = null;
			ModState.EncounterMonsterPreviews = null;
			if (flag)
			{
				BppLog.Debug("EncounterTracker", "Cleared encounter state: " + reason);
				return;
			}
			BppLog.Debug("EncounterTracker", "Encounter state already empty: " + reason);
		}

		// Token: 0x0200006B RID: 107
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x040001BA RID: 442
			public static Action<List<Card>> <0>__OnCardDealt;
		}
	}
}
