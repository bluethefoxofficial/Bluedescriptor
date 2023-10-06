using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using MelonLoader;
using ABI_RC.Core.Player;
using TMPro;

public class UIListCreator : MonoBehaviour
{
    public GameObject QuickMenu;  // Assign the QuickMenu GameObject in the inspector
    public GameObject listItemPrefab;  // Assign a prefab for the list item (a simple Text or Button UI element)
    GameObject canvasGO;

    public List<CVRPlayerEntity> plList = new List<CVRPlayerEntity> { }; // Example list. Replace with your actual list.

    public void hide()
    {
       canvasGO.SetActive(false);
    }

    public void show()
    {
        canvasGO.SetActive(true);
    }

    public void GenerateUICanvas()
    {
        QuickMenu = GameObject.Find("QuickMenu");
        if (QuickMenu == null)
        {
            MelonLogger.Error("no");
            return;
        }

    
        GameObject existingObject = GameObject.Find("GeneratedUICanvas");
        if (existingObject == null)
        {
            canvasGO = new GameObject("GeneratedUICanvas");
        }
        else
        {
            canvasGO = existingObject;
        }
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();
        canvasGO.gameObject.transform.localScale = new Vector3(0.005f, 0.01f, 0f);
        canvasGO.gameObject.transform.localPosition = new Vector3(-0.62f, 0f, 0f);
        canvasGO.transform.SetParent(QuickMenu.transform, false);
        canvas.renderMode = RenderMode.WorldSpace;

        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(canvasGO.transform, false);
        Image bgImage = bg.AddComponent<Image>();
        bgImage.color = new Color(0.5f, 0f, 0.5f, 1f);
        RectTransform bgRect = bg.GetComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0, 0);
        bgRect.anchorMax = new Vector2(1, 1);
        bgRect.sizeDelta = Vector2.zero;

        GameObject listContainer = new GameObject("ListContainer");
        listContainer.transform.SetParent(canvasGO.transform, false);

        // Add a ScrollRect to make the list scrollable
        ScrollRect scrollRect = listContainer.AddComponent<ScrollRect>();
        scrollRect.content = listContainer.GetComponent<RectTransform>();
        listContainer.AddComponent<Mask>().showMaskGraphic = false;
        listContainer.AddComponent<Image>().color = Color.clear;  // transparent background

        GameObject content = new GameObject("Content");
        content.transform.SetParent(listContainer.transform, false);
        VerticalLayoutGroup layoutGroup = content.AddComponent<VerticalLayoutGroup>();
        layoutGroup.childForceExpandWidth = true;
        layoutGroup.childForceExpandHeight = false;
        layoutGroup.spacing = 5;

        scrollRect.content = content.GetComponent<RectTransform>();
        if (plList == null || plList.Count == 0)
        {
            GameObject aloneItem = new GameObject("AloneItem");
            aloneItem.transform.SetParent(content.transform, false);
            TextMeshProUGUI aloneTextMesh = aloneItem.AddComponent<TextMeshProUGUI>();
            aloneTextMesh.text = "All alone";
        }
        else
        {
            foreach (var item in plList)
            {
                GameObject listItem = Instantiate(listItemPrefab, content.transform);
                TextMeshProUGUI textMesh = listItem.AddComponent<TextMeshProUGUI>();
                textMesh.text = item.PlayerDescriptor.userName;
            }
        }

    }
    public void resetlistui()
    {
        //remove all items and readd them
    }
    private void Start()
    {
        GenerateUICanvas();  // You can call this elsewhere if you prefer
    }
}
