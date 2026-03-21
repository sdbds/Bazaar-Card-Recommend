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
	// Token: 0x0200002E RID: 46
	internal sealed class MonsterPreviewSkillCardFactory : IPreviewCardFactory
	{
		// Token: 0x060001F2 RID: 498 RVA: 0x0000CCDC File Offset: 0x0000AEDC
		public Task<GameObject> CreateCardAsync(PreviewCardSpec spec, Transform parent)
		{
			MonsterPreviewSkillCardFactory.<CreateCardAsync>d__3 <CreateCardAsync>d__;
			<CreateCardAsync>d__.<>t__builder = AsyncTaskMethodBuilder<GameObject>.Create();
			<CreateCardAsync>d__.<>4__this = this;
			<CreateCardAsync>d__.spec = spec;
			<CreateCardAsync>d__.parent = parent;
			<CreateCardAsync>d__.<>1__state = -1;
			<CreateCardAsync>d__.<>t__builder.Start<MonsterPreviewSkillCardFactory.<CreateCardAsync>d__3>(ref <CreateCardAsync>d__);
			return <CreateCardAsync>d__.<>t__builder.Task;
		}

		// Token: 0x060001F3 RID: 499 RVA: 0x0000CD2F File Offset: 0x0000AF2F
		public Task UpdateCardAsync(GameObject cardObject, PreviewCardSpec spec)
		{
			return Task.CompletedTask;
		}

		// Token: 0x060001F4 RID: 500 RVA: 0x0000CD38 File Offset: 0x0000AF38
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
			SkillController skillController;
			CardController cardController;
			if (cardObject.TryGetComponent<SkillController>(out skillController))
			{
				skillController.Cleanup();
				skillController.EnableMovement(true);
			}
			else if (cardObject.TryGetComponent<CardController>(out cardController))
			{
				cardController.EnableMovement(true);
			}
			cardObject.transform.localScale = Vector3.one;
			if (PreviewCardLifecyclePolicy.ShouldReturnToPool(PreviewCardKind.Skill))
			{
				cardObject.PoolObject(false);
				return;
			}
			Object.Destroy(cardObject);
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x0000CDB8 File Offset: 0x0000AFB8
		private static SkillCard BuildCard(PreviewCardSpec spec, object staticData)
		{
			if (spec == null || string.IsNullOrWhiteSpace(spec.TemplateId))
			{
				return null;
			}
			Guid templateId;
			if (!Guid.TryParse(spec.TemplateId, out templateId))
			{
				return null;
			}
			ITCard itcard = MonsterPreviewSkillCardFactory.GetTemplate(staticData, templateId) as ITCard;
			if (itcard == null)
			{
				return null;
			}
			SkillCard skillCard = new SkillCard
			{
				InstanceId = InstanceId.New("ppskill"),
				TemplateId = templateId,
				Template = itcard,
				Tier = (ETier)Mathf.Clamp(spec.Tier, 0, 5),
				Type = ECardType.Skill,
				Attributes = new Dictionary<ECardAttributeType, int>(),
				Tags = new HashSet<ECardTag>(),
				HiddenTags = new HashSet<EHiddenTag>(),
				Heroes = new HashSet<EHero>(),
				Owner = null,
				Section = null,
				LeftSocketId = null
			};
			if (spec.Attributes != null)
			{
				foreach (KeyValuePair<int, int> keyValuePair in spec.Attributes)
				{
					if (Enum.IsDefined(typeof(ECardAttributeType), keyValuePair.Key))
					{
						skillCard.Attributes[(ECardAttributeType)keyValuePair.Key] = keyValuePair.Value;
					}
				}
			}
			return skillCard;
		}

		// Token: 0x060001F6 RID: 502 RVA: 0x0000CF0C File Offset: 0x0000B10C
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

		// Token: 0x060001F7 RID: 503 RVA: 0x0000CF60 File Offset: 0x0000B160
		private void ConfigureSpawned(GameObject cardObject)
		{
			SkillController skillController;
			if (cardObject.TryGetComponent<SkillController>(out skillController))
			{
				skillController.ShowCard(true);
				skillController.EnableMovement(false);
				return;
			}
			CardController cardController;
			if (cardObject.TryGetComponent<CardController>(out cardController))
			{
				cardController.ShowCard(true);
				cardController.EnableMovement(false);
			}
		}

		// Token: 0x060001F8 RID: 504 RVA: 0x0000CFA0 File Offset: 0x0000B1A0
		private static Task RefreshSpawnedSkillAsync(GameObject cardObject, SkillCard card)
		{
			MonsterPreviewSkillCardFactory.<RefreshSpawnedSkillAsync>d__9 <RefreshSpawnedSkillAsync>d__;
			<RefreshSpawnedSkillAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<RefreshSpawnedSkillAsync>d__.cardObject = cardObject;
			<RefreshSpawnedSkillAsync>d__.card = card;
			<RefreshSpawnedSkillAsync>d__.<>1__state = -1;
			<RefreshSpawnedSkillAsync>d__.<>t__builder.Start<MonsterPreviewSkillCardFactory.<RefreshSpawnedSkillAsync>d__9>(ref <RefreshSpawnedSkillAsync>d__);
			return <RefreshSpawnedSkillAsync>d__.<>t__builder.Task;
		}

		// Token: 0x060001F9 RID: 505 RVA: 0x0000CFEC File Offset: 0x0000B1EC
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
				return false;
			}
			Type parameterType = this._instantiateCardMethod.GetParameters()[2].ParameterType;
			if (!parameterType.IsEnum)
			{
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
				this._spawnSection = Enum.ToObject(parameterType, 0);
			}
			else
			{
				this._spawnSection = Enum.ToObject(parameterType, 0);
			}
			return this._spawnSection != null;
		}

		// Token: 0x060001FA RID: 506 RVA: 0x0000D0F0 File Offset: 0x0000B2F0
		private Task<GameObject> InstantiateAsync(AssetLoader loader, Card card, GameObject parent)
		{
			MonsterPreviewSkillCardFactory.<InstantiateAsync>d__11 <InstantiateAsync>d__;
			<InstantiateAsync>d__.<>t__builder = AsyncTaskMethodBuilder<GameObject>.Create();
			<InstantiateAsync>d__.<>4__this = this;
			<InstantiateAsync>d__.loader = loader;
			<InstantiateAsync>d__.card = card;
			<InstantiateAsync>d__.parent = parent;
			<InstantiateAsync>d__.<>1__state = -1;
			<InstantiateAsync>d__.<>t__builder.Start<MonsterPreviewSkillCardFactory.<InstantiateAsync>d__11>(ref <InstantiateAsync>d__);
			return <InstantiateAsync>d__.<>t__builder.Task;
		}

		// Token: 0x0400011D RID: 285
		private MethodInfo _instantiateCardMethod;

		// Token: 0x0400011E RID: 286
		private object _spawnSection;

		// Token: 0x0400011F RID: 287
		private object _staticData;
	}
}
