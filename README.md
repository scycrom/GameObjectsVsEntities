# GameObjects vs Entities

A Unity benchmark project that compares the **classic GameObject/MonoBehaviour** workflow against **Unity DOTS / ECS** under equivalent workloads. It lets you flip between backends and benchmark modules at runtime and watch FPS, CPU and GPU timings update side-by-side.

## Unity Version

- Unity **6000.5.0b5** (Unity 6.5 beta)

## What it benchmarks

The project runs two benchmark modules, each implemented twice (once with GameObjects, once with Entities):

| Module | Description |
| --- | --- |
| **Cubes** | A configurable grid of cubes animated with a radial ripple. The grid size can be scaled live to push the spawn count up or down. |
| **ComplexWorld** | A heavier scene built from the *PolygonStreetRacer* asset pack to stress more realistic content. |

A single `MainScene` hosts the HUD and a `SceneSwitcher` that additively loads the active benchmark scene:

- `EntitiesScene` / `GameObjectsScene` — Cubes module
- `ComplexWorldEntitiesScene` / `ComplexWorldGameObjectsScene` — ComplexWorld module

## Controls

| Input | Action |
| --- | --- |
| `Tab` | Toggle backend (ECS ↔ GameObject) |
| `M` | Toggle module (Cubes ↔ ComplexWorld) |
| `Numpad 2` / `Numpad 1` | Increment / decrement cube grid size |
| `Numpad 5` / `Numpad 4` | Double / halve cube grid size |

The on-screen HUD exposes the same actions as buttons and displays the current backend, module, spawn count, FPS, CPU ms and GPU ms.

## Project layout

```
Assets/
├── Scenes/                MainScene + the four benchmark scenes
├── Scripts/
│   ├── ECS/               DOTS authoring, components, systems, spawner
│   ├── GameObjects/       MonoBehaviour spawner and mover
│   ├── SceneSwitcher.cs   Additive scene swapping between backends/modules
│   ├── HudController.cs   UI Toolkit HUD + frame timing sampling
│   └── SpawnerCommon.cs   Shared grid layout, ripple phase, input
├── UI Toolkit/            HUD UXML/USS
└── PolygonStreetRacer/    (not included — see below)
```

## Missing assets

> **The `PolygonStreetRacer` asset pack is not included in this repository** due to its license. The two **ComplexWorld** scenes (`ComplexWorldEntitiesScene` and `ComplexWorldGameObjectsScene`) depend on it and will be missing references without it.
>
> The **Cubes** module (`EntitiesScene`, `GameObjectsScene`) works without any third-party assets.
>
> To run the ComplexWorld benchmark, purchase and import the *POLYGON Street Racer* pack from Synty Studios into `Assets/PolygonStreetRacer/`.

## Getting started

1. Clone the repo.
2. Open the project with Unity **6000.5.0b5** (or a matching 6.5 beta).
3. Open `Assets/Scenes/MainScene.unity` and press Play.
4. Use `Tab` / `M` to switch between backends and modules.