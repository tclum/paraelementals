using UnityEngine;
using UnityEditor;

public class PlaceTownNPCs : EditorWindow
{
    [MenuItem("Para-Elementals/Place Town NPCs")]
    public static void PlaceNPCs()
    {
        // Load NPC sprites
        Sprite[] sprites = new Sprite[]
        {
            AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Characters/TownPeople/Townpeople2.png"),
            AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Characters/TownPeople/Townpeople3.png"),
            AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Characters/TownPeople/Townpeople4.png"),
            AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Characters/TownPeople/Townpeople5.png"),
        };

        // NPC positions — near each interactable
        (string name, Vector3 position, int spriteIndex)[] npcs = new[]
        {
            ("NPC_Merchant",       new Vector3(-8.5f, -0.5f, 0f), 0),
            ("NPC_Crafter",        new Vector3(-4.5f, -0.5f, 0f), 1),
            ("NPC_Storage",        new Vector3(0.5f,  -0.5f, 0f), 2),
            ("NPC_SkillTrainer",   new Vector3(4.5f,  -0.5f, 0f), 3),
            ("NPC_CharStation",    new Vector3(-6.5f, -0.5f, 0f), 0),
        };

        // Find or create NPCs container
        GameObject container = GameObject.Find("NPCs");
        if (container == null)
            container = new GameObject("NPCs");

        foreach (var (npcName, position, spriteIndex) in npcs)
        {
            // Skip if already exists
            if (GameObject.Find(npcName) != null)
                continue;

            GameObject npc = new GameObject(npcName);
            npc.transform.SetParent(container.transform);
            npc.transform.position = position;

            // Sprite renderer
            SpriteRenderer sr = npc.AddComponent<SpriteRenderer>();
            if (sprites[spriteIndex] != null)
                sr.sprite = sprites[spriteIndex];
            else
                Debug.LogWarning($"[PlaceNPCs] Sprite {spriteIndex} not found.");

            sr.sortingOrder = 1;

            // Scale to match player size
            npc.transform.localScale = new Vector3(0.01f, 0.01f, 1f);
        }

        Debug.Log("[PlaceNPCs] Town NPCs placed!");
        EditorUtility.DisplayDialog("NPCs Placed",
            "5 town NPCs placed in HomeBase!\n\nAdjust their positions in the scene view as needed.",
            "OK");
    }
}
