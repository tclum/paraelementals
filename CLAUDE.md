# Para-Elementals — Claude Code Context

## Project Overview
**Engine:** Unity 6  
**Language:** C#  
**Genre:** 2D Side-Scrolling Roguelike Dungeon Crawler  
**Inspiration:** Diablo + Terraria + Hades + Dead Cells

---

## Project Structure
```
Assets/
├── Animations/
│   ├── Enemies/
│   ├── NPCs/
│   └── Player/
├── Art/
│   ├── Characters/         # Player character sprites
│   ├── Enemies/
│   │   └── SLIMELORD/      # 11-frame walk animation
│   ├── Tilesets/
│   │   └── town/           # 32x32 tileset PNGs (sliced)
│   └── VFX/
├── Editor/                 # Editor setup scripts (menu: Para-Elementals →)
├── Prefabs/
│   ├── Enemies/            # EnemyBaddie.prefab, Boss 1.prefab
│   ├── Items/              # WorldItemPickup.prefab, ShardPickup.prefab
│   ├── Player/             # Player.prefab
│   └── UI/                 # CharacterCard.prefab, DungeonButton.prefab
├── ScriptableObjects/
│   ├── BrushGuy.asset      # CharacterStats
│   ├── Mitty.asset         # CharacterStats
│   ├── SpiritBlob.asset    # CharacterStats (starter, no combat)
│   └── CharacterRoster.asset
├── Scenes/
│   ├── MainMenu.unity
│   ├── HomeBase.unity      # Town hub with tilemap floor
│   ├── DungeonSelect.unity
│   ├── SideScroll_TestScene.unity  # Dungeon Floor 1
│   └── HomeBase.unity
└── Scripts/
    ├── Combat/
    │   ├── AttackHitbox.cs         # Elemental damage + status effects
    │   ├── DamageFlash.cs
    │   ├── Element.cs              # Element enum + ElementalSystem static class
    │   ├── ElementalEntity.cs      # Gives entity an element
    │   ├── ElementalStatus.cs      # Applies burn/slow/stun/shock/knockback
    │   ├── Health.cs               # TakeDamage, Heal, HealthChanged, Died events
    │   └── IDamageable.cs
    ├── Core/
    │   ├── CurrencyManager.cs      # Shards — DontDestroyOnLoad singleton
    │   ├── HomeBaseInteractable.cs # F key proximity interaction
    │   ├── HomeBaseManager.cs      # Manages HomeBase panels
    │   ├── HomeBasePauseManager.cs # Pause + inventory for HomeBase
    │   ├── LevelManager.cs         # Spawn points → boss → results
    │   ├── PlayerSpawnPoint.cs     # Moves player to position on Start
    │   ├── SideScrollCameraFollow.cs
    │   └── SimpleCameraFollow.cs
    ├── Enemies/
    │   ├── BossController.cs       # 2-phase boss (phase 2 at 50% HP)
    │   ├── EnemySpawnPoint.cs      # Wave spawner, fires OnCleared event
    │   ├── EnemySpawner.cs         # Legacy random spawner
    │   ├── LootDropper.cs          # Item + shard drops
    │   └── SideScrollEnemyController.cs
    ├── Inventory/
    │   ├── InventoryManager.cs     # DontDestroyOnLoad singleton
    │   └── InventorySlot.cs
    ├── Items/
    │   ├── ItemData.cs             # ScriptableObject
    │   ├── ShardPickup.cs          # World shard pickup with bob + attract
    │   └── WorldItemPickup.cs
    ├── Player/
    │   ├── CharacterRoster.cs      # ScriptableObject — all characters + unlock state
    │   ├── CharacterStats.cs       # ScriptableObject — per-character stats + element
    │   ├── PlayerStats.cs          # DontDestroyOnLoad — current stat values
    │   ├── PlayerVisual.cs         # Swaps sprite on character change
    │   ├── SideScrollPlayerCombat.cs
    │   ├── SideScrollPlayerController.cs
    │   └── SideScrollPlayerRespawn.cs
    ├── UI/
    │   ├── CharacterCardUI.cs
    │   ├── CharacterSelectManager.cs
    │   ├── DeathScreen.cs
    │   ├── DungeonButton.cs
    │   ├── DungeonSelectManager.cs
    │   ├── HUDManager.cs           # Health/Stamina/Mana/Rage bars
    │   ├── MainMenuManager.cs
    │   ├── PauseManager.cs         # Dungeon pause screen
    │   ├── ResultsScreen.cs
    │   ├── ShardUI.cs
    │   └── HealthBarUI.cs
    └── World/
        └── RoomGenerator.cs
```

---

## Core Systems

### Game Flow
```
MainMenu → HomeBase → DungeonSelect → Dungeon → Results/Death → HomeBase
```

### Scene Names (exact, case-sensitive)
- `MainMenu`
- `HomeBase`
- `DungeonSelect`
- `SideScroll_TestScene` (Floor 1)

### Persistent Singletons (DontDestroyOnLoad)
- `PlayerStats` — character stats, element, can fight
- `CurrencyManager` — shard currency
- `InventoryManager` — item slots

### Health System
```csharp
Health.TakeDamage(int amount)
Health.Heal(int amount)
Health.HealthChanged  // event Action<int, int> (current, max)
Health.Died           // event Action
```

### Elemental System
Elements: `Fire, Water, Wind, Earth, Lightning`

