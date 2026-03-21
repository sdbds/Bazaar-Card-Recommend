using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

namespace BazaarPlusPlus
{
	// Token: 0x0200002B RID: 43
	internal sealed class MonsterPreviewBoard : IDisposable
	{
		// Token: 0x1700004E RID: 78
		// (get) Token: 0x060001B4 RID: 436 RVA: 0x0000A0A8 File Offset: 0x000082A8
		public bool IsAlive
		{
			get
			{
				return this._boardRoot != null;
			}
		}

		// Token: 0x060001B5 RID: 437 RVA: 0x0000A0B8 File Offset: 0x000082B8
		public MonsterPreviewBoard(string name, IPreviewCardFactory factory, IPreviewCardFactory skillFactory)
		{
			this._factory = factory;
			this._skillFactory = skillFactory;
			this._boardRoot = new GameObject(name);
			this._visualRoot = new GameObject(name + "_Visual");
			this._itemContentRoot = new GameObject(name + "_ItemContent");
			this._skillContentRoot = new GameObject(name + "_SkillContent");
			this._visualRoot.transform.SetParent(this._boardRoot.transform, false);
			this._itemContentRoot.transform.SetParent(this._boardRoot.transform, false);
			this._skillContentRoot.transform.SetParent(this._boardRoot.transform, false);
			this.BuildVisuals();
			this.BuildBoardSlots();
			this.EnsureSkillSlots(3);
			this.RefreshLayout();
			this.SetVisible(false);
			BppLog.Info("MonsterPreviewBoard", "Created board root='" + this._boardRoot.name + "'");
		}

		// Token: 0x060001B6 RID: 438 RVA: 0x0000A297 File Offset: 0x00008497
		public void SetPresentation(PreviewBoardPresentation presentation)
		{
			if (!this.IsAlive)
			{
				return;
			}
			this._presentation = (presentation ?? new PreviewBoardPresentation());
			this.RefreshLayout();
		}

		// Token: 0x060001B7 RID: 439 RVA: 0x0000A2B8 File Offset: 0x000084B8
		public void SetDebugOptions(PreviewBoardDebugOptions debugOptions)
		{
			if (!this.IsAlive)
			{
				return;
			}
			this._debugOptions = (debugOptions ?? new PreviewBoardDebugOptions());
			this.RefreshDebugVisuals();
		}

		// Token: 0x060001B8 RID: 440 RVA: 0x0000A2DC File Offset: 0x000084DC
		public void SetMonsterInfo(PreviewBoardModel model)
		{
			if (!this.IsAlive)
			{
				return;
			}
			IReadOnlyDictionary<string, string> metadata = (model != null) ? model.Metadata : null;
			this._monsterHealthText = MonsterPreviewBoard.GetMetadataValue(metadata, new string[]
			{
				"health",
				"hp"
			});
			this._monsterXpText = MonsterPreviewBoard.GetMetadataValue(metadata, new string[]
			{
				"reward_xp",
				"xp"
			});
			this._monsterGoldText = MonsterPreviewBoard.GetMetadataValue(metadata, new string[]
			{
				"reward_gold",
				"gold"
			});
			this.RefreshMonsterInfoTexts();
		}

		// Token: 0x060001B9 RID: 441 RVA: 0x0000A36C File Offset: 0x0000856C
		public void SetVisible(bool visible)
		{
			if (this._boardRoot != null && this._boardRoot.activeSelf != visible)
			{
				this._boardRoot.SetActive(visible);
				BppLog.Info("MonsterPreviewBoard", string.Format("SetVisible root='{0}' visible={1}", this._boardRoot.name, visible));
			}
		}

		// Token: 0x060001BA RID: 442 RVA: 0x0000A3C6 File Offset: 0x000085C6
		public void UpdateAnchor(Vector3 position, Quaternion rotation)
		{
			if (!this.IsAlive)
			{
				return;
			}
			this._boardRoot.transform.SetPositionAndRotation(position, rotation);
			this.RefreshLayout();
		}

		// Token: 0x060001BB RID: 443 RVA: 0x0000A3EC File Offset: 0x000085EC
		public Task RebuildAsync(IReadOnlyList<PreviewCardSpec> cards, IReadOnlyList<PreviewCardSpec> skillCards, Func<bool> isCancelled)
		{
			MonsterPreviewBoard.<RebuildAsync>d__74 <RebuildAsync>d__;
			<RebuildAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<RebuildAsync>d__.<>4__this = this;
			<RebuildAsync>d__.cards = cards;
			<RebuildAsync>d__.skillCards = skillCards;
			<RebuildAsync>d__.isCancelled = isCancelled;
			<RebuildAsync>d__.<>1__state = -1;
			<RebuildAsync>d__.<>t__builder.Start<MonsterPreviewBoard.<RebuildAsync>d__74>(ref <RebuildAsync>d__);
			return <RebuildAsync>d__.<>t__builder.Task;
		}

		// Token: 0x060001BC RID: 444 RVA: 0x0000A448 File Offset: 0x00008648
		public void Clear()
		{
			if (!this.IsAlive)
			{
				return;
			}
			foreach (GameObject gameObject in this._cards)
			{
				if (gameObject != null)
				{
					this._factory.DestroyCard(gameObject);
				}
			}
			this._cards.Clear();
			this._cardSizes.Clear();
			foreach (GameObject gameObject2 in this._cardCenterMarkers)
			{
				if (gameObject2 != null)
				{
					Object.Destroy(gameObject2);
				}
			}
			this._cardCenterMarkers.Clear();
			foreach (GameObject gameObject3 in this._cardAnchors)
			{
				if (gameObject3 != null)
				{
					Object.Destroy(gameObject3);
				}
			}
			this._cardAnchors.Clear();
			foreach (GameObject gameObject4 in this._skillCards)
			{
				if (gameObject4 != null)
				{
					this._skillFactory.DestroyCard(gameObject4);
				}
			}
			this._skillCards.Clear();
		}

