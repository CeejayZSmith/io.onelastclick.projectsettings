# Changelog

## [1.1.0] - 2026-05-31

### Changed

- Settings are now registered once for the whole editor session instead of being registered when entering play mode and cleared when leaving play mode or quitting the editor. In the editor the registry is only rebuilt after a compilation, and a registered instance is refreshed when its settings are saved.
- Editor-only settings are now created or loaded into the editor database right after a compilation instead of on first use.
- Editor play mode registration now happens before the first scene loads, so settings are available to every `Awake` call rather than only after play mode has been entered.

### Fixed

- Editing settings while in editor play mode no longer leaves a stale instance registered.

## [1.0.0] - 2026-05-30

### Added

- `[ProjectSettings]` attribute to mark a `ScriptableObject` as project settings. Supports optional file name, file directory, settings provider name, settings provider directory, and an `editorOnly` flag that excludes the settings from player builds.
- `ProjectSettingsDatabase.Get<T>()` and `Get(Type)` for reading project settings at runtime. Available in play mode only.
- `IProjectSettingsPreSaveProcessor` interface, invoked before settings are written to disk so a settings type can normalise or validate its own data.
- Settings entries in Edit → Project Settings, each rendering the settings object with its normal inspector.
- Settings persisted to the `ProjectSettings` folder instead of `Assets`.
- Build preprocessor that bundles non-editor-only settings and registers them at startup in player builds.
