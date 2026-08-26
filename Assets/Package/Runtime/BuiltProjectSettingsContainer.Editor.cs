#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace OneFinalClick.ProjectSettings
{
    internal partial class BuiltProjectSettingsContainer
    {
        public static void CreateAtAssetPath(ScriptableObject[] editorReferences, string assetPath)
        {
            var container = CreateInstance<BuiltProjectSettingsContainer>();
            container._settings = new ScriptableObject[editorReferences.Length];

            AssetDatabase.CreateAsset(container, assetPath);
            
            for (int i = 0; i < editorReferences.Length; i++)
            {
                var source = editorReferences[i];

                if (source == null)
                {
                    continue;
                }

                var copy = Instantiate(source);
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