		// Token: 0x060001BD RID: 445 RVA: 0x0000A5D0 File Offset: 0x000087D0
		public void Dispose()
		{
			this.Clear();
			if (this._boardRoot != null)
			{
				Object.Destroy(this._boardRoot);
			}
		}

		// Token: 0x060001BE RID: 446 RVA: 0x0000A5F4 File Offset: 0x000087F4
		private Task RebuildItemsAsync(IReadOnlyList<PreviewCardSpec> cards, Func<bool> isCancelled)
		{
			MonsterPreviewBoard.<RebuildItemsAsync>d__77 <RebuildItemsAsync>d__;
			<RebuildItemsAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<RebuildItemsAsync>d__.<>4__this = this;
			<RebuildItemsAsync>d__.cards = cards;
			<RebuildItemsAsync>d__.isCancelled = isCancelled;
			<RebuildItemsAsync>d__.<>1__state = -1;
			<RebuildItemsAsync>d__.<>t__builder.Start<MonsterPreviewBoard.<RebuildItemsAsync>d__77>(ref <RebuildItemsAsync>d__);
			return <RebuildItemsAsync>d__.<>t__builder.Task;
		}

		// Token: 0x060001BF RID: 447 RVA: 0x0000A648 File Offset: 0x00008848
		private Task RebuildSkillsAsync(IReadOnlyList<PreviewCardSpec> skillCards, Func<bool> isCancelled)
		{
			MonsterPreviewBoard.<RebuildSkillsAsync>d__78 <RebuildSkillsAsync>d__;
			<RebuildSkillsAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<RebuildSkillsAsync>d__.<>4__this = this;
			<RebuildSkillsAsync>d__.skillCards = skillCards;
			<RebuildSkillsAsync>d__.isCancelled = isCancelled;
			<RebuildSkillsAsync>d__.<>1__state = -1;
			<RebuildSkillsAsync>d__.<>t__builder.Start<MonsterPreviewBoard.<RebuildSkillsAsync>d__78>(ref <RebuildSkillsAsync>d__);
			return <RebuildSkillsAsync>d__.<>t__builder.Task;
		}

		// Token: 0x060001C0 RID: 448 RVA: 0x0000A69C File Offset: 0x0000889C
		private void RefreshLayout()
		{
			if (!this.IsAlive)
			{
				return;
			}
			this.RefreshVisuals();
			this.RefreshDebugVisuals();
			this._itemContentRoot.transform.localPosition = this._presentation.LocalOffset + new Vector3(0f, 0.2f, 0f);
			this._itemContentRoot.transform.localRotation = Quaternion.identity;
			this._itemContentRoot.transform.localScale = Vector3.one;
			this._skillContentRoot.transform.localPosition = this._presentation.LocalOffset + new Vector3(this._presentation.BoardSize.x * 0.5f + this._presentation.SkillBoardWidth * 0.5f + 0.04f, 0.2f, 0f);
			this._skillContentRoot.transform.localRotation = Quaternion.identity;
			this._skillContentRoot.transform.localScale = Vector3.one;
			this.RefreshBoardSlots();
			this.RefreshSkillSlots();
			for (int i = 0; i < this._cardAnchors.Count; i++)
			{
				this.RefreshCardAnchor(i);
				this.RefreshCard(i);
			}
		}

		// Token: 0x060001C1 RID: 449 RVA: 0x0000A7D4 File Offset: 0x000089D4
		private void BuildVisuals()
		{
			this._boardPlate = MonsterPreviewBoard.CreatePrimitive("BoardPlate", false, true);
			this._boardPlate.layer = LayerMask.NameToLayer("Input");
			this._boardPlate.AddComponent<PreviewBoardSurfaceMarker>();
			Renderer renderer;
			if (this._boardPlate.TryGetComponent<Renderer>(out renderer))
			{
				renderer.enabled = false;
			}
			this._boardPlate.transform.SetParent(this._visualRoot.transform, false);
			this._boardFill = MonsterPreviewBoard.CreatePrimitive("BoardFill", false, false);
			this._boardFill.transform.SetParent(this._visualRoot.transform, false);
			this._skillBoardFill = MonsterPreviewBoard.CreatePrimitive("SkillBoardFill", true, false);
			this._skillBoardFill.transform.SetParent(this._visualRoot.transform, false);
			this._brandingBoardFill = MonsterPreviewBoard.CreatePrimitive("BrandingBoardFill", true, false);
			this._brandingBoardFill.transform.SetParent(this._visualRoot.transform, false);
			this._monsterInfoBoardFill = MonsterPreviewBoard.CreatePrimitive("MonsterInfoBoardFill", true, false);
			this._monsterInfoBoardFill.transform.SetParent(this._visualRoot.transform, false);
			MonsterPreviewBoard.SetRendererSorting(this._monsterInfoBoardFill, 20);
			this._monsterInfoTextStripFill = MonsterPreviewBoard.CreatePrimitive("MonsterInfoTextStripFill", true, false);
			this._monsterInfoTextStripFill.transform.SetParent(this._visualRoot.transform, false);
			MonsterPreviewBoard.SetRendererSorting(this._monsterInfoTextStripFill, 21);
			this._boardCenterMarker = MonsterPreviewBoard.CreateMarker("BoardCenter", 0.16f, false);
			this._boardCenterMarker.transform.SetParent(this._visualRoot.transform, false);
			for (int i = 0; i < 4; i++)
			{
				GameObject gameObject = MonsterPreviewBoard.CreatePrimitive(string.Format("BoardBorder_{0}", i), false, false);
				gameObject.transform.SetParent(this._visualRoot.transform, false);
				this._borderSegments.Add(gameObject);
			}
			for (int j = 0; j < 4; j++)
			{
				GameObject gameObject2 = MonsterPreviewBoard.CreatePrimitive(string.Format("SkillBoardBorder_{0}", j), true, false);
				gameObject2.transform.SetParent(this._visualRoot.transform, false);
				this._skillBoardBorders.Add(gameObject2);
			}
			for (int k = 0; k < 4; k++)
			{
				GameObject gameObject3 = MonsterPreviewBoard.CreatePrimitive(string.Format("BrandingBoardBorder_{0}", k), true, false);
				gameObject3.transform.SetParent(this._visualRoot.transform, false);
				this._brandingBoardBorders.Add(gameObject3);
			}
			for (int l = 0; l < 4; l++)
			{
				GameObject gameObject4 = MonsterPreviewBoard.CreatePrimitive(string.Format("MonsterInfoBoardBorder_{0}", l), true, false);
				gameObject4.transform.SetParent(this._visualRoot.transform, false);
				this._monsterInfoBoardBorders.Add(gameObject4);
			}
			GameObject gameObject5 = new GameObject("BrandingText");
			gameObject5.transform.SetParent(this._visualRoot.transform, false);
			this._brandingText = gameObject5.AddComponent<TextMesh>();
			this._brandingText.text = "BazaarPlusPlus";
			this._brandingText.anchor = 4;
			this._brandingText.alignment = 1;
			this._brandingText.characterSize = 0.24f;
			this._brandingText.fontSize = 64;
			this._brandingText.font = MonsterPreviewBoard.GetBoardTextFont();
			this._brandingText.color = new Color(0.82f, 0.93f, 1f, 1f);
			MonsterPreviewBoard.SetRendererSorting(this._brandingText.gameObject, 30);
			this.CreateMonsterInfoText("MonsterInfoHealth", 0.22f, 80);
			this.CreateMonsterInfoText("MonsterInfoDivider", 0.22f, 72);
			this.CreateMonsterInfoText("MonsterInfoRewards", 0.17f, 60);
			this.RefreshMonsterInfoTexts();
		}

