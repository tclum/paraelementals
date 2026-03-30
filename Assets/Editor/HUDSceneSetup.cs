using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class HUDSceneSetup : EditorWindow
{
    [MenuItem("Para-Elementals/Setup In-Game HUD")]
    public static void SetupHUD()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            EditorUtility.DisplayDialog("No Canvas", "No Canvas found in scene. Please add a Canvas first.", "OK");
            return;
        }

        // Remove existing HUD
        Transform existing = canvas.transform.Find("HUDRoot");
        if (existing != null)
            DestroyImmediate(existing.gameObject);

        // HUD Root — top left anchor
        GameObject hudRoot = new GameObject("HUDRoot");
        hudRoot.transform.SetParent(canvas.transform, false);
        RectTransform hudRT = hudRoot.AddComponent<RectTransform>();
        hudRT.anchorMin = new Vector2(0f, 1f);
        hudRT.anchorMax = new Vector2(0f, 1f);
        hudRT.pivot = new Vector2(0f, 1f);
        hudRT.anchoredPosition = new Vector2(20f, -20f);
        hudRT.sizeDelta = new Vector2(280f, 200f);

        // Add vertical layout
        VerticalLayoutGroup vlg = hudRoot.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 8f;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.childAlignment = TextAnchor.UpperLeft;

        ContentSizeFitter csf = hudRoot.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // Create all bars
        var (healthBar, healthSlider, healthFill, healthText) = CreateStatBar(
            hudRoot.transform, "HealthBar", "HP",
            new Color(0.9f, 0.15f, 0.15f), new Color(0.6f, 0.05f, 0.05f));

        var (staminaBar, staminaSlider, staminaFill, staminaText) = CreateStatBar(
            hudRoot.transform, "StaminaBar", "ST",
            new Color(0.2f, 0.8f, 0.2f), new Color(0.05f, 0.4f, 0.05f));

        var (manaBar, manaSlider, manaFill, manaText) = CreateStatBar(
            hudRoot.transform, "ManaBar", "MP",
            new Color(0.2f, 0.4f, 0.9f), new Color(0.05f, 0.1f, 0.5f));

        var (rageBar, rageSlider, rageFill, rageText) = CreateStatBar(
            hudRoot.transform, "RageBar", "RG",
            new Color(0.9f, 0.3f, 0.05f), new Color(0.5f, 0.1f, 0.02f));

        // Add HUDManager and wire references
        HUDManager hud = canvas.GetComponentInChildren<HUDManager>();
        if (hud == null)
        {
            GameObject hudMgrGO = new GameObject("HUDManager");
            hudMgrGO.transform.SetParent(canvas.transform, false);
            hud = hudMgrGO.AddComponent<HUDManager>();
        }

        SetField(hud, "_healthBarRoot", healthBar);
        SetField(hud, "_healthSlider", healthSlider);
        SetField(hud, "_healthText", healthText);
        SetField(hud, "_healthFill", healthFill);

        SetField(hud, "_staminaBarRoot", staminaBar);
        SetField(hud, "_staminaSlider", staminaSlider);
        SetField(hud, "_staminaText", staminaText);
        SetField(hud, "_staminaFill", staminaFill);

        SetField(hud, "_manaBarRoot", manaBar);
        SetField(hud, "_manaSlider", manaSlider);
        SetField(hud, "_manaText", manaText);
        SetField(hud, "_manaFill", manaFill);

        SetField(hud, "_rageBarRoot", rageBar);
        SetField(hud, "_rageSlider", rageSlider);
        SetField(hud, "_rageText", rageText);
        SetField(hud, "_rageFill", rageFill);

        Debug.Log("[Setup] HUD setup complete!");
        EditorUtility.DisplayDialog("HUD Setup Complete",
            "In-game HUD created in top-left!\n\n" +
            "- Health bar (red) — always visible\n" +
            "- Stamina bar (green)\n" +
            "- Mana bar (blue)\n" +
            "- Rage bar (orange)\n\n" +
            "Bars auto-show/hide based on CharacterStats.\n\n" +
            "Still needed:\n" +
            "- Add PlayerStats to Player prefab\n" +
            "- Create CharacterStats ScriptableObjects\n" +
            "- Assign SpiritBlob stats to Player",
            "OK");
    }

    private static (GameObject root, Slider slider, Image fill, TextMeshProUGUI text)
        CreateStatBar(Transform parent, string name, string label, Color fillColor, Color bgColor)
    {
        // Bar root
        GameObject root = new GameObject(name);
        root.transform.SetParent(parent, false);
        RectTransform rootRT = root.AddComponent<RectTransform>();
        rootRT.sizeDelta = new Vector2(280f, 32f);
        LayoutElement le = root.AddComponent<LayoutElement>();
        le.preferredHeight = 32f;
        le.flexibleWidth = 1f;

        // Background
        Image bgImg = root.AddComponent<Image>();
        bgImg.color = new Color(0.1f, 0.1f, 0.1f, 0.85f);

        // Label
        GameObject labelGO = new GameObject("Label");
        labelGO.transform.SetParent(root.transform, false);
        RectTransform labelRT = labelGO.AddComponent<RectTransform>();
        labelRT.anchorMin = new Vector2(0f, 0f);
        labelRT.anchorMax = new Vector2(0f, 1f);
        labelRT.pivot = new Vector2(0f, 0.5f);
        labelRT.anchoredPosition = new Vector2(6f, 0f);
        labelRT.sizeDelta = new Vector2(28f, 0f);
        TextMeshProUGUI labelTMP = labelGO.AddComponent<TextMeshProUGUI>();
        labelTMP.text = label;
        labelTMP.fontSize = 14f;
        labelTMP.color = Color.white;
        labelTMP.fontStyle = FontStyles.Bold;
        labelTMP.alignment = TextAlignmentOptions.MidlineLeft;

        // Slider
        GameObject sliderGO = new GameObject("Slider");
        sliderGO.transform.SetParent(root.transform, false);
        RectTransform sliderRT = sliderGO.AddComponent<RectTransform>();
        sliderRT.anchorMin = new Vector2(0f, 0f);
        sliderRT.anchorMax = new Vector2(1f, 1f);
        sliderRT.offsetMin = new Vector2(38f, 3f);
        sliderRT.offsetMax = new Vector2(-6f, -3f);
        Slider slider = sliderGO.AddComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 1f;
        slider.interactable = false;

        // Fill area
        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderGO.transform, false);
        RectTransform fillAreaRT = fillArea.AddComponent<RectTransform>();
        fillAreaRT.anchorMin = Vector2.zero;
        fillAreaRT.anchorMax = Vector2.one;
        fillAreaRT.offsetMin = fillAreaRT.offsetMax = Vector2.zero;

        // Fill
        GameObject fillGO = new GameObject("Fill");
        fillGO.transform.SetParent(fillArea.transform, false);
        RectTransform fillRT = fillGO.AddComponent<RectTransform>();
        fillRT.anchorMin = Vector2.zero;
        fillRT.anchorMax = Vector2.one;
        fillRT.offsetMin = fillRT.offsetMax = Vector2.zero;
        Image fillImg = fillGO.AddComponent<Image>();
        fillImg.color = fillColor;

        slider.fillRect = fillRT;

        // Value text
        GameObject textGO = new GameObject("ValueText");
        textGO.transform.SetParent(sliderGO.transform, false);
        RectTransform textRT = textGO.AddComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = textRT.offsetMax = Vector2.zero;
        TextMeshProUGUI valueTMP = textGO.AddComponent<TextMeshProUGUI>();
        valueTMP.text = "—";
        valueTMP.fontSize = 13f;
        valueTMP.color = Color.white;
        valueTMP.alignment = TextAlignmentOptions.Center;
        valueTMP.fontStyle = FontStyles.Bold;

        return (root, slider, fillImg, valueTMP);
    }

    private static void SetField(object obj, string name, object value)
    {
        var field = obj.GetType().GetField(name,
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(obj, value);
    }
}
