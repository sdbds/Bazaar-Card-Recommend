using System;
using System.Collections.Generic;
using BazaarGameClient.Domain.Models.Cards;
using BazaarGameShared.Domain.Cards.Enchantments;
using BazaarGameShared.Domain.Core.Types;

namespace BazaarPlusPlus.Game.ItemEnchantPreview.Preview
{
	// Token: 0x02000057 RID: 87
	public static class ItemEnchantPreviewSnapshotFactory
	{
		// Token: 0x0600029B RID: 667 RVA: 0x0000FDD8 File Offset: 0x0000DFD8
		public static ItemEnchantPreviewSnapshot Create(ItemCard itemCard, EEnchantmentType previewEnchantment, TEnchantment previewTemplate)
		{
			Dictionary<ECardAttributeType, int> dictionary = new Dictionary<ECardAttributeType, int>(itemCard.Attributes);
			if (((previewTemplate != null) ? previewTemplate.Attributes : null) != null)
			{
				foreach (KeyValuePair<ECardAttributeType, int> keyValuePair in previewTemplate.Attributes)
				{
					dictionary[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return new ItemEnchantPreviewSnapshot
			{
				InstanceId = itemCard.InstanceId.ToString(),
				TemplateId = itemCard.TemplateId.ToString(),
				Section = itemCard.Section,
				CurrentEnchantment = itemCard.Enchantment,
				PreviewEnchantment = previewEnchantment,
				PreviewAttributes = dictionary
			};
		}
	}
}
