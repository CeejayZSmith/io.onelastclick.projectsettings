#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace OneLastClick.ProjectSettings
{
    internal partial class BuiltProjectSettingsContainer
    {
        public static void CreateAtAssetPath(ScriptableObject[] editorReferences, string assetPath)
        {
            BuiltProjectSettingsContainer container = CreateInstance<BuiltProjectSettingsContainer>();
            container._settings = new ScriptableObject[editorReferences.Length];

            AssetDatabase.CreateAsset(container, assetPath);

            for (int i = 0; i < editorReferences.Length; i++)
            {
                ScriptableObject source = editorReferences[i];

                if (source == null)
                {
                    continue;
                }

                ScriptableObject copy = Instantiate(source);
                copy.name = source.name;
                container._settings[i] = copy;
                AssetDatabase.AddObjectToAsset(copy, container);
            }

            EditorUtility.SetDirty(container);
            AssetDatabase.SaveAssetIfDirty(container);
            AssetDatabase.ImportAsset(assetPath);
        }
    }
}
#endif