		// Token: 0x060001C2 RID: 450 RVA: 0x0000ABA4 File Offset: 0x00008DA4
		private void BuildBoardSlots()
		{
			for (int i = 0; i < 10; i++)
			{
				GameObject gameObject = new GameObject(string.Format("BoardSlot_{0}", i));
				gameObject.transform.SetParent(this._itemContentRoot.transform, false);
				this._boardSlots.Add(gameObject);
				GameObject gameObject2 = MonsterPreviewBoard.CreateMarker(string.Format("BoardSlotMarker_{0}", i), 0.08f, false);
				gameObject2.transform.SetParent(gameObject.transform, false);
				this._boardSlotMarkers.Add(gameObject2);
			}
		}

		// Token: 0x060001C3 RID: 451 RVA: 0x0000AC34 File Offset: 0x00008E34
		private void EnsureSkillSlots(int requestedCount)
		{
			int num = Mathf.Max(3, requestedCount);
			for (int i = this._skillSlots.Count; i < num; i++)
			{
				GameObject gameObject = new GameObject(string.Format("SkillSlot_{0}", i));
				gameObject.transform.SetParent(this._skillContentRoot.transform, false);
				this._skillSlots.Add(gameObject);
				GameObject gameObject2 = MonsterPreviewBoard.CreatePrimitive(string.Format("SkillSlotMarker_{0}", i), true, false);
				gameObject2.transform.SetParent(gameObject.transform, false);
				this._skillSlotMarkers.Add(gameObject2);
			}
		}

		// Token: 0x060001C4 RID: 452 RVA: 0x0000ACD0 File Offset: 0x00008ED0
		private void RefreshBoardSlots()
		{
			for (int i = 0; i < this._boardSlots.Count; i++)
			{
				GameObject gameObject = this._boardSlots[i];
				if (!(gameObject == null))
				{
					gameObject.transform.localPosition = new Vector3(this.GetBoardSlotCenterX(i), 0f, 0f);
					gameObject.transform.localRotation = Quaternion.identity;
					gameObject.transform.localScale = Vector3.one;
				}
			}
		}

		// Token: 0x060001C5 RID: 453 RVA: 0x0000AD4C File Offset: 0x00008F4C
		private void RefreshSkillSlots()
		{
			for (int i = 0; i < this._skillSlots.Count; i++)
			{
				GameObject gameObject = this._skillSlots[i];
				if (!(gameObject == null))
				{
					gameObject.transform.localPosition = new Vector3(0f, 0f, this.GetSkillSlotCenterZ(i));
					gameObject.transform.localRotation = Quaternion.identity;
					gameObject.transform.localScale = Vector3.one;
					MonsterPreviewBoard.SetActive(gameObject, i < this._activeSkillSlotCount);
					if (i < this._skillSlotMarkers.Count && this._skillSlotMarkers[i] != null)
					{
						this._skillSlotMarkers[i].transform.localPosition = Vector3.zero;
						this._skillSlotMarkers[i].transform.localRotation = Quaternion.identity;
						this._skillSlotMarkers[i].transform.localScale = new Vector3(Mathf.Max(0.01f, this._presentation.SkillBoardWidth - 0.24f), 0.03f, Mathf.Max(0.01f, this.GetSkillSlotDepth() - 0.12f));
						MonsterPreviewBoard.SetActive(this._skillSlotMarkers[i], i < this._activeSkillSlotCount);
					}
				}
			}
		}

		// Token: 0x060001C6 RID: 454 RVA: 0x0000AEA8 File Offset: 0x000090A8
		private void RefreshCardAnchor(int index)
		{
			if (index < 0 || index >= this._cardAnchors.Count)
			{
				return;
			}
			GameObject gameObject = this._cardAnchors[index];
			if (gameObject == null)
			{
				return;
			}
			int num = Mathf.Clamp(this.GetCardSize(index), 1, 3);
			int cardStartSlot = this.GetCardStartSlot(index);
			int slotIndex = Mathf.Min(9, cardStartSlot + num - 1);
			float x = (this.GetBoardSlotCenterX(cardStartSlot) + this.GetBoardSlotCenterX(slotIndex)) * 0.5f;
			Vector3 cardSpacing = this._presentation.CardSpacing;
			gameObject.transform.localPosition = new Vector3(x, cardSpacing.y * (float)index, cardSpacing.z * (float)index);
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
		}

