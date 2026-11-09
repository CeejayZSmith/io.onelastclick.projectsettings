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

            AssetDatabase.CreateAsset(container, assetPath);

            container = AssetDatabase.LoadAssetAtPath<BuiltProjectSettingsContainer>(assetPath);

            if (container == null)
            {
                throw new System.InvalidOperationException($"Failed to create project settings container at '{assetPath}'.");
            }

            container._settings = new ScriptableObject[editorReferences.Length];

            for (int i = 0; i < editorReferences.Length; i++)
            {
                ScriptableObject source = editorReferences[i];

                if (source == null)
                {
                    continue;
                }

                ScriptableObject copy = Instantiate(source);
                copy.name = $"{source.name}_{i}";
                container._settings[i] = copy;
                AssetDatabase.AddObjectToAsset(copy, container);
            }

            EditorUtility.SetDirty(container);
            AssetDatabase.SaveAssets();
        }
    }
}
#endif
