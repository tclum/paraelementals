using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class HomeBasePauseSetup : EditorWindow
{
    [MenuItem("Para-Elementals/Setup HomeBase Pause + Inventory")]
    public static void Setup()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            EditorUtility.DisplayDialog("No Canvas", "No Canvas found. Open HomeBase scene first.", "OK");
            return;
        }

        // Remove existing
        foreach (string name in new[] { "HomeBasePausePanel", "HomeBaseInventoryPanel", "HomeBasePauseManager" })
        {
            Transform t = canvas.transform.Find(name);
            if (t != null) DestroyImmediate(t.gameObject);
        }

        GameObject pausePanel = CreatePausePanel(canvas);
        GameObject inventoryPanel = CreateInventoryPanel(canvas);
        SetupManager(canvas, pausePanel, inventoryPanel);

        EditorUtility.DisplayDialog("HomeBase Pause Setup Complete",
            "HomeBase pause screen + inventory setup complete!\n\n" +
            "- Press Escape to open/close pause menu\n" +
            "- Inventory button shows items + shard count\n" +
            "- Resume, Main Menu, Quit buttons",
            "OK");
    }

    private static GameObject CreatePausePanel(Canvas canvas)
    {
        GameObject panel = new GameObject("HomeBasePausePanel");
        panel.transform.SetParent(canvas.transform, false);
        RectTransform rt = panel.AddComponent<RectTransform>();
        StretchFull(rt);
        panel.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.75f);
        panel.SetActive(false);

        // Center container
        GameObject container = new GameObject("Container");
        container.transform.SetParent(panel.transform, false);
        RectTransform containerRT = container.AddComponent<RectTransform>();
        containerRT.anchorMin = containerRT.anchorMax = new Vector2(0.5f, 0.5f);
        containerRT.sizeDelta = new Vector2(400f, 420f);
        containerRT.anchoredPosition = Vector2.zero;
        container.AddComponent<Image>().color = new Color(0.05f, 0.05f, 0.15f, 1f);

        VerticalLayoutGroup vlg = container.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 14f;
        vlg.padding = new RectOffset(40, 40, 40, 40);
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.childAlignment = TextAnchor.UpperCenter;

        CreateLabel(container.transform, "Title", "PAUSED",
            56, new Color(0.4f, 0.8f, 1f), FontStyles.Bold);

        CreateButton(container.transform, "ResumeButton", "RESUME",
            new Color(0.2f, 0.6f, 0.2f));
        CreateButton(container.transform, "InventoryButton", "INVENTORY",
            new Color(0.2f, 0.3f, 0.6f));
        CreateButton(container.transform, "MainMenuButton", "MAIN MENU",
            new Color(0.2f, 0.2f, 0.5f));
        CreateButton(container.transform, "QuitButton", "QUIT TO DESKTOP",
            new Color(0.5f, 0.1f, 0.1f));

        return panel;
    }

    private static GameObject CreateInventoryPanel(Canvas canvas)
    {
        GameObject panel = new GameObject("HomeBaseInventoryPanel");
        panel.transform.SetParent(canvas.transform, false);
        RectTransform rt = panel.AddComponent<RectTransform>();
        StretchFull(rt);
        panel.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.85f);
        panel.SetActive(false);

        // Main container
        GameObject container = new GameObject("Container");
        container.transform.SetParent(panel.transform, false);
        RectTransform containerRT = container.AddComponent<RectTransform>();
        containerRT.anchorMin = containerRT.anchorMax = new Vector2(0.5f, 0.5f);
        containerRT.sizeDelta = new Vector2(700f, 550f);
        containerRT.anchoredPosition = Vector2.zero;
        container.AddComponent<Image>().color = new Color(0.05f, 0.05f, 0.15f, 1f);

        // Title
        CreateLabelAnchored(container.transform, "Title", "INVENTORY",
            new Vector2(0f, 240f), new Vector2(600f, 60f),
            48, new Color(0.4f, 0.8f, 1f), FontStyles.Bold);

        // Shard count
        TextMeshProUGUI shardText = CreateLabelAnchored(container.transform, "ShardCount",
            "💎 0 Shards", new Vector2(0f, 185f), new Vector2(500f, 45f),
            32, new Color(0.9f, 0.8f, 0.2f), FontStyles.Bold);

        // Divider label
        CreateLabelAnchored(container.transform, "ItemsLabel", "— ITEMS —",
            new Vector2(0f, 140f), new Vector2(400f, 35f),
            22, new Color(0.6f, 0.6f, 0.8f), FontStyles.Normal);

        // Scroll view for inventory slots
        GameObject scrollView = new GameObject("ScrollView");
        scrollView.transform.SetParent(container.transform, false);
        RectTransform scrollRT = scrollView.AddComponent<RectTransform>();
        scrollRT.anchorMin = scrollRT.anchorMax = new Vector2(0.5f, 0.5f);
        scrollRT.anchoredPosition = new Vector2(0f, -30f);
        scrollRT.sizeDelta = new Vector2(620f, 270f);
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
        GridLayoutGroup grid = content.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(160f, 60f);
        grid.spacing = new Vector2(10f, 10f);
        grid.padding = new RectOffset(10, 10, 10, 10);
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 3;
        ContentSizeFitter csf = content.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        scroll.content = contentRT;

        // Close button
        CreateButtonAnchored(container.transform, "CloseButton", "CLOSE",
            new Vector2(0f, -245f), new Vector2(200f, 50f),
            new Color(0.5f, 0.1f, 0.1f));

        return (panel);
    }

    private static void SetupManager(Canvas canvas, GameObject pausePanel, GameObject inventoryPanel)
    {
        GameObject mgrGO = new GameObject("HomeBasePauseManager");
        mgrGO.transform.SetParent(canvas.transform, false);
        HomeBasePauseManager mgr = mgrGO.AddComponent<HomeBasePauseManager>();

        SetField(mgr, "_pausePanel", pausePanel);
        SetField(mgr, "_inventoryPanel", inventoryPanel);

        Transform container = pausePanel.transform.Find("Container");
        if (container != null)
        {
            SetField(mgr, "_resumeButton", container.Find("ResumeButton")?.GetComponent<Button>());
            SetField(mgr, "_inventoryButton", container.Find("InventoryButton")?.GetComponent<Button>());
            SetField(mgr, "_mainMenuButton", container.Find("MainMenuButton")?.GetComponent<Button>());
            SetField(mgr, "_quitButton", container.Find("QuitButton")?.GetComponent<Button>());
        }

        // Find inventory panel elements
        Transform invContainer = inventoryPanel.transform.Find("Container");
        if (invContainer != null)
        {
            SetField(mgr, "_shardCountText", invContainer.Find("ShardCount")?.GetComponent<TextMeshProUGUI>());
            SetField(mgr, "_inventoryCloseButton", invContainer.Find("CloseButton")?.GetComponent<Button>());

            Transform scrollView = invContainer.Find("ScrollView");
            if (scrollView != null)
            {
                Transform viewport = scrollView.Find("Viewport");
                if (viewport != null)
                    SetField(mgr, "_inventorySlotContainer", viewport.Find("Content"));
            }
        }
    }

    // ── Helpers ────────────────────────────────────────────────────────────

    private static void StretchFull(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
    }

    private static void CreateLabel(Transform parent, string name, string text,
        float fontSize, Color color, FontStyles style)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(320f, 70f);
        LayoutElement le = go.AddComponent<LayoutElement>();
        le.preferredHeight = 70f;
        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = style;
    }

    private static TextMeshProUGUI CreateLabelAnchored(Transform parent, string name, string text,
        Vector2 position, Vector2 size, float fontSize, Color color, FontStyles style)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = position;
        rt.sizeDelta = size;
        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = style;
        return tmp;
    }

    private static GameObject CreateButton(Transform parent, string name,
        string label, Color color)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(320f, 55f);
        LayoutElement le = go.AddComponent<LayoutElement>();
        le.preferredHeight = 55f;
        go.AddComponent<Image>().color = color;
        go.AddComponent<Button>();

        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(go.transform, false);
        RectTransform textRT = textGO.AddComponent<RectTransform>();
        StretchFull(textRT);
        TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 26f;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;
        return go;
    }

    private static GameObject CreateButtonAnchored(Transform parent, string name,
        string label, Vector2 position, Vector2 size, Color color)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = position;
        rt.sizeDelta = size;
        go.AddComponent<Image>().color = color;
        go.AddComponent<Button>();

        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(go.transform, false);
        RectTransform textRT = textGO.AddComponent<RectTransform>();
        StretchFull(textRT);
        TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 24f;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;
        return go;
    }

    private static void SetField(object obj, string name, object value)
    {
        var field = obj.GetType().GetField(name,
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(obj, value);
    }
}
