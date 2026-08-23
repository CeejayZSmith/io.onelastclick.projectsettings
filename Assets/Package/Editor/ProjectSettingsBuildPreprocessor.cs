using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FinalClick.ProjectSettings.Editor
{
    public class ProjectSettingsBuildPreprocessor : IProcessSceneWithReport
    {
        public int callbackOrder => -1;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void EditorPlaymodeInitialize()
        {
            CreateBootRegisterer();
        }

        public void OnProcessScene(Scene scene, BuildReport report)
        {
            // EditorPlaymodeInitialize handles in editor, as it needs to be created before the awake of other objects.
            if(Application.isPlaying == true)
            {
                return;
            }
            
            // Only inject into the first scene boot scene, as then it will always be started first.
            if (SceneManager.GetSceneAt(0) != scene)
            {
                return;
            }

            CreateBootRegisterer();
            Debug.Log($"Injecting ProjectSettings into {scene.name}");
        }

        private static void CreateBootRegisterer()
        {
            var gameObject = new GameObject("ProjectSettingsRegisterer");
            var registerer = gameObject.AddComponent<ProjectSettingsRegisterer>();
            registerer.UpdateReferences();
        }
    }
}