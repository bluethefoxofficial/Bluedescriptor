using ABI_RC.Core.Base;
using ABI_RC.Core.InteractionSystem;
using ABI_RC.Core.Networking.API;
using ABI_RC.Core.Networking.API.Responses;
using ABI_RC.Core.Networking.IO.UserGeneratedContent;
using Bluedescriptor_Rewritten.UISYSTEM;
using HighlightPlus;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIListCreator : MonoBehaviour
{
    public GameObject QuickMenu;
    public GameObject listItemPrefab;
    GameObject canvasGO;
    GameObject cont;
    Material mat = new Material(Shader.Find("TextMeshPro/Distance Field"));
    Assembly asm = Assembly.GetExecutingAssembly();
    public List<string[]> plList = new List<string[]>();

    const float FadeDuration = 0.5f; // Duration of the fade effect in seconds
    public void Show() => canvasGO.SetActive(true);

    public void Hide() => canvasGO.SetActive(false);

    public void applylist(List<string[]> replacement) => plList = replacement;

    public void GenerateUICanvas()
    {
        if (!InitializeQuickMenu())
            return;

        if (!canvasGO)
            CreateCanvasWithUI();
        else
            canvasGO = GameObject.Find("GeneratedUICanvas");
    }

    bool InitializeQuickMenu()
    {
        QuickMenu = GameObject.Find("QuickMenu");

        if (QuickMenu == null)
        {
            QuickMenu = GameObject.Find("QuickMenuParent"); //for the upcoming update

            if (QuickMenu == null)
            {
                MelonLogger.Error("QuickMenu GameObject not found.");
                return false;
            }

            return true;
        }

        return true;
    }

    void CreateCanvasWithUI()
    {
        canvasGO = new GameObject("GeneratedUICanvas");
        mat.mainTexture = new UIfunctions().LoadTextureFromAssembly(asm, "Bluedescriptor_Rewritten.res.Font.atlas.png");

        SetupCanvasComponents();
        CreateBackgroundForCanvas();
        PopulateListContainer();

        canvasGO.SetActive(false);
    }

    void SetupCanvasComponents()
    {
        var canvas = canvasGO.AddComponent<Canvas>();
        var sr = new GameObject();

        sr.transform.SetParent(canvas.transform, false);
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();
        canvasGO.transform.localScale = new Vector3(0.005f, 0.01f, 1f);
        canvasGO.transform.localPosition = new Vector3(-0.72f, 0f, 0f);
        canvasGO.transform.SetParent(QuickMenu.transform, false);
        canvas.renderMode = RenderMode.WorldSpace;
    }

    void CreateBackgroundForCanvas()
    {
        var bg = new GameObject("Background");
        bg.transform.SetParent(canvasGO.transform, false);
        var bgImage = bg.AddComponent<UnityEngine.UI.Image>();
        var asm = Assembly.GetExecutingAssembly();
        var bmat = new Material(Shader.Find("UI/Default"));

        bgImage.material = bmat;
        bmat.mainTexture = new UIfunctions().LoadTextureFromAssembly(asm, "Bluedescriptor_Rewritten.res.PLI.png");
        var bgRect = bg.GetComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0, 0);
        bgRect.anchorMax = new Vector2(1, 1);
        bgRect.sizeDelta = canvasGO.transform.localScale;
    }

    void PopulateListContainer()
    {
        // Check if canvasGO is already created and active
        if (canvasGO == null)
        {
            MelonLogger.Error("canvasGO is not set. Please ensure it is initialized and active before populating the list container.");
            return;
        }

        canvasGO.AddComponent<HighlightEffect>();

        var rgb = canvasGO.AddComponent<Rigidbody>();
        rgb.isKinematic = true;
        rgb.detectCollisions = true;

        var listContainer = new GameObject("ListContainer");
        listContainer.transform.SetParent(canvasGO.transform, false);
        listContainer.transform.localPosition = new Vector3(5, -3, 0);
        listContainer.transform.transform.localScale = new Vector3(1,0.9f,1);

        // Set up the ScrollView
        var scrollView = new GameObject("ScrollView");
        scrollView.transform.SetParent(listContainer.transform, false);
        scrollView.AddComponent<RectTransform>();
        AddBoxCollider(scrollView);
        var scrollRect = scrollView.AddComponent<ScrollRect>();
        scrollView.AddComponent<ABI.CCK.Components.CVRInteractable>();

        // Set up the Viewport
        var viewport = new GameObject("Viewport");

        viewport.transform.localScale = new Vector3(
           viewport.transform.localScale.x,
           viewport.transform.localScale.y,
           viewport.transform.localScale.z + 1);

        viewport.transform.SetParent(scrollView.transform, false);
        var viewportRect = viewport.AddComponent<RectTransform>();
        viewportRect.anchorMin = new Vector2(0, 0);
        viewportRect.anchorMax = new Vector2(1, 1);
        viewportRect.sizeDelta = new Vector2(0, 0);
        viewportRect.pivot = new Vector2(0.5f, 0.5f);
        viewport.AddComponent<Mask>().showMaskGraphic = false;
        viewport.AddComponent<Image>().color = new Color(0, 0, 0, 0.5f);
        viewport.AddComponent<CVR_Menu_Pointer_Reciever>();

        // Set up the Scrollbar
        var scrollbar = new GameObject("Scrollbar");

        scrollbar.transform.localScale = new Vector3(
            scrollbar.transform.localScale.x,
            scrollbar.transform.localScale.y,
            scrollbar.transform.localScale.z + 1);

        scrollbar.transform.SetParent(scrollView.transform, false); // Parent to the ScrollView
        var scrollbarComponent = scrollbar.AddComponent<Scrollbar>();
        scrollbarComponent.direction = Scrollbar.Direction.BottomToTop; // Set the direction of the scrollbar

        // Create the Scrollbar Handle
        var handle = new GameObject("Handle");
        handle.transform.SetParent(scrollbar.transform, false); // Parent to the Scrollbar
        var handleImage = handle.AddComponent<Image>();
        handleImage.color = Color.white; // Set a default color for visibility, change as needed
        scrollbarComponent.handleRect = handle.GetComponent<RectTransform>();
        handle.GetComponent<RectTransform>().sizeDelta = new Vector2(2, 0); // Set the width of the handle

        handle.transform.localScale = new Vector3(
           handle.transform.localScale.x,
           handle.transform.localScale.y,
           handle.transform.localScale.z + 1);

        // Configure the Scrollbar
        var scrollbarRect = scrollbar.GetComponent<RectTransform>();
        scrollbarRect.sizeDelta = new Vector2(2, 0); // Set the width of the scrollbar to 30px
        scrollbarRect.anchorMin = new Vector2(1, 0);
        scrollbarRect.anchorMax = new Vector2(1, 1);
        scrollbarRect.pivot = new Vector2(1, 0.5f);
        scrollbarRect.anchoredPosition = new Vector2(-15, 0); // Position the scrollbar on the right, half width of the scrollbar for proper alignment
        scrollbarRect.offsetMin = new Vector2(scrollbarRect.offsetMin.x, 0); // Set bottom offset to 0
        scrollbarRect.offsetMax = new Vector2(scrollbarRect.offsetMax.x, 0); // Set top offset to 0 to ensure full height

        // Link the Scrollbar to the ScrollRect
        scrollRect.verticalScrollbar = scrollbarComponent;
        scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
        scrollRect.verticalScrollbarSpacing = -3;

        // Set up the Content
        var content = new GameObject("Content-BDM");
        cont = content; // Assuming 'cont' is a field of the class
        content.transform.SetParent(viewport.transform, false);
        var contentRect = content.AddComponent<RectTransform>();
        contentRect.sizeDelta = new Vector2(0, 0); // Set sizeDelta to zero to allow ContentSizeFitter to work
        contentRect.anchorMin = new Vector2(0, 0);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.pivot = new Vector2(0.5f, 1); // Pivot set to top center

        // Add layout components to the Content
        content.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        var verticalLayoutGroup = content.AddComponent<VerticalLayoutGroup>();
        verticalLayoutGroup.childForceExpandWidth = true;
        verticalLayoutGroup.childForceExpandHeight = false;
        verticalLayoutGroup.childControlWidth = true;
        verticalLayoutGroup.childControlHeight = true;
        verticalLayoutGroup.childAlignment = TextAnchor.UpperCenter;
        verticalLayoutGroup.spacing = 2f;
        content.AddComponent<CVR_Menu_Pointer_Reciever>();
        content.AddComponent<HighlightEffect>();
        // Link the Content to the ScrollRect
        scrollRect.content = contentRect;
        scrollRect.viewport = viewportRect;
        scrollRect.horizontal = false; // Only vertical scrolling is needed

        // Populate the list with items
        PopulateListWithItems(content);
    }
    private void AddBoxCollider(GameObject gameObject)
    {
        if (gameObject.GetComponent<RectTransform>() is RectTransform rectTransform)
        {
            BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.size = new Vector3(rectTransform.rect.width, rectTransform.rect.height, 0.01f); // Assuming a small depth as it's a 2D UI
            boxCollider.isTrigger = true; // Set to trigger to prevent physics interactions
        }
    }
    void PopulateListWithItems(GameObject content)
    {
        if (plList != null && plList.Count > 0)
        {
            foreach (var item in plList)
            {
                if (item[1].ToLower() == "true")
                {
                    CreateButton("[Friend] " + item[0], content.transform, () =>
                    {
                        var payload = new { userID = item[2] };
                        ApiConnection.MakeRequest<UserDetailsResponse>(ApiConnection.ApiOperation.UserDetails, payload);
                    
                    },Color.yellow);
                }
                else
                {
                    CreateButton("[Friend] " + item[0], content.transform, () =>
                    {
                        var payload = new { userID = item[2] };
                        ApiConnection.MakeRequest<UserDetailsResponse>(ApiConnection.ApiOperation.UserDetails, payload);

                    });
                }
            }

            MelonLogger.Log("Items added to content.");
        }
        else
        {
            Debug.LogError("Player list (plList) is null or empty!");
        }
    }


    void CreateButton(string text, Transform parent, UnityAction onClick, Color? col = null, Color? hoverColor = null)
    {
        var color = col ?? Color.white;
        var hoverCol = hoverColor ?? new Color(0.75f, 0.75f, 0.75f); // Default hover color, if not specified

        var buttonObject = new GameObject("Button - " + text);
        buttonObject.transform.SetParent(parent, false);

        buttonObject.AddComponent<HighlightEffect>();
        buttonObject.AddComponent<ABI.CCK.Components.CVRInteractable>();

        // Adding the RectTransform
        var rectTransform = buttonObject.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(200, 35); // Adjust size as needed

        // Adding a Button component
        var button = buttonObject.AddComponent<Button>();

        // Define the colors for different button states
        var colors = button.colors;
        colors.normalColor = color;
        colors.highlightedColor = hoverCol;
        colors.pressedColor = color * new Color(0.8f, 0.8f, 0.8f);
        button.colors = colors;

        // Adding a TextMeshProUGUI component
        var textObject = new GameObject("Text");
        var textComponent = textObject.AddComponent<TextMeshProUGUI>();
        var textRect = textObject.AddComponent<RectTransform>();

        textObject.transform.SetParent(buttonObject.transform, false);
        textObject.AddComponent<CanvasRenderer>(); // Ensure CanvasRenderer is added

        // Load the TMP_FontAsset
        var fontAsset = Resources.FindObjectsOfTypeAll<TMP_FontAsset>()[0];
        if (fontAsset == null)
        {
            MelonLogger.Error("Failed to load TMP_FontAsset.");
        }
        else
        {
            textComponent.font = fontAsset;
        }

        // Set text properties
        textComponent.fontSize = 24; // Adjust font size as needed, was previously very small
        textComponent.color = Color.black; // Text color
        textComponent.text = text;

        // Ensure text is properly positioned and sized
        textRect.sizeDelta = new Vector2(200, 35);
        textRect.anchoredPosition = Vector2.zero;

        // Assign the onClick event
        if (onClick != null)
        {
            button.onClick.AddListener(onClick);
        }

        // Set the buttonObject GameObject to active
        buttonObject.SetActive(true);
    }



    void CreateTextItem(string text, Transform parent, Color? col = null)
    {
        var color = col ?? Color.white;
        var listItem = new GameObject("ListItem - " + text);
        listItem.transform.SetParent(parent, false);
        listItem.AddComponent<HighlightEffect>();
        // Ensure the RectTransform has a visible size
        var rectTransform = listItem.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(200, 35);  // Adjust as needed, Y should be more than 1

        // Adding a TextMeshProUGUI component
        var textComponent = listItem.AddComponent<TextMeshProUGUI>();

        // Load the TMP_FontAsset. Make sure 'Fonts & Materials/ARIAL SDF' is the correct path
        var fontAsset = Resources.FindObjectsOfTypeAll<TMP_FontAsset>()[0];

        if (fontAsset == null)
        {
            MelonLogger.Error("Failed to load TMP_FontAsset.");
        }

        textComponent.font = fontAsset;

        // Set text properties
        textComponent.fontSize = 4;  // Adjust font size as needed
        textComponent.color = color;
        textComponent.text = text;

        // Set the listItem GameObject to active in case it defaults to inactive
        listItem.SetActive(true);
    }

    public void resetlistui()
    {
        var contentGO = cont;

        if (contentGO == null)
        {
            // MelonLogger.Error("GameObject 'Content-BDM' not found!");
            return; // Exit the method early
        }

        var contentTransform = contentGO.transform;

        // Clear the existing items in the list
        foreach (Transform child in contentTransform)
            Destroy(child.gameObject);

        // Repopulate the list with the updated player list
        if (plList != null)
        {
            foreach (var item in plList)
                if (item[1].ToLower() == "true")
                {
                    CreateTextItem("[Friend] " + item[0],contentTransform, Color.blue);
                }
                else
                {
                    CreateTextItem(item[0], contentTransform.transform);
                }
        }
        else
        {
            MelonLogger.Error("plList is null!");
        }
    }
}