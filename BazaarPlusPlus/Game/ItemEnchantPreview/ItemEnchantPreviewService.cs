using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BazaarGameClient.Domain.Models.Cards;
using BazaarGameShared.Domain.Cards.Enchantments;
using BazaarGameShared.Domain.Core.Types;
using BazaarPlusPlus.Game.ItemEnchantPreview.Preview;
using TheBazaar;
using TheBazaar.Tooltips;

namespace BazaarPlusPlus.Game.ItemEnchantPreview
{
	// Token: 0x02000053 RID: 83
	public static class ItemEnchantPreviewService
	{
		// Token: 0x06000285 RID: 645 RVA: 0x0000F80A File Offset: 0x0000DA0A
		public static List<TooltipSegment> BuildPreviewSegments(Card card)
		{
			return ItemEnchantPreviewService.BuildPreviewSegments(card as ItemCard, ModState.AvailableEnchantments);
		}

		// Token: 0x06000286 RID: 646 RVA: 0x0000F81C File Offset: 0x0000DA1C
		public static List<TooltipSegment> BuildPreviewSegments(ItemCard itemCard, IEnumerable<EEnchantmentType> availableEnchantments)
		{
			List<TooltipSegment> result = new List<TooltipSegment>();
			if (itemCard == null || !ItemEnchantPreviewEligibility.IsEligible(itemCard, Data.IsInCombat))
			{
				return result;
			}
			ReadOnlyDictionary<EEnchantmentType, TEnchantment> enchantments = itemCard.GetEnchantments();
			if (enchantments == null || enchantments.Count == 0)
			{
				return result;
			}
			List<EEnchantmentType> availableEnchantments2 = (availableEnchantments != null) ? (from enchantment in availableEnchantments.Distinct<EEnchantmentType>()
			where enchantments.ContainsKey(enchantment)
			select enchantment).ToList<EEnchantmentType>() : null;
			IEnumerable<EEnchantmentType> enumerable = ItemEnchantPreviewCandidateSelector.SelectCandidates(itemCard.Enchantment, availableEnchantments2, enchantments.Keys);
			List<TooltipSegment> list = new List<TooltipSegment>();
			foreach (EEnchantmentType eenchantmentType in enumerable)
			{
				TEnchantment tenchantment;
				if (enchantments.TryGetValue(eenchantmentType, out tenchantment))
				{
					ItemEnchantPreviewSnapshot snapshot = ItemEnchantPreviewSnapshotFactory.Create(itemCard, eenchantmentType, tenchantment);
					List<TooltipSegment> collection;
					if (ItemEnchantPreviewCache.TryGet(snapshot, out collection))
					{
						list.AddRange(collection);
					}
					else
					{
						List<TooltipSegment> list2 = ItemEnchantPreviewRenderer.Render(ItemEnchantPreviewCardCloneFactory.Create(itemCard, snapshot), tenchantment);
						ItemEnchantPreviewCache.Save(snapshot, list2);
						list.AddRange(list2);
					}
				}
			}
			return list;
		}
	}
}
