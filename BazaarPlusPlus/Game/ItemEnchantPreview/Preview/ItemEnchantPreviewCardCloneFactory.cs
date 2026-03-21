using System;
using System.Collections.Generic;
using BazaarGameClient.Domain.Models.Cards;
using BazaarGameShared.Domain.Core.Types;

namespace BazaarPlusPlus.Game.ItemEnchantPreview.Preview
{
	// Token: 0x02000054 RID: 84
	public static class ItemEnchantPreviewCardCloneFactory
	{
		// Token: 0x06000287 RID: 647 RVA: 0x0000F938 File Offset: 0x0000DB38
		public static ItemCard Create(ItemCard source, ItemEnchantPreviewSnapshot snapshot)
		{
			return new ItemCard
			{
				InstanceId = source.InstanceId,
				TemplateId = source.TemplateId,
				Attributes = new Dictionary<ECardAttributeType, int>(snapshot.PreviewAttributes),
				Heroes = new HashSet<EHero>(source.Heroes),
				HiddenTags = new HashSet<EHiddenTag>(source.HiddenTags),
				Size = source.Size,
				Tags = new HashSet<ECardTag>(source.Tags),
				Tier = source.Tier,
				Type = source.Type,
				Owner = source.Owner,
				LeftSocketId = source.LeftSocketId,
				Section = source.Section,
				State = source.State,
				Template = source.Template,
				Enchantment = new EEnchantmentType?(snapshot.PreviewEnchantment)
			};
		}
	}
}
