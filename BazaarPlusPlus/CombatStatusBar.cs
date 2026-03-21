using System;
using System.Runtime.CompilerServices;
using BepInEx.Configuration;
using TheBazaar;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace BazaarPlusPlus
{
	// Token: 0x0200000D RID: 13
	[NullableContext(2)]
	[Nullable(0)]
	internal sealed class CombatStatusBar : MonoBehaviour
	{
		// Token: 0x0600004F RID: 79 RVA: 0x00003078 File Offset: 0x00001278
		private void EnsureUi()
		{
			if (this._canvasObject != null)
			{
				return;
			}
			this._canvasObject = new GameObject("CombatStatusBarCanvas", new Type[]
			{
				typeof(RectTransform),
				typeof(Canvas),
				typeof(CanvasScaler),
				typeof(GraphicRaycaster)
			});
			this._canvasObject.transform.SetParent(base.transform, false);
			this._canvas = this._canvasObject.GetComponent<Canvas>();
			this._canvas.renderMode = 0;
			this._canvas.sortingOrder = 10;
			CanvasScaler component = this._canvasObject.GetComponent<CanvasScaler>();
			component.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			component.referenceResolution = new Vector2(1920f, 1080f);
			component.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
			component.matchWidthOrHeight = 0.55f;
			RectTransform rectTransform = (RectTransform)this._canvasObject.transform;
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;
			rectTransform.offsetMin = Vector2.zero;
			rectTransform.offsetMax = Vector2.zero;
			RectTransform rectTransform2 = CombatStatusBar.CreateRect("SafeAreaRoot", this._canvasObject.transform);
			rectTransform2.anchorMin = Vector2.zero;
			rectTransform2.anchorMax = Vector2.one;
			rectTransform2.offsetMin = Vector2.zero;
			rectTransform2.offsetMax = Vector2.zero;
			this._barRoot = CombatStatusBar.CreateRect("BarRoot", rectTransform2);
			this._barRoot.anchorMin = new Vector2(0.5f, 0f);
			this._barRoot.anchorMax = new Vector2(0.5f, 0f);
			this._barRoot.pivot = new Vector2(0.5f, 0f);
			this._barRoot.anchoredPosition = new Vector2(0f, 0f);
			this._barRoot.sizeDelta = new Vector2(561f, 60f);
			this._barBackground = CombatStatusBar.AddImage(this._barRoot.gameObject, new Color(0.06f, 0.07f, 0.09f, 0.9f));
			this._barGlow = CombatStatusBar.AddChildImage("BarGlow", this._barRoot, new Color(0.28f, 0.22f, 0.12f, 0.1f));
			this._barGlow.rectTransform.offsetMin = new Vector2(3f, 3f);
			this._barGlow.rectTransform.offsetMax = new Vector2(-3f, -3f);
			HorizontalLayoutGroup horizontalLayoutGroup = this._barRoot.gameObject.AddComponent<HorizontalLayoutGroup>();
			horizontalLayoutGroup.spacing = 4f;
			horizontalLayoutGroup.padding = new RectOffset(4, 4, 4, 4);
			horizontalLayoutGroup.childAlignment = 4;
			horizontalLayoutGroup.childControlHeight = true;
			horizontalLayoutGroup.childControlWidth = true;
			horizontalLayoutGroup.childForceExpandHeight = true;
			horizontalLayoutGroup.childForceExpandWidth = false;
			this.CreateReadoutSegment("TimeSegment", this._barRoot, 116f, out this._timeBackground, out this._timeLabel, out this._timeValue);
			CombatStatusBar.SetLabel(this._timeLabel, "Time");
			this._timeDivider = this.CreateDivider(this._barRoot);
			this.CreateReadoutSegment("FrameSegment", this._barRoot, 116f, out this._frameBackground, out this._frameLabel, out this._frameValue);
			CombatStatusBar.SetLabel(this._frameLabel, "Frame");
			this._frameDivider = this.CreateDivider(this._barRoot);
			RectTransform parent = this.CreateInteractiveSegment("MultiplierSegment", this._barRoot, 168f, out this._multiplierBackground, out this._multiplierLabel);
			CombatStatusBar.SetLabel(this._multiplierLabel, "Multiplier");
			this.CreateMultiplierContent(parent);
			this._multiplierDivider = this.CreateDivider(this._barRoot);
			RectTransform parent2 = this.CreateInteractiveSegment("PauseSegment", this._barRoot, 110f, out this._pauseBackground, out this._pauseLabel);
			CombatStatusBar.SetLabel(this._pauseLabel, "Pause");
			this.CreatePauseContent(parent2);
		}

		// Token: 0x06000050 RID: 80 RVA: 0x0000346C File Offset: 0x0000166C
		private void DisposeUi()
		{
			if (this._canvasObject == null)
			{
				return;
			}
			Object.Destroy(this._canvasObject);
			this._canvasObject = null;
			this._canvas = null;
			this._barRoot = null;
			this._barBackground = null;
			this._barGlow = null;
			this._timeLabel = null;
			this._timeValue = null;
			this._timeBackground = null;
			this._frameLabel = null;
			this._frameValue = null;
			this._frameBackground = null;
			this._multiplierLabel = null;
			this._multiplierValue = null;
			this._decrementButton = null;
			this._decrementButtonText = null;
			this._decrementButtonBackground = null;
			this._incrementButton = null;
			this._incrementButtonText = null;
			this._incrementButtonBackground = null;
			this._multiplierBackground = null;
			this._pauseLabel = null;
			this._pauseButton = null;
			this._pauseButtonText = null;
			this._pauseButtonBackground = null;
			this._pauseBackground = null;
			this._timeDivider = null;
			this._frameDivider = null;
			this._multiplierDivider = null;
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00003557 File Offset: 0x00001757
		private void SetUiVisible(bool visible)
		{
			if (this._canvasObject != null && this._canvasObject.activeSelf != visible)
			{
				this._canvasObject.SetActive(visible);
			}
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00003584 File Offset: 0x00001784
		private void RefreshUi()
		{
			if (this._canvasObject == null)
			{
				return;
			}
			bool flag = this.ShouldDraw();
			this.SetUiVisible(flag);
			if (!flag)
			{
				return;
			}
			Color color = Color.Lerp(new Color(0.06f, 0.07f, 0.09f, 0.9f), new Color(0.16f, 0.12f, 0.08f, 0.96f), this._visualBlend);
			Color color2 = Color.Lerp(new Color(0.2f, 0.24f, 0.28f, 0.08f), new Color(0.44f, 0.3f, 0.14f, 0.2f), this._visualBlend);
			Color color3 = Color.Lerp(new Color(0.11f, 0.13f, 0.16f, 0.9f), new Color(0.23f, 0.18f, 0.11f, 0.92f), this._visualBlend);
			Color color4 = Color.Lerp(new Color(0.62f, 0.67f, 0.74f, 0.88f), new Color(0.9f, 0.84f, 0.62f, 0.96f), this._visualBlend);
			Color color5 = Color.Lerp(new Color(0.84f, 0.87f, 0.93f, 0.96f), new Color(1f, 0.96f, 0.9f, 1f), this._visualBlend);
			Color color6 = Color.Lerp(new Color(0.42f, 0.46f, 0.53f, 0.22f), new Color(0.84f, 0.74f, 0.44f, 0.42f), this._visualBlend);
			CombatStatusBar.SetImageColor(this._barBackground, color);
			CombatStatusBar.SetImageColor(this._barGlow, color2);
			CombatStatusBar.SetImageColor(this._timeBackground, color3);
			CombatStatusBar.SetImageColor(this._frameBackground, color3);
			CombatStatusBar.SetImageColor(this._multiplierBackground, color3);
			CombatStatusBar.SetImageColor(this._pauseBackground, color3);
			CombatStatusBar.SetImageColor(this._timeDivider, color6);
			CombatStatusBar.SetImageColor(this._frameDivider, color6);
			CombatStatusBar.SetImageColor(this._multiplierDivider, color6);
			CombatStatusBar.SetTextColor(this._timeLabel, color4);
			CombatStatusBar.SetTextColor(this._frameLabel, color4);
			CombatStatusBar.SetTextColor(this._multiplierLabel, color4);
			CombatStatusBar.SetTextColor(this._pauseLabel, color4);
			CombatStatusBar.SetTextColor(this._timeValue, color5);
			CombatStatusBar.SetTextColor(this._frameValue, color5);
			CombatStatusBar.SetTextColor(this._multiplierValue, color5);
			CombatStatusBar.SetLabel(this._timeLabel, CombatStatusBar.GetDisplayedTimeLabel());
			if (this._timeValue != null)
			{
				this._timeValue.text = CombatStatusBar.GetDisplayedTimeText();
			}
			if (this._frameValue != null)
			{
				this._frameValue.text = CombatStatusBar.GetDisplayedFrameText();
			}
			if (this._multiplierValue != null)
			{
				this._multiplierValue.text = CombatStatusBar.FormatCombatSpeedLabel();
			}
			Color normalColor = Color.Lerp(new Color(0.26f, 0.3f, 0.36f, 0.92f), new Color(0.48f, 0.33f, 0.13f, 0.95f), this._visualBlend);
			Color pressedColor = Color.Lerp(new Color(0.35f, 0.39f, 0.46f, 1f), new Color(0.66f, 0.47f, 0.16f, 1f), this._visualBlend);
			Color disabledColor = new Color(0.22f, 0.24f, 0.28f, 0.45f);
			CombatStatusBar.ApplyButtonColors(this._decrementButton, this._decrementButtonBackground, this._decrementButtonText, CombatStatusBar.CanStepCombatSpeed(-1), normalColor, pressedColor, disabledColor, color5);
			CombatStatusBar.ApplyButtonColors(this._incrementButton, this._incrementButtonBackground, this._incrementButtonText, CombatStatusBar.CanStepCombatSpeed(1), normalColor, pressedColor, disabledColor, color5);
			bool interactable = CombatStatusBar.CanToggleCombatPause();
			Color normalColor2 = CombatStatusBar.IsCombatPaused ? Color.Lerp(new Color(0.28f, 0.33f, 0.4f, 0.95f), new Color(0.54f, 0.4f, 0.16f, 0.96f), this._visualBlend) : Color.Lerp(new Color(0.24f, 0.27f, 0.33f, 0.9f), new Color(0.41f, 0.31f, 0.13f, 0.93f), this._visualBlend);
			Color pressedColor2 = CombatStatusBar.IsCombatPaused ? Color.Lerp(new Color(0.36f, 0.42f, 0.5f, 1f), new Color(0.68f, 0.5f, 0.18f, 1f), this._visualBlend) : Color.Lerp(new Color(0.32f, 0.36f, 0.43f, 1f), new Color(0.56f, 0.41f, 0.15f, 1f), this._visualBlend);
			Color disabledColor2 = new Color(0.22f, 0.24f, 0.28f, 0.45f);
			CombatStatusBar.ApplyButtonColors(this._pauseButton, this._pauseButtonBackground, this._pauseButtonText, interactable, normalColor2, pressedColor2, disabledColor2, color5);
			if (this._pauseButtonText != null)
			{
				this._pauseButtonText.text = (CombatStatusBar.IsCombatPaused ? ">" : "||");
			}
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00003ABC File Offset: 0x00001CBC
		[NullableContext(1)]
		private void CreateMultiplierContent(RectTransform parent)
		{
			RectTransform rectTransform = CombatStatusBar.CreateRect("MultiplierRow", parent);
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;
			rectTransform.offsetMin = new Vector2(10f, 6f);
			rectTransform.offsetMax = new Vector2(-10f, -20f);
			HorizontalLayoutGroup horizontalLayoutGroup = rectTransform.gameObject.AddComponent<HorizontalLayoutGroup>();
			horizontalLayoutGroup.spacing = 6f;
			horizontalLayoutGroup.childAlignment = 4;
			horizontalLayoutGroup.childControlWidth = true;
			horizontalLayoutGroup.childControlHeight = true;
			horizontalLayoutGroup.childForceExpandWidth = false;
			horizontalLayoutGroup.childForceExpandHeight = true;
			ValueTuple<Button, Image, Text> valueTuple = this.CreateButton("DecrementButton", rectTransform, "<", 34f);
			this._decrementButton = valueTuple.Item1;
			this._decrementButtonBackground = valueTuple.Item2;
			this._decrementButtonText = valueTuple.Item3;
			this._multiplierValue = CombatStatusBar.CreateText("MultiplierValue", rectTransform, 15, 1, 4);
			LayoutElement layoutElement = this._multiplierValue.gameObject.AddComponent<LayoutElement>();
			layoutElement.minWidth = 52f;
			layoutElement.preferredWidth = 64f;
			layoutElement.flexibleWidth = 1f;
			valueTuple = this.CreateButton("IncrementButton", rectTransform, ">", 34f);
			this._incrementButton = valueTuple.Item1;
			this._incrementButtonBackground = valueTuple.Item2;
			this._incrementButtonText = valueTuple.Item3;
			this._decrementButton.onClick.AddListener(delegate()
			{
				CombatStatusBar.StepCombatSpeed(-1);
			});
			this._incrementButton.onClick.AddListener(delegate()
			{
				CombatStatusBar.StepCombatSpeed(1);
			});
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00003C68 File Offset: 0x00001E68
		[NullableContext(1)]
		private void CreatePauseContent(RectTransform parent)
		{
			RectTransform rectTransform = CombatStatusBar.CreateRect("PauseButtonArea", parent);
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;
			rectTransform.offsetMin = new Vector2(12f, 6f);
			rectTransform.offsetMax = new Vector2(-12f, -20f);
			ValueTuple<Button, Image, Text> valueTuple = this.CreateButton("PauseButton", rectTransform, "||", 0f);
			this._pauseButton = valueTuple.Item1;
			this._pauseButtonBackground = valueTuple.Item2;
			this._pauseButtonText = valueTuple.Item3;
			CombatStatusBar.StretchToParent((RectTransform)this._pauseButton.transform, 0f, 0f, 0f, 0f);
			LayoutElement layoutElement = this._pauseButton.gameObject.AddComponent<LayoutElement>();
			layoutElement.preferredWidth = 0f;
			layoutElement.flexibleWidth = 1f;
			this._pauseButton.onClick.AddListener(delegate()
			{
				CombatStatusBar.ToggleCombatPause();
			});
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00003D7C File Offset: 0x00001F7C
		[NullableContext(1)]
		private RectTransform CreateReadoutSegment(string name, Transform parent, float width, out Image background, out Text label, out Text value)
		{
			RectTransform rectTransform = this.CreateSegmentShell(name, parent, width, out background);
			label = CombatStatusBar.CreateText("Label", rectTransform, 10, 0, 4);
			CombatStatusBar.AnchorTopStretch(label.rectTransform, 6f, 12f);
			value = CombatStatusBar.CreateText("Value", rectTransform, 15, 1, 4);
			value.verticalOverflow = 1;
			CombatStatusBar.StretchToParent(value.rectTransform, 8f, 8f, 18f, 6f);
			return rectTransform;
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00003DFC File Offset: 0x00001FFC
		[NullableContext(1)]
		private RectTransform CreateInteractiveSegment(string name, Transform parent, float width, out Image background, out Text label)
		{
			RectTransform rectTransform = this.CreateSegmentShell(name, parent, width, out background);
			label = CombatStatusBar.CreateText("Label", rectTransform, 10, 0, 4);
			CombatStatusBar.AnchorTopStretch(label.rectTransform, 6f, 12f);
			return rectTransform;
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00003E40 File Offset: 0x00002040
		[NullableContext(1)]
		private RectTransform CreateSegmentShell(string name, Transform parent, float width, out Image background)
		{
			RectTransform rectTransform = CombatStatusBar.CreateRect(name, parent);
			LayoutElement layoutElement = rectTransform.gameObject.AddComponent<LayoutElement>();
			layoutElement.preferredWidth = width;
			layoutElement.minWidth = width;
			layoutElement.flexibleHeight = 1f;
			background = CombatStatusBar.AddImage(rectTransform.gameObject, Color.white);
			return rectTransform;
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00003E8C File Offset: 0x0000208C
		[NullableContext(1)]
		private Image CreateDivider(Transform parent)
		{
			RectTransform rectTransform = CombatStatusBar.CreateRect("Divider", parent);
			LayoutElement layoutElement = rectTransform.gameObject.AddComponent<LayoutElement>();
			layoutElement.preferredWidth = 1f;
			layoutElement.minWidth = 1f;
			layoutElement.flexibleHeight = 1f;
			Image result = CombatStatusBar.AddImage(rectTransform.gameObject, Color.white);
			rectTransform.offsetMin = new Vector2(0f, 8f);
			rectTransform.offsetMax = new Vector2(0f, -8f);
			return result;
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00003F0C File Offset: 0x0000210C
		[NullableContext(1)]
		[return: TupleElementNames(new string[]
		{
			"button",
			"background",
			"label"
		})]
		[return: Nullable(new byte[]
		{
			0,
			1,
			1,
			1
		})]
		private ValueTuple<Button, Image, Text> CreateButton(string name, Transform parent, string text, float preferredWidth)
		{
			RectTransform rectTransform = CombatStatusBar.CreateRect(name, parent);
			if (preferredWidth > 0f)
			{
				LayoutElement layoutElement = rectTransform.gameObject.AddComponent<LayoutElement>();
				layoutElement.minWidth = preferredWidth;
				layoutElement.preferredWidth = preferredWidth;
				layoutElement.flexibleWidth = 0f;
			}
			Image image = CombatStatusBar.AddImage(rectTransform.gameObject, Color.white);
			image.raycastTarget = true;
			Button button = rectTransform.gameObject.AddComponent<Button>();
			button.targetGraphic = image;
			button.transition = Selectable.Transition.ColorTint;
			button.colors = CombatStatusBar.BuildColorBlock(Color.white, Color.white, Color.white);
			Text text2 = CombatStatusBar.CreateText("Label", rectTransform, 16, 1, 4);
			text2.raycastTarget = false;
			CombatStatusBar.StretchToParent(text2.rectTransform, 0f, 0f, 0f, 0f);
			text2.text = text;
			return new ValueTuple<Button, Image, Text>(button, image, text2);
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00003FDE File Offset: 0x000021DE
		[NullableContext(1)]
		private static RectTransform CreateRect(string name, Transform parent)
		{
			RectTransform component = new GameObject(name, new Type[]
			{
				typeof(RectTransform)
			}).GetComponent<RectTransform>();
			component.SetParent(parent, false);
			component.localScale = Vector3.one;
			return component;
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00004014 File Offset: 0x00002214
		[NullableContext(1)]
		private static void AnchorTopStretch(RectTransform rect, float top, float height)
		{
			rect.anchorMin = new Vector2(0f, 1f);
			rect.anchorMax = new Vector2(1f, 1f);
			rect.pivot = new Vector2(0.5f, 1f);
			rect.anchoredPosition = new Vector2(0f, -top);
			rect.sizeDelta = new Vector2(0f, height);
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00004084 File Offset: 0x00002284
		[NullableContext(1)]
		private static void StretchToParent(RectTransform rect, float left, float right, float top, float bottom)
		{
			rect.anchorMin = Vector2.zero;
			rect.anchorMax = Vector2.one;
			rect.pivot = new Vector2(0.5f, 0.5f);
			rect.offsetMin = new Vector2(left, bottom);
			rect.offsetMax = new Vector2(-right, -top);
		}

		// Token: 0x0600005D RID: 93 RVA: 0x000040D9 File Offset: 0x000022D9
		[NullableContext(1)]
		private static Image AddChildImage(string name, Transform parent, Color color)
		{
			RectTransform rectTransform = CombatStatusBar.CreateRect(name, parent);
			CombatStatusBar.StretchToParent(rectTransform, 0f, 0f, 0f, 0f);
			return CombatStatusBar.AddImage(rectTransform.gameObject, color);
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00004107 File Offset: 0x00002307
		[NullableContext(1)]
		private static Image AddImage(GameObject gameObject, Color color)
		{
			Image image = gameObject.AddComponent<Image>();
			image.sprite = CombatStatusBar.GetRoundedSprite();
			image.type = Image.Type.Sliced;
			image.color = color;
			image.raycastTarget = false;
			return image;
		}

		// Token: 0x0600005F RID: 95 RVA: 0x00004130 File Offset: 0x00002330
		[NullableContext(1)]
		private static Text CreateText(string name, Transform parent, int fontSize, FontStyle fontStyle, TextAnchor alignment)
		{
			Text text = CombatStatusBar.CreateRect(name, parent).gameObject.AddComponent<Text>();
			text.font = CombatStatusBar.GetUiFont();
			text.fontSize = fontSize;
			text.fontStyle = fontStyle;
			text.alignment = alignment;
			text.horizontalOverflow = 1;
			text.verticalOverflow = 0;
			text.supportRichText = false;
			text.raycastTarget = false;
			return text;
		}

		// Token: 0x06000060 RID: 96 RVA: 0x0000418C File Offset: 0x0000238C
		private static ColorBlock BuildColorBlock(Color normal, Color pressed, Color disabled)
		{
			ColorBlock defaultColorBlock = ColorBlock.defaultColorBlock;
			defaultColorBlock.normalColor = normal;
			defaultColorBlock.highlightedColor = normal;
			defaultColorBlock.selectedColor = normal;
			defaultColorBlock.pressedColor = pressed;
			defaultColorBlock.disabledColor = disabled;
			defaultColorBlock.colorMultiplier = 1f;
			defaultColorBlock.fadeDuration = 0.05f;
			return defaultColorBlock;
		}

		// Token: 0x06000061 RID: 97 RVA: 0x000041E0 File Offset: 0x000023E0
		private static void ApplyButtonColors(Button button, Image background, Text label, bool interactable, Color normalColor, Color pressedColor, Color disabledColor, Color textColor)
		{
			if (button == null || background == null || label == null)
			{
				return;
			}
			button.interactable = interactable;
			button.colors = CombatStatusBar.BuildColorBlock(normalColor, pressedColor, disabledColor);
			background.color = (interactable ? normalColor : disabledColor);
			label.color = (interactable ? textColor : new Color(textColor.r, textColor.g, textColor.b, 0.45f));
		}

		// Token: 0x06000062 RID: 98 RVA: 0x0000425C File Offset: 0x0000245C
		private static void SetImageColor(Image image, Color color)
		{
			if (image != null)
			{
				image.color = color;
			}
		}

		// Token: 0x06000063 RID: 99 RVA: 0x0000426E File Offset: 0x0000246E
		private static void SetTextColor(Text text, Color color)
		{
			if (text != null)
			{
				text.color = color;
			}
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00004280 File Offset: 0x00002480
		[NullableContext(1)]
		private static void SetLabel([Nullable(2)] Text label, string content)
		{
			if (label != null)
			{
				label.text = content;
			}
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00004292 File Offset: 0x00002492
		[NullableContext(1)]
		private static Font GetUiFont()
		{
			if (CombatStatusBar._uiFont == null)
			{
				CombatStatusBar._uiFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
			}
			return CombatStatusBar._uiFont;
		}

		// Token: 0x06000066 RID: 102 RVA: 0x000042B0 File Offset: 0x000024B0
		[NullableContext(1)]
		private static Sprite GetRoundedSprite()
		{
			if (CombatStatusBar._roundedSprite != null)
			{
				return CombatStatusBar._roundedSprite;
			}
			Texture2D texture2D = new Texture2D(32, 32, TextureFormat.ARGB32, false)
			{
				filterMode = FilterMode.Bilinear,
				wrapMode = TextureWrapMode.Clamp
			};
			for (int i = 0; i < 32; i++)
			{
				for (int j = 0; j < 32; j++)
				{
					float a = CombatStatusBar.IsInsideRoundedRect(j, i, 32, 12) ? 1f : 0f;
					texture2D.SetPixel(j, i, new Color(1f, 1f, 1f, a));
				}
			}
			texture2D.Apply();
			CombatStatusBar._roundedSprite = Sprite.Create(texture2D, new Rect(0f, 0f, 32f, 32f), new Vector2(0.5f, 0.5f), 100f, 0U, SpriteMeshType.FullRect, new Vector4(12f, 12f, 12f, 12f));
			return CombatStatusBar._roundedSprite;
		}

		// Token: 0x06000067 RID: 103 RVA: 0x0000439C File Offset: 0x0000259C
		private static bool IsInsideRoundedRect(int x, int y, int size, int radius)
		{
			int num = Mathf.Clamp(x, radius, size - radius - 1);
			int num2 = Mathf.Clamp(y, radius, size - radius - 1);
			int num3 = x - num;
			int num4 = y - num2;
			return num3 * num3 + num4 * num4 <= radius * radius;
		}

		// Token: 0x06000068 RID: 104 RVA: 0x000043D8 File Offset: 0x000025D8
		[NullableContext(1)]
		internal static void InitializeConfig(ConfigFile config)
		{
			CombatStatusBar._enableCombatStatusBarConfig = config.Bind<bool>("CombatStatusBar", "Enabled", false, "Whether to show the combat status bar with elapsed time and speed controls");
			CombatStatusBar._defaultCombatSpeedConfig = config.Bind<float>("CombatStatusBar", "SpeedMultiplier", 1f, new ConfigDescription("Default combat playback speed multiplier. Supported values: 0.25, 0.50, 1.00, 1.50, 2.00, 3.00", new AcceptableValueList<float>(new float[]
			{
				0.25f,
				0.5f,
				1f,
				1.5f,
				2f,
				3f
			}), Array.Empty<object>()));
			CombatStatusBar.CombatSpeedMultiplier = CombatStatusBar.SetConfiguredDefaultSpeed(CombatStatusBar._defaultCombatSpeedConfig.Value);
			BppLog.Info("CombatStatusBar", string.Format("Combat config initialized: enabled={0}, speed={1:F2}x", CombatStatusBar._enableCombatStatusBarConfig.Value, CombatStatusBar.CombatSpeedMultiplier));
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00004480 File Offset: 0x00002680
		internal static bool IsEnabled()
		{
			ConfigEntry<bool> enableCombatStatusBarConfig = CombatStatusBar._enableCombatStatusBarConfig;
			return enableCombatStatusBarConfig != null && enableCombatStatusBarConfig.Value;
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00004494 File Offset: 0x00002694
		private static float SetConfiguredDefaultSpeed(float configuredSpeed)
		{
			float num = CombatStatusBar.NormalizeConfiguredDefaultSpeed(configuredSpeed);
			if (num == configuredSpeed)
			{
				return CombatStatusBar.SetCombatSpeed(configuredSpeed);
			}
			CombatStatusBar.CombatSpeedMultiplier = num;
			CombatStatusBar._defaultCombatSpeedConfig.Value = num;
			return CombatStatusBar.CombatSpeedMultiplier;
		}

		// Token: 0x0600006B RID: 107 RVA: 0x000044CC File Offset: 0x000026CC
		private void OnEnable()
		{
			Events.Event combatStarted = Events.CombatStarted;
			Action action;
			if ((action = CombatStatusBar.<>O.<0>__OnCombatStarted) == null)
			{
				action = (CombatStatusBar.<>O.<0>__OnCombatStarted = new Action(CombatStatusBar.OnCombatStarted));
			}
			combatStarted.AddListener(action, this);
			Events.Event combatEnded = Events.CombatEnded;
			Action action2;
			if ((action2 = CombatStatusBar.<>O.<1>__OnCombatEnded) == null)
			{
				action2 = (CombatStatusBar.<>O.<1>__OnCombatEnded = new Action(CombatStatusBar.OnCombatEnded));
			}
			combatEnded.AddListener(action2, this);
			this.EnsureUi();
			this.RefreshUi();
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00004534 File Offset: 0x00002734
		private void OnDisable()
		{
			Events.Event combatStarted = Events.CombatStarted;
			Action action;
			if ((action = CombatStatusBar.<>O.<0>__OnCombatStarted) == null)
			{
				action = (CombatStatusBar.<>O.<0>__OnCombatStarted = new Action(CombatStatusBar.OnCombatStarted));
			}
			combatStarted.RemoveListener(action);
			Events.Event combatEnded = Events.CombatEnded;
			Action action2;
			if ((action2 = CombatStatusBar.<>O.<1>__OnCombatEnded) == null)
			{
				action2 = (CombatStatusBar.<>O.<1>__OnCombatEnded = new Action(CombatStatusBar.OnCombatEnded));
			}
			combatEnded.RemoveListener(action2);
			this.SetUiVisible(false);
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00004592 File Offset: 0x00002792
		private void OnDestroy()
		{
			this.DisposeUi();
		}

		// Token: 0x0600006E RID: 110 RVA: 0x0000459C File Offset: 0x0000279C
		private void Update()
		{
			Keyboard current = Keyboard.current;
			if (current != null && current[KeyBindings.Toggle.CombatStatusBar].wasPressedThisFrame)
			{
				this._visible = !this._visible;
			}
			this._visualBlend = CombatStatusBar.AdvanceVisualBlend(this._visualBlend, CombatStatusBar.IsCombatPlaybackActive, Time.unscaledDeltaTime);
			this.EnsureUi();
			this.RefreshUi();
		}

		// Token: 0x0600006F RID: 111 RVA: 0x000045FA File Offset: 0x000027FA
		private bool ShouldDraw()
		{
			return CombatStatusBar.ShouldRenderForState(this._visible, CombatStatusBar.IsEnabled());
		}

		// Token: 0x06000070 RID: 112 RVA: 0x0000460C File Offset: 0x0000280C
		private static void OnCombatStarted()
		{
			CombatStatusBar.BeginCombatPlayback();
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00004613 File Offset: 0x00002813
		private static void OnCombatEnded()
		{
			CombatStatusBar.EndCombatPlayback();
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000072 RID: 114 RVA: 0x0000461A File Offset: 0x0000281A
		// (set) Token: 0x06000073 RID: 115 RVA: 0x00004621 File Offset: 0x00002821
		internal static bool IsCombatPlaybackActive { get; private set; }

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000074 RID: 116 RVA: 0x00004629 File Offset: 0x00002829
		// (set) Token: 0x06000075 RID: 117 RVA: 0x00004630 File Offset: 0x00002830
		internal static bool IsCombatPaused { get; private set; }

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000076 RID: 118 RVA: 0x00004638 File Offset: 0x00002838
		// (set) Token: 0x06000077 RID: 119 RVA: 0x0000463F File Offset: 0x0000283F
		internal static float CombatSpeedMultiplier { get; private set; } = 1f;

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000078 RID: 120 RVA: 0x00004647 File Offset: 0x00002847
		// (set) Token: 0x06000079 RID: 121 RVA: 0x0000464E File Offset: 0x0000284E
		internal static int ProcessedCombatFrames { get; private set; }

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x0600007A RID: 122 RVA: 0x00004656 File Offset: 0x00002856
		// (set) Token: 0x0600007B RID: 123 RVA: 0x0000465D File Offset: 0x0000285D
		internal static int TotalCombatFrames { get; private set; }

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x0600007C RID: 124 RVA: 0x00004665 File Offset: 0x00002865
		// (set) Token: 0x0600007D RID: 125 RVA: 0x0000466C File Offset: 0x0000286C
		internal static TimeSpan LastCombatLogicalElapsed { get; private set; }

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x0600007E RID: 126 RVA: 0x00004674 File Offset: 0x00002874
		// (set) Token: 0x0600007F RID: 127 RVA: 0x0000467B File Offset: 0x0000287B
		internal static bool HasCompletedCombatPlayback { get; private set; }

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000080 RID: 128 RVA: 0x00004683 File Offset: 0x00002883
		[Nullable(0)]
		internal static ReadOnlySpan<float> CombatSpeedSteps
		{
			[NullableContext(0)]
			get
			{
				return CombatStatusBar.SpeedSteps;
			}
		}

		// Token: 0x06000081 RID: 129 RVA: 0x0000468F File Offset: 0x0000288F
		internal static void BeginCombatPlayback()
		{
			CombatStatusBar.IsCombatPlaybackActive = true;
			CombatStatusBar.ProcessedCombatFrames = 0;
		}

		// Token: 0x06000082 RID: 130 RVA: 0x0000469D File Offset: 0x0000289D
		internal static void EndCombatPlayback()
		{
			CombatStatusBar.LastCombatLogicalElapsed = CombatStatusBar.GetCombatLogicalElapsed();
			CombatStatusBar.HasCompletedCombatPlayback = true;
			CombatStatusBar.SetCombatPaused(false);
			CombatStatusBar.IsCombatPlaybackActive = false;
		}

		// Token: 0x06000083 RID: 131 RVA: 0x000046BC File Offset: 0x000028BC
		internal static void SetCombatFrameTotal(int totalFrames)
		{
			CombatStatusBar.TotalCombatFrames = Math.Max(totalFrames, 0);
			CombatStatusBar.ProcessedCombatFrames = 0;
		}

		// Token: 0x06000084 RID: 132 RVA: 0x000046D0 File Offset: 0x000028D0
		internal static void AdvanceCombatFrame()
		{
			if (!CombatStatusBar.IsCombatPlaybackActive)
			{
				return;
			}
			CombatStatusBar.ProcessedCombatFrames++;
			if (CombatStatusBar.TotalCombatFrames > 0 && CombatStatusBar.ProcessedCombatFrames > CombatStatusBar.TotalCombatFrames)
			{
				CombatStatusBar.ProcessedCombatFrames = CombatStatusBar.TotalCombatFrames;
			}
		}

		// Token: 0x06000085 RID: 133 RVA: 0x00004704 File Offset: 0x00002904
		internal static TimeSpan GetCombatLogicalElapsed()
		{
			return TimeSpan.FromMilliseconds((double)CombatStatusBar.ProcessedCombatFrames * 50.0);
		}

		// Token: 0x06000086 RID: 134 RVA: 0x0000471C File Offset: 0x0000291C
		internal static float StepCombatSpeed(int direction)
		{
			int num = CombatStatusBar.GetCurrentSpeedStepIndex();
			num = Math.Clamp(num + direction, 0, CombatStatusBar.SpeedSteps.Length - 1);
			return CombatStatusBar.SetCombatSpeed(CombatStatusBar.SpeedSteps[num]);
		}

		// Token: 0x06000087 RID: 135 RVA: 0x0000474E File Offset: 0x0000294E
		internal static float SetCombatSpeed(float speed)
		{
			if (!CombatStatusBar.IsSupportedSpeedStep(speed))
			{
				return CombatStatusBar.CombatSpeedMultiplier;
			}
			CombatStatusBar.CombatSpeedMultiplier = speed;
			CombatStatusBar.PersistCombatSpeed(CombatStatusBar.CombatSpeedMultiplier);
			return CombatStatusBar.CombatSpeedMultiplier;
		}

		// Token: 0x06000088 RID: 136 RVA: 0x00004773 File Offset: 0x00002973
		internal static bool ShouldRenderForState(bool overlayVisible, bool enabled)
		{
			return overlayVisible && enabled && ModState.IsInGameRun;
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00004784 File Offset: 0x00002984
		internal static bool CanStepCombatSpeed(int direction)
		{
			int num = CombatStatusBar.GetCurrentSpeedStepIndex() + direction;
			return num >= 0 && num < CombatStatusBar.SpeedSteps.Length;
		}

		// Token: 0x0600008A RID: 138 RVA: 0x000047A9 File Offset: 0x000029A9
		internal static float NormalizeConfiguredDefaultSpeed(float configuredSpeed)
		{
			if (!CombatStatusBar.IsSupportedSpeedStep(configuredSpeed))
			{
				return 1f;
			}
			return configuredSpeed;
		}

		// Token: 0x0600008B RID: 139 RVA: 0x000047BA File Offset: 0x000029BA
		[NullableContext(0)]
		internal static string FormatCombatSpeedLabel()
		{
			return string.Format("{0:0.00}x", CombatStatusBar.CombatSpeedMultiplier);
		}

		// Token: 0x0600008C RID: 140 RVA: 0x000047D0 File Offset: 0x000029D0
		[NullableContext(0)]
		internal static string GetDisplayedTimeLabel()
		{
			if (!CombatStatusBar.IsCombatPlaybackActive)
			{
				return "LastCombat";
			}
			return "Time";
		}

		// Token: 0x0600008D RID: 141 RVA: 0x000047E4 File Offset: 0x000029E4
		[NullableContext(0)]
		internal static string GetDisplayedTimeText()
		{
			if (CombatStatusBar.IsCombatPlaybackActive)
			{
				return CombatStatusBar.FormatElapsed(CombatStatusBar.GetCombatLogicalElapsed());
			}
			if (!CombatStatusBar.HasCompletedCombatPlayback)
			{
				return "-:--:--";
			}
			return CombatStatusBar.FormatElapsed(CombatStatusBar.LastCombatLogicalElapsed);
		}

		// Token: 0x0600008E RID: 142 RVA: 0x00004810 File Offset: 0x00002A10
		[NullableContext(0)]
		internal static string GetDisplayedFrameText()
		{
			if (!CombatStatusBar.IsCombatPlaybackActive)
			{
				return "Standby";
			}
			return CombatStatusBar.ProcessedCombatFrames.ToString();
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00004838 File Offset: 0x00002A38
		internal static float AdvanceVisualBlend(float current, bool active, float deltaTime)
		{
			float num = active ? 1f : 0f;
			float num2 = Math.Max(deltaTime, 0f) * 5f;
			if (current < num)
			{
				return Math.Min(current + num2, num);
			}
			if (current > num)
			{
				return Math.Max(current - num2, num);
			}
			return current;
		}

		// Token: 0x06000090 RID: 144 RVA: 0x00004884 File Offset: 0x00002A84
		internal static void ResetStateForTests()
		{
			CombatStatusBar.IsCombatPlaybackActive = false;
			CombatStatusBar.IsCombatPaused = false;
			CombatStatusBar.CombatSpeedMultiplier = 1f;
			CombatStatusBar.ProcessedCombatFrames = 0;
			CombatStatusBar.TotalCombatFrames = 0;
			CombatStatusBar.LastCombatLogicalElapsed = TimeSpan.Zero;
			CombatStatusBar.HasCompletedCombatPlayback = false;
		}

		// Token: 0x06000091 RID: 145 RVA: 0x000048B8 File Offset: 0x00002AB8
		internal static bool CanToggleCombatPause()
		{
			return CombatStatusBar.IsCombatPlaybackActive && Singleton<GameServiceManager>.Instance != null;
		}

		// Token: 0x06000092 RID: 146 RVA: 0x000048CE File Offset: 0x00002ACE
		internal static bool ToggleCombatPause()
		{
			return CombatStatusBar.SetCombatPaused(!CombatStatusBar.IsCombatPaused);
		}

		// Token: 0x06000093 RID: 147 RVA: 0x000048E0 File Offset: 0x00002AE0
		internal static bool SetCombatPaused(bool paused)
		{
			GameServiceManager instance = Singleton<GameServiceManager>.Instance;
			if (instance == null)
			{
				return CombatStatusBar.IsCombatPaused;
			}
			instance.PauseOrUnpauseGame(paused);
			CombatStatusBar.IsCombatPaused = instance.GamePaused;
			return CombatStatusBar.IsCombatPaused;
		}

		// Token: 0x06000094 RID: 148 RVA: 0x0000491C File Offset: 0x00002B1C
		private static int GetCurrentSpeedStepIndex()
		{
			int result = 0;
			float num = float.MaxValue;
			for (int i = 0; i < CombatStatusBar.SpeedSteps.Length; i++)
			{
				float num2 = Math.Abs(CombatStatusBar.SpeedSteps[i] - CombatStatusBar.CombatSpeedMultiplier);
				if (num2 < num)
				{
					num = num2;
					result = i;
				}
			}
			return result;
		}

		// Token: 0x06000095 RID: 149 RVA: 0x00004960 File Offset: 0x00002B60
		private static bool IsSupportedSpeedStep(float speed)
		{
			for (int i = 0; i < CombatStatusBar.SpeedSteps.Length; i++)
			{
				if (Math.Abs(CombatStatusBar.SpeedSteps[i] - speed) < 0.0001f)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00004998 File Offset: 0x00002B98
		[NullableContext(0)]
		private static string FormatElapsed(TimeSpan elapsed)
		{
			int num = (int)elapsed.TotalMinutes;
			return string.Format("{0}:{1:00}:{2:00}", num, elapsed.Seconds, elapsed.Milliseconds / 10);
		}

		// Token: 0x06000097 RID: 151 RVA: 0x000049D9 File Offset: 0x00002BD9
		private static void PersistCombatSpeed(float speed)
		{
			if (CombatStatusBar._defaultCombatSpeedConfig != null)
			{
				CombatStatusBar._defaultCombatSpeedConfig.Value = speed;
			}
		}

		// Token: 0x04000022 RID: 34
		private const float BarHeight = 60f;

		// Token: 0x04000023 RID: 35
		private const float BarBottomMargin = 0f;

		// Token: 0x04000024 RID: 36
		private const float SegmentSpacing = 4f;

		// Token: 0x04000025 RID: 37
		private const int CanvasSortingOrder = 10;

		// Token: 0x04000026 RID: 38
		private static Sprite _roundedSprite;

		// Token: 0x04000027 RID: 39
		private static Font _uiFont;

		// Token: 0x04000028 RID: 40
		private GameObject _canvasObject;

		// Token: 0x04000029 RID: 41
		private Canvas _canvas;

		// Token: 0x0400002A RID: 42
		private RectTransform _barRoot;

		// Token: 0x0400002B RID: 43
		private Image _barBackground;

		// Token: 0x0400002C RID: 44
		private Image _barGlow;

		// Token: 0x0400002D RID: 45
		private Text _timeLabel;

		// Token: 0x0400002E RID: 46
		private Text _timeValue;

		// Token: 0x0400002F RID: 47
		private Image _timeBackground;

		// Token: 0x04000030 RID: 48
		private Text _frameLabel;

		// Token: 0x04000031 RID: 49
		private Text _frameValue;

		// Token: 0x04000032 RID: 50
		private Image _frameBackground;

		// Token: 0x04000033 RID: 51
		private Text _multiplierLabel;

		// Token: 0x04000034 RID: 52
		private Text _multiplierValue;

		// Token: 0x04000035 RID: 53
		private Button _decrementButton;

		// Token: 0x04000036 RID: 54
		private Text _decrementButtonText;

		// Token: 0x04000037 RID: 55
		private Image _decrementButtonBackground;

		// Token: 0x04000038 RID: 56
		private Button _incrementButton;

		// Token: 0x04000039 RID: 57
		private Text _incrementButtonText;

		// Token: 0x0400003A RID: 58
		private Image _incrementButtonBackground;

		// Token: 0x0400003B RID: 59
		private Image _multiplierBackground;

		// Token: 0x0400003C RID: 60
		private Text _pauseLabel;

		// Token: 0x0400003D RID: 61
		private Button _pauseButton;

		// Token: 0x0400003E RID: 62
		private Text _pauseButtonText;

		// Token: 0x0400003F RID: 63
		private Image _pauseButtonBackground;

		// Token: 0x04000040 RID: 64
		private Image _pauseBackground;

		// Token: 0x04000041 RID: 65
		private Image _timeDivider;

		// Token: 0x04000042 RID: 66
		private Image _frameDivider;

		// Token: 0x04000043 RID: 67
		private Image _multiplierDivider;

		// Token: 0x04000044 RID: 68
		private static ConfigEntry<bool> _enableCombatStatusBarConfig;

		// Token: 0x04000045 RID: 69
		private static ConfigEntry<float> _defaultCombatSpeedConfig;

		// Token: 0x04000046 RID: 70
		private bool _visible = true;

		// Token: 0x04000047 RID: 71
		private float _visualBlend;

		// Token: 0x04000048 RID: 72
		[Nullable(0)]
		private static readonly float[] SpeedSteps = new float[]
		{
			0.25f,
			0.5f,
			1f,
			1.5f,
			2f,
			3f
		};

		// Token: 0x02000065 RID: 101
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x040001A1 RID: 417
			[Nullable(0)]
			public static Action <0>__OnCombatStarted;

			// Token: 0x040001A2 RID: 418
			[Nullable(0)]
			public static Action <1>__OnCombatEnded;
		}
	}
}
