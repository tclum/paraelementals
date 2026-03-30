using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class MainMenuSceneSetup : EditorWindow
{
    [MenuItem("Para-Elementals/Setup Main Menu Scene")]
    public static void SetupMainMenu()
    {
        // Warn if not in the right scene
        var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if (activeScene.name != "MainMenu")
        {
            bool proceed = EditorUtility.DisplayDialog("Wrong Scene?",
                $"Current scene is '{activeScene.name}'. This will set up the Main Menu UI here. Continue?",
                "Yes", "Cancel");
            if (!proceed) return;
        }

        // Create or find Canvas
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

        SetupFadePanel(canvas);
        SetupMainPanel(canvas);
        SetupCreditsPanel(canvas);
        SetupMenuManager(canvas);

        // Create a camera if none exists
        if (FindFirstObjectByType<Camera>() == null)
        {
            GameObject cam = new GameObject("Main Camera");
            cam.AddComponent<Camera>();
            cam.tag = "MainCamera";
        }

        EditorUtility.DisplayDialog("Main Menu Setup Complete",
            "Main Menu scene setup complete!\n\n" +
            "Still needed:\n" +
            "- Set 'Game Scene Name' in MainMenuManager to your level scene name\n" +
            "- Add this scene to Build Settings (File → Build Settings → Add Open Scenes)\n" +
            "- Customize title text and colors",
            "OK");

        Debug.Log("[Setup] Main Menu scene setup complete!");
    }

    private static void SetupFadePanel(Canvas canvas)
    {
        // Full screen black fade panel
        GameObject fade = new GameObject("FadePanel");
        fade.transform.SetParent(canvas.transform, false);
        RectTransform rt = fade.AddComponent<RectTransform>();
        StretchFull(rt);
        Image img = fade.AddComponent<Image>();
        img.color = Color.black;
        CanvasGroup cg = fade.AddComponent<CanvasGroup>();
        cg.blocksRaycasts = false;
        cg.alpha = 1f;
    }

    private static void SetupMainPanel(Canvas canvas)
    {
        // Background
        GameObject bg = new GameObject("MainPanel");
        bg.transform.SetParent(canvas.transform, false);
        RectTransform bgRT = bg.AddComponent<RectTransform>();
        StretchFull(bgRT);
        Image bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color(0.05f, 0.05f, 0.15f, 1f);

        // Title
        CreateLabel(bg.transform, "TitleText", "PARA-ELEMENTALS",
            new Vector2(0, 200), 80, new Color(0.4f, 0.8f, 1f), FontStyles.Bold);

        CreateLabel(bg.transform, "SubtitleText", "A Roguelike Dungeon Crawler",
            new Vector2(0, 130), 28, new Color(0.6f, 0.6f, 0.8f), FontStyles.Normal);

        // Buttons
        CreateMenuButton(bg.transform, "PlayButton", "PLAY", new Vector2(0, 0));
        CreateMenuButton(bg.transform, "CreditsButton", "CREDITS", new Vector2(0, -70));
        CreateMenuButton(bg.transform, "QuitButton", "QUIT", new Vector2(0, -140));
    }

    private static void SetupCreditsPanel(Canvas canvas)
    {
        GameObject credits = new GameObject("CreditsPanel");
        credits.transform.SetParent(canvas.transform, false);
        RectTransform rt = credits.AddComponent<RectTransform>();
        StretchFull(rt);
        Image bg = credits.AddComponent<Image>();
        bg.color = new Color(0.05f, 0.05f, 0.15f, 1f);
        credits.SetActive(false);

        CreateLabel(credits.transform, "TitleText", "CREDITS",
            new Vector2(0, 200), 60, new Color(0.4f, 0.8f, 1f), FontStyles.Bold);

        CreateLabel(credits.transform, "CreditsText",
            "Game Design & Development\nYour Name Here\n\nBuilt with Unity 6",
            new Vector2(0, 50), 28, Color.white, FontStyles.Normal);

        CreateMenuButton(credits.transform, "BackButton", "BACK", new Vector2(0, -150));
    }

    private static void SetupMenuManager(Canvas canvas)
    {
        // Find all the pieces
        Transform fade = canvas.transform.Find("FadePanel");
        Transform mainPanel = canvas.transform.Find("MainPanel");
        Transform creditsPanel = canvas.transform.Find("CreditsPanel");

        GameObject mgr = new GameObject("MainMenuManager");
        mgr.transform.SetParent(canvas.transform, false);
        MainMenuManager mm = mgr.AddComponent<MainMenuManager>();

        SetPrivateField(mm, "_mainPanel", mainPanel?.gameObject);
        SetPrivateField(mm, "_creditsPanel", creditsPanel?.gameObject);
        SetPrivateField(mm, "_fadePanel", fade?.GetComponent<CanvasGroup>());

        if (mainPanel != null)
        {
            SetPrivateField(mm, "_playButton", mainPanel.Find("PlayButton")?.GetComponent<Button>());
            SetPrivateField(mm, "_creditsButton", mainPanel.Find("CreditsButton")?.GetComponent<Button>());
            SetPrivateField(mm, "_quitButton", mainPanel.Find("QuitButton")?.GetComponent<Button>());
        }

        if (creditsPanel != null)
            SetPrivateField(mm, "_creditsBackButton", creditsPanel.Find("BackButton")?.GetComponent<Button>());
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
        Vector2 position, float fontSize, Color color, FontStyles style)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = position;
        rt.sizeDelta = new Vector2(900, 100);
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = style;
        return tmp;
    }

    private static GameObject CreateMenuButton(Transform parent, string name, string label, Vector2 position)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = position;
        rt.sizeDelta = new Vector2(300, 55);
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);

        Image img = go.AddComponent<Image>();
        img.color = new Color(0.15f, 0.15f, 0.35f, 1f);

        Button btn = go.AddComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.highlightedColor = new Color(0.3f, 0.3f, 0.6f, 1f);
        cb.pressedColor = new Color(0.1f, 0.1f, 0.25f, 1f);
        btn.colors = cb;

        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(go.transform, false);
        RectTransform textRT = textGO.AddComponent<RectTransform>();
        StretchFull(textRT);
        TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 28;
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
