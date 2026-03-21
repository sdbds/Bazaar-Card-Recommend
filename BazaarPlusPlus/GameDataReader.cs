using System;
using System.Collections.Generic;
using System.Linq;
using BazaarGameClient.Domain.Models;
using BazaarGameClient.Domain.Models.Cards;
using BazaarGameShared.Domain.Cards;
using BazaarGameShared.Domain.Core.Types;
using BazaarGameShared.Domain.Players;
using BazaarGameShared.Infra.Messages.GameSimEvents;
using TheBazaar;
using TheBazaar.ProfileData;

namespace BazaarPlusPlus
{
	// Token: 0x02000013 RID: 19
	internal static class GameDataReader
	{
		// Token: 0x060000C7 RID: 199 RVA: 0x000066AC File Offset: 0x000048AC
		public static RunInfo GetRunInfo()
		{
			if (Data.Run == null)
			{
				BppLog.Warn("GameDataReader", "GetRunInfo requested while Data.Run is null");
				RunInfo runInfo = new RunInfo();
				IProfile profile = Data.Profile;
				runInfo.Name = ((profile != null) ? profile.Username : null);
				runInfo.AvailableEncounters = (ModState.AvailableEncounters ?? new List<RunInfo.CardInfo>());
				runInfo.CurrentEncounterChoices = (ModState.CurrentEncounterChoices ?? new List<RunInfo.CardInfo>());
				return runInfo;
			}
			Player opponent = Data.Run.Opponent;
			string component = "GameDataReader";
			string format = "Building run snapshot: hero={0}, day={1}, opponent={2}";
			Player player = Data.Run.Player;
			BppLog.Debug(component, string.Format(format, (player != null) ? new EHero?(player.Hero) : null, Data.Run.Day, (opponent == null) ? "none" : opponent.Hero.ToString()));
			RunInfo runInfo2 = new RunInfo();
			runInfo2.Wins = Data.Run.Victories;
			runInfo2.Losses = Data.Run.Losses;
			runInfo2.Hero = Data.Run.Player.Hero.ToString();
			runInfo2.Day = (int)Data.Run.Day;
			runInfo2.Gold = Data.Run.Player.GetAttributeValue(EPlayerAttributeType.Gold);
			runInfo2.Income = Data.Run.Player.GetAttributeValue(EPlayerAttributeType.Income);
			runInfo2.Cards = GameDataReader.GetCardInfo(GameDataReader.GetItemsAsCards(Data.Run.Player.Hand));
			runInfo2.Stash = GameDataReader.GetCardInfo(GameDataReader.GetItemsAsCards(Data.Run.Player.Stash));
			runInfo2.Skills = GameDataReader.GetSkillInfo(Data.Run.Player.Skills);
			Player opponent2 = Data.Run.Opponent;
			runInfo2.OppCards = GameDataReader.GetCardInfo(GameDataReader.GetItemsAsCards((opponent2 != null) ? opponent2.Hand : null));
			Player opponent3 = Data.Run.Opponent;
			runInfo2.OppStash = GameDataReader.GetCardInfo(GameDataReader.GetItemsAsCards((opponent3 != null) ? opponent3.Stash : null));
			Player opponent4 = Data.Run.Opponent;
			runInfo2.OppSkills = GameDataReader.GetSkillInfo((opponent4 != null) ? opponent4.Skills : null);
			runInfo2.Health = Data.Run.Player.GetAttributeValue(EPlayerAttributeType.HealthMax);
			runInfo2.Shield = Data.Run.Player.GetAttributeValue(EPlayerAttributeType.Shield);
			runInfo2.Regen = Data.Run.Player.GetAttributeValue(EPlayerAttributeType.HealthRegen);
			runInfo2.Level = Data.Run.Player.GetAttributeValue(EPlayerAttributeType.Level);
			runInfo2.Prestige = Data.Run.Player.GetAttributeValue(EPlayerAttributeType.Prestige);
			IProfile profile2 = Data.Profile;
			runInfo2.Name = ((profile2 != null) ? profile2.Username : null);
			Player opponent5 = Data.Run.Opponent;
			runInfo2.OppHealth = ((opponent5 != null) ? opponent5.GetAttributeValue(EPlayerAttributeType.HealthMax) : null);
			Player opponent6 = Data.Run.Opponent;
			runInfo2.OppRegen = ((opponent6 != null) ? opponent6.GetAttributeValue(EPlayerAttributeType.HealthRegen) : null);
			Player opponent7 = Data.Run.Opponent;
			string oppName;
			if (opponent7 == null || opponent7.Hero != EHero.Common)
			{
				SimPvpOpponent simPvpOpponent = Data.SimPvpOpponent;
				oppName = ((simPvpOpponent != null) ? simPvpOpponent.Name : null);
			}
			else
			{
				oppName = "PvE";
			}
			runInfo2.OppName = oppName;
			Player opponent8 = Data.Run.Opponent;
			runInfo2.OppHero = ((opponent8 != null) ? opponent8.Hero.ToString() : null);
			Player opponent9 = Data.Run.Opponent;
			runInfo2.OppShield = ((opponent9 != null) ? opponent9.GetAttributeValue(EPlayerAttributeType.Shield) : null);
			Player opponent10 = Data.Run.Opponent;
			runInfo2.OppGold = ((opponent10 != null) ? opponent10.GetAttributeValue(EPlayerAttributeType.Gold) : null);
			Player opponent11 = Data.Run.Opponent;
			runInfo2.OppIncome = ((opponent11 != null) ? opponent11.GetAttributeValue(EPlayerAttributeType.Income) : null);
			Player opponent12 = Data.Run.Opponent;
			runInfo2.OppLevel = ((opponent12 != null) ? opponent12.GetAttributeValue(EPlayerAttributeType.Level) : null);
			Player opponent13 = Data.Run.Opponent;
			runInfo2.OppPrestige = ((opponent13 != null) ? opponent13.GetAttributeValue(EPlayerAttributeType.Prestige) : null);
			runInfo2.PlayMode = (Data.SelectedPlayMode == EPlayMode.Ranked);
			runInfo2.AvailableEncounters = (ModState.AvailableEncounters ?? new List<RunInfo.CardInfo>());
			runInfo2.CurrentEncounterChoices = (ModState.CurrentEncounterChoices ?? new List<RunInfo.CardInfo>());
			return runInfo2;
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x00006AFC File Offset: 0x00004CFC
		public static List<Card> GetItemsAsCards(IPlayerInventory container)
		{
			if (((container != null) ? container.Container : null) == null)
			{
				BppLog.Debug("GameDataReader", "Inventory container missing, returning empty card list");
				return new List<Card>();
			}
			return container.Container.GetSocketables().Cast<Card>().ToList<Card>();
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x00006B38 File Offset: 0x00004D38
		public static List<RunInfo.SkillInfo> GetSkillInfo(IEnumerable<SkillCard> skills)
		{
			List<RunInfo.SkillInfo> list = new List<RunInfo.SkillInfo>();
			if (skills == null)
			{
				BppLog.Debug("GameDataReader", "Skill collection missing, returning empty skill list");
				return list;
			}
			foreach (SkillCard skillCard in skills)
			{
				if (skillCard.Template != null)
				{
					list.Add(new RunInfo.SkillInfo
					{
						TemplateId = skillCard.TemplateId,
						Tier = skillCard.Tier,
						Name = skillCard.Template.Localization.Title.Text,
						Attributes = skillCard.Attributes
					});
				}
			}
			return list;
		}

		// Token: 0x060000CA RID: 202 RVA: 0x00006BE8 File Offset: 0x00004DE8
		public static List<RunInfo.CardInfo> GetCardInfo(List<Card> cards)
		{
			List<RunInfo.CardInfo> list = new List<RunInfo.CardInfo>();
			if (cards == null || cards.Count == 0)
			{
				return list;
			}
			foreach (Card card in cards)
			{
				List<RunInfo.CardInfo> list2 = list;
				RunInfo.CardInfo cardInfo = new RunInfo.CardInfo();
				cardInfo.TemplateId = card.TemplateId;
				cardInfo.Tier = card.Tier;
				cardInfo.Left = card.LeftSocketId;
				cardInfo.Instance = card.GetInstanceId();
				cardInfo.Attributes = card.Attributes;
				cardInfo.Tags = card.Tags;
				ITCard template = card.Template;
				cardInfo.Name = ((template != null) ? template.InternalName : null);
				cardInfo.Enchant = card.GetEnchantment().ToString();
				list2.Add(cardInfo);
			}
			return list;
		}
	}
}
