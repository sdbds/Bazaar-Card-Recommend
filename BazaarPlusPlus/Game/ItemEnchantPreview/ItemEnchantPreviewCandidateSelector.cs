using System;
using System.Collections.Generic;
using System.Linq;
using BazaarGameShared.Domain.Core.Types;

namespace BazaarPlusPlus.Game.ItemEnchantPreview
{
	// Token: 0x02000050 RID: 80
	public static class ItemEnchantPreviewCandidateSelector
	{
		// Token: 0x0600027C RID: 636 RVA: 0x0000F510 File Offset: 0x0000D710
		public static IReadOnlyList<EEnchantmentType> SelectCandidates(EEnchantmentType? currentEnchantment, IEnumerable<EEnchantmentType> availableEnchantments, IEnumerable<EEnchantmentType> allEnchantments)
		{
			List<EEnchantmentType> list = ((availableEnchantments != null) ? availableEnchantments.Distinct<EEnchantmentType>().ToList<EEnchantmentType>() : null) ?? new List<EEnchantmentType>();
			List<EEnchantmentType> list2 = ((allEnchantments != null) ? allEnchantments.Distinct<EEnchantmentType>().ToList<EEnchantmentType>() : null) ?? new List<EEnchantmentType>();
			return ((list.Count > 0) ? list : list2).Where(delegate(EEnchantmentType enchantment)
			{
				EEnchantmentType? currentEnchantment2 = currentEnchantment;
				return !(enchantment == currentEnchantment2.GetValueOrDefault() & currentEnchantment2 != null);
			}).ToList<EEnchantmentType>();
		}
	}
}
