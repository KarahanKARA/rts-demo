youtube.com/watch?v=KDW9y0abvKY&embeds_referring_euri=https%3A%2F%2Fkorathosgames.com%2F&source_ve_path=MjM4NTE

> A small-scale RTS (Real-Time Strategy) base-building and unit-control demo.

## 🧠 Project Details

- ✅ Built in **Unity 2021.3.45f1 LTS**
- 🎯 Core features:
  - Grid-based building placement with snapping
  - Unit production & combat
  - Responsive UI & info panels
  - Object pooling
  - LineRenderer-based drag selection
  - ScriptableObject architecture
  - Resolution-independent camera & UI
  - Custom spawn point system with visual feedback

---

## 🎮 Controls

| Action                  | Input                      |
|------------------------|----------------------------|
| Select a unit          | Left-click on unit         |
| Drag-select units      | Hold left-click & drag     |
| Move units             | Right-click on ground      |
| Attack target          | Right-click on enemy/building |
| Set spawn point        | Right-click with building selected |
| Destroy selection      | Click trash icon in info panel |

---

## 🏗️ Gameplay Summary

- Place buildings (Barracks, Power Plants, etc.) on the grid.
- Select a barracks to spawn units using the buttons.
- Units can be commanded to move or attack using right-click.
- Each building has health, size, and production rules.
- Buildings show a **flag** (valid) or **cross** (invalid) spawn point.
- Units automatically pathfind and retry attacks if blocked.

---

## 🔧 Systems Used

- **Object Pooling** → `ObjectPoolManager`
- **Grid Occupancy** → `GridManager`
- **A\* Pathfinding** → `Pathfinder.cs`
- **Event-based Input** → `ClickInputRouter`
- **LineRenderer Selection** → `UnitSelectionHandler`
- **Factory Pattern** → `UnitFactoryMB`, `BuildingFactoryMB`
- **Data-Driven Setup** → ScriptableObjects for Units & Buildings
- **Sprite Atlas** → Packed via Unity’s Atlas system
- **Responsive Camera** → Dynamically resizes based on resolution & layout
- **UI Panels** → TMP-based with dynamic info population

---

## 💬 Notes

- Custom spawn point is placed using right-click on ground with a **building selected**.
- Selection line is shown using a `LineRenderer`, not a UI element → fully resolution-independent.
- UI scales with `CanvasScaler` and adapts to both portrait and landscape.
- Health bars and info panels are updated in real-time using `Events`.

---
