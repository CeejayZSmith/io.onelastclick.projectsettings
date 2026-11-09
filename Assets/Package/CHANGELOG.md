# Changelog

## [1.0.0] - 2026-05-30

### Added

- `[ProjectSettings]` attribute to mark a `ScriptableObject` as project settings. Supports optional file name, file directory, settings provider name, settings provider directory, and an `editorOnly` flag that excludes the settings from player builds.
- `ProjectSettingsDatabase.Get<T>()` and `Get(Type)` for reading project settings at runtime. Available in play mode only.
- `IProjectSettingsPreSaveProcessor` interface, invoked before settings are written to disk so a settings type can normalise or validate its own data.
- Settings entries in Edit → Project Settings, each rendering the settings object with its normal inspector.
- Settings persisted to the `ProjectSettings` folder instead of `Assets`.
- Build preprocessor that bundles non-editor-only settings and registers them at startup in player builds.
