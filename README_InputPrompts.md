# Input Prompts

Shows on-screen icons for the buttons bound in the New Input System, and adapts
to the **last device used** (Keyboard/Mouse, Xbox, PlayStation; generic gamepad
fallback). New Input System only.

## Install (one click)

`Tools > Input Prompts > Installer` (or the **Input Prompts** toolbar button).

Fill in:
- **Sprite Folder**: root folder of your Kenney sprites. No need to split by
  brand; names are prefixed `xbox_` / `playstation_` / `keyboard_` / `mouse_`.
- **Input Actions**: your `.inputactions` asset.
- **Legend Prefab (UI_Canvas)**: your legend prefab.
- **SO Output Folder**: defaults to
  `Assets/_/Database/ScriptableObject/InputPrompts` (created if missing).

Click **Install**. It creates the ScriptableObjects, fills the icon sets,
generates the legend entries, wires everything, and drops the legend plus a
tracker into the open scene. The Console prints a matched/unmatched report and
the two TODO reminders below.

## Manual steps after install

1. **Composite glyph**: in `IconSet_Keyboard` > Composites, add
   `2DVector` -> your WASD cluster sprite. The installer cannot guess this one.
2. **Prune the PromptActionList**: keep only the verbs you want in the corner.
   Remove `Move`, `Look`, `Next`, `Previous`, `Point`, `Navigate`,
   `ScrollWheel`, `TrackedDevice*`, etc.

## Scene notes

- **EventSystem**: if your `UI_Canvas` prefab contains an EventSystem, delete it.
  The scene already has one; two trigger a "Multiple EventSystems" warning.
- **Canvas Sort Order**: the legend uses its own Canvas. If it renders behind
  other UI, raise that Canvas' **Sort Order**.

## Contextual prompt (world-space)

Besides the persistent legend, you can show a contextual prompt in the world
(e.g. "A : Grab" above an NPC or a door). It reuses the same resolver and device
channel, and re-resolves live when the device changes while shown.

1. Use the `InputPrompts_Canvas` legend for the corner UI, and a world-space
   prompt prefab (`InteractionPrompt_World_2D` / `InteractionPrompt_World_3D` in
   the sample) for contextual prompts. Each prompt prefab is a World Space Canvas
   carrying an `InteractionPrompt` + an `InputLegendRow`.
2. Drive it from your own proximity logic: call `Show()` when the player can
   interact and `Hide()` when they leave. `Configure(action, label)` lets one
   prompt serve several interactables.
3. The sample includes `InteractionTrigger2D` / `InteractionTrigger3D`: drop one
   on the interactable with a trigger collider, assign the prompt, set the player
   tag. The player needs a (non-kinematic, or moving) Rigidbody for trigger events.
4. In 3D with a moving camera, add `FaceCamera` to the prompt canvas so it stays
   readable. Not needed in 2D or with a fixed camera.

Proximity detection is gameplay logic, so the triggers ship as samples, not in the
runtime. After importing the sample, re-assign the prompt's `Channel`/`Resolver`
(scene references are not serialized into prefabs).

## Notes

- Run the installer **once** on a clean scene. It is a bootstrap, not idempotent.
- Composites use **Option A**: one glyph per composite (no per-key display).
- Detection is event-driven (`InputSystem.onActionChange`), with an actuation
  threshold to ignore stick drift. No PlayerInput required.
- Sprites: Kenney input prompts (CC0).
- Future evolution (not implemented): "pressed" sprite feedback per key.
