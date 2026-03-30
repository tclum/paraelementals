using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class ParaElementalsSceneSetup : EditorWindow
{
    [MenuItem("Para-Elementals/Setup Level Scene")]
    public static void SetupScene()
    {
        // Find or create main Canvas
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("Canvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }

        SetupDeathScreen(canvas);
        SetupResultsScreen(canvas);
        SetupLevelManager();
        SetupSpawnPoints();

        Debug.Log("[Setup] Para-Elementals scene setup complete!");
        EditorUtility.DisplayDialog("Setup Complete",
            "Scene setup complete!\n\n" +
            "Still needed:\n" +
            "- Assign enemy prefab to EnemySpawnPoint components\n" +
            "- Assign boss prefab to LevelManager\n" +
            "- Place spawn points where you want enemies",
            "OK");
    }

    private static void SetupDeathScreen(Canvas canvas)
    {
        // Remove existing
        Transform existing = canvas.transform.Find("DeathScreenRoot");
        if (existing != null)
            DestroyImmediate(existing.gameObject);

        // Root
        GameObject root = new GameObject("DeathScreenRoot");
        root.transform.SetParent(canvas.transform, false);
        RectTransform rootRect = root.AddComponent<RectTransform>();
        StretchFull(rootRect);
        CanvasGroup cg = root.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;

        // Background panel
        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(root.transform, false);
        RectTransform bgRect = bg.AddComponent<RectTransform>();
        StretchFull(bgRect);
        Image bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color(0f, 0f, 0f, 0.85f);

        // YOU DIED text
        GameObject titleGO = new GameObject("TitleText");
        titleGO.transform.SetParent(root.transform, false);
        RectTransform titleRect = titleGO.AddComponent<RectTransform>();
        titleRect.anchoredPosition = new Vector2(0, 80);
        titleRect.sizeDelta = new Vector2(600, 120);
        titleRect.anchorMin = titleRect.anchorMax = new Vector2(0.5f, 0.5f);
        TextMeshProUGUI titleTMP = titleGO.AddComponent<TextMeshProUGUI>();
        titleTMP.text = "YOU DIED";
        titleTMP.fontSize = 72;
        titleTMP.color = new Color(0.8f, 0.1f, 0.1f, 1f);
        titleTMP.alignment = TextAlignmentOptions.Center;
        titleTMP.fontStyle = FontStyles.Bold;

        // Restart button
        GameObject btnGO = CreateButton(root.transform, "RestartButton", "Try Again", new Vector2(0, -40));

        // Add DeathScreen script to root
        DeathScreen ds = root.AddComponent<DeathScreen>();
        SetPrivateField(ds, "_canvasGroup", cg);
        SetPrivateField(ds, "_titleText", titleTMP);
        SetPrivateField(ds, "_restartButton", btnGO.GetComponent<Button>());

        Debug.Log("[Setup] Death screen created.");
    }

    private static void SetupResultsScreen(Canvas canvas)
    {
        Transform existing = canvas.transform.Find("ResultsScreenRoot");
        if (existing != null)
            DestroyImmediate(existing.gameObject);

        GameObject root = new GameObject("ResultsScreenRoot");
        root.transform.SetParent(canvas.transform, false);
        RectTransform rootRect = root.AddComponent<RectTransform>();
        StretchFull(rootRect);
        CanvasGroup cg = root.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;

        // Background
        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(root.transform, false);
        RectTransform bgRect = bg.AddComponent<RectTransform>();
        StretchFull(bgRect);
        Image bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color(0f, 0f, 0f, 0.85f);

        // Title
        TextMeshProUGUI titleTMP = CreateLabel(root.transform, "TitleText",
            "LEVEL COMPLETE", new Vector2(0, 120), 60, new Color(0.2f, 0.9f, 0.2f));

        // Stats
        TextMeshProUGUI enemiesTMP = CreateLabel(root.transform, "EnemiesKilledText",
            "Enemies Defeated: 0", new Vector2(0, 30), 32, Color.white);

        TextMeshProUGUI timeTMP = CreateLabel(root.transform, "TimeText",
            "Time: 00:00", new Vector2(0, -20), 32, Color.white);

        // Buttons
        GameObject continueBtn = CreateButton(root.transform, "ContinueButton", "Continue", new Vector2(-100, -100));
        GameObject menuBtn = CreateButton(root.transform, "MainMenuButton", "Main Menu", new Vector2(100, -100));

        // Add ResultsScreen script
        ResultsScreen rs = root.AddComponent<ResultsScreen>();
        SetPrivateField(rs, "_canvasGroup", cg);
        SetPrivateField(rs, "_titleText", titleTMP);
        SetPrivateField(rs, "_enemiesKilledText", enemiesTMP);
        SetPrivateField(rs, "_timeText", timeTMP);
        SetPrivateField(rs, "_continueButton", continueBtn.GetComponent<Button>());
        SetPrivateField(rs, "_mainMenuButton", menuBtn.GetComponent<Button>());

        Debug.Log("[Setup] Results screen created.");
    }

    private static void SetupLevelManager()
    {
        LevelManager existing = FindFirstObjectByType<LevelManager>();
        if (existing != null)
        {
            Debug.Log("[Setup] LevelManager already exists, skipping.");
            return;
        }

        GameObject lm = new GameObject("LevelManager");
        lm.AddComponent<LevelManager>();
        Debug.Log("[Setup] LevelManager created. Assign boss reference in Inspector.");
    }

    private static void SetupSpawnPoints()
    {
        // Create 3 default spawn points spread across the level
        Vector3[] positions = new Vector3[]
        {
            new Vector3(-8f, -2f, 0f),
            new Vector3(0f, -2f, 0f),
            new Vector3(8f, -2f, 0f),
        };

        for (int i = 0; i < positions.Length; i++)
        {
            // Check if one already exists at this position
            string name = $"SpawnPoint_{i + 1}";
            GameObject existing = GameObject.Find(name);
            if (existing != null)
                continue;

            GameObject sp = new GameObject(name);
            sp.transform.position = positions[i];
            EnemySpawnPoint spScript = sp.AddComponent<EnemySpawnPoint>();
            Debug.Log($"[Setup] Created {name} at {positions[i]}. Assign enemy prefab in Inspector.");
        }
    }

    // ── Helpers ────────────────────────────────────────────────────────────

    private static void StretchFull(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    private static TextMeshProUGUI CreateLabel(Transform parent, string name, string text,
        Vector2 position, float fontSize, Color color)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = position;
        rt.sizeDelta = new Vector2(600, 60);
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        return tmp;
    }

    private static GameObject CreateButton(Transform parent, string name, string label, Vector2 position)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = position;
        rt.sizeDelta = new Vector2(200, 50);
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);

        Image img = go.AddComponent<Image>();
        img.color = new Color(0.2f, 0.2f, 0.2f, 1f);

        Button btn = go.AddComponent<Button>();

        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(go.transform, false);
        RectTransform textRT = textGO.AddComponent<RectTransform>();
        StretchFull(textRT);
        TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 24;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;

        return go;
    }

    private static void SetPrivateField(object obj, string fieldName, object value)
    {
        var field = obj.GetType().GetField(fieldName,
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(obj, value);
    }
}
