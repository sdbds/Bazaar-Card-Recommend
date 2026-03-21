using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BazaarGameClient.Domain.Models.Cards;
using BazaarGameShared.Domain.Cards;
using BazaarGameShared.Domain.Core;
using BazaarGameShared.Domain.Core.Types;
using UnityEngine;

namespace BazaarPlusPlus
{
	// Token: 0x0200002D RID: 45
	internal sealed class MonsterPreviewItemCardFactory : IPreviewCardFactory
	{
		// Token: 0x060001E7 RID: 487 RVA: 0x0000C6FC File Offset: 0x0000A8FC
		public Task<GameObject> CreateCardAsync(PreviewCardSpec spec, Transform parent)
		{
			MonsterPreviewItemCardFactory.<CreateCardAsync>d__3 <CreateCardAsync>d__;
			<CreateCardAsync>d__.<>t__builder = AsyncTaskMethodBuilder<GameObject>.Create();
			<CreateCardAsync>d__.<>4__this = this;
			<CreateCardAsync>d__.spec = spec;
			<CreateCardAsync>d__.parent = parent;
			<CreateCardAsync>d__.<>1__state = -1;
			<CreateCardAsync>d__.<>t__builder.Start<MonsterPreviewItemCardFactory.<CreateCardAsync>d__3>(ref <CreateCardAsync>d__);
			return <CreateCardAsync>d__.<>t__builder.Task;
		}

		// Token: 0x060001E8 RID: 488 RVA: 0x0000C74F File Offset: 0x0000A94F
		public Task UpdateCardAsync(GameObject cardObject, PreviewCardSpec spec)
		{
			return Task.CompletedTask;
		}

		// Token: 0x060001E9 RID: 489 RVA: 0x0000C758 File Offset: 0x0000A958
		public void DestroyCard(GameObject cardObject)
		{
			if (cardObject == null)
			{
				return;
			}
			ShowcaseCardMarker component = cardObject.GetComponent<ShowcaseCardMarker>();
			if (component != null)
			{
				Object.Destroy(component);
			}
			ItemController itemController;
			CardController cardController;
			if (cardObject.TryGetComponent<ItemController>(out itemController))
			{
				itemController.Cleanup();
				itemController.EnableMovement(true);
			}
			else if (cardObject.TryGetComponent<CardController>(out cardController))
			{
				cardController.EnableMovement(true);
			}
			cardObject.transform.localScale = Vector3.one;
			if (PreviewCardLifecyclePolicy.ShouldReturnToPool(PreviewCardKind.Item))
			{
				cardObject.PoolObject(false);
				return;
			}
			Object.Destroy(cardObject);
		}

		// Token: 0x060001EA RID: 490 RVA: 0x0000C7D8 File Offset: 0x0000A9D8
		private static ItemCard BuildCard(PreviewCardSpec entry, object staticData)
		{
			if (entry == null || string.IsNullOrWhiteSpace(entry.TemplateId))
			{
				BppLog.Warn("MonsterPreviewItemCardFactory", "Empty preview card spec");
				return null;
			}
			Guid templateId;
			if (!Guid.TryParse(entry.TemplateId, out templateId))
			{
				BppLog.Warn("MonsterPreviewItemCardFactory", "Invalid template id: " + entry.TemplateId);
				return null;
			}
			ITCard itcard = MonsterPreviewItemCardFactory.GetTemplate(staticData, templateId) as ITCard;
			if (itcard == null)
			{
				BppLog.Warn("MonsterPreviewItemCardFactory", "Template not found: " + entry.TemplateId);
				return null;
			}
			ItemCard itemCard = new ItemCard
			{
				InstanceId = InstanceId.New("ppmon"),
				TemplateId = templateId,
				Template = itcard,
				Tier = (ETier)Mathf.Clamp(entry.Tier, 0, 5),
				Size = MonsterPreviewItemCardFactory.ParseSize(entry.Size, itcard.Size),
				Type = ECardType.Item,
				Attributes = new Dictionary<ECardAttributeType, int>(),
				Tags = new HashSet<ECardTag>(),
				HiddenTags = new HashSet<EHiddenTag>(),
				Heroes = new HashSet<EHero>(),
				Owner = null,
				Section = null,
				LeftSocketId = null
			};
			EEnchantmentType value;
			if (!string.IsNullOrWhiteSpace(entry.Enchant) && !string.Equals(entry.Enchant, "None", StringComparison.OrdinalIgnoreCase) && Enum.TryParse<EEnchantmentType>(entry.Enchant, out value))
			{
				itemCard.Enchantment = new EEnchantmentType?(value);
			}
			if (entry.Attributes != null)
			{
				foreach (KeyValuePair<int, int> keyValuePair in entry.Attributes)
				{
					if (Enum.IsDefined(typeof(ECardAttributeType), keyValuePair.Key))
					{
						itemCard.Attributes[(ECardAttributeType)keyValuePair.Key] = keyValuePair.Value;
					}
				}
			}
			BppLog.Debug("MonsterPreviewItemCardFactory", string.Format("BuildCard result template={0} tier={1} size={2} type={3} enchant={4} attrs={5} templateName={6}", new object[]
			{
				entry.TemplateId,
				itemCard.Tier,
				itemCard.Size,
				itemCard.Type,
				itemCard.Enchantment,
				itemCard.Attributes.Count,
				itcard.InternalName
			}));
			return itemCard;
		}

