using ABI.CCK.Components;
using ABI_RC.Core.InteractionSystem;
using Bluedescriptor_Rewritten.UISYSTEM;
using HighlightPlus;
using MelonLoader;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Timers;

public class UIListCreator 


{
    private Timer fadeTimer;
    private float fadeValue = 0f;
    private bool fadeIn = true;
    private CanvasGroup canvasGroup;
    private float fadeDuration = 0.2f;
    private float fadeProgress = 0f;
    private bool isFading = false;

    public UIListCreator()
    {
        fadeTimer = new Timer(10); // Tick every 100ms
        fadeTimer.Elapsed += OnFadeTimerElapsed;
        fadeTimer.AutoReset = true;
        fadeTimer.Enabled = true;
    }
    private void OnFadeTimerElapsed(object sender, ElapsedEventArgs e)
    {
        if (isFading)
        {
            // Adjust fadeValue based on fading in or out
            if (fadeIn)
            {
                fadeValue += Time.deltaTime / fadeDuration; // Increment for fade in
            }
            else
            {
                fadeValue -= Time.deltaTime / fadeDuration; // Decrement for fade out
            }

            // Clamp fadeValue between 0 and 1
            fadeValue = Mathf.Clamp01(fadeValue);

            // Check if the fade has completed
            if ((fadeIn && fadeValue >= 1f) || (!fadeIn && fadeValue <= 0f))
            {
                isFading = false;
                fadeTimer.Stop();

                // Directly modify Unity objects as MelonLoader handles main thread dispatch
                canvasGroup.alpha = fadeValue;
                if (!fadeIn)
                {
                    canvasGO.SetActive(false); // Deactivate after fade out
                }
            }
            else
            {
                // Directly modify Unity objects as MelonLoader handles main thread dispatch
                canvasGroup.alpha = fadeValue;
            }
        }
    }

    public void StartFadeIn()
    {
        fadeIn = true;
        isFading = true;
        fadeValue = 0f;
        fadeTimer.Start();
    }

    // Method to start fading out
    public void StartFadeOut()
    {
        fadeIn = false;
        isFading = true;
        fadeValue = 0f;
        fadeTimer.Start();
    }

    public void Show()
    {


        canvasGO.SetActive(true);

StartFadeIn();
    }

    public void Hide()
    {
        fadeIn = false;
        isFading = true;
        fadeValue = 1f; // Start from fully visible if fading out
        fadeTimer.Start();
    }



    public void ManualFadeUpdate()
    {
        if (isFading)
        {
            fadeProgress += Time.deltaTime / fadeDuration;

            if (fadeProgress >= 1f)
            {
                isFading = false;
                fadeProgress = 1f;
                if (!fadeIn)
                {
                    canvasGO.SetActive(false); // Deactivate after fade out
                }
            }

            float alpha = fadeIn ? fadeProgress : 1 - fadeProgress;
            canvasGroup.alpha = Mathf.Clamp01(alpha);
        }
    }

