using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class DungeonSelectSceneSetup : EditorWindow
{
    [MenuItem("Para-Elementals/Setup Dungeon Select Scene")]
    public static void SetupDungeonSelect()
    {
        // Canvas
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("Canvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasGO.AddComponent<GraphicRaycaster>();
        }

        // EventSystem
        if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        }

        // Camera
        if (FindFirstObjectByType<Camera>() == null)
        {
            GameObject cam = new GameObject("Main Camera");
            cam.AddComponent<Camera>();
            cam.tag = "MainCamera";
        }

        // Fade panel
        GameObject fadePanel = CreateFadePanel(canvas);

        // Background
        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(canvas.transform, false);
        RectTransform bgRT = bg.AddComponent<RectTransform>();
        StretchFull(bgRT);
        Image bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color(0.05f, 0.05f, 0.15f, 1f);

        // Title
        CreateLabel(canvas.transform, "TitleText", "SELECT DUNGEON",
            new Vector2(0f, 420f), new Vector2(800f, 80f), 56,
            new Color(0.4f, 0.8f, 1f), FontStyles.Bold);

        // Left panel — dungeon list
        GameObject listPanel = new GameObject("DungeonListPanel");
        listPanel.transform.SetParent(canvas.transform, false);
        RectTransform listRT = listPanel.AddComponent<RectTransform>();
        listRT.anchorMin = new Vector2(0.05f, 0.1f);
        listRT.anchorMax = new Vector2(0.35f, 0.85f);
        listRT.offsetMin = listRT.offsetMax = Vector2.zero;
        Image listBg = listPanel.AddComponent<Image>();
        listBg.color = new Color(0.08f, 0.08f, 0.2f, 1f);

        // Scroll view for dungeon list
        GameObject scrollView = new GameObject("ScrollView");
        scrollView.transform.SetParent(listPanel.transform, false);
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
        viewport.AddComponent<Image>().color = new Color(0, 0, 0, 0);
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
        vlg.spacing = 8f;
        vlg.padding = new RectOffset(8, 8, 8, 8);
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
        ContentSizeFitter csf = content.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        scroll.content = contentRT;

        // Right panel — dungeon info
        GameObject infoPanel = new GameObject("DungeonInfoPanel");
        infoPanel.transform.SetParent(canvas.transform, false);
        RectTransform infoRT = infoPanel.AddComponent<RectTransform>();
        infoRT.anchorMin = new Vector2(0.38f, 0.1f);
        infoRT.anchorMax = new Vector2(0.95f, 0.85f);
        infoRT.offsetMin = infoRT.offsetMax = Vector2.zero;
        Image infoBg = infoPanel.AddComponent<Image>();
        infoBg.color = new Color(0.08f, 0.08f, 0.2f, 1f);

        // Preview image
        GameObject preview = new GameObject("PreviewImage");
        preview.transform.SetParent(infoPanel.transform, false);
        RectTransform previewRT = preview.AddComponent<RectTransform>();
        previewRT.anchorMin = new Vector2(0.1f, 0.45f);
        previewRT.anchorMax = new Vector2(0.9f, 0.95f);
        previewRT.offsetMin = previewRT.offsetMax = Vector2.zero;
        Image previewImg = preview.AddComponent<Image>();
        previewImg.color = new Color(0.15f, 0.15f, 0.3f, 1f);

        // Info texts
        TextMeshProUGUI nameText = CreateLabel(infoPanel.transform, "SelectedNameText",
            "Select a Dungeon", new Vector2(0f, 60f), new Vector2(500f, 50f),
            36, Color.white, FontStyles.Bold);

        TextMeshProUGUI difficultyText = CreateLabel(infoPanel.transform, "SelectedDifficultyText",
            "Difficulty: —", new Vector2(0f, 20f), new Vector2(500f, 40f),
            24, new Color(0.8f, 0.6f, 0.2f), FontStyles.Normal);

        TextMeshProUGUI descText = CreateLabel(infoPanel.transform, "SelectedDescriptionText",
            "Choose a dungeon from the list to see details.",
            new Vector2(0f, -40f), new Vector2(480f, 80f),
            20, new Color(0.7f, 0.7f, 0.7f), FontStyles.Normal);

        // Buttons
        GameObject enterBtn = CreateButton(canvas.transform, "EnterButton", "ENTER DUNGEON",
            new Vector2(200f, -440f), new Vector2(250f, 55f),
            new Color(0.2f, 0.6f, 0.2f, 1f));

        GameObject backBtn = CreateButton(canvas.transform, "BackButton", "BACK",
            new Vector2(-200f, -440f), new Vector2(180f, 55f),
            new Color(0.4f, 0.1f, 0.1f, 1f));

        // Create dungeon button prefab
        GameObject dungeonBtnPrefab = CreateDungeonButtonPrefab();

        // DungeonSelectManager
        GameObject mgrGO = new GameObject("DungeonSelectManager");
        mgrGO.transform.SetParent(canvas.transform, false);
        DungeonSelectManager mgr = mgrGO.AddComponent<DungeonSelectManager>();

        SetPrivateField(mgr, "_dungeonListContainer", contentRT);
        SetPrivateField(mgr, "_dungeonButtonPrefab", dungeonBtnPrefab);
        SetPrivateField(mgr, "_selectedNameText", nameText);
        SetPrivateField(mgr, "_selectedDescriptionText", descText);
        SetPrivateField(mgr, "_selectedDifficultyText", difficultyText);
        SetPrivateField(mgr, "_selectedPreviewImage", previewImg);
        SetPrivateField(mgr, "_enterButton", enterBtn.GetComponent<Button>());
        SetPrivateField(mgr, "_backButton", backBtn.GetComponent<Button>());
        SetPrivateField(mgr, "_fadePanel", fadePanel.GetComponent<CanvasGroup>());

        // Set up default dungeon entries
        DungeonEntry floor1 = new DungeonEntry
        {
            DungeonName = "Dungeon Floor 1",
            SceneName = "SideScroll_TestScene",
            Description = "The first dungeon floor.\nFight through waves of enemies\nand defeat the boss to escape.",
            Difficulty = "Normal",
            IsUnlocked = true,
        };

        DungeonEntry floor2 = new DungeonEntry
        {
            DungeonName = "Dungeon Floor 2",
            SceneName = "DungeonFloor2",
            Description = "Coming soon...",
            Difficulty = "Hard",
            IsUnlocked = false,
        };

        DungeonEntry[] dungeons = new DungeonEntry[] { floor1, floor2 };
        SetPrivateField(mgr, "_dungeons", dungeons);

        EditorUtility.DisplayDialog("Dungeon Select Setup Complete",
            "Dungeon Select scene setup complete!\n\n" +
            "Scene has:\n" +
            "- Dungeon list panel (left)\n" +
            "- Dungeon info panel (right)\n" +
            "- Enter and Back buttons\n\n" +
            "Dungeon Floor 1 is unlocked and ready.",
            "OK");

        Debug.Log("[Setup] Dungeon Select scene setup complete!");
    }

    private static GameObject CreateFadePanel(Canvas canvas)
    {
        GameObject fade = new GameObject("FadePanel");
        fade.transform.SetParent(canvas.transform, false);
        RectTransform rt = fade.AddComponent<RectTransform>();
        StretchFull(rt);
        Image img = fade.AddComponent<Image>();
        img.color = Color.black;
        CanvasGroup cg = fade.AddComponent<CanvasGroup>();
        cg.blocksRaycasts = false;
        cg.alpha = 1f;
        return fade;
    }

    private static GameObject CreateDungeonButtonPrefab()
    {
        GameObject btn = new GameObject("DungeonButton");

        RectTransform rt = btn.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(200f, 70f);

        Image bg = btn.AddComponent<Image>();
        bg.color = new Color(0.15f, 0.15f, 0.35f, 1f);

        Button button = btn.AddComponent<Button>();
        DungeonButton db = btn.AddComponent<DungeonButton>();
        SetPrivateField(db, "_background", bg);
        SetPrivateField(db, "_button", button);

        // Name text
        GameObject nameGO = new GameObject("NameText");
        nameGO.transform.SetParent(btn.transform, false);
        RectTransform nameRT = nameGO.AddComponent<RectTransform>();
        nameRT.anchorMin = new Vector2(0f, 0.5f);
        nameRT.anchorMax = new Vector2(1f, 1f);
        nameRT.offsetMin = new Vector2(10f, 0f);
        nameRT.offsetMax = new Vector2(-10f, 0f);
        TextMeshProUGUI nameTMP = nameGO.AddComponent<TextMeshProUGUI>();
        nameTMP.fontSize = 22;
        nameTMP.color = Color.white;
        nameTMP.fontStyle = FontStyles.Bold;
        SetPrivateField(db, "_nameText", nameTMP);

        // Difficulty text
        GameObject diffGO = new GameObject("DifficultyText");
        diffGO.transform.SetParent(btn.transform, false);
        RectTransform diffRT = diffGO.AddComponent<RectTransform>();
        diffRT.anchorMin = new Vector2(0f, 0f);
        diffRT.anchorMax = new Vector2(1f, 0.5f);
        diffRT.offsetMin = new Vector2(10f, 0f);
        diffRT.offsetMax = new Vector2(-10f, 0f);
        TextMeshProUGUI diffTMP = diffGO.AddComponent<TextMeshProUGUI>();
        diffTMP.fontSize = 18;
        diffTMP.color = new Color(0.7f, 0.7f, 0.7f);
        SetPrivateField(db, "_difficultyText", diffTMP);

        // Save as prefab
        string path = "Assets/Prefabs/UI/DungeonButton.prefab";
        System.IO.Directory.CreateDirectory("Assets/Prefabs/UI");
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(btn, path);
        DestroyImmediate(btn);
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
        Image img = go.AddComponent<Image>();
        img.color = color;
        Button btn = go.AddComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.highlightedColor = color * 1.3f;
        btn.colors = cb;

        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(go.transform, false);
        RectTransform textRT = textGO.AddComponent<RectTransform>();
        StretchFull(textRT);
        TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 24;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;
        return go;
    }

    private static void SetPrivateField(object obj, string fieldName, object value)
    {
        var field = obj.GetType().GetField(fieldName,
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(obj, value);
    }
}
