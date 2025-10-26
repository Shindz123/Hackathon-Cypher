using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Lexicon.Editor
{
    public class BuildSettingsChecker : EditorWindow
    {
        [MenuItem("Lexicon/Check Build Settings")]
        public static void CheckBuildSettings()
        {
            Debug.Log("=== BUILD SETTINGS CHECK ===");
            
            // Get current scenes in build
            var scenes = EditorBuildSettings.scenes;
            
            if (scenes.Length == 0)
            {
                Debug.LogError("❌ NO SCENES in Build Settings!");
                Debug.LogError("   FIX: File → Build Settings → Add Open Scenes");
                Debug.LogError("   You need to add your PuzzleScene to Build Settings!");
                return;
            }
            
            Debug.Log($"Found {scenes.Length} scene(s) in Build Settings:");
            
            bool hasCurrentScene = false;
            string currentSceneName = EditorSceneManager.GetActiveScene().name;
            
            for (int i = 0; i < scenes.Length; i++)
            {
                string scenePath = scenes[i].path;
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                bool enabled = scenes[i].enabled;
                
                string status = enabled ? "✅" : "❌ DISABLED";
                Debug.Log($"  {i}. {status} {sceneName} ({scenePath})");
                
                if (sceneName == currentSceneName)
                    hasCurrentScene = true;
            }
            
            Debug.Log("=== CHECKING REQUIRED SCENES ===");
            
            // Check for PuzzleScene
            bool hasPuzzleScene = false;
            bool hasMainMenu = false;
            
            foreach (var scene in scenes)
            {
                string name = System.IO.Path.GetFileNameWithoutExtension(scene.path);
                if (name == "PuzzleScene") hasPuzzleScene = true;
                if (name == "MainMenu") hasMainMenu = true;
            }
            
            if (!hasPuzzleScene)
            {
                Debug.LogError("❌ CRITICAL: 'PuzzleScene' not found in Build Settings!");
                Debug.LogError("   FIX: Open your puzzle scene → File → Build Settings → Add Open Scenes");
            }
            else
            {
                Debug.Log("✅ PuzzleScene found in Build Settings");
            }
            
            if (!hasMainMenu)
            {
                Debug.LogWarning("⚠️  'MainMenu' scene not found (optional but recommended)");
            }
            else
            {
                Debug.Log("✅ MainMenu found in Build Settings");
            }
            
            Debug.Log("================================");
            
            if (hasPuzzleScene)
            {
                Debug.Log("✅ Build Settings look good! Scene transitions should work.");
            }
            else
            {
                Debug.LogError("❌ Please add PuzzleScene to Build Settings before testing level transitions!");
            }
            
            Debug.Log("================================");
        }
        
        [MenuItem("Lexicon/Add Current Scene to Build")]
        public static void AddCurrentSceneToBuild()
        {
            var currentScene = EditorSceneManager.GetActiveScene();
            
            if (!currentScene.IsValid())
            {
                Debug.LogError("No valid scene is currently open!");
                return;
            }
            
            // Get current build scenes
            var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            
            // Check if already exists
            string scenePath = currentScene.path;
            bool exists = false;
            
            foreach (var scene in scenes)
            {
                if (scene.path == scenePath)
                {
                    exists = true;
                    break;
                }
            }
            
            if (exists)
            {
                Debug.LogWarning($"Scene '{currentScene.name}' is already in Build Settings");
            }
            else
            {
                // Add scene
                scenes.Add(new EditorBuildSettingsScene(scenePath, true));
                EditorBuildSettings.scenes = scenes.ToArray();
                
                Debug.Log($"✅ Added '{currentScene.name}' to Build Settings at index {scenes.Count - 1}");
            }
            
            // Show Build Settings window
            EditorWindow.GetWindow(System.Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));
        }
    }
}

