# Changelog

All notable changes to this package are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this package adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.2.1] - 2026-06-07

### Fixed
- Invalid package manifest: replaced the leftover placeholders
  (name, author, URLs) so the package installs via Git/UPM.

## [1.2.0] - 2026-06-06

### Added
- Contextual world-space prompt (`InteractionPrompt`): shows an action's icon and
  label above an object, reusing the same resolver and device channel as the legend.
  Re-resolves live on device switch while shown.
- `FaceCamera` billboard helper for world-space prompts in 3D (injected camera, with
  a `Camera.main` fallback). Not needed in 2D or with a fixed camera.
- Sample interaction triggers (`InteractionTrigger2D`, `InteractionTrigger3D`) under
  Samples to drive a prompt from a proximity zone.

## [1.1.0] - 2026-06-06

### Added
- Curated `PromptActionList` generation (skips Move/Look/Point/Navigate/etc.).
- Installer is now safe to re-run: reuses existing ScriptableObjects instead of
  duplicating them, and skips scene setup if a legend/tracker already exists.
- TextMeshPro Essential Resources check with a clear console hint.

### Fixed
- Device flicker caused by stick drift (actuation threshold on detection).

## [1.0.0] - 2026-06-05

### Added
- Last-used device detection (`InputDeviceTracker`) via `InputSystem.onActionChange`,
  with an actuation threshold to ignore stick drift. No PlayerInput required.
- ScriptableObject event channel (`InputDeviceChangedChannel`) raised on device
  family change.
- Icon resolver (`InputIconResolver`) mapping a bound control to a sprite, per
  device family (Xbox, PlayStation, Keyboard/Mouse) with a generic gamepad fallback.
- Composite support (Option A): one glyph per composite (e.g. `2DVector` -> WASD
  cluster), with a text fallback when no sprite is set.
- Persistent corner legend UI (`InputLegendView` + `InputLegendRow`) driven by a
  data-only `PromptActionList`.
- Editor auto-fill: populate an icon set from a sprite folder using Kenney naming
  conventions; generate legend entries from an `.inputactions` asset.
- One-click installer (`Tools > Input Prompts > Installer` and a main toolbar
  button) that creates and wires every asset, then drops the legend and tracker
  into the open scene.

### Known limitations
- Composites display a fixed glyph; remapped keys / AZERTY are not reflected in
  the icon (the text fallback still is).
- No "pressed" sprite feedback (planned).