using UnityEngine;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Lexicon.Utils
{
    /// <summary>
    /// Fixes the Input System compatibility issue with EventSystem.
    /// This script replaces StandaloneInputModule with InputSystemUIInputModule when needed.
    /// </summary>
    public class InputSystemFixer : MonoBehaviour
    {
        #if UNITY_EDITOR
        [MenuItem("Lexicon/Fix Input System EventSystem")]
        public static void FixEventSystemInputModule()
        {
            // Find all EventSystem objects in the scene
            EventSystem[] eventSystems = FindObjectsByType<EventSystem>(FindObjectsSortMode.None);
            
            foreach (EventSystem es in eventSystems)
            {
                // Check if it has StandaloneInputModule
                StandaloneInputModule standaloneModule = es.GetComponent<StandaloneInputModule>();
                
                if (standaloneModule != null)
                {
                    Debug.Log($"Found StandaloneInputModule on {es.gameObject.name}");
                    
                    // Remove it
                    DestroyImmediate(standaloneModule);
                    
                    // Try to add InputSystemUIInputModule
                    try
                    {
                        var inputSystemModuleType = System.Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
                        
                        if (inputSystemModuleType != null)
                        {
                            es.gameObject.AddComponent(inputSystemModuleType);
                            Debug.Log($"Added InputSystemUIInputModule to {es.gameObject.name}");
                        }
                        else
                        {
                            Debug.LogWarning("InputSystemUIInputModule type not found. Make sure Input System package is installed.");
                        }
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"Failed to add InputSystemUIInputModule: {e.Message}");
                    }
                }
            }
            
            if (eventSystems.Length == 0)
            {
                Debug.Log("No EventSystem found in scene. Create one via GameObject > UI > Event System");
            }
            else
            {
                Debug.Log($"Processed {eventSystems.Length} EventSystem(s)");
            }
        }
        
        [MenuItem("Lexicon/Create New Input System EventSystem")]
        public static void CreateInputSystemEventSystem()
        {
            // Check if EventSystem already exists
            if (FindFirstObjectByType<EventSystem>() != null)
            {
                Debug.LogWarning("EventSystem already exists in scene!");
                return;
            }
            
            // Create new GameObject
            GameObject esObject = new GameObject("EventSystem");
            EventSystem es = esObject.AddComponent<EventSystem>();
            
            // Try to add InputSystemUIInputModule
            try
            {
                var inputSystemModuleType = System.Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
                
                if (inputSystemModuleType != null)
                {
                    esObject.AddComponent(inputSystemModuleType);
                    Debug.Log("Created new EventSystem with InputSystemUIInputModule");
                }
                else
                {
                    Debug.LogWarning("InputSystemUIInputModule type not found. Adding StandaloneInputModule as fallback.");
                    esObject.AddComponent<StandaloneInputModule>();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to add InputSystemUIInputModule: {e.Message}");
                esObject.AddComponent<StandaloneInputModule>();
            }
            
            Selection.activeGameObject = esObject;
        }
        #endif
    }
}

