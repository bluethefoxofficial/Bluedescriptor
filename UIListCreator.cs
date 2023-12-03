using System;
using System.Collections.Generic;
using System.Reflection;
using ABI.CCK.Components;
using ABI_RC.Core.InteractionSystem;
using ABI_RC.Core.Networking.API;
using ABI_RC.Core.Networking.API.Responses;
using Bluedescriptor_Rewritten.UISYSTEM;
using HighlightPlus;
using MelonLoader;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x0200000C RID: 12
public class UIListCreator : MonoBehaviour
{
	// Token: 0x06000041 RID: 65 RVA: 0x00003316 File Offset: 0x00001516
	public void Show()
	{
		this.canvasGO.SetActive(true);
	}

	// Token: 0x06000042 RID: 66 RVA: 0x00003325 File Offset: 0x00001525
	public void Hide()
	{
		this.canvasGO.SetActive(false);
	}

	// Token: 0x06000043 RID: 67 RVA: 0x00003334 File Offset: 0x00001534
	public void applylist(List<string[]> replacement)
	{
		this.plList = replacement;
	}

	// Token: 0x06000044 RID: 68 RVA: 0x00003340 File Offset: 0x00001540
	public void GenerateUICanvas()
	{
		bool flag = !this.InitializeQuickMenu();
		if (!flag)
		{
			bool flag2 = !this.canvasGO;
			if (flag2)
			{
				this.CreateCanvasWithUI();
			}
			else
			{
				this.canvasGO = GameObject.Find("GeneratedUICanvas");
			}
		}
	}

	// Token: 0x06000045 RID: 69 RVA: 0x00003388 File Offset: 0x00001588
	private bool InitializeQuickMenu()
	{
		this.QuickMenu = GameObject.Find("QuickMenu");
		bool flag = this.QuickMenu == null;
		bool result;
		if (flag)
		{
			this.QuickMenu = GameObject.Find("QuickMenuParent");
			bool flag2 = this.QuickMenu == null;
			if (flag2)
			{
				MelonLogger.Error("QuickMenu GameObject not found.");
				result = false;
			}
			else
			{
				result = true;
			}
		}
		else
		{
			result = true;
		}
		return result;
	}

	// Token: 0x06000046 RID: 70 RVA: 0x000033F0 File Offset: 0x000015F0
	private void CreateCanvasWithUI()
	{
		this.canvasGO = new GameObject("GeneratedUICanvas");
		this.mat.mainTexture = new UIfunctions().LoadTextureFromAssembly(this.asm, "Bluedescriptor_Rewritten.res.Font.atlas.png");
		this.SetupCanvasComponents();
		this.CreateBackgroundForCanvas();
		this.PopulateListContainer();
		this.canvasGO.SetActive(false);
	}

	// Token: 0x06000047 RID: 71 RVA: 0x00003454 File Offset: 0x00001654
	private void SetupCanvasComponents()
	{
		Canvas canvas = this.canvasGO.AddComponent<Canvas>();
		GameObject gameObject = new GameObject();
		gameObject.transform.SetParent(canvas.transform, false);
		this.canvasGO.AddComponent<CanvasScaler>();
		this.canvasGO.AddComponent<GraphicRaycaster>();
		this.canvasGO.transform.localScale = new Vector3(0.005f, 0.01f, 1f);
		this.canvasGO.transform.localPosition = new Vector3(-0.72f, 0f, 0f);
		this.canvasGO.transform.SetParent(this.QuickMenu.transform, false);
		canvas.renderMode = 2;
	}

	// Token: 0x06000048 RID: 72 RVA: 0x00003510 File Offset: 0x00001710
	private void CreateBackgroundForCanvas()
	{
		GameObject gameObject = new GameObject("Background");
		gameObject.transform.SetParent(this.canvasGO.transform, false);
		Image image = gameObject.AddComponent<Image>();
		Assembly executingAssembly = Assembly.GetExecutingAssembly();
		Material material = new Material(Shader.Find("UI/Default"));
		image.material = material;
		material.mainTexture = new UIfunctions().LoadTextureFromAssembly(executingAssembly, "Bluedescriptor_Rewritten.res.PLI.png");
		RectTransform component = gameObject.GetComponent<RectTransform>();
		component.anchorMin = new Vector2(0f, 0f);
		component.anchorMax = new Vector2(1f, 1f);
		component.sizeDelta = this.canvasGO.transform.localScale;
	}