		// Token: 0x060001C7 RID: 455 RVA: 0x0000AF70 File Offset: 0x00009170
		private void RefreshCard(int index)
		{
			if (index < 0 || index >= this._cards.Count)
			{
				return;
			}
			GameObject gameObject = this._cards[index];
			if (gameObject == null)
			{
				return;
			}
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = this._presentation.CardScale;
		}

		// Token: 0x060001C8 RID: 456 RVA: 0x0000AFE0 File Offset: 0x000091E0
		private void RefreshVisuals()
		{
			if (this._visualRoot == null || this._boardPlate == null || this._borderSegments.Count < 4)
			{
				return;
			}
			Vector2 boardSize = this._presentation.BoardSize;
			float num = Mathf.Max(0.01f, boardSize.x);
			float num2 = Mathf.Max(0.01f, this._presentation.SkillBoardWidth);
			float num3 = num * 0.5f;
			float num4 = boardSize.y * 0.5f;
			float num5 = Mathf.Max(0.01f, this._presentation.BoardThickness);
			float num6 = Mathf.Max(0.01f, this._presentation.BorderThickness);
			float num7 = Mathf.Max(num5, this._presentation.BorderHeight);
			float num8 = num3 + 0.04f + num2 * 0.5f;
			float num9 = num3 + 0.08f + num2 + 0.25f;
			float num10 = num2 + 0.04f + 0.5f;
			float num11 = Mathf.Max(0.01f, num2 * 0.5f);
			float num12 = num3 + 0.04f + num10 * 0.5f;
			float num13 = num4 + 0.04f + num11 * 0.5f;
			float x = Mathf.Max(0.01f, num10 - 0.16f);
			float z = Mathf.Max(0.01f, num11 * 0.8f);
			this._visualRoot.transform.localPosition = Vector3.zero;
			this._visualRoot.transform.localRotation = Quaternion.identity;
			this._visualRoot.transform.localScale = Vector3.one;
			float y = boardSize.y;
			float z2 = 0f;
			this._boardPlate.transform.localPosition = new Vector3(0f, 0f, z2);
			this._boardPlate.transform.localRotation = Quaternion.identity;
			this._boardPlate.transform.localScale = new Vector3(num, num5, y);
			if (this._boardFill != null)
			{
				this._boardFill.transform.localPosition = new Vector3(0f, num5 * 0.2f, z2);
				this._boardFill.transform.localRotation = Quaternion.identity;
				this._boardFill.transform.localScale = new Vector3(Mathf.Max(0.01f, num - num6 * 0.5f), Mathf.Max(0.01f, num5 * 0.35f), Mathf.Max(0.01f, y - num6 * 0.5f));
			}
			if (this._skillBoardFill != null)
			{
				this._skillBoardFill.transform.localPosition = new Vector3(num8, num5 * 0.2f, z2);
				this._skillBoardFill.transform.localRotation = Quaternion.identity;
				this._skillBoardFill.transform.localScale = new Vector3(num2, Mathf.Max(0.01f, num5 * 0.35f), y);
			}
			if (this._brandingBoardFill != null)
			{
				this._brandingBoardFill.transform.localPosition = new Vector3(num9, num5 * 0.2f, z2);
				this._brandingBoardFill.transform.localRotation = Quaternion.identity;
				this._brandingBoardFill.transform.localScale = new Vector3(0.5f, Mathf.Max(0.01f, num5 * 0.35f), y);
			}
			if (this._monsterInfoBoardFill != null)
			{
				this._monsterInfoBoardFill.transform.localPosition = new Vector3(num12, num5 * 0.2f, num13);
				this._monsterInfoBoardFill.transform.localRotation = Quaternion.identity;
				this._monsterInfoBoardFill.transform.localScale = new Vector3(num10, Mathf.Max(0.01f, num5 * 0.35f), num11);
			}
			if (this._monsterInfoTextStripFill != null)
			{
				this._monsterInfoTextStripFill.transform.localPosition = new Vector3(num12, num5 * 0.3f, num13);
				this._monsterInfoTextStripFill.transform.localRotation = Quaternion.identity;
				this._monsterInfoTextStripFill.transform.localScale = new Vector3(x, Mathf.Max(0.01f, num5 * 0.45f), z);
			}
			if (this._boardCenterMarker != null)
			{
				this._boardCenterMarker.transform.localPosition = new Vector3(0f, num7 + 0.08f, 0f);
				this._boardCenterMarker.transform.localRotation = Quaternion.identity;
				this._boardCenterMarker.transform.localScale = Vector3.one * 0.16f;
			}
			MonsterPreviewBoard.UpdateBorder(this._borderSegments[0], new Vector3(0f, num7 * 0.5f, -num4), new Vector3(num + num6, num7, num6));
			MonsterPreviewBoard.UpdateBorder(this._borderSegments[1], new Vector3(0f, num7 * 0.5f, num4), new Vector3(num + num6, num7, num6));
			MonsterPreviewBoard.UpdateBorder(this._borderSegments[2], new Vector3(-num3, num7 * 0.5f, z2), new Vector3(num6, num7, y + num6));
			MonsterPreviewBoard.UpdateBorder(this._borderSegments[3], new Vector3(num3, num7 * 0.5f, z2), new Vector3(num6, num7, y + num6));
			if (this._skillBoardBorders.Count >= 4)
			{
				float num14 = num2 * 0.5f;
				MonsterPreviewBoard.UpdateBorder(this._skillBoardBorders[0], new Vector3(num8, num7 * 0.5f, -num4), new Vector3(num2 + num6, num7, num6));
				MonsterPreviewBoard.UpdateBorder(this._skillBoardBorders[1], new Vector3(num8, num7 * 0.5f, num4), new Vector3(num2 + num6, num7, num6));
				MonsterPreviewBoard.UpdateBorder(this._skillBoardBorders[2], new Vector3(num8 - num14, num7 * 0.5f, z2), new Vector3(num6, num7, y + num6));
				MonsterPreviewBoard.UpdateBorder(this._skillBoardBorders[3], new Vector3(num8 + num14, num7 * 0.5f, z2), new Vector3(num6, num7, y + num6));
			}
			if (this._brandingBoardBorders.Count >= 4)
			{
				float num15 = 0.25f;
				MonsterPreviewBoard.UpdateBorder(this._brandingBoardBorders[0], new Vector3(num9, num7 * 0.5f, -num4), new Vector3(0.5f + num6, num7, num6));
				MonsterPreviewBoard.UpdateBorder(this._brandingBoardBorders[1], new Vector3(num9, num7 * 0.5f, num4), new Vector3(0.5f + num6, num7, num6));
				MonsterPreviewBoard.UpdateBorder(this._brandingBoardBorders[2], new Vector3(num9 - num15, num7 * 0.5f, z2), new Vector3(num6, num7, y + num6));
				MonsterPreviewBoard.UpdateBorder(this._brandingBoardBorders[3], new Vector3(num9 + num15, num7 * 0.5f, z2), new Vector3(num6, num7, y + num6));
			}
			if (this._monsterInfoBoardBorders.Count >= 4)
			{
				float num16 = num10 * 0.5f;
				float num17 = num11 * 0.5f;
				MonsterPreviewBoard.UpdateBorder(this._monsterInfoBoardBorders[0], new Vector3(num12, num7 * 0.5f, num13 - num17), new Vector3(num10 + num6, num7, num6));
				MonsterPreviewBoard.UpdateBorder(this._monsterInfoBoardBorders[1], new Vector3(num12, num7 * 0.5f, num13 + num17), new Vector3(num10 + num6, num7, num6));
				MonsterPreviewBoard.UpdateBorder(this._monsterInfoBoardBorders[2], new Vector3(num12 - num16, num7 * 0.5f, num13), new Vector3(num6, num7, num11 + num6));
				MonsterPreviewBoard.UpdateBorder(this._monsterInfoBoardBorders[3], new Vector3(num12 + num16, num7 * 0.5f, num13), new Vector3(num6, num7, num11 + num6));
			}
			if (this._brandingText != null)
			{
				this._brandingText.transform.localPosition = new Vector3(num9, num7 + 0.01f, 0f);
				this._brandingText.transform.localRotation = Quaternion.Euler(90f, 90f, 0f);
				this._brandingText.transform.localScale = Vector3.one * 0.2f;
			}
			this.RefreshMonsterInfoTextLayout(num12, num13, num10, num7);
		}

