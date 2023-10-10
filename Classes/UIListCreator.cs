using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using MelonLoader;

using TMPro;



using Bluedescriptor_Rewritten.UISYSTEM;
using System.Reflection;
using System.Runtime.Versioning;
using System;
using ABI_RC.Core.InteractionSystem;

public class UIListCreator : MonoBehaviour
{
    public GameObject QuickMenu;
    public GameObject listItemPrefab;
    private GameObject canvasGO;
    private GameObject cont;
    Material mat = new Material(Shader.Find("TextMeshPro/Distance Field"));
    Assembly asm = Assembly.GetExecutingAssembly();
    public List<string[]> plList = new List<string[]>();



    private const float FadeDuration = 0.5f; // Duration of the fade effect in seconds

    public void Show()
    {
        canvasGO.SetActive(true);
    }

    public void Hide()
    {
        canvasGO.SetActive(false);
    }

    public void applylist(List<string[]> replacement)
    {
        plList = replacement;
    }

    public void GenerateUICanvas()
    {
        if (!InitializeQuickMenu())
            return;

        if (!canvasGO)
            CreateCanvasWithUI();
        else
            canvasGO = GameObject.Find("GeneratedUICanvas");
    }

    private bool InitializeQuickMenu()
    {
        QuickMenu = GameObject.Find("QuickMenu");
        if (QuickMenu == null)
        {
            MelonLogger.Error("QuickMenu GameObject not found.");
            return false;
        }
        return true;
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
        GameObject sr = new GameObject();

        sr.transform.SetParent(canvas.transform, false);
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();
        canvasGO.transform.localScale = new Vector3(0.005f, 0.01f, 1f);
        canvasGO.transform.localPosition = new Vector3(-0.62f, 0f, 0f);
        canvasGO.transform.SetParent(QuickMenu.transform, false);
        canvas.renderMode = RenderMode.WorldSpace;
    }

    private void CreateBackgroundForCanvas()
    {
        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(canvasGO.transform, false);
        UnityEngine.UI.Image bgImage = bg.AddComponent<UnityEngine.UI.Image>();
        Assembly asm = Assembly.GetExecutingAssembly();
        Material bmat = new Material(Shader.Find("UI/Default"));
        bgImage.material = bmat;
      bmat.mainTexture =new UIfunctions().LoadTextureFromAssembly(asm, "Bluedescriptor_Rewritten.res.PLI.png");
        RectTransform bgRect = bg.GetComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0, 0);
        bgRect.anchorMax = new Vector2(1, 1);
        bgRect.sizeDelta = canvasGO.transform.localScale;
    }

    private void PopulateListContainer()
    {
        GameObject listContainer = new GameObject("ListContainer");
        listContainer.transform.SetParent(canvasGO.transform, false);
        listContainer.transform.localPosition = new Vector3(3,-6,0);

        // Set up the ScrollView
        GameObject scrollView = new GameObject("ScrollView");
        scrollView.transform.SetParent(listContainer.transform, false);
        ScrollRect scrollRect = scrollView.AddComponent<ScrollRect>();
        scrollView.AddComponent<CVR_Menu_Pointer_Reciever>();

        // Set up the Viewport
        GameObject viewport = new GameObject("Viewport");
        viewport.transform.SetParent(scrollView.transform, false);
        RectTransform viewportRect = viewport.AddComponent<RectTransform>();
        viewportRect.anchorMin = new Vector2(0, 0);
        viewportRect.anchorMax = new Vector2(1, 1);
        viewportRect.sizeDelta = new Vector2(0, 0);
        viewportRect.pivot = new Vector2(0.5f, 0.5f);
        viewport.AddComponent<Mask>().showMaskGraphic = false;
        viewport.AddComponent<Image>().color = new Color(0, 0, 0, 0.5f); // Optional: set a background color for the viewport

        // Set up the Content
        GameObject content = new GameObject("Content-BDM");
        cont = content;
        content.transform.SetParent(viewport.transform, false);
        SetupListLayout(content);

        // Link the Content to the ScrollRect
        scrollRect.content = content.GetComponent<RectTransform>();

        PopulateListWithItems(content);
    }



    private void SetupListLayout(GameObject content)
    {
        VerticalLayoutGroup layoutGroup = content.AddComponent<VerticalLayoutGroup>();
        layoutGroup.childForceExpandWidth = true;
        layoutGroup.childForceExpandHeight = false;
        layoutGroup.spacing = 3;
    }

    private void PopulateListWithItems(GameObject content)
    {
        if (plList == null || plList.Count == 0)
        {
           
        }
        else
        {
            foreach (var item in plList)
            {
                CreateTextItem(item[0], content.transform);
            }
        }
    }





    private void CreateTextItem(string text, Transform parent)
    {
        try
        {
            GameObject listItem = new GameObject("ListItem - " + text);
            listItem.transform.SetParent(parent, false);

            // Ensure the RectTransform has some size
            RectTransform rectTransform = listItem.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(200, 1);  // Adjust as needed

            TextMeshProUGUI textComponent = listItem.AddComponent<TextMeshProUGUI>();

            textComponent.font = Resources.Load<TMP_FontAsset>("Fonts/ARIAL");

            textComponent.fontSize = 5;
            textComponent.fontSharedMaterial = mat;
            textComponent.material = mat;
            textComponent.color = UnityEngine.Color.white;
            textComponent.text = text;


        }
        catch (Exception e)
        {
            MelonLogger.Error("Failed to create list item \n" + e);
        }
    }


    public void resetlistui()
    {
        GameObject contentGO = cont;
        if (contentGO == null)
        {
            // MelonLogger.Error("GameObject 'Content-BDM' not found!");
            return; // Exit the method early
        }

        Transform contentTransform = contentGO.transform;

        // Clear the existing items in the list
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }

        // Repopulate the list with the updated player list
        if (plList != null)
        {
            foreach (var item in plList)
            {
                CreateTextItem(item[0], contentTransform);
            }
        }
        else
        {
            MelonLogger.Error("plList is null!");
        }
    }

}