	// Token: 0x06000049 RID: 73 RVA: 0x000035D0 File Offset: 0x000017D0
	private void PopulateListContainer()
	{
		bool flag = this.canvasGO == null;
		if (flag)
		{
			MelonLogger.Error("canvasGO is not set. Please ensure it is initialized and active before populating the list container.");
		}
		else
		{
			this.canvasGO.AddComponent<HighlightEffect>();
			Rigidbody rigidbody = this.canvasGO.AddComponent<Rigidbody>();
			rigidbody.isKinematic = true;
			rigidbody.detectCollisions = true;
			GameObject gameObject = new GameObject("ListContainer");
			gameObject.transform.SetParent(this.canvasGO.transform, false);
			gameObject.transform.localPosition = new Vector3(5f, -3f, 0f);
			gameObject.transform.transform.localScale = new Vector3(1f, 0.9f, 1f);
			GameObject gameObject2 = new GameObject("ScrollView");
			gameObject2.transform.SetParent(gameObject.transform, false);
			gameObject2.AddComponent<RectTransform>();
			this.AddBoxCollider(gameObject2);
			ScrollRect scrollRect = gameObject2.AddComponent<ScrollRect>();
			gameObject2.AddComponent<CVRInteractable>();
			GameObject gameObject3 = new GameObject("Viewport");
			gameObject3.transform.localScale = new Vector3(gameObject3.transform.localScale.x, gameObject3.transform.localScale.y, gameObject3.transform.localScale.z + 1f);
			gameObject3.transform.SetParent(gameObject2.transform, false);
			RectTransform rectTransform = gameObject3.AddComponent<RectTransform>();
			rectTransform.anchorMin = new Vector2(0f, 0f);
			rectTransform.anchorMax = new Vector2(1f, 1f);
			rectTransform.sizeDelta = new Vector2(0f, 0f);
			rectTransform.pivot = new Vector2(0.5f, 0.5f);
			gameObject3.AddComponent<Mask>().showMaskGraphic = false;
			gameObject3.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.5f);
			gameObject3.AddComponent<CVR_Menu_Pointer_Reciever>();
			GameObject gameObject4 = new GameObject("Scrollbar");
			gameObject4.transform.localScale = new Vector3(gameObject4.transform.localScale.x, gameObject4.transform.localScale.y, gameObject4.transform.localScale.z + 1f);
			gameObject4.transform.SetParent(gameObject2.transform, false);
			Scrollbar scrollbar = gameObject4.AddComponent<Scrollbar>();
			scrollbar.direction = 2;
			GameObject gameObject5 = new GameObject("Handle");
			gameObject5.transform.SetParent(gameObject4.transform, false);
			Image image = gameObject5.AddComponent<Image>();
			image.color = Color.white;
			scrollbar.handleRect = gameObject5.GetComponent<RectTransform>();
			gameObject5.GetComponent<RectTransform>().sizeDelta = new Vector2(2f, 0f);
			gameObject5.transform.localScale = new Vector3(gameObject5.transform.localScale.x, gameObject5.transform.localScale.y, gameObject5.transform.localScale.z + 1f);
			RectTransform component = gameObject4.GetComponent<RectTransform>();
			component.sizeDelta = new Vector2(2f, 0f);
			component.anchorMin = new Vector2(1f, 0f);
			component.anchorMax = new Vector2(1f, 1f);
			component.pivot = new Vector2(1f, 0.5f);
			component.anchoredPosition = new Vector2(-15f, 0f);
			component.offsetMin = new Vector2(component.offsetMin.x, 0f);
			component.offsetMax = new Vector2(component.offsetMax.x, 0f);
			scrollRect.verticalScrollbar = scrollbar;
			scrollRect.verticalScrollbarVisibility = 2;
			scrollRect.verticalScrollbarSpacing = -3f;
			GameObject gameObject6 = new GameObject("Content-BDM");
			this.cont = gameObject6;
			gameObject6.transform.SetParent(gameObject3.transform, false);
			RectTransform rectTransform2 = gameObject6.AddComponent<RectTransform>();
			rectTransform2.sizeDelta = new Vector2(0f, 0f);
			rectTransform2.anchorMin = new Vector2(0f, 0f);
			rectTransform2.anchorMax = new Vector2(1f, 1f);
			rectTransform2.pivot = new Vector2(0.5f, 1f);
			gameObject6.AddComponent<ContentSizeFitter>().verticalFit = 2;
			VerticalLayoutGroup verticalLayoutGroup = gameObject6.AddComponent<VerticalLayoutGroup>();
			verticalLayoutGroup.childForceExpandWidth = true;
			verticalLayoutGroup.childForceExpandHeight = false;
			verticalLayoutGroup.childControlWidth = true;
			verticalLayoutGroup.childControlHeight = true;
			verticalLayoutGroup.childAlignment = 1;
			verticalLayoutGroup.spacing = 2f;
			gameObject6.AddComponent<CVR_Menu_Pointer_Reciever>();
			gameObject6.AddComponent<HighlightEffect>();
			scrollRect.content = rectTransform2;
			scrollRect.viewport = rectTransform;
			scrollRect.horizontal = false;
			this.PopulateListWithItems(gameObject6);
		}
	}

	// Token: 0x0600004A RID: 74 RVA: 0x00003AEC File Offset: 0x00001CEC
	private void AddBoxCollider(GameObject gameObject)
	{
		RectTransform component = gameObject.GetComponent<RectTransform>();
		bool flag = component != null;
		if (flag)
		{
			BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
			boxCollider.size = new Vector3(component.rect.width, component.rect.height, 0.01f);
			boxCollider.isTrigger = true;
		}
	}

	// Token: 0x0600004B RID: 75 RVA: 0x00003B48 File Offset: 0x00001D48
	private void PopulateListWithItems(GameObject content)
	{
		bool flag = this.plList != null && this.plList.Count > 0;
		if (flag)
		{
			using (List<string[]>.Enumerator enumerator = this.plList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string[] item = enumerator.Current;
					bool flag2 = item[1].ToLower() == "true";
					if (flag2)
					{
						this.CreateButton("[Friend] " + item[0], content.transform, delegate
						{
							var <>f__AnonymousType = new
							{
								userID = item[2]
							};
							ApiConnection.MakeRequest<UserDetailsResponse>(210, <>f__AnonymousType, null);
						}, new Color?(Color.yellow), null);
					}
					else
					{
						this.CreateButton("[Friend] " + item[0], content.transform, delegate
						{
							var <>f__AnonymousType = new
							{
								userID = item[2]
							};
							ApiConnection.MakeRequest<UserDetailsResponse>(210, <>f__AnonymousType, null);
						}, null, null);
					}
				}
			}
			MelonLogger.Log("Items added to content.");
		}
		else
		{
			Debug.LogError("Player list (plList) is null or empty!");
		}
	}

	// Token: 0x0600004C RID: 76 RVA: 0x00003C88 File Offset: 0x00001E88
	private void CreateButton(string text, Transform parent, UnityAction onClick, Color? col = null, Color? hoverColor = null)
	{
		Color color = col ?? Color.white;
		Color highlightedColor = hoverColor ?? new Color(0.75f, 0.75f, 0.75f);
		GameObject gameObject = new GameObject("Button - " + text);
		gameObject.transform.SetParent(parent, false);
		gameObject.AddComponent<HighlightEffect>();
		gameObject.AddComponent<CVRInteractable>();
		RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
		rectTransform.sizeDelta = new Vector2(200f, 35f);
		Button button = gameObject.AddComponent<Button>();
		ColorBlock colors = button.colors;
		colors.normalColor = color;
		colors.highlightedColor = highlightedColor;
		colors.pressedColor = color * new Color(0.8f, 0.8f, 0.8f);
		button.colors = colors;
		GameObject gameObject2 = new GameObject("Text");
		TextMeshProUGUI textMeshProUGUI = gameObject2.AddComponent<TextMeshProUGUI>();
		RectTransform rectTransform2 = gameObject2.AddComponent<RectTransform>();
		gameObject2.transform.SetParent(gameObject.transform, false);
		gameObject2.AddComponent<CanvasRenderer>();
		TMP_FontAsset tmp_FontAsset = Resources.FindObjectsOfTypeAll<TMP_FontAsset>()[0];
		bool flag = tmp_FontAsset == null;
		if (flag)
		{
			MelonLogger.Error("Failed to load TMP_FontAsset.");
		}
		else
		{
			textMeshProUGUI.font = tmp_FontAsset;
		}
		textMeshProUGUI.fontSize = 24f;
		textMeshProUGUI.color = Color.black;
		textMeshProUGUI.text = text;
		rectTransform2.sizeDelta = new Vector2(200f, 35f);
		rectTransform2.anchoredPosition = Vector2.zero;
		bool flag2 = onClick != null;
		if (flag2)
		{
			button.onClick.AddListener(onClick);
		}
		gameObject.SetActive(true);
	}

	// Token: 0x0600004D RID: 77 RVA: 0x00003E4C File Offset: 0x0000204C
	private void CreateTextItem(string text, Transform parent, Color? col = null)
	{
		Color color = col ?? Color.white;
		GameObject gameObject = new GameObject("ListItem - " + text);
		gameObject.transform.SetParent(parent, false);
		gameObject.AddComponent<HighlightEffect>();
		RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
		rectTransform.sizeDelta = new Vector2(200f, 35f);
		TextMeshProUGUI textMeshProUGUI = gameObject.AddComponent<TextMeshProUGUI>();
		TMP_FontAsset tmp_FontAsset = Resources.FindObjectsOfTypeAll<TMP_FontAsset>()[0];
		bool flag = tmp_FontAsset == null;
		if (flag)
		{
			MelonLogger.Error("Failed to load TMP_FontAsset.");
		}
		textMeshProUGUI.font = tmp_FontAsset;
		textMeshProUGUI.fontSize = 4f;
		textMeshProUGUI.color = color;
		textMeshProUGUI.text = text;
		gameObject.SetActive(true);
	}

	// Token: 0x0600004E RID: 78 RVA: 0x00003F10 File Offset: 0x00002110
	public void resetlistui()
	{
		GameObject gameObject = this.cont;
		bool flag = gameObject == null;
		if (!flag)
		{
			Transform transform = gameObject.transform;
			foreach (object obj in transform)
			{
				Transform transform2 = (Transform)obj;
				Object.Destroy(transform2.gameObject);
			}
			bool flag2 = this.plList != null;
			if (flag2)
			{
				foreach (string[] array in this.plList)
				{
					bool flag3 = array[1].ToLower() == "true";
					if (flag3)
					{
						this.CreateTextItem("[Friend] " + array[0], transform, new Color?(Color.blue));
					}
					else
					{
						this.CreateTextItem(array[0], transform.transform, null);
					}
				}
			}
			else
			{
				MelonLogger.Error("plList is null!");
			}
		}
	}

	// Token: 0x04000016 RID: 22
	public GameObject QuickMenu;

	// Token: 0x04000017 RID: 23
	public GameObject listItemPrefab;

	// Token: 0x04000018 RID: 24
	private GameObject canvasGO;

	// Token: 0x04000019 RID: 25
	private GameObject cont;

	// Token: 0x0400001A RID: 26
	private Material mat = new Material(Shader.Find("TextMeshPro/Distance Field"));

	// Token: 0x0400001B RID: 27
	private Assembly asm = Assembly.GetExecutingAssembly();

	// Token: 0x0400001C RID: 28
	public List<string[]> plList = new List<string[]>();

	// Token: 0x0400001D RID: 29
	private const float FadeDuration = 0.5f;
}
