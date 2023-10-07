using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using MelonLoader;

using TMPro;



using Bluedescriptor_Rewritten.UISYSTEM;
using System.Reflection;
using System.Runtime.Versioning;
using System;

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
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();
        canvasGO.transform.localScale = new Vector3(0.005f, 0.01f, 0f);
        canvasGO.transform.localPosition = new Vector3(-0.62f, 0f, 0f);
        canvasGO.transform.SetParent(QuickMenu.transform, false);
        canvas.renderMode = RenderMode.WorldSpace;
    }

    private void CreateBackgroundForCanvas()
    {
        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(canvasGO.transform, false);
        UnityEngine.UI.Image bgImage = bg.AddComponent<UnityEngine.UI.Image>();
        bgImage.color = new UnityEngine.Color(0.5f, 0f, 0.5f, 0.4f);
        RectTransform bgRect = bg.GetComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0, 0);
        bgRect.anchorMax = new Vector2(1, 1);
        bgRect.sizeDelta = Vector2.zero;
    }

    private void PopulateListContainer()
    {
        GameObject listContainer = new GameObject("ListContainer");
        listContainer.transform.SetParent(canvasGO.transform, false);

        // No more scroll view setup here

        GameObject content = new GameObject("Content-BDM");
        cont = content;
        content.transform.SetParent(listContainer.transform, false);
        SetupListLayout(content);

        PopulateListWithItems(content);
    }


    private void SetupListLayout(GameObject content)
    {
        VerticalLayoutGroup layoutGroup = content.AddComponent<VerticalLayoutGroup>();
        layoutGroup.childForceExpandWidth = true;
        layoutGroup.childForceExpandHeight = false;
        layoutGroup.spacing = 5;
    }

    private void PopulateListWithItems(GameObject content)
    {
        if (plList == null || plList.Count == 0)
        {
          /*  GameObject aloneItem = new GameObject("AloneItem");
            aloneItem.transform.SetParent(content.transform, false);
            TextMeshProUGUI aloneTextMesh = aloneItem.AddComponent<TextMeshProUGUI>();
            aloneTextMesh.text = "All alone";*/
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
            rectTransform.sizeDelta = new Vector2(200, 10);  // Adjust as needed

            TextMeshProUGUI textComponent = listItem.AddComponent<TextMeshProUGUI>();

            textComponent.font = Resources.Load("Fonts/ARIAL", typeof(Font) as Font);
            textComponent.fontSize = 24;
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
