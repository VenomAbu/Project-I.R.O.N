# Project I.R.O.N

![Unity](https://img.shields.io/badge/Unity-6000.3.10f1-black?logo=unity)
![Render Pipeline](https://img.shields.io/badge/Rendering-URP_2D-blue)
![Language](https://img.shields.io/badge/Language-C%2523-green?logo=csharp)

**Project I.R.O.N** is a Unity 2D top-down tank survival game with roguelite progression: drive a tank with inertia-based physics, survive timed waves driven by a budget-based difficulty director, level up mid-combat to draft passive upgrade cards, collect and manage consumable items, and explore a hub city with shop and story cutscenes built on a branching dialogue system.

---

## Tech Stack

| | |
|---|---|
| **Engine** | Unity 6000.3.10f1 (Unity 6) |
| **Rendering** | Universal Render Pipeline (URP 2D) |
| **Camera** | Cinemachine 3.x (gameplay follow + per-line cutscene cameras) |
| **UI** | uGUI + TextMeshPro |
| **Input** | Unity Input Manager (keyboard + mouse) |
| **Language** | C# |

## Main Scenes

Build order (index used by scene loading):

| Index | Scene | Purpose |
|---|---|---|
| 0 | `Assets/Project I.R.O.N/Scenes/StartScreenR.unity` | Main menu |
| 1 | `Assets/Project I.R.O.N/Scenes/CityScreenR.unity` | City hub (mission select + shop access) |
| 2 | `Assets/Project I.R.O.N/Scenes/Missão.unity` | Combat mission (300 s survival stage) |
| 3 | `Assets/Project I.R.O.N/Scenes/StoreScreen.unity` | Shop |

## Running the Project

1. Open the project folder in Unity Hub.
2. Use **Unity 6000.3.10f1** or a compatible Unity 6 version.
3. Open `Assets/Project I.R.O.N/Scenes/StartScreenR.unity`.
4. Press **Play**.

## Controls

| Action | Input |
|---|---|
| Move / steer tank | `WASD` or arrow keys (vertical = throttle, horizontal = rotate) |
| Aim turret | Mouse position |
| Fire cannon | Left mouse button |
| Advance dialogue / confirm | `Space` or left mouse button |
| Use inventory item | Click the item slot in the HUD |

---

## Gameplay Loop

1. Start a mission from the **City hub**.
2. Survive **300 seconds** against escalating enemy waves.
3. Kill enemies to gain **XP**, level up, and **draft 1 of 3 passive upgrade cards**.
4. Collect dropped items (**Repair Kit, Gas, Ammo, Bomb, Coins**) and use them from the HUD inventory.
5. Win the stage to earn a **coin bonus**, then spend coins in the **Shop**.

## Main Structure

```
Assets/Project I.R.O.N/
├── Scripts/          # All gameplay code (see breakdown below)
├── Prefabs/          # Tank, enemies, projectiles, items, UI
├── Scenes/           # Menu, city hub, mission, shop
├── Sounds/           # Audio clips wired through SoundAssets
└── Sprites/          # Art assets
```

### Script Breakdown

| Script | Responsibility |
|---|---|
| `Character.cs` | Abstract base class: HP, `TakeDamage`, `Heal`, `HealPercentage`, abstract `Die()` |
| `MainTank.cs` | Player controller: throttle movement, fuel, coins, ammo, item pickup, death flow |
| `TurretSystem.cs` | Mouse aiming, fire rate, damage calculation, ricochet routine |
| `Projectile.cs` / `BounceProjectile.cs` | Straight shot with life-steal hook / camera-bounds ricochet projectile |
| `Enemy.cs` | Chase AI, contact damage with cooldown, XP reward, loot rolls |
| `WaveController.cs` | Stage timer, pressure/budget difficulty math, wave cadence |
| `SpawnController.cs` | Budget-based enemy purchasing and spawn point selection |
| `XpManager.cs` | XP singleton, level-up curve, attribute growth |
| `LevelUpManager.cs` | Pauses game and deals 3 unique upgrade cards |
| `PassiveCardUI.cs` | Card visual bindings and click handling |
| `PassiveManager.cs` | Passive levels and their gameplay effects |
| `Item.cs` / `ItemAssets.cs` | Item enum, stacking rules, use effects, sprite database |
| `Inventory.cs` / `ItemWorld.cs` | Event-driven inventory model / world pickups |
| `UI_Inventory.cs` | HUD grid that rebuilds on inventory change events |
| `DialogueManager.cs` | Node-graph cutscenes with branching choices and camera cuts |
| `CityMenu.cs` / `MainMenu.cs` | Scene navigation buttons |
| `GameOverManager.cs` / `VictoryManager.cs` | End-of-round panels, rewards, restart flow |
| `CameraController.cs` | Simple player-follow camera |
| `SoundAssets.cs` | Singleton registry for every `AudioSource` in the game |

---

## Systems in Detail

### Player Tank (`MainTank.cs`)

Movement is **throttle-based**, not teleport-based:

- Vertical input builds up `currentThrottle`; terrain resistance bleeds it back down over time.
- Throttle is clamped between `maxReverseSpeed` and `maxSpeed` and applied as `rb.linearVelocity = transform.up * currentThrottle`, so the tank accelerates, coasts, and reverses like a vehicle.
- Horizontal input rotates the hull with `rb.MoveRotation`.
- Collisions zero out velocity and throttle instantly.
- **Fuel system:** moving drains 1 gas per second. At empty fuel, both speed caps are halved instead of hard-stopping the player.
- **Reactive engine audio:** engine pitch lerps between idle (0.8) and max (1.5) based on current speed percentage.

### Turret & Combat (`TurretSystem.cs`)

- The turret smoothly aims toward the mouse using `Mathf.MoveTowardsAngle`.
- Firing consumes **ammo** and respects a `shootCooldown`.

**Damage formula (main shot):**

```
damage = (10 × turretDamage + tankLevel × 8) × (1 + 0.10 × damageBoostLevel)
```

The projectile scale also grows +10% per `projectileSize` level, and each hit triggers the **life steal** passive (heals 10% of damage dealt per level, minimum 1 HP).

**Ricochet secondary fire:** every `ricochetInterval` seconds the turret launches `ricochetQuantity` bouncing projectiles that reflect off the camera viewport bounds (with ±10° random deflection, capped bounce count, and auto-cleanup after 15 s).

### Enemies (`Enemy.cs`)

- Chase the player directly, rotating the sprite to face its target.
- Deal **contact damage** on overlap with a per-enemy `damageInterval` cooldown.
- Each enemy defines `xpReward`, `contactDamage`, and `budgetCost` (used by the spawner's economy).
- On death: awards XP to `XpManager`, rolls an **8% chance to drop a random world item** at the death position, and destroys itself.

### Wave Director (`WaveController.cs` + `SpawnController.cs`)

Difficulty scales on two axes computed every frame:

```
pressure = floor(1 + minutesSurvived)
budget   = 4 × pressure
waveDelay = 1.2–2.0 s (first minute) → 0.9–1.5 s (until 3 min) → 0.6–1.2 s (after)
```

The spawner runs a **shopping algorithm**: while budget remains, it builds a list of enemy configs whose `unlockTimeSeconds` has passed and whose `budgetCost` fits the remaining budget, randomly picks one, spawns it at a random spawn point, and debits the cost — with a safety break so leftover budget can never soft-lock the loop.

Surviving the full `stageDuration` (300 s) triggers the **Victory** flow and disables the spawner.

### XP & Roguelite Level-Up (`XpManager.cs`, `LevelUpManager.cs`)

- Level-up requirement follows a curve: `nextLevelXP = 30 + 20×level + (3×level)^1.6`, with surplus XP carrying over.
- Each level grants **+5 max HP** and pauses the game to deal **3 unique cards** drawn without repetition from the passive database.
- Cards show category (Passive/Weapon), icon, description, and the next level number.

**Available passives (`PassiveManager.cs`):**

| Passive | Effect |
|---|---|
| Damage Boost | +10% projectile damage per level |
| Projectile Size | +10% projectile scale per level |
| Life Steal | Heal 10% of damage dealt per level (min 1) |
| Regeneration | Restore 1% max HP per second per level |
| HP Boost | +10% max HP per level (recomputed against base HP, heals the difference immediately) |

### Items & Inventory (`Item.cs`, `Inventory.cs`, `UI_Inventory.cs`)

- Pure C# inventory class with an `OnItemListChanged` C# event — the HUD subscribes and redraws itself, keeping logic and view decoupled.
- Stackable items merge amounts; clicking a slot uses the item and decrements the stack.

| Item | Effect |
|---|---|
| Repair Kit | Heals 20% of max HP |
| Gas | Refuels +50 gas (clamped to tank capacity) |
| Ammo | +20 ammo |
| Bomb | Kills every enemy currently in the scene |
| Coin | Bypasses inventory — goes straight to the wallet |

Coins are earned from pickups and mission victory (+50 bonus).

### Branching Dialogue & Cutscenes (`DialogueManager.cs`)

A **Twine-style node graph authored entirely in the Inspector**:

- Each `DialogueLine` has a `nodeId`, speaker name, portrait sprite, sentence, an optional per-line **CinemachineCamera**, and an array of `DialogueChoice` (button text + destination node id).
- Lines are indexed into a `Dictionary<string, DialogueLine>` at startup, so any passage can be jumped to by id — supporting linear scenes *and* branching conversations.
- When a line has choices, click-to-advance locks and runtime-generated buttons navigate the graph.
- Cutscenes freeze gameplay (`Time.timeScale = 0`), swap cameras per line, restore the player camera on exit, and can toggle staged actors (e.g., a fake horde prop).

### Scene Flow & Managers

- `MainMenu`: play (with delayed transition) and quit.
- `CityMenu`: start mission, open shop, return to menu.
- `GameOverManager`: 2-second beat after death, defeat screen, restart reloads the active scene with time restored.
- `VictoryManager`: stops combat music, plays the victory sting, awards coins, returns to the city hub.
- `SoundAssets`: singleton holding all game `AudioSource`s (weapons, hits, UI hover/click, stingers, and per-scene music tracks).
- Every manager guards against `Time.timeScale = 0` before reading input or updating timers.

---

## Quick Configuration Points

| System | Where to tune |
|---|---|
| Tank speed / reverse / resistance | `MainTank.accelerationSpeed`, `maxSpeed`, `maxReverseSpeed`, `terrainResistance` |
| Fuel economy | `MainTank.maxGas`, drain rate in `UseGas()` |
| Fire rate & ammo | `TurretSystem.shootCooldown`, starting ammo on the prefab |
| Base damage | `turretDamage` constants inside `TurretSystem.Shoot()` / `LaunchRicochets()` |
| Ricochet behavior | `ricochetInterval`, `ricochetQuantity`, `BounceProjectile.maxBounces` |
| Enemy stats & economy | `Enemy.xpReward`, `contactDamage`, `budgetCost` on each enemy prefab |
| Difficulty curve | `WaveController.stageDuration` and the delay table in `GetCurrentDelay()` |
| Enemy roster unlock times | `SpawnController.enemyConfigs[].unlockTimeSeconds` |
| Passive cards | `LevelUpManager.passiveDatabase` (icon, name, description) |
| Passive strength | Multipliers inside `PassiveManager.cs` and `TurretSystem.cs` |
| Item effects | `Item.UseItem()` switch statement |
| Dialogue script | `DialogueManager.lines` array (node ids, portraits, cameras, choices) |

## Development Notes

- **OOP structure:** `Character` is an abstract base class; `MainTank` and `Enemy` override `TakeDamage`/`Die`, so all combat flows through one polymorphic damage pipeline.
- **Event-driven UI:** the inventory never touches the HUD directly — it raises a C# event and `UI_Inventory` refreshes itself.
- **Designer-friendly tuning:** most balance values live in Inspector fields with `[Header]` attributes; enemy waves, passives, items, and dialogue require no code changes to rebalance.
- Singletons (`XpManager`, `SoundAssets`, `ItemAssets`) provide cross-scene service access with duplicate destruction in `Awake`.
- Code comments are written in Portuguese (pt-BR) as learning documentation.

## About

Project I.R.O.N is a 2D arcade tank-survival game developed in Unity as a portfolio project covering the full production loop: player feel (physics-based driving), combat systems, roguelite progression, an economic wave director, item/inventory architecture, branching narrative cutscenes, and complete menu/hub/shop scene flow.

---
*Built with Unity 6 · URP 2D · C#*
