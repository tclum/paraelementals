# Para-Elementals — Dungeon Floor 2 Design Brief

## Theme
**Underground Cave / Crystal Caverns**  
Dark, underground cave system with glowing crystals. 
Harder than Floor 1, introduces Lightning enemies and a new boss.

---

## Scene Name
`DungeonFloor2`

---

## Layout
Side-scrolling level, wider than Floor 1.
- Longer ground with platforms at varying heights
- 4 enemy spawn points (vs 3 in Floor 1)
- Boss arena at the far right end

---

## Enemies

### Cave Crawler (regular enemy)
- Element: **Lightning**
- HP: 8
- Move Speed: 3 (faster than EnemyBaddie)
- Contact damage: 1
- Detection range: 8
- Shard drop: 2-4
- Behavior: same as SideScrollEnemyController
- Use EnemyBaddie prefab as base (swap sprite when available)

### Crystal Golem (Boss)
- Element: **Earth**
- HP: 80
- Move Speed: 1.5
- Phase 2 at 40% HP: spawns 2 Cave Crawlers, turns blue, gains shield charge
- Shard drop: 25-40
- Contact damage: 3
- Attack cooldown: 2s
- Use BossController as base

---

## Spawn Points
- 4 spawn points spread across the level
- Each spawns 3 Cave Crawlers
- Spawn on proximity (player walks near)
- Boss activates after all 4 cleared

---

## Win Condition
Defeat the Crystal Golem → ResultsScreen → return to HomeBase

---

## Scene Setup Steps for Claude Code
1. Create new scene `DungeonFloor2` in `Assets/Scenes/`
2. Run `Para-Elementals → Setup Level Scene` editor tool
3. Run `Para-Elementals → Setup In-Game HUD` editor tool
4. Run `Para-Elementals → Setup In-Game UI (Pause + Shards)` editor tool
5. Create ground GameObject (Layer: Ground, BoxCollider2D, scale X:40 Y:0.5)
6. Add 2-3 platform GameObjects at different heights
7. Create PlayerSpawnPoint at X:-15, Y:1
8. Create 4 EnemySpawnPoint GameObjects spread across X: -10, -3, 4, 11
9. Assign EnemyBaddie prefab to each spawn point
10. Set each spawn point: spawnCount=3, spawnOnPlayerProximity=true
11. Create Boss GameObject (inactive): add BossController, Health(80), ElementalEntity(Earth), LootDropper
12. Position boss at X:16, Y:1
13. Wire Boss to LevelManager
14. Add ResultsScreen nextScene = "HomeBase"
15. Add DungeonFloor2 to Build Settings

---

## Key Files to Reference
- `Assets/Scripts/Enemies/SideScrollEnemyController.cs` — enemy behavior
- `Assets/Scripts/Enemies/BossController.cs` — boss behavior  
- `Assets/Scripts/Enemies/EnemySpawnPoint.cs` — spawn point
- `Assets/Scripts/Core/LevelManager.cs` — level flow
- `Assets/Prefabs/Enemies/EnemyBaddie.prefab` — enemy prefab to reuse
- `Assets/Prefabs/Enemies/Boss 1.prefab` — boss prefab to reference