		// Token: 0x060001C9 RID: 457 RVA: 0x0000B890 File Offset: 0x00009A90
		private void RefreshDebugVisuals()
		{
			bool flag = this.IsDebugVisualEnabled();
			MonsterPreviewBoard.SetActive(this._boardCenterMarker, flag && this._debugOptions.ShowAnchorPoint);
			TextMesh brandingText = this._brandingText;
			MonsterPreviewBoard.SetActive((brandingText != null) ? brandingText.gameObject : null, true);
			for (int i = 0; i < this._boardSlotMarkers.Count; i++)
			{
				MonsterPreviewBoard.SetActive(this._boardSlotMarkers[i], flag && this._debugOptions.ShowItemSlots);
			}
			for (int j = 0; j < this._skillSlotMarkers.Count; j++)
			{
				MonsterPreviewBoard.SetActive(this._skillSlotMarkers[j], flag && this._debugOptions.ShowSkillSlots);
			}
			for (int k = 0; k < this._cardCenterMarkers.Count; k++)
			{
				MonsterPreviewBoard.SetActive(this._cardCenterMarkers[k], flag && this._debugOptions.ShowCardBounds);
			}
		}

		// Token: 0x060001CA RID: 458 RVA: 0x0000B980 File Offset: 0x00009B80
		private float GetBoardSlotCenterX(int slotIndex)
		{
			float num = Mathf.Max(0.01f, this._presentation.BoardSize.x);
			float num2 = num / 10f;
			return -num * 0.5f + num2 * ((float)slotIndex + 0.5f);
		}

		// Token: 0x060001CB RID: 459 RVA: 0x0000B9C4 File Offset: 0x00009BC4
		private float GetSkillSlotCenterZ(int slotIndex)
		{
			float skillSlotDepth = this.GetSkillSlotDepth();
			return this._presentation.BoardSize.y * 0.5f - skillSlotDepth * ((float)slotIndex + 0.5f);
		}

		// Token: 0x060001CC RID: 460 RVA: 0x0000B9FC File Offset: 0x00009BFC
		private float GetSkillSlotDepth()
		{
			int num = Mathf.Max(3, this._activeSkillSlotCount);
			return Mathf.Max(0.01f, this._presentation.BoardSize.y / (float)num);
		}

		// Token: 0x060001CD RID: 461 RVA: 0x0000BA34 File Offset: 0x00009C34
		private int GetLeadingEmptySkillSlots(int filledCount)
		{
			int num = Mathf.Max(3, this._activeSkillSlotCount);
			return Mathf.Max(0, num - filledCount) / 2;
		}

		// Token: 0x060001CE RID: 462 RVA: 0x0000BA5C File Offset: 0x00009C5C
		private int GetCardStartSlot(int cardIndex)
		{
			int num = this.GetLeadingEmptySlots();
			for (int i = 0; i < cardIndex; i++)
			{
				num += Mathf.Clamp(this.GetCardSize(i), 1, 3);
			}
			int num2 = Mathf.Clamp(this.GetCardSize(cardIndex), 1, 3);
			return Mathf.Clamp(num, 0, Mathf.Max(0, 10 - num2));
		}

		// Token: 0x060001CF RID: 463 RVA: 0x0000BAB0 File Offset: 0x00009CB0
		private int GetLeadingEmptySlots()
		{
			int num = 0;
			for (int i = 0; i < this._cardSizes.Count; i++)
			{
				num += Mathf.Clamp(this._cardSizes[i], 1, 3);
			}
			return Mathf.Max(0, 10 - num) / 2;
		}