    private IEnumerator FadeCanvas(CanvasGroup cg, float start, float end, float duration, Action onComplete = null)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            cg.alpha = Mathf.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        cg.alpha = end;
        onComplete?.Invoke();
    }
    public void applylist(List<string[]> replacement)
    {
        plList = replacement;
    }

    
    public void GenerateUICanvas()
    {
        bool flag = !InitializeQuickMenu();
        if (!flag)
        {
            bool flag2 = !canvasGO;
            if (flag2)
            {
                CreateCanvasWithUI();
            }
            else
            {
                canvasGO = GameObject.Find("GeneratedUICanvas");
            }
        }
    }

 
    private bool InitializeQuickMenu()
    {
        QuickMenu = GameObject.Find("QuickMenu");
        bool flag = QuickMenu == null;
        bool result;
        if (flag)
        {
            QuickMenu = GameObject.Find("QuickMenuParent");
            bool flag2 = QuickMenu == null;
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


    private void CreateCanvasWithUI()
    {
        canvasGO = new GameObject("GeneratedUICanvas");
        mat.mainTexture = new UIfunctions().LoadTextureFromAssembly(asm, "Bluedescriptor_Rewritten.res.Font.atlas.png");
        SetupCanvasComponents();
        CreateBackgroundForCanvas();
        PopulateListContainer();
        canvasGO.SetActive(false);
    }

    private void SetupCanvasComponents()
    {
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        GameObject gameObject = new GameObject();
        gameObject.transform.SetParent(canvas.transform, false);
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();
        canvasGO.transform.localScale = new Vector3(0.005f, 0.01f, 1f);
        canvasGO.transform.localPosition = new Vector3(-0.72f, 0f, 0f);
        canvasGO.transform.SetParent(QuickMenu.transform, false);
        canvas.renderMode = RenderMode.WorldSpace;
        // Ensure canvasGO has a CanvasGroup component
        canvasGroup = canvasGO.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = canvasGO.AddComponent<CanvasGroup>();
        }

        // Start with the canvas hidden
        canvasGroup.alpha = 0f;
        canvasGO.SetActive(false);
    }

    private void CreateBackgroundForCanvas()
    {
        GameObject gameObject = new GameObject("Background");
        gameObject.transform.SetParent(canvasGO.transform, false);
        Image image = gameObject.AddComponent<Image>();
        Assembly executingAssembly = Assembly.GetExecutingAssembly();
        Material material = new Material(Shader.Find("UI/Default"));
        image.material = material;
        material.mainTexture = new UIfunctions().LoadTextureFromAssembly(executingAssembly, "Bluedescriptor_Rewritten.res.PLI.png");
        RectTransform component = gameObject.GetComponent<RectTransform>();
        component.anchorMin = new Vector2(0f, 0f);
        component.anchorMax = new Vector2(1f, 1f);
        component.sizeDelta = canvasGO.transform.localScale;
    }


    private void PopulateListContainer()
    {
        bool flag = canvasGO == null;
        if (flag)
        {
            MelonLogger.Error("canvasGO is not set. Please ensure it is initialized and active before populating the list container.");
        }
        else
        {
            canvasGO.AddComponent<HighlightEffect>();
            Rigidbody rigidbody = canvasGO.AddComponent<Rigidbody>();
            rigidbody.isKinematic = true;
            rigidbody.detectCollisions = true;
            GameObject gameObject = new GameObject("ListContainer");
            gameObject.transform.SetParent(canvasGO.transform, false);
            gameObject.transform.localPosition = new Vector3(5f, -3f, 0f);
            gameObject.transform.transform.localScale = new Vector3(1f, 0.9f, 1f);
            GameObject gameObject2 = new GameObject("ScrollView");
            gameObject2.transform.SetParent(gameObject.transform, false);
            gameObject2.AddComponent<RectTransform>();
            AddBoxCollider(gameObject2);
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
            scrollbar.direction = Scrollbar.Direction.TopToBottom;
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
            scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            scrollRect.verticalScrollbarSpacing = -3f;
            GameObject gameObject6 = new GameObject("Content-BDM");
            cont = gameObject6;
            gameObject6.transform.SetParent(gameObject3.transform, false);
            RectTransform rectTransform2 = gameObject6.AddComponent<RectTransform>();
            rectTransform2.sizeDelta = new Vector2(0f, 0f);
            rectTransform2.anchorMin = new Vector2(0f, 0f);
            rectTransform2.anchorMax = new Vector2(1f, 1f);
            rectTransform2.pivot = new Vector2(0.5f, 1f);
            gameObject6.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            VerticalLayoutGroup verticalLayoutGroup = gameObject6.AddComponent<VerticalLayoutGroup>();
            verticalLayoutGroup.childForceExpandWidth = true;
            verticalLayoutGroup.childForceExpandHeight = false;
            verticalLayoutGroup.childControlWidth = true;
            verticalLayoutGroup.childControlHeight = true;
            verticalLayoutGroup.childAlignment = TextAnchor.UpperLeft;
            verticalLayoutGroup.spacing = 2f;
            gameObject6.AddComponent<CVR_Menu_Pointer_Reciever>();
            gameObject6.AddComponent<HighlightEffect>();
            scrollRect.content = rectTransform2;
            scrollRect.viewport = rectTransform;
            scrollRect.horizontal = false;
            PopulateListWithItems(gameObject6);
        }
    }

 
    private void AddBoxCollider(GameObject gameObject)
    {
        RectTransform component = gameObject.GetComponent<RectTransform>();
        bool flag = component != null;
        if (flag)
        {
            var boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.size = new Vector3(component.rect.width, component.rect.height, 0.01f);
            boxCollider.isTrigger = true;
        }
    }

    private void PopulateListWithItems(GameObject content)
    {
        GameObject cont = this.cont;
        if (cont != null)
            return;
        Transform transform = cont.transform;
        foreach (Component component in transform)
            GameObject.Destroy(component.gameObject);
        if (plList != null)
        {
            foreach (string[] pl in plList)
            {
                if (pl[1].ToLower() == "true")
                    CreateTextItem("[Friend] " + pl[0], transform, new Color?(Color.blue));
                else
                    CreateTextItem(pl[0], transform.transform);
            }
        }
        else
            MelonLogger.Error("plList is null!");
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


    public void resetlistui()
    {
        GameObject gameObject = cont;
        bool flag = gameObject == null;
        if (!flag)
        {
            Transform transform = gameObject.transform;
            foreach (object obj in transform)
            {
                Transform transform2 = (Transform)obj;
                GameObject.Destroy(transform2.gameObject);
            }
            bool flag2 = plList != null;
            if (flag2)
            {
                foreach (string[] array in plList)
                {
                    bool flag3 = array[1].ToLower() == "true";
                    if (flag3)
                    {
                        CreateTextItem("[Friend] " + array[0], transform, new Color?(Color.blue));
                    }
                    else
                    {
                        CreateTextItem(array[0], transform.transform, null);
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
