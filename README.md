# OneLastClick — Project Settings

A small lightweight Unity package that lets you store `ScriptableObject` settings in **Project Settings** instead of the `Assets` folder.

Settings appear in **Edit → Project Settings**, are serialized into the project's `ProjectSettings` directory, and can be accessed at runtime without creating or managing assets yourself.

## Installation

Add the package to your `manifest.json`:

```json
{
  "dependencies": {
    "io.onelastclick.projectsettings": "https://github.com/ceejayzsmith/io.onelastclick.projectsettings.git?path=/Assets/Package"
  }
}
```

---

## Quick Start

Add the attribute `[ProjectSettings]` to a ScriptableObject type.

```csharp
using OneLastClick.ProjectSettings;
using UnityEngine;

[ProjectSettings]
public class ExampleProjectSettings : ScriptableObject
{
    [SerializeField] public int IntValue;
    [SerializeField] public GameObject Prefab;
    [SerializeField] public string Message = "Hello, World!";
}
```

<img width="1340" height="512" alt="image" src="https://github.com/user-attachments/assets/40403f98-bd71-45c6-8ff2-086b20d0b290" />


The package will automatically:

- Create a Project Settings entry.
- Store the data in the project's `ProjectSettings` folder (not `Assets`).
- Display the object, using its normal inspector, in the Project Settings

You can edit it from **Edit → Project Settings → Example Project Settings**.

---

## Accessing Settings

### Runtime

For runtime settings, use `ProjectSettingsDatabase.Get<T>()`.

```csharp
var settings = ProjectSettingsDatabase.Get<ExampleProjectSettings>();

Debug.Log(settings.Message);
```

The package automatically injects a runtime copy of every non-editor-only settings object into the first loaded scene, so `Get<T>()` always returns a valid instance.

### Editor

#### Getting

In editor code, use `ProjectSettingsEditorDatabase.GetOrCreateDefault<T>()`.

```csharp
var settings = ProjectSettingsEditorDatabase.GetOrCreateDefault<ExampleProjectSettings>();
```

If the settings file does not exist yet, it is created automatically.

#### Saving/Modifying

```csharp
var settings = ProjectSettingsEditorDatabase.GetOrCreateDefault<ExampleProjectSettings>();

settings.IntValue = 42;
ProjectSettingsEditorDatabase.SaveProjectSetting(settings);
```

---

## Customizing the Settings Entry

`ProjectSettingsAttribute` accepts optional parameters for customizing where and how the settings appear.

```csharp
[ProjectSettings(
    fileName: "Example.asset",
    fileDirectory: "OneLastClick",
    settingsProviderName: "Example Settings",
    settingsProviderDirectory: "OneLastClick/Gameplay"
)]
public class ExampleProjectSettings : ScriptableObject
{
}
```
<img width="1330" height="456" alt="image" src="https://github.com/user-attachments/assets/d2e9a628-09c4-46a1-a28a-cf0a1e19bac2" />


### Attribute Parameters

| Parameter | Description |
|-----------|-------------|
| `fileName` | Overrides the serialized file name stored in `ProjectSettings`. |
| `fileDirectory` | Places the settings file inside a subdirectory within `ProjectSettings`. |
| `settingsProviderName` | Changes the display name shown in the Project Settings window. |
| `settingsProviderDirectory` | Places the settings page inside a nested Project Settings category. |
| `editorOnly` | Marks the settings as editor-only. They are not included at runtime. |

### Example

```csharp
[ProjectSettings(
    settingsProviderDirectory: "OneLastClick/Rendering",
    settingsProviderName: "Lighting Settings"
)]
public class LightingProjectSettings : ScriptableObject
{
    public Material DefaultMaterial;
}
```

This appears under:

> **Project Settings → OneLastClick → Rendering → Lighting Settings**

---

## Editor-Only Settings

Some settings are only needed by editor tools.

```csharp
[ProjectSettings(editorOnly: true)]
public class BuildProjectSettings : ScriptableObject
{
    public string BuildOutputDirectory;
    public bool EnableDevelopmentBuild;
}
```

These settings:

- Are available through `ProjectSettingsEditorDatabase`.
- Are shown in Project Settings.
- Are **not** injected into runtime builds.
- Cannot be retrieved with `ProjectSettingsDatabase.Get<T>()`.

---

## Where Are Settings Stored?

Settings are serialized into Unity's `ProjectSettings` folder rather than `Assets`.

Example project structure:

```text
ProjectSettings/
├── ExampleProjectSettings.asset
└── OneLastClick/
    └── Example.asset
```

---

## How It Works

1. Any `ScriptableObject` marked with `[ProjectSettings]` is discovered automatically.
2. A `SettingsProvider` is registered for it.
3. Unity renders the object using the standard inspector.
4. Changes are saved immediately back into the `ProjectSettings` file.
5. For non-editor-only settings, a runtime instance is made available through `ProjectSettingsDatabase.Get<T>()`.

---

## API Summary

| API | Description |
|-----|-------------|
| `ProjectSettingsDatabase.Get<T>()` | Returns the runtime instance of a project settings object. |
| `ProjectSettingsEditorDatabase.GetOrCreateDefault<T>()` | Loads or creates the editor settings asset. |
| `ProjectSettingsEditorDatabase.SaveProjectSetting()` | Persists changes made from editor code. |

---

## Notes

- Only classes deriving from `ScriptableObject` should be marked with `[ProjectSettings]`.
- The inspector supports any serializable Unity fields (`SerializeField`, `SerializeReference`, nested serializable types, object references, etc.).
- Runtime access is only available for settings that are **not** marked `editorOnly`.
