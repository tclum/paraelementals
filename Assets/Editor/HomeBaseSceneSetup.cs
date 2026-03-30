using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class HomeBaseSceneSetup : EditorWindow
{
    [MenuItem("Para-Elementals/Setup Home Base Scene")]
    public static void SetupHomeBase()
    {
        // Create Canvas
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

        // Add EventSystem if missing
        if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        }

        // Create interact prompt
        GameObject prompt = CreateInteractPrompt(canvas);

        // Create all panels
        GameObject craftingPanel = CreatePanel(canvas, "CraftingPanel", "CRAFTING STATION",
            "Craft items from gathered materials.\n\nComing soon...",
            new Color(0.1f, 0.2f, 0.1f, 0.95f));

        GameObject storagePanel = CreatePanel(canvas, "StoragePanel", "ITEM STORAGE",
            "Store and retrieve items between runs.\n\nComing soon...",
            new Color(0.1f, 0.1f, 0.2f, 0.95f));

        GameObject shopPanel = CreatePanel(canvas, "ShopPanel", "MERCHANT",
            "Buy and sell items with the merchant.\n\nComing soon...",
            new Color(0.2f, 0.15f, 0.05f, 0.95f));

        GameObject skillsPanel = CreatePanel(canvas, "SkillsPanel", "SKILL UPGRADES",
            "Spend skill points to upgrade your abilities.\n\nComing soon...",
            new Color(0.15f, 0.05f, 0.2f, 0.95f));

        // Create HomeBaseManager
        GameObject mgr = new GameObject("HomeBaseManager");
        HomeBaseManager hbm = mgr.AddComponent<HomeBaseManager>();
        SetPrivateField(hbm, "_craftingPanel", craftingPanel);
        SetPrivateField(hbm, "_storagePanel", storagePanel);
        SetPrivateField(hbm, "_shopPanel", shopPanel);
        SetPrivateField(hbm, "_skillsPanel", skillsPanel);
        SetPrivateField(hbm, "_interactPrompt", prompt);

        // Create interactable objects in world
        CreateInteractable("DungeonPortal", new Vector3(8f, 0f, 0f),
            HomeBaseInteractable_Type.DungeonPortal, "Press F to enter dungeon",
            new Color(0.5f, 0.2f, 1f, 1f));

        CreateInteractable("CraftingStation", new Vector3(-4f, 0f, 0f),
            HomeBaseInteractable_Type.CraftingStation, "Press F to craft",
            new Color(0.2f, 0.8f, 0.2f, 1f));

        CreateInteractable("ItemStorage", new Vector3(0f, 0f, 0f),
            HomeBaseInteractable_Type.ItemStorage, "Press F to open storage",
            new Color(0.2f, 0.4f, 0.8f, 1f));

        CreateInteractable("Merchant", new Vector3(-8f, 0f, 0f),
            HomeBaseInteractable_Type.Shop, "Press F to trade",
            new Color(0.8f, 0.6f, 0.1f, 1f));

        CreateInteractable("SkillTree", new Vector3(4f, 0f, 0f),
            HomeBaseInteractable_Type.SkillUpgrade, "Press F to upgrade skills",
            new Color(0.8f, 0.2f, 0.6f, 1f));

        // Create ground
        if (GameObject.Find("Ground") == null)
        {
            GameObject ground = new GameObject("Ground");
            ground.transform.position = new Vector3(0f, -1.5f, 0f);
            ground.transform.localScale = new Vector3(30f, 0.3f, 1f);
            var sr = ground.AddComponent<SpriteRenderer>();
            sr.color = new Color(0.3f, 0.2f, 0.1f, 1f);
            var col = ground.AddComponent<BoxCollider2D>();
            ground.layer = LayerMask.NameToLayer("Ground");
        }

        EditorUtility.DisplayDialog("Home Base Setup Complete",
            "Home Base scene setup complete!\n\n" +
            "Still needed:\n" +
            "- Add your player prefab to the scene\n" +
            "- Add a camera follow script\n" +
            "- Customize the interactable visuals\n" +
            "- Set Dungeon Scene Name in HomeBaseManager",
            "OK");

        Debug.Log("[Setup] Home Base scene setup complete!");
    }

    private static GameObject CreateInteractPrompt(Canvas canvas)
    {
        GameObject prompt = new GameObject("InteractPrompt");
        prompt.transform.SetParent(canvas.transform, false);
        RectTransform rt = prompt.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0f);
        rt.anchorMax = new Vector2(0.5f, 0f);
        rt.anchoredPosition = new Vector2(0f, 120f);
        rt.sizeDelta = new Vector2(400f, 60f);

        Image bg = prompt.AddComponent<Image>();
        bg.color = new Color(0f, 0f, 0f, 0.7f);

        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(prompt.transform, false);
        RectTransform textRT = textGO.AddComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = textRT.offsetMax = Vector2.zero;
        TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = "Press F to interact";
        tmp.fontSize = 24;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;

        prompt.SetActive(false);
        return prompt;
    }

    private static GameObject CreatePanel(Canvas canvas, string name, string title,
        string body, Color bgColor)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(canvas.transform, false);
        RectTransform rt = panel.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(600f, 400f);
        rt.anchoredPosition = Vector2.zero;

        Image bg = panel.AddComponent<Image>();
        bg.color = bgColor;

        // Title
        GameObject titleGO = new GameObject("Title");
        titleGO.transform.SetParent(panel.transform, false);
        RectTransform titleRT = titleGO.AddComponent<RectTransform>();
        titleRT.anchorMin = new Vector2(0f, 0.75f);
        titleRT.anchorMax = new Vector2(1f, 1f);
        titleRT.offsetMin = titleRT.offsetMax = Vector2.zero;
        TextMeshProUGUI titleTMP = titleGO.AddComponent<TextMeshProUGUI>();
        titleTMP.text = title;
        titleTMP.fontSize = 36;
        titleTMP.color = Color.white;
        titleTMP.alignment = TextAlignmentOptions.Center;
        titleTMP.fontStyle = FontStyles.Bold;

        // Body
        GameObject bodyGO = new GameObject("Body");
        bodyGO.transform.SetParent(panel.transform, false);
        RectTransform bodyRT = bodyGO.AddComponent<RectTransform>();
        bodyRT.anchorMin = new Vector2(0f, 0.2f);
        bodyRT.anchorMax = new Vector2(1f, 0.75f);
        bodyRT.offsetMin = new Vector2(20f, 0f);
        bodyRT.offsetMax = new Vector2(-20f, 0f);
        TextMeshProUGUI bodyTMP = bodyGO.AddComponent<TextMeshProUGUI>();
        bodyTMP.text = body;
        bodyTMP.fontSize = 22;
        bodyTMP.color = new Color(0.8f, 0.8f, 0.8f);
        bodyTMP.alignment = TextAlignmentOptions.Center;

        // Close button
        GameObject closeBtn = new GameObject("CloseButton");
        closeBtn.transform.SetParent(panel.transform, false);
        RectTransform closeBtnRT = closeBtn.AddComponent<RectTransform>();
        closeBtnRT.anchorMin = new Vector2(0.5f, 0f);
        closeBtnRT.anchorMax = new Vector2(0.5f, 0f);
        closeBtnRT.anchoredPosition = new Vector2(0f, 40f);
        closeBtnRT.sizeDelta = new Vector2(150f, 45f);
        Image closeBtnImg = closeBtn.AddComponent<Image>();
        closeBtnImg.color = new Color(0.5f, 0.1f, 0.1f, 1f);
        Button btn = closeBtn.AddComponent<Button>();

        GameObject closeTxt = new GameObject("Text");
        closeTxt.transform.SetParent(closeBtn.transform, false);
        RectTransform closeTxtRT = closeTxt.AddComponent<RectTransform>();
        closeTxtRT.anchorMin = Vector2.zero;
        closeTxtRT.anchorMax = Vector2.one;
        closeTxtRT.offsetMin = closeTxtRT.offsetMax = Vector2.zero;
        TextMeshProUGUI closeTMP = closeTxt.AddComponent<TextMeshProUGUI>();
        closeTMP.text = "CLOSE";
        closeTMP.fontSize = 22;
        closeTMP.color = Color.white;
        closeTMP.alignment = TextAlignmentOptions.Center;

        panel.SetActive(false);
        return panel;
    }

    // Workaround — use int since we can't use the enum directly in editor script easily
    private enum HomeBaseInteractable_Type
    {
        DungeonPortal, CraftingStation, ItemStorage, Shop, SkillUpgrade
    }

    private static void CreateInteractable(string name, Vector3 position,
        HomeBaseInteractable_Type type, string prompt, Color color)
    {
        if (GameObject.Find(name) != null) return;

        GameObject go = new GameObject(name);
        go.transform.position = position;

        // Visual placeholder
        GameObject visual = new GameObject("Visual");
        visual.transform.SetParent(go.transform, false);
        SpriteRenderer sr = visual.AddComponent<SpriteRenderer>();
        sr.color = color;
        sr.sprite = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        visual.transform.localScale = new Vector3(1.5f, 2f, 1f);

        // Label above
        GameObject label = new GameObject("Label");
        label.transform.SetParent(go.transform, false);
        label.transform.localPosition = new Vector3(0f, 1.5f, 0f);

        // Collider for interaction range
        CircleCollider2D col = go.AddComponent<CircleCollider2D>();
        col.radius = 0.5f;
        col.isTrigger = true;

        // Interactable script
        HomeBaseInteractable interactable = go.AddComponent<HomeBaseInteractable>();
        SetPrivateField(interactable, "_type", (int)type);
        SetPrivateField(interactable, "_promptText", prompt);
    }

    private static void SetPrivateField(object obj, string fieldName, object value)
    {
        var field = obj.GetType().GetField(fieldName,
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null)
        {
            // Handle enum fields
            if (field.FieldType.IsEnum && value is int intVal)
                field.SetValue(obj, System.Enum.ToObject(field.FieldType, intVal));
            else
                field.SetValue(obj, value);
        }
    }
}
