using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class InGameUISetup : EditorWindow
{
    [MenuItem("Para-Elementals/Setup In-Game UI (Pause + Shards)")]
    public static void SetupInGameUI()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            EditorUtility.DisplayDialog("No Canvas", "No Canvas found. Open SideScroll_TestScene first.", "OK");
            return;
        }

        SetupPauseScreen(canvas);
        SetupShardUI(canvas);
        SetupPauseManager(canvas);
        SetupCurrencyManager();

        EditorUtility.DisplayDialog("In-Game UI Setup Complete",
            "Setup complete!\n\n" +
            "- Pause screen added (Escape to toggle)\n" +
            "- Shard counter added below HUD bars\n" +
            "- PauseManager added to Canvas\n" +
            "- CurrencyManager added to scene\n\n" +
            "Still needed:\n" +
            "- Create Shard prefab in Assets/Prefabs\n" +
            "- Assign shard prefab to enemy LootDropper components",
            "OK");
    }

    private static void SetupPauseScreen(Canvas canvas)
    {
        Transform existing = canvas.transform.Find("PausePanel");
        if (existing != null)
            DestroyImmediate(existing.gameObject);

        // Full screen overlay
        GameObject panel = new GameObject("PausePanel");
        panel.transform.SetParent(canvas.transform, false);
        RectTransform panelRT = panel.AddComponent<RectTransform>();
        StretchFull(panelRT);
        Image panelBg = panel.AddComponent<Image>();
        panelBg.color = new Color(0f, 0f, 0f, 0.75f);
        panel.SetActive(false);

        // Center container
        GameObject container = new GameObject("Container");
        container.transform.SetParent(panel.transform, false);
        RectTransform containerRT = container.AddComponent<RectTransform>();
        containerRT.anchorMin = containerRT.anchorMax = new Vector2(0.5f, 0.5f);
        containerRT.sizeDelta = new Vector2(400f, 350f);
        containerRT.anchoredPosition = Vector2.zero;
        Image containerBg = container.AddComponent<Image>();
        containerBg.color = new Color(0.05f, 0.05f, 0.15f, 1f);

        VerticalLayoutGroup vlg = container.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 16f;
        vlg.padding = new RectOffset(40, 40, 40, 40);
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.childAlignment = TextAnchor.UpperCenter;

        // Title
        CreateLabel(container.transform, "Title", "PAUSED",
            56, new Color(0.4f, 0.8f, 1f), FontStyles.Bold);

        // Buttons
        CreatePauseButton(container.transform, "ResumeButton", "RESUME",
            new Color(0.2f, 0.6f, 0.2f));
        CreatePauseButton(container.transform, "MainMenuButton", "MAIN MENU",
            new Color(0.2f, 0.2f, 0.5f));
        CreatePauseButton(container.transform, "QuitButton", "QUIT TO DESKTOP",
            new Color(0.5f, 0.1f, 0.1f));
    }

    private static void SetupShardUI(Canvas canvas)
    {
        // Find HUDRoot
        Transform hudRoot = canvas.transform.Find("HUDRoot");
        if (hudRoot == null)
        {
            Debug.LogWarning("[Setup] HUDRoot not found — run Setup In-Game HUD first.");
            return;
        }

        // Remove existing shard display
        Transform existing = hudRoot.Find("ShardDisplay");
        if (existing != null)
            DestroyImmediate(existing.gameObject);

        // Shard display row
        GameObject shardDisplay = new GameObject("ShardDisplay");
        shardDisplay.transform.SetParent(hudRoot, false);
        RectTransform shardRT = shardDisplay.AddComponent<RectTransform>();
        shardRT.sizeDelta = new Vector2(280f, 36f);

        LayoutElement le = shardDisplay.AddComponent<LayoutElement>();
        le.preferredHeight = 36f;
        le.flexibleWidth = 1f;

        Image bg = shardDisplay.AddComponent<Image>();
        bg.color = new Color(0.1f, 0.1f, 0.1f, 0.85f);

        // Shard icon label
        GameObject iconGO = new GameObject("Icon");
        iconGO.transform.SetParent(shardDisplay.transform, false);
        RectTransform iconRT = iconGO.AddComponent<RectTransform>();
        iconRT.anchorMin = new Vector2(0f, 0f);
        iconRT.anchorMax = new Vector2(0f, 1f);
        iconRT.pivot = new Vector2(0f, 0.5f);
        iconRT.anchoredPosition = new Vector2(6f, 0f);
        iconRT.sizeDelta = new Vector2(28f, 0f);
        TextMeshProUGUI iconTMP = iconGO.AddComponent<TextMeshProUGUI>();
        iconTMP.text = "💎";
        iconTMP.fontSize = 18f;
        iconTMP.alignment = TextAlignmentOptions.Center;

        // Shard count text
        GameObject countGO = new GameObject("ShardCount");
        countGO.transform.SetParent(shardDisplay.transform, false);
        RectTransform countRT = countGO.AddComponent<RectTransform>();
        countRT.anchorMin = new Vector2(0f, 0f);
        countRT.anchorMax = new Vector2(1f, 1f);
        countRT.offsetMin = new Vector2(38f, 0f);
        countRT.offsetMax = new Vector2(-6f, 0f);
        TextMeshProUGUI countTMP = countGO.AddComponent<TextMeshProUGUI>();
        countTMP.text = "0 Shards";
        countTMP.fontSize = 18f;
        countTMP.color = new Color(0.9f, 0.8f, 0.2f);
        countTMP.fontStyle = FontStyles.Bold;
        countTMP.alignment = TextAlignmentOptions.MidlineLeft;

        // Add ShardUI component to HUDRoot
        ShardUI shardUI = canvas.GetComponentInChildren<ShardUI>();
        if (shardUI == null)
        {
            GameObject shardUIMgrGO = new GameObject("ShardUI");
            shardUIMgrGO.transform.SetParent(canvas.transform, false);
            shardUI = shardUIMgrGO.AddComponent<ShardUI>();
        }
        SetField(shardUI, "_shardText", countTMP);
    }

    private static void SetupPauseManager(Canvas canvas)
    {
        PauseManager existing = canvas.GetComponentInChildren<PauseManager>();
        if (existing != null) return;

        GameObject mgrGO = new GameObject("PauseManager");
        mgrGO.transform.SetParent(canvas.transform, false);
        PauseManager pm = mgrGO.AddComponent<PauseManager>();

        Transform pausePanel = canvas.transform.Find("PausePanel");
        if (pausePanel != null)
        {
            SetField(pm, "_pausePanel", pausePanel.gameObject);

            Transform container = pausePanel.Find("Container");
            if (container != null)
            {
                SetField(pm, "_resumeButton", container.Find("ResumeButton")?.GetComponent<Button>());
                SetField(pm, "_mainMenuButton", container.Find("MainMenuButton")?.GetComponent<Button>());
                SetField(pm, "_quitButton", container.Find("QuitButton")?.GetComponent<Button>());
            }
        }
    }

    private static void SetupCurrencyManager()
    {
        if (FindFirstObjectByType<CurrencyManager>() != null) return;

        GameObject go = new GameObject("CurrencyManager");
        go.AddComponent<CurrencyManager>();
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

    private static GameObject CreatePauseButton(Transform parent, string name,
        string label, Color color)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(320f, 55f);
        LayoutElement le = go.AddComponent<LayoutElement>();
        le.preferredHeight = 55f;
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
        tmp.fontSize = 26f;
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
