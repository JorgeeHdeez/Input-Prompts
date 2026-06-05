# Changelog

All notable changes to this package are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this package adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

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