		// Token: 0x060001D0 RID: 464 RVA: 0x0000BAF7 File Offset: 0x00009CF7
		private int GetCardSize(int index)
		{
			if (index < 0 || index >= this._cardSizes.Count)
			{
				return 1;
			}
			return Mathf.Clamp(this._cardSizes[index], 1, 3);
		}

		// Token: 0x060001D1 RID: 465 RVA: 0x0000BB20 File Offset: 0x00009D20
		private static int GetCardSize(PreviewCardSpec spec)
		{
			return Mathf.Clamp((spec != null) ? spec.Size : 1, 1, 3);
		}

		// Token: 0x060001D2 RID: 466 RVA: 0x0000BB35 File Offset: 0x00009D35
		private static void UpdateBorder(GameObject border, Vector3 position, Vector3 scale)
		{
			if (border == null)
			{
				return;
			}
			border.transform.localPosition = position;
			border.transform.localRotation = Quaternion.identity;
			border.transform.localScale = scale;
		}

		// Token: 0x060001D3 RID: 467 RVA: 0x0000BB6C File Offset: 0x00009D6C
		private TextMesh CreateMonsterInfoText(string name, float characterSize, int fontSize)
		{
			GameObject gameObject = new GameObject(name);
			gameObject.transform.SetParent(this._visualRoot.transform, false);
			TextMesh textMesh = gameObject.AddComponent<TextMesh>();
			textMesh.anchor = 4;
			textMesh.alignment = 1;
			textMesh.characterSize = characterSize;
			textMesh.fontSize = fontSize;
			textMesh.font = MonsterPreviewBoard.GetBoardTextFont();
			textMesh.richText = true;
			textMesh.color = new Color(0.9f, 0.98f, 0.92f, 1f);
			MonsterPreviewBoard.SetRendererSorting(gameObject, 30);
			this._monsterInfoTexts.Add(textMesh);
			return textMesh;
		}

		// Token: 0x060001D4 RID: 468 RVA: 0x0000BC00 File Offset: 0x00009E00
		private void RefreshMonsterInfoTexts()
		{
			if (this._monsterInfoTexts.Count > 0 && this._monsterInfoTexts[0] != null)
			{
				this._monsterInfoTexts[0].text = this._monsterHealthText;
				this._monsterInfoTexts[0].color = MonsterPreviewBoard.MonsterHealthTextColor;
			}
			if (this._monsterInfoTexts.Count > 1 && this._monsterInfoTexts[1] != null)
			{
				this._monsterInfoTexts[1].text = "│";
				this._monsterInfoTexts[1].color = MonsterPreviewBoard.MonsterDividerTextColor;
				this._monsterInfoTexts[1].fontStyle = 0;
			}
			if (this._monsterInfoTexts.Count > 2 && this._monsterInfoTexts[2] != null)
			{
				this._monsterInfoTexts[2].text = string.Concat(new string[]
				{
					"<color=#63CEEC>",
					this._monsterXpText,
					"</color> / <color=#FFCC1B>",
					this._monsterGoldText,
					"</color>"
				});
				this._monsterInfoTexts[2].color = Color.white;
				this._monsterInfoTexts[2].fontStyle = 1;
			}
		}

		// Token: 0x060001D5 RID: 469 RVA: 0x0000BD50 File Offset: 0x00009F50
		private void RefreshMonsterInfoTextLayout(float boardCenterX, float boardCenterZ, float boardWidth, float borderHeight)
		{
			if (this._monsterInfoTexts.Count == 0)
			{
				return;
			}
			float num = boardCenterX + boardWidth * 0.02f;
			float num2 = num - boardWidth * 0.04f;
			float num3 = num + boardWidth * 0.15f;
			for (int i = 0; i < this._monsterInfoTexts.Count; i++)
			{
				TextMesh textMesh = this._monsterInfoTexts[i];
				if (!(textMesh == null))
				{
					float num4;
					if (i != 0)
					{
						if (i != 1)
						{
							num4 = num3;
						}
						else
						{
							num4 = num;
						}
					}
					else
					{
						num4 = num2;
					}
					float x = num4;
					TextMesh textMesh2 = textMesh;
					TextAnchor anchor;
					if (i != 0)
					{
						if (i != 1)
						{
							anchor = 4;
						}
						else
						{
							anchor = 4;
						}
					}
					else
					{
						anchor = 5;
					}
					textMesh2.anchor = anchor;
					textMesh2 = textMesh;
					TextAlignment alignment;
					if (i != 0)
					{
						if (i != 1)
						{
							alignment = 1;
						}
						else
						{
							alignment = 1;
						}
					}
					else
					{
						alignment = 2;
					}
					textMesh2.alignment = alignment;
					textMesh.transform.localPosition = new Vector3(x, borderHeight + 0.01f, boardCenterZ);
					textMesh.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
					Vector3 vector;
					if (i != 0)
					{
						if (i != 1)
						{
							vector = Vector3.one * 0.22f;
						}
						else
						{
							vector = Vector3.one * 0.24f;
						}
					}
					else
					{
						vector = Vector3.one * 0.21f;
					}
					Vector3 localScale = vector;
					textMesh.transform.localScale = localScale;
				}
			}
		}

		// Token: 0x060001D6 RID: 470 RVA: 0x0000BEB0 File Offset: 0x0000A0B0
		private static string GetMetadataValue(IReadOnlyDictionary<string, string> metadata, params string[] keys)
		{
			if (metadata == null || keys == null)
			{
				return "?";
			}
			foreach (string text in keys)
			{
				string text2;
				if (!string.IsNullOrWhiteSpace(text) && metadata.TryGetValue(text, out text2) && !string.IsNullOrWhiteSpace(text2))
				{
					return text2;
				}
			}
			return "?";
		}

		// Token: 0x060001D7 RID: 471 RVA: 0x0000BEFD File Offset: 0x0000A0FD
		private static Font GetBoardTextFont()
		{
			if (MonsterPreviewBoard._boardTextFont == null)
			{
				MonsterPreviewBoard._boardTextFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
			}
			return MonsterPreviewBoard._boardTextFont;
		}

		// Token: 0x060001D8 RID: 472 RVA: 0x0000BF1C File Offset: 0x0000A11C
		private static void SetRendererSorting(GameObject target, int sortingOrder)
		{
			Renderer renderer;
			if (target != null && target.TryGetComponent<Renderer>(out renderer))
			{
				renderer.sortingOrder = sortingOrder;
			}
		}

		// Token: 0x060001D9 RID: 473 RVA: 0x0000BF43 File Offset: 0x0000A143
		private bool IsDebugVisualEnabled()
		{
			return this._presentation.DebugEnabled && this._debugOptions.Enabled;
		}

		// Token: 0x060001DA RID: 474 RVA: 0x0000BF5F File Offset: 0x0000A15F
		private static void SetActive(GameObject target, bool active)
		{
			if (target != null && target.activeSelf != active)
			{
				target.SetActive(active);
			}
		}

		// Token: 0x060001DB RID: 475 RVA: 0x0000BF7C File Offset: 0x0000A17C
		private static GameObject CreatePrimitive(string name, bool isSkill, bool keepCollider = false)
		{
			GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
			gameObject.name = name;
			Collider component = gameObject.GetComponent<Collider>();
			if (component != null && !keepCollider)
			{
				Object.Destroy(component);
			}
			MonsterPreviewBoard.ApplyBoardMaterial(gameObject, name.Contains("Border"), isSkill);
			return gameObject;
		}

		// Token: 0x060001DC RID: 476 RVA: 0x0000BFC4 File Offset: 0x0000A1C4
		private static GameObject CreateMarker(string name, float size, bool isSkill)
		{
			GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			gameObject.name = name;
			Collider component = gameObject.GetComponent<Collider>();
			if (component != null)
			{
				Object.Destroy(component);
			}
			MonsterPreviewBoard.ApplyDebugMarkerMaterial(gameObject, isSkill);
			gameObject.transform.localScale = Vector3.one * size;
			return gameObject;
		}

		// Token: 0x060001DD RID: 477 RVA: 0x0000C014 File Offset: 0x0000A214
		private static void ApplyBoardMaterial(GameObject target, bool isBorder, bool isSkill)
		{
			Renderer renderer;
			if (!target.TryGetComponent<Renderer>(out renderer))
			{
				return;
			}
			Shader shader = Shader.Find("Sprites/Default") ?? Shader.Find("Unlit/Color");
			if (shader == null)
			{
				return;
			}
			Material material = new Material(shader);
			bool flag = target.name.Contains("Fill");
			bool flag2 = target.name.Contains("Branding");
			bool flag3 = target.name.Contains("MonsterInfoTextStrip");
			material.color = (flag2 ? (isBorder ? MonsterPreviewBoard.BrandingBoardBorderColor : MonsterPreviewBoard.BrandingBoardFillColor) : (flag3 ? MonsterPreviewBoard.MonsterInfoTextStripColor : (target.name.Contains("MonsterInfo") ? (isBorder ? MonsterPreviewBoard.MonsterInfoBoardBorderColor : MonsterPreviewBoard.MonsterInfoBoardFillColor) : (isSkill ? (isBorder ? MonsterPreviewBoard.SkillBoardBorderColor : MonsterPreviewBoard.SkillBoardFillColor) : (flag ? MonsterPreviewBoard.ItemBoardFillColor : (isBorder ? MonsterPreviewBoard.ItemBoardBorderColor : MonsterPreviewBoard.ItemBoardAccentColor))))));
			renderer.sharedMaterial = material;
		}

		// Token: 0x060001DE RID: 478 RVA: 0x0000C108 File Offset: 0x0000A308
		private static void ApplyDebugMarkerMaterial(GameObject target, bool isSkill)
		{
			Renderer renderer;
			if (!target.TryGetComponent<Renderer>(out renderer))
			{
				return;
			}
			Shader shader = Shader.Find("Sprites/Default") ?? Shader.Find("Unlit/Color");
			if (shader == null)
			{
				return;
			}
			Material material = new Material(shader);
			material.color = (isSkill ? MonsterPreviewBoard.DebugSkillMarkerColor : MonsterPreviewBoard.DebugItemMarkerColor);
			renderer.sharedMaterial = material;
		}

		// Token: 0x040000D6 RID: 214
		private const int BoardSlotCount = 10;

		// Token: 0x040000D7 RID: 215
		private const int DefaultSkillSlotCount = 3;

		// Token: 0x040000D8 RID: 216
		private const float ContentYOffset = 0.2f;

		// Token: 0x040000D9 RID: 217
		private const float BrandingBoardWidth = 0.5f;

		// Token: 0x040000DA RID: 218
		private const float MonsterInfoBoardGap = 0.04f;

		// Token: 0x040000DB RID: 219
		private const float MonsterInfoBoardDepthFactor = 0.5f;

		// Token: 0x040000DC RID: 220
		private const float MonsterInfoTextStripWidthInset = 0.08f;

		// Token: 0x040000DD RID: 221
		private const float MonsterInfoTextStripDepthFactor = 0.8f;

		// Token: 0x040000DE RID: 222
		private const int MonsterInfoBoardSortOrder = 20;

		// Token: 0x040000DF RID: 223
		private const int MonsterInfoTextStripSortOrder = 21;

		// Token: 0x040000E0 RID: 224
		private const int MonsterInfoTextSortOrder = 30;

		// Token: 0x040000E1 RID: 225
		private const float BoardCenterMarkerSize = 0.16f;

		// Token: 0x040000E2 RID: 226
		private const float SlotMarkerSize = 0.08f;

		// Token: 0x040000E3 RID: 227
		private const float CardCenterMarkerSize = 0.12f;

		// Token: 0x040000E4 RID: 228
		private const float SkillBoardGap = 0.04f;

		// Token: 0x040000E5 RID: 229
		private const float SkillSlotInset = 0.12f;

		// Token: 0x040000E6 RID: 230
		private const float SkillCardScaleFactor = 0.8f;