Strong matchups (2x damage):
- Fire → Earth
- Water → Fire  
- Wind → Earth
- Earth → Lightning
- Lightning → Water

Status effects:
- Fire → Burn (DoT)
- Water → Slow
- Wind → Knockback
- Earth → Stun
- Lightning → Shock

Usage:
```csharp
// On AttackHitbox
_attackHitbox.SetElement(Element.Fire);

// On target entity
ElementalEntity entity = target.GetComponent<ElementalEntity>();
int damage = entity.CalculateIncomingDamage(baseDamage, attackElement);
```

### Level Flow (Dungeon)
1. `LevelManager` finds all `EnemySpawnPoint` in scene
2. Each spawn point fires `OnCleared` when all its enemies die
3. When all spawn points cleared → Boss activates
4. Boss defeated → `ResultsScreen.ShowResults()`

```csharp
// EnemySpawnPoint key fields
[SerializeField] private GameObject _enemyPrefab;
[SerializeField] private int _spawnCount = 3;
[SerializeField] private bool _spawnOnStart = true;
[SerializeField] private bool _spawnOnPlayerProximity = false;

// LevelManager key fields  
[SerializeField] private BossController _boss;
[SerializeField] private bool _requireAllEnemiesForBoss = true;
```

### Loot System
```csharp
// LootDropper — attach to enemy prefab
[SerializeField] private ShardPickup _shardPrefab;
[SerializeField] private int _minShards;
[SerializeField] private int _maxShards;
[SerializeField] private int _shardBurstCount;
[SerializeField] private WorldItemPickup _pickupPrefab;
[SerializeField] private LootEntry[] _lootTable;
```

### Currency
```csharp
CurrencyManager.Instance.AddShards(int amount);
CurrencyManager.Instance.SpendShards(int amount); // returns bool
CurrencyManager.Instance.Shards; // current total
```

### Character Stats (ScriptableObject)
```csharp
CharacterStats {
    string CharacterName;
    Sprite CharacterSprite;
    Element CharacterElement;
    bool HasHealth; int MaxHealth;
    bool HasStamina; float MaxStamina;
    bool HasMana; float MaxMana;
    bool HasRage; float MaxRage;
    bool CanFight;
    int BaseDamage;
    float MoveSpeed;
    float JumpForce;
}
```

---

## Conventions

### Naming
- Private fields: `_camelCase`
- Public properties: `PascalCase`
- Events: `PascalCase` (e.g. `OnCleared`, `HealthChanged`)
- ScriptableObjects: stored in `Assets/ScriptableObjects/`
- Prefabs: stored in `Assets/Prefabs/[Category]/`

### Input
Uses **Unity Input System** (new). Always use:
```csharp
using UnityEngine.InputSystem;
Keyboard.current.spaceKey.wasPressedThisFrame
Keyboard.current.fKey.wasPressedThisFrame
```
Never use legacy `Input.GetKeyDown()`.

### Physics
- Ground layer: `"Ground"` (LayerMask)
- Player tag: `"Player"`
- Rigidbody2D used for all movement
- `linearVelocity` not `velocity` (Unity 6)

### UI
- Always uses **TextMeshPro** (`TMPro`)
- Canvas render mode: Screen Space Overlay
- All scenes need an **EventSystem** for UI clicks
- Panels start inactive (`SetActive(false)`)

### Singletons Pattern
```csharp
public static MyManager Instance { get; private set; }

private void Awake()
{
    if (Instance != null && Instance != this) { Destroy(gameObject); return; }
    Instance = this;
    DontDestroyOnLoad(gameObject); // only if persistent
}

private void OnDestroy()
{
    if (Instance == this) Instance = null;
}
```

### Scene Setup
Each dungeon scene needs:
- `PlayerSpawnPoint` — positions player on load
- `LevelManager` — manages enemy spawns and boss
- `EnemySpawnPoint`(s) — spawns enemy waves
- `Boss` GameObject (inactive at start)
- `Canvas` with: HUD, DeathScreenRoot, ResultsScreenRoot, PausePanel
- `EventSystem`
- `CurrencyManager` (if not already persistent)
- Ground on `"Ground"` layer with `BoxCollider2D`

---

## Editor Tools (Para-Elementals menu)
- `Setup Level Scene` — creates spawn points, death/results screens, LevelManager
- `Setup Main Menu Scene` — creates main menu UI
- `Setup Home Base Scene` — creates interactables and panels
- `Setup In-Game HUD` — creates health/stamina/mana/rage bars + shard counter
- `Setup In-Game UI (Pause + Shards)` — creates pause screen
- `Setup Dungeon Select Scene` — creates dungeon selection UI
- `Setup Character Select UI` — creates character selection panel
- `Setup HomeBase Pause + Inventory` — creates HomeBase pause menu
- `Place Town NPCs` — places NPC sprites in HomeBase

---

## Existing Enemies
- **EnemyBaddie** — basic melee enemy, Earth element, 10 HP, 1-3 shard drop
- **Slime Lord (Boss)** — Fire element, 50 HP, 11-frame walk animation, 15-25 shard drop, phase 2 at 50% HP (red color, faster, charge attack)

## Existing Characters
- **Spirit Blob** — starter only, stamina only, no combat
- **Brush Guy** — Water element, HP+Mana, can fight
- **Mitty** — Wind element, HP+Mana, can fight