		// Token: 0x060001EB RID: 491 RVA: 0x0000CA38 File Offset: 0x0000AC38
		private static ECardSize ParseSize(int size, ECardSize fallback)
		{
			switch (size)
			{
			case 1:
				return ECardSize.Small;
			case 2:
				return ECardSize.Medium;
			case 3:
				return ECardSize.Large;
			default:
				return fallback;
			}
		}

		// Token: 0x060001EC RID: 492 RVA: 0x0000CA58 File Offset: 0x0000AC58
		private static object GetTemplate(object staticData, Guid templateId)
		{
			if (staticData == null)
			{
				return null;
			}
			MethodInfo method = staticData.GetType().GetMethod("GetCardById", BindingFlags.Instance | BindingFlags.Public, null, new Type[]
			{
				typeof(Guid)
			}, null);
			if (method == null)
			{
				return null;
			}
			return method.Invoke(staticData, new object[]
			{
				templateId
			});
		}

		// Token: 0x060001ED RID: 493 RVA: 0x0000CAAC File Offset: 0x0000ACAC
		private void ConfigureSpawned(GameObject cardObject)
		{
			ItemController itemController;
			if (cardObject.TryGetComponent<ItemController>(out itemController))
			{
				itemController.ShowCard(true);
				itemController.EnableMovement(false);
				return;
			}
			CardController cardController;
			if (cardObject.TryGetComponent<CardController>(out cardController))
			{
				cardController.EnableMovement(false);
				cardController.ShowCard(true);
			}
		}

		// Token: 0x060001EE RID: 494 RVA: 0x0000CAEC File Offset: 0x0000ACEC
		private static Task RefreshSpawnedItemAsync(GameObject cardObject, ItemCard card)
		{
			MonsterPreviewItemCardFactory.<RefreshSpawnedItemAsync>d__10 <RefreshSpawnedItemAsync>d__;
			<RefreshSpawnedItemAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<RefreshSpawnedItemAsync>d__.cardObject = cardObject;
			<RefreshSpawnedItemAsync>d__.card = card;
			<RefreshSpawnedItemAsync>d__.<>1__state = -1;
			<RefreshSpawnedItemAsync>d__.<>t__builder.Start<MonsterPreviewItemCardFactory.<RefreshSpawnedItemAsync>d__10>(ref <RefreshSpawnedItemAsync>d__);
			return <RefreshSpawnedItemAsync>d__.<>t__builder.Task;
		}

		// Token: 0x060001EF RID: 495 RVA: 0x0000CB38 File Offset: 0x0000AD38
		private bool EnsureApi(AssetLoader loader)
		{
			if (this._instantiateCardMethod != null && this._spawnSection != null)
			{
				return true;
			}
			this._instantiateCardMethod = loader.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).FirstOrDefault(delegate(MethodInfo method)
			{
				if (!string.Equals(method.Name, "InstantiateCardAsync", StringComparison.Ordinal))
				{
					return false;
				}
				ParameterInfo[] parameters = method.GetParameters();
				return parameters.Length == 3 && parameters[0].ParameterType == typeof(Card) && parameters[1].ParameterType == typeof(GameObject) && parameters[2].ParameterType.IsEnum;
			});
			if (this._instantiateCardMethod == null)
			{
				BppLog.Warn("MonsterPreviewItemCardFactory", "InstantiateCardAsync API not found");
				return false;
			}
			Type parameterType = this._instantiateCardMethod.GetParameters()[2].ParameterType;
			if (!parameterType.IsEnum)
			{
				BppLog.Warn("MonsterPreviewItemCardFactory", "Spawn section parameter is not enum");
				return false;
			}
			string[] names = Enum.GetNames(parameterType);
			if (names.Contains("Opponent"))
			{
				this._spawnSection = Enum.Parse(parameterType, "Opponent");
			}
			else if (names.Contains("Board"))
			{
				this._spawnSection = Enum.Parse(parameterType, "Board");
			}
			else if (names.Contains("Storage"))
			{
				this._spawnSection = Enum.Parse(parameterType, "Storage");
			}
			else
			{
				this._spawnSection = Enum.ToObject(parameterType, 0);
			}
			BppLog.Debug("MonsterPreviewItemCardFactory", string.Format("Resolved instantiate API with spawnSection={0}", this._spawnSection));
			return this._spawnSection != null;
		}

		// Token: 0x060001F0 RID: 496 RVA: 0x0000CC78 File Offset: 0x0000AE78
		private Task<GameObject> InstantiateAsync(AssetLoader loader, Card card, GameObject parent)
		{
			MonsterPreviewItemCardFactory.<InstantiateAsync>d__12 <InstantiateAsync>d__;
			<InstantiateAsync>d__.<>t__builder = AsyncTaskMethodBuilder<GameObject>.Create();
			<InstantiateAsync>d__.<>4__this = this;
			<InstantiateAsync>d__.loader = loader;
			<InstantiateAsync>d__.card = card;
			<InstantiateAsync>d__.parent = parent;
			<InstantiateAsync>d__.<>1__state = -1;
			<InstantiateAsync>d__.<>t__builder.Start<MonsterPreviewItemCardFactory.<InstantiateAsync>d__12>(ref <InstantiateAsync>d__);
			return <InstantiateAsync>d__.<>t__builder.Task;
		}

		// Token: 0x0400011A RID: 282
		private MethodInfo _instantiateCardMethod;

		// Token: 0x0400011B RID: 283
		private object _spawnSection;

		// Token: 0x0400011C RID: 284
		private object _staticData;
	}
}