		// Token: 0x040000E7 RID: 231
		private static readonly Color ItemBoardFillColor = new Color(0.34f, 0.29f, 0.24f, 0.88f);

		// Token: 0x040000E8 RID: 232
		private static readonly Color ItemBoardBorderColor = new Color(1f, 0.2f, 0.2f, 0.95f);

		// Token: 0x040000E9 RID: 233
		private static readonly Color ItemBoardAccentColor = new Color(1f, 0.1f, 0.1f, 0.18f);

		// Token: 0x040000EA RID: 234
		private static readonly Color SkillBoardFillColor = new Color(0.24f, 0.29f, 0.33f, 0.16f);

		// Token: 0x040000EB RID: 235
		private static readonly Color SkillBoardBorderColor = new Color(0.25f, 0.7f, 1f, 0.95f);

		// Token: 0x040000EC RID: 236
		private static readonly Color BrandingBoardFillColor = new Color(0.17f, 0.2f, 0.23f, 0.62f);

		// Token: 0x040000ED RID: 237
		private static readonly Color BrandingBoardBorderColor = new Color(0.18f, 0.24f, 0.34f, 0.95f);

		// Token: 0x040000EE RID: 238
		private static readonly Color MonsterInfoBoardFillColor = new Color(0.16f, 0.22f, 0.18f, 0.72f);

		// Token: 0x040000EF RID: 239
		private static readonly Color MonsterInfoBoardBorderColor = new Color(0.16f, 0.2f, 0.18f, 1f);

		// Token: 0x040000F0 RID: 240
		private static readonly Color MonsterInfoTextStripColor = new Color(0.03f, 0.04f, 0.04f, 0.98f);

		// Token: 0x040000F1 RID: 241
		private static readonly Color MonsterHealthTextColor = new Color(0.56078434f, 0.91764706f, 0.19215687f, 1f);

		// Token: 0x040000F2 RID: 242
		private static readonly Color MonsterDividerTextColor = new Color(1f, 1f, 1f, 0.8f);

		// Token: 0x040000F3 RID: 243
		private static readonly Color DebugItemMarkerColor = new Color(1f, 0f, 0f, 0.95f);

		// Token: 0x040000F4 RID: 244
		private static readonly Color DebugSkillMarkerColor = new Color(0.25f, 0.75f, 1f, 0.95f);

		// Token: 0x040000F5 RID: 245
		private static Font _boardTextFont;

		// Token: 0x040000F6 RID: 246
		private readonly IPreviewCardFactory _factory;

		// Token: 0x040000F7 RID: 247
		private readonly IPreviewCardFactory _skillFactory;

		// Token: 0x040000F8 RID: 248
		private readonly GameObject _boardRoot;

		// Token: 0x040000F9 RID: 249
		private readonly GameObject _visualRoot;

		// Token: 0x040000FA RID: 250
		private readonly GameObject _itemContentRoot;

		// Token: 0x040000FB RID: 251
		private readonly GameObject _skillContentRoot;

		// Token: 0x040000FC RID: 252
		private readonly List<GameObject> _boardSlots = new List<GameObject>();

		// Token: 0x040000FD RID: 253
		private readonly List<GameObject> _boardSlotMarkers = new List<GameObject>();

		// Token: 0x040000FE RID: 254
		private readonly List<GameObject> _cardAnchors = new List<GameObject>();

		// Token: 0x040000FF RID: 255
		private readonly List<GameObject> _cards = new List<GameObject>();

		// Token: 0x04000100 RID: 256
		private readonly List<GameObject> _cardCenterMarkers = new List<GameObject>();

		// Token: 0x04000101 RID: 257
		private readonly List<GameObject> _borderSegments = new List<GameObject>();

		// Token: 0x04000102 RID: 258
		private readonly List<int> _cardSizes = new List<int>();

		// Token: 0x04000103 RID: 259
		private readonly List<GameObject> _skillSlots = new List<GameObject>();

		// Token: 0x04000104 RID: 260
		private readonly List<GameObject> _skillSlotMarkers = new List<GameObject>();

		// Token: 0x04000105 RID: 261
		private readonly List<GameObject> _skillCards = new List<GameObject>();

		// Token: 0x04000106 RID: 262
		private int _activeSkillSlotCount = 3;

		// Token: 0x04000107 RID: 263
		private GameObject _boardPlate;

		// Token: 0x04000108 RID: 264
		private GameObject _boardFill;

		// Token: 0x04000109 RID: 265
		private GameObject _skillBoardFill;

		// Token: 0x0400010A RID: 266
		private GameObject _brandingBoardFill;

		// Token: 0x0400010B RID: 267
		private GameObject _monsterInfoBoardFill;

		// Token: 0x0400010C RID: 268
		private GameObject _monsterInfoTextStripFill;

		// Token: 0x0400010D RID: 269
		private readonly List<GameObject> _skillBoardBorders = new List<GameObject>();

		// Token: 0x0400010E RID: 270
		private readonly List<GameObject> _brandingBoardBorders = new List<GameObject>();

		// Token: 0x0400010F RID: 271
		private readonly List<GameObject> _monsterInfoBoardBorders = new List<GameObject>();

		// Token: 0x04000110 RID: 272
		private TextMesh _brandingText;

		// Token: 0x04000111 RID: 273
		private readonly List<TextMesh> _monsterInfoTexts = new List<TextMesh>();

		// Token: 0x04000112 RID: 274
		private GameObject _boardCenterMarker;

		// Token: 0x04000113 RID: 275
		private PreviewBoardPresentation _presentation = new PreviewBoardPresentation();

		// Token: 0x04000114 RID: 276
		private PreviewBoardDebugOptions _debugOptions = new PreviewBoardDebugOptions();

		// Token: 0x04000115 RID: 277
		private string _monsterHealthText = "?";

		// Token: 0x04000116 RID: 278
		private string _monsterXpText = "?";

		// Token: 0x04000117 RID: 279
		private string _monsterGoldText = "?";
	}
}
