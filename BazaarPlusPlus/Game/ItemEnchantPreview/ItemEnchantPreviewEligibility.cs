using System;
using BazaarGameClient.Domain.Models;
using BazaarGameClient.Domain.Models.Cards;
using BazaarGameShared.Domain.Core.Types;
using TheBazaar;

namespace BazaarPlusPlus.Game.ItemEnchantPreview
{
	// Token: 0x02000051 RID: 81
	public static class ItemEnchantPreviewEligibility
	{
		// Token: 0x0600027D RID: 637 RVA: 0x0000F583 File Offset: 0x0000D783
		public static bool IsEligible(Card card, bool isInCombat)
		{
			return card != null && (ItemEnchantPreviewEligibility.IsEligible(card.Type, card.Section, isInCombat) || (card.Type == ECardType.Item && !isInCombat && ItemEnchantPreviewEligibility.IsOpponentBoardItem(card)));
		}

		// Token: 0x0600027E RID: 638 RVA: 0x0000F5B4 File Offset: 0x0000D7B4
		public static bool IsEligible(ECardType cardType, EInventorySection? section, bool isInCombat)
		{
			if (isInCombat || cardType != ECardType.Item)
			{
				return false;
			}
			EInventorySection? einventorySection = section;
			EInventorySection einventorySection2 = EInventorySection.Hand;
			return (einventorySection.GetValueOrDefault() == einventorySection2 & einventorySection != null) || section.GetValueOrDefault() == EInventorySection.Stash;
		}

		// Token: 0x0600027F RID: 639 RVA: 0x0000F5F0 File Offset: 0x0000D7F0
		private static bool IsOpponentBoardItem(Card card)
		{
			if (card.Owner == null)
			{
				return true;
			}
			Player owner = card.Owner;
			Run run = Data.Run;
			if (owner == ((run != null) ? run.Opponent : null))
			{
				EInventorySection? section = card.Section;
				EInventorySection einventorySection = EInventorySection.Hand;
				return section.GetValueOrDefault() == einventorySection & section != null;
			}
			return false;
		}
	}
}
