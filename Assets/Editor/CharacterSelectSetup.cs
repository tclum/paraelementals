using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class CharacterSelectSetup : EditorWindow
{
    [MenuItem("Para-Elementals/Setup Character Select UI")]
    public static void SetupCharacterSelect()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            EditorUtility.DisplayDialog("No Canvas", "No Canvas found. Open HomeBase scene first.", "OK");
            return;
        }

        // Remove existing
        Transform existing = canvas.transform.Find("CharacterSelectPanel");
        if (existing != null)
            DestroyImmediate(existing.gameObject);

        // Main panel — full screen overlay
        GameObject panel = new GameObject("CharacterSelectPanel");
        panel.transform.SetParent(canvas.transform, false);
        RectTransform panelRT = panel.AddComponent<RectTransform>();
        StretchFull(panelRT);
        Image panelBg = panel.AddComponent<Image>();
        panelBg.color = new Color(0f, 0f, 0f, 0.85f);
        panel.SetActive(false);

        // Title
        CreateLabel(panel.transform, "Title", "CHOOSE YOUR CHARACTER",
            new Vector2(0f, 380f), new Vector2(900f, 70f), 52,
            new Color(0.4f, 0.8f, 1f), FontStyles.Bold);

        // Left — character list
        GameObject listBg = new GameObject("ListBackground");
        listBg.transform.SetParent(panel.transform, false);
        RectTransform listBgRT = listBg.AddComponent<RectTransform>();
        listBgRT.anchorMin = new Vector2(0.05f, 0.1f);
        listBgRT.anchorMax = new Vector2(0.38f, 0.82f);
        listBgRT.offsetMin = listBgRT.offsetMax = Vector2.zero;
        listBg.AddComponent<Image>().color = new Color(0.08f, 0.08f, 0.2f, 1f);

        // Scroll view
        GameObject scrollView = new GameObject("ScrollView");
        scrollView.transform.SetParent(listBg.transform, false);
        RectTransform scrollRT = scrollView.AddComponent<RectTransform>();
        StretchFull(scrollRT);
        scrollRT.offsetMin = new Vector2(10f, 10f);
        scrollRT.offsetMax = new Vector2(-10f, -10f);
        ScrollRect scroll = scrollView.AddComponent<ScrollRect>();
        scroll.horizontal = false;

        GameObject viewport = new GameObject("Viewport");
        viewport.transform.SetParent(scrollView.transform, false);
        RectTransform viewportRT = viewport.AddComponent<RectTransform>();
        StretchFull(viewportRT);
        viewport.AddComponent<Image>().color = Color.clear;
        Mask mask = viewport.AddComponent<Mask>();
        mask.showMaskGraphic = false;
        scroll.viewport = viewportRT;

        GameObject content = new GameObject("Content");
        content.transform.SetParent(viewport.transform, false);
        RectTransform contentRT = content.AddComponent<RectTransform>();
        contentRT.anchorMin = new Vector2(0f, 1f);
        contentRT.anchorMax = new Vector2(1f, 1f);
        contentRT.pivot = new Vector2(0.5f, 1f);
        contentRT.offsetMin = contentRT.offsetMax = Vector2.zero;
        VerticalLayoutGroup vlg = content.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 10f;
        vlg.padding = new RectOffset(8, 8, 8, 8);
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
        ContentSizeFitter csf = content.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        scroll.content = contentRT;

        // Right — info panel
        GameObject infoBg = new GameObject("InfoBackground");
        infoBg.transform.SetParent(panel.transform, false);
        RectTransform infoBgRT = infoBg.AddComponent<RectTransform>();
        infoBgRT.anchorMin = new Vector2(0.42f, 0.1f);
        infoBgRT.anchorMax = new Vector2(0.95f, 0.82f);
        infoBgRT.offsetMin = infoBgRT.offsetMax = Vector2.zero;
        infoBg.AddComponent<Image>().color = new Color(0.08f, 0.08f, 0.2f, 1f);

        // Preview image
        GameObject preview = new GameObject("PreviewImage");
        preview.transform.SetParent(infoBg.transform, false);
        RectTransform previewRT = preview.AddComponent<RectTransform>();
        previewRT.anchorMin = new Vector2(0.25f, 0.5f);
        previewRT.anchorMax = new Vector2(0.75f, 0.95f);
        previewRT.offsetMin = previewRT.offsetMax = Vector2.zero;
        Image previewImg = preview.AddComponent<Image>();
        previewImg.color = new Color(0.15f, 0.15f, 0.3f, 1f);
        previewImg.preserveAspect = true;

        // Name
        TextMeshProUGUI nameText = CreateLabel(infoBg.transform, "NameText",
            "Spirit Blob", new Vector2(0f, 60f), new Vector2(500f, 50f),
            38, Color.white, FontStyles.Bold);
        nameText.GetComponent<RectTransform>().anchorMin =
        nameText.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);

        // Stats
        TextMeshProUGUI statsText = CreateLabel(infoBg.transform, "StatsText",
            "Stamina: 100", new Vector2(0f, -10f), new Vector2(400f, 100f),
            22, new Color(0.8f, 0.8f, 0.8f), FontStyles.Normal);
        statsText.GetComponent<RectTransform>().anchorMin =
        statsText.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);

        // Unlock hint
        TextMeshProUGUI hintText = CreateLabel(infoBg.transform, "UnlockHintText",
            "🔒 Complete the first dungeon to unlock",
            new Vector2(0f, -100f), new Vector2(450f, 60f),
            20, new Color(0.9f, 0.6f, 0.1f), FontStyles.Italic);
        hintText.GetComponent<RectTransform>().anchorMin =
        hintText.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
        hintText.gameObject.SetActive(false);

        // Buttons
        GameObject confirmBtn = CreateButton(panel.transform, "ConfirmButton", "SELECT CHARACTER",
            new Vector2(150f, -410f), new Vector2(260f, 55f), new Color(0.2f, 0.6f, 0.2f));

        GameObject closeBtn = CreateButton(panel.transform, "CloseButton", "CLOSE",
            new Vector2(-150f, -410f), new Vector2(160f, 55f), new Color(0.5f, 0.1f, 0.1f));

        // Create card prefab
        GameObject cardPrefab = CreateCharacterCardPrefab();

        // Add CharacterSelectManager to canvas
        CharacterSelectManager mgr = canvas.GetComponentInChildren<CharacterSelectManager>();
        if (mgr == null)
        {
            GameObject mgrGO = new GameObject("CharacterSelectManager");
            mgrGO.transform.SetParent(canvas.transform, false);
            mgr = mgrGO.AddComponent<CharacterSelectManager>();
        }

        SetField(mgr, "_panel", panel);
        SetField(mgr, "_characterListContainer", contentRT);
        SetField(mgr, "_characterCardPrefab", cardPrefab);
        SetField(mgr, "_previewImage", previewImg);
        SetField(mgr, "_nameText", nameText);
        SetField(mgr, "_statsText", statsText);
        SetField(mgr, "_unlockHintText", hintText);
        SetField(mgr, "_confirmButton", confirmBtn.GetComponent<Button>());
        SetField(mgr, "_closeButton", closeBtn.GetComponent<Button>());

        // Add CharacterSelect interactable to scene
        if (GameObject.Find("CharacterStation") == null)
        {
            GameObject station = new GameObject("CharacterStation");
            station.transform.position = new Vector3(-6f, 0f, 0f);

            GameObject visual = new GameObject("Visual");
            visual.transform.SetParent(station.transform, false);
            SpriteRenderer sr = visual.AddComponent<SpriteRenderer>();
            sr.color = new Color(0.4f, 0.8f, 1f, 1f);
            sr.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            visual.transform.localScale = new Vector3(1.5f, 2f, 1f);

            CircleCollider2D col = station.AddComponent<CircleCollider2D>();
            col.radius = 0.5f;
            col.isTrigger = true;

            HomeBaseInteractable interactable = station.AddComponent<HomeBaseInteractable>();
            SetField(interactable, "_type", 5); // CharacterSelect = 5
            SetField(interactable, "_promptText", "Press F to choose character");
            SetField(interactable, "_interactRadius", 2.5f);
        }

        EditorUtility.DisplayDialog("Character Select Setup Complete",
            "Character Select UI created!\n\n" +
            "Still needed:\n" +
            "- Create CharacterRoster ScriptableObject\n" +
            "- Add characters to the roster\n" +
            "- Assign roster to CharacterSelectManager",
            "OK");

        Debug.Log("[Setup] Character Select UI setup complete!");
    }

    private static GameObject CreateCharacterCardPrefab()
    {
        GameObject card = new GameObject("CharacterCard");
        RectTransform rt = card.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(200f, 100f);

        Image bg = card.AddComponent<Image>();
        bg.color = new Color(0.15f, 0.15f, 0.35f);

        Button btn = card.AddComponent<Button>();
        CharacterCardUI cardUI = card.AddComponent<CharacterCardUI>();
        SetField(cardUI, "_background", bg);
        SetField(cardUI, "_button", btn);

        // Character image
        GameObject imgGO = new GameObject("CharacterImage");
        imgGO.transform.SetParent(card.transform, false);
        RectTransform imgRT = imgGO.AddComponent<RectTransform>();
        imgRT.anchorMin = new Vector2(0f, 0f);
        imgRT.anchorMax = new Vector2(0.4f, 1f);
        imgRT.offsetMin = new Vector2(5f, 5f);
        imgRT.offsetMax = new Vector2(-5f, -5f);
        Image charImg = imgGO.AddComponent<Image>();
        charImg.color = new Color(0.3f, 0.3f, 0.5f);
        charImg.preserveAspect = true;
        SetField(cardUI, "_characterImage", charImg);

        // Name text
        GameObject nameGO = new GameObject("NameText");
        nameGO.transform.SetParent(card.transform, false);
        RectTransform nameRT = nameGO.AddComponent<RectTransform>();
        nameRT.anchorMin = new Vector2(0.4f, 0.5f);
        nameRT.anchorMax = new Vector2(1f, 1f);
        nameRT.offsetMin = new Vector2(5f, 0f);
        nameRT.offsetMax = new Vector2(-5f, 0f);
        TextMeshProUGUI nameTMP = nameGO.AddComponent<TextMeshProUGUI>();
        nameTMP.fontSize = 20f;
        nameTMP.color = Color.white;
        nameTMP.fontStyle = FontStyles.Bold;
        nameTMP.alignment = TextAlignmentOptions.MidlineLeft;
        SetField(cardUI, "_nameText", nameTMP);

        // Lock icon
        GameObject lockGO = new GameObject("LockIcon");
        lockGO.transform.SetParent(card.transform, false);
        RectTransform lockRT = lockGO.AddComponent<RectTransform>();
        lockRT.anchorMin = new Vector2(0.7f, 0.1f);
        lockRT.anchorMax = new Vector2(1f, 0.5f);
        lockRT.offsetMin = lockRT.offsetMax = Vector2.zero;
        TextMeshProUGUI lockTMP = lockGO.AddComponent<TextMeshProUGUI>();
        lockTMP.text = "🔒";
        lockTMP.fontSize = 24f;
        lockTMP.alignment = TextAlignmentOptions.Center;
        lockGO.SetActive(false);
        SetField(cardUI, "_lockIcon", lockGO);

        string path = "Assets/Prefabs/UI/CharacterCard.prefab";
        System.IO.Directory.CreateDirectory("Assets/Prefabs/UI");
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(card, path);
        DestroyImmediate(card);
        return prefab;
    }

    private static void StretchFull(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
    }

    private static TextMeshProUGUI CreateLabel(Transform parent, string name, string text,
        Vector2 position, Vector2 size, float fontSize, Color color, FontStyles style)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = position;
        rt.sizeDelta = size;
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = style;
        return tmp;
    }

    private static GameObject CreateButton(Transform parent, string name, string label,
        Vector2 position, Vector2 size, Color color)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = position;
        rt.sizeDelta = size;
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        go.AddComponent<Image>().color = color;
        go.AddComponent<Button>();

        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(go.transform, false);
        RectTransform textRT = textGO.AddComponent<RectTransform>();
        StretchFull(textRT);
        TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 22f;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;
        return go;
    }

    private static void SetField(object obj, string name, object value)
    {
        var field = obj.GetType().GetField(name,
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null)
        {
            if (field.FieldType.IsEnum && value is int intVal)
                field.SetValue(obj, System.Enum.ToObject(field.FieldType, intVal));
            else
                field.SetValue(obj, value);
        }
    }
}
