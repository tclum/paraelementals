using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Para-Elementals → Build Dungeon Floor 2
/// Creates DungeonFloor2.unity per LEVEL2_BRIEF.md and wires all references.
/// Run from: Para-Elementals > Build Dungeon Floor 2
/// </summary>
public class DungeonFloor2Setup : EditorWindow
{
    private const string SceneName = "DungeonFloor2";
    private const string ScenePath = "Assets/Scenes/DungeonFloor2.unity";

    [MenuItem("Para-Elementals/Build Dungeon Floor 2")]
    public static void BuildDungeonFloor2()
    {
        // ── Step 1: Create & save new scene ───────────────────────────────
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;

        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        EditorSceneManager.SaveScene(scene, ScenePath);
        AssetDatabase.Refresh();

        // ── Steps 2–4: Existing editor tools ──────────────────────────────
        // 2) Setup Level Scene  →  Canvas + DeathScreen + ResultsScreen + LevelManager + 3 placeholder spawns
        ParaElementalsSceneSetup.SetupScene();
        // 3) Setup In-Game HUD  →  HUDRoot with HP/ST/MP/RG bars
        HUDSceneSetup.SetupHUD();
        // 4) Setup In-Game UI (Pause + Shards)
        InGameUISetup.SetupInGameUI();

        // Ensure EventSystem exists (UI requires it)
        EnsureEventSystem();

        // ── Step 5: Ground ────────────────────────────────────────────────
        GameObject ground = CreateGround();

        // ── Step 6: Platforms (3) ─────────────────────────────────────────
        CreatePlatform("Platform_A", new Vector3(-8f, 2f, 0f), new Vector2(5f, 0.5f));
        CreatePlatform("Platform_B", new Vector3(2f,  4f, 0f), new Vector2(5f, 0.5f));
        CreatePlatform("Platform_C", new Vector3(11f, 3f, 0f), new Vector2(4f, 0.5f));

        // ── Step 7: PlayerSpawnPoint ──────────────────────────────────────
        GameObject psp = new GameObject("PlayerSpawnPoint");
        psp.transform.position = new Vector3(-15f, 1f, 0f);
        psp.AddComponent<PlayerSpawnPoint>();

        // ── Delete placeholder spawns created by SetupScene ──────────────
        for (int i = 1; i <= 3; i++)
        {
            GameObject old = GameObject.Find($"SpawnPoint_{i}");
            if (old != null) Object.DestroyImmediate(old);
        }

        // ── Steps 8–10: Four EnemySpawnPoints ────────────────────────────
        float[] spawnXPositions = { -10f, -3f, 4f, 11f };
        GameObject enemyBaddiePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(
            "Assets/Prefabs/Enemies/EnemyBaddie.prefab");

        EnemySpawnPoint[] spawnPoints = new EnemySpawnPoint[4];
        for (int i = 0; i < 4; i++)
        {
            GameObject sp = new GameObject($"SpawnPoint_{i + 1}");
            sp.transform.position = new Vector3(spawnXPositions[i], 0f, 0f);
            EnemySpawnPoint eps = sp.AddComponent<EnemySpawnPoint>();

            SetField(eps, "_enemyPrefab",           enemyBaddiePrefab);
            SetField(eps, "_spawnCount",             3);
            SetField(eps, "_spawnOnStart",           false);
            SetField(eps, "_spawnOnPlayerProximity", true);
            SetField(eps, "_proximityRange",         10f);

            spawnPoints[i] = eps;
        }

        // ── Steps 11–12: Boss GameObject ──────────────────────────────────
        GameObject boss = new GameObject("Boss_CrystalGolem");
        boss.transform.position = new Vector3(16f, 1f, 0f);

        // Visual child (BossController flips scale on this root)
        GameObject visualRoot = new GameObject("VisualRoot");
        visualRoot.transform.SetParent(boss.transform, false);
        SpriteRenderer bossSR = visualRoot.AddComponent<SpriteRenderer>();
        bossSR.color = new Color(0.55f, 0.75f, 0.55f); // crystal-greenish tint

        // Physics — BossController needs Rigidbody2D
        Rigidbody2D bossRb = boss.AddComponent<Rigidbody2D>();
        bossRb.gravityScale = 3f;
        bossRb.freezeRotation = true;
        boss.AddComponent<BoxCollider2D>();

        // BossController (Crystal Golem stats from brief)
        BossController bc = boss.AddComponent<BossController>();
        SetField(bc, "_moveSpeed",          1.5f);
        SetField(bc, "_contactDamage",      3);
        SetField(bc, "_attackCooldown",     2f);
        SetField(bc, "_phase2HealthPercent", 0.4f);   // phase 2 at 40% HP
        SetField(bc, "_phase2Color",        Color.blue);
        SetField(bc, "_visualRoot",         visualRoot.transform);
        SetField(bc, "_spriteRenderer",     bossSR);

        // Health — 80 HP
        Health bossHealth = boss.AddComponent<Health>();
        SetField(bossHealth, "_maxHealth", 80);

        // ElementalEntity — Earth
        ElementalEntity bossElement = boss.AddComponent<ElementalEntity>();
        bossElement.SetElement(Element.Earth);

        // LootDropper — 25-40 shards
        LootDropper lootDropper = boss.AddComponent<LootDropper>();
        ShardPickup shardPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(
            "Assets/Prefabs/Items/ShardPickup.prefab")?.GetComponent<ShardPickup>();
        SetField(lootDropper, "_shardPrefab",     shardPrefab);
        SetField(lootDropper, "_minShards",        25);
        SetField(lootDropper, "_maxShards",        40);
        SetField(lootDropper, "_shardBurstCount",  5);

        // Boss starts inactive (LevelManager activates it after all spawns cleared)
        boss.SetActive(false);

        // ── Step 13: Wire Boss to LevelManager ────────────────────────────
        LevelManager lm = Object.FindFirstObjectByType<LevelManager>();
        if (lm != null)
        {
            SetField(lm, "_boss", bc);
            SetField(lm, "_requireAllEnemiesForBoss", true);
            Debug.Log("[Floor2Setup] LevelManager._boss wired to CrystalGolem.");
        }
        else
        {
            Debug.LogError("[Floor2Setup] LevelManager not found — boss not wired.");
        }

        // ── Step 14: ResultsScreen nextScene = "HomeBase" ─────────────────
        ResultsScreen rs = Object.FindFirstObjectByType<ResultsScreen>();
        if (rs != null)
        {
            SetField(rs, "_nextScene", "HomeBase");
            Debug.Log("[Floor2Setup] ResultsScreen._nextScene = \"HomeBase\".");
        }
        else
        {
            Debug.LogError("[Floor2Setup] ResultsScreen not found — nextScene not set.");
        }

        // ── Step 15: Add to Build Settings ────────────────────────────────
        AddSceneToBuildSettings(ScenePath);

        // ── Save ──────────────────────────────────────────────────────────
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        AssetDatabase.Refresh();

        Debug.Log("[Floor2Setup] DungeonFloor2 build complete!");
        EditorUtility.DisplayDialog("Dungeon Floor 2 — Done",
            "DungeonFloor2.unity created and saved.\n\n" +
            "Wired:\n" +
            "  ✓ LevelManager → CrystalGolem boss\n" +
            "  ✓ 4 EnemySpawnPoints (proximity, EnemyBaddie x3 each)\n" +
            "  ✓ ResultsScreen nextScene = \"HomeBase\"\n" +
            "  ✓ Added to Build Settings\n\n" +
            "Still needed:\n" +
            "  - Assign Crystal Golem sprite\n" +
            "  - Assign ShardPickup/WorldItemPickup prefabs to enemy LootDroppers\n" +
            "  - Wire Camera to player if using SideScrollCameraFollow\n" +
            "  - Add SideScrollCameraFollow to Main Camera",
            "OK");
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static GameObject CreateGround()
    {
        GameObject ground = new GameObject("Ground");

        // Layer
        int groundLayer = LayerMask.NameToLayer("Ground");
        if (groundLayer >= 0)
            ground.layer = groundLayer;
        else
            Debug.LogWarning("[Floor2Setup] 'Ground' layer not found — ground layer not set.");

        // Visual scale (represents a wide flat tile)
        ground.transform.position = new Vector3(0f, -1f, 0f);
        ground.transform.localScale = new Vector3(40f, 0.5f, 1f);

        // SpriteRenderer so the ground is visible in Editor
        SpriteRenderer sr = ground.AddComponent<SpriteRenderer>();
        sr.color = new Color(0.35f, 0.25f, 0.15f); // dark-brown cave floor

        // Collider
        ground.AddComponent<BoxCollider2D>();

        return ground;
    }

    private static void CreatePlatform(string name, Vector3 position, Vector2 scale)
    {
        GameObject platform = new GameObject(name);

        int groundLayer = LayerMask.NameToLayer("Ground");
        if (groundLayer >= 0)
            platform.layer = groundLayer;

        platform.transform.position = position;
        platform.transform.localScale = new Vector3(scale.x, scale.y, 1f);

        SpriteRenderer sr = platform.AddComponent<SpriteRenderer>();
        sr.color = new Color(0.25f, 0.4f, 0.5f); // crystal-bluish platform

        platform.AddComponent<BoxCollider2D>();
    }

    private static void EnsureEventSystem()
    {
        if (Object.FindFirstObjectByType<EventSystem>() != null)
            return;

        GameObject es = new GameObject("EventSystem");
        es.AddComponent<EventSystem>();
        es.AddComponent<StandaloneInputModule>();
    }

    private static void AddSceneToBuildSettings(string scenePath)
    {
        List<EditorBuildSettingsScene> scenes =
            new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);

        // Skip if already present
        foreach (var s in scenes)
        {
            if (s.path == scenePath)
            {
                Debug.Log($"[Floor2Setup] {SceneName} already in Build Settings.");
                return;
            }
        }

        scenes.Add(new EditorBuildSettingsScene(scenePath, true));
        EditorBuildSettings.scenes = scenes.ToArray();
        Debug.Log($"[Floor2Setup] Added {SceneName} to Build Settings (index {scenes.Count - 1}).");
    }

    private static void SetField(object obj, string fieldName, object value)
    {
        FieldInfo fi = obj.GetType().GetField(
            fieldName,
            BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        if (fi != null)
            fi.SetValue(obj, value);
        else
            Debug.LogWarning($"[Floor2Setup] Field '{fieldName}' not found on {obj.GetType().Name}");
    }
}
