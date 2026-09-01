# Project Settings - [Full Documentation Here](https://docs.onelastclick.io/packages/project-settings/start-here/getting-started/)

A small lightweight Unity package that lets you store `ScriptableObject` settings in **Project Settings** instead of the `Assets` folder. Add the attribute `[ProjectSettings]` to a ScriptableObject type.

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

access within the editor is automatically avaible

<img width="1340" height="512" alt="image" src="https://github.com/user-attachments/assets/40403f98-bd71-45c6-8ff2-086b20d0b290" />


The package will automatically:

- Create a Project Settings entry.
- Store the data in the project's `ProjectSettings` folder (not `Assets`).
- Display the object, using its normal inspector, in the Project Settings

You can edit it from **Edit → Project Settings → Example Project Settings**.
