using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using Lexicon.UI;

namespace Lexicon.Editor
{
    public class UIPrefabCreator : EditorWindow
    {
        [MenuItem("Lexicon/Create UI Prefabs")]
        public static void ShowWindow()
        {
            GetWindow<UIPrefabCreator>("Lexicon UI Prefab Creator");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("Create Lexicon UI Prefabs", EditorStyles.boldLabel);
            GUILayout.Space(10);
            
            if (GUILayout.Button("Create Word Mapping Entry Prefab", GUILayout.Height(30)))
            {
                CreateWordMappingEntryPrefab();
            }
            
            GUILayout.Space(5);
            
            if (GUILayout.Button("Create Translation Word Input Prefab", GUILayout.Height(30)))
            {
                CreateTranslationWordInputPrefab();
            }
            
            GUILayout.Space(5);
            
            if (GUILayout.Button("Create Puzzle Select Button Prefab", GUILayout.Height(30)))
            {
                CreatePuzzleSelectButtonPrefab();
            }
            
            GUILayout.Space(20);
            GUILayout.Label("All prefabs will be created in Assets/Prefabs/UI/", EditorStyles.helpBox);
        }
        
        private static void CreateWordMappingEntryPrefab()
        {
            // Create directory if needed
            CreateDirectories();
            
            // Create root object
            GameObject root = new GameObject("WordMappingEntry");
            RectTransform rootRect = root.AddComponent<RectTransform>();
            rootRect.sizeDelta = new Vector2(300, 40);
            
            // Add background
            GameObject bg = new GameObject("Background");
            bg.transform.SetParent(root.transform);
            Image bgImage = bg.AddComponent<Image>();
            bgImage.color = new Color(0.8f, 0.8f, 0.8f, 0.8f);
            RectTransform bgRect = bg.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            
            // Add riddle word text
            GameObject riddleText = CreateTextObject("RiddleWordText", root.transform, new Vector2(-100, 0), "leaf");
            riddleText.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.MidlineLeft;
            
            // Add arrow
            GameObject arrow = CreateTextObject("Arrow", root.transform, new Vector2(0, 0), "→");
            
            // Add actual meaning text
            GameObject meaningText = CreateTextObject("ActualMeaningText", root.transform, new Vector2(100, 0), "bird");
            meaningText.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.MidlineRight;
            
            // Add delete button
            GameObject deleteBtn = new GameObject("DeleteButton");
            deleteBtn.transform.SetParent(root.transform);
            Button btn = deleteBtn.AddComponent<Button>();
            Image btnImage = deleteBtn.AddComponent<Image>();
            btnImage.color = new Color(1f, 0.5f, 0.5f);
            RectTransform btnRect = deleteBtn.GetComponent<RectTransform>();
            btnRect.anchorMin = new Vector2(1, 0.5f);
            btnRect.anchorMax = new Vector2(1, 0.5f);
            btnRect.pivot = new Vector2(1, 0.5f);
            btnRect.anchoredPosition = new Vector2(-5, 0);
            btnRect.sizeDelta = new Vector2(30, 30);
            
            GameObject btnText = CreateTextObject("Text", deleteBtn.transform, Vector2.zero, "✕");
            
            // Add WordMappingEntry component
            WordMappingEntry entryComponent = root.AddComponent<WordMappingEntry>();
            
            // Use reflection to set serialized fields
            var type = typeof(WordMappingEntry);
            type.GetField("riddleWordText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(entryComponent, riddleText.GetComponent<TextMeshProUGUI>());
            type.GetField("actualMeaningText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(entryComponent, meaningText.GetComponent<TextMeshProUGUI>());
            type.GetField("backgroundImage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(entryComponent, bgImage);
            type.GetField("deleteButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(entryComponent, btn);
            
            // Save as prefab
            string path = "Assets/Prefabs/UI/WordMappingEntry.prefab";
            PrefabUtility.SaveAsPrefabAsset(root, path);
            DestroyImmediate(root);
            
            Debug.Log($"Created Word Mapping Entry prefab at {path}");
            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<GameObject>(path));
        }
        
        private static void CreateTranslationWordInputPrefab()
        {
            CreateDirectories();
            
            // Create root
            GameObject root = new GameObject("TranslationWordInput");
            RectTransform rootRect = root.AddComponent<RectTransform>();
            rootRect.sizeDelta = new Vector2(400, 50);
            
            // Add label
            GameObject label = CreateTextObject("Label", root.transform, new Vector2(-150, 0), "\"word\" means:");
            label.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.MidlineLeft;
            
            // Add input field
            GameObject input = new GameObject("InputField");
            input.transform.SetParent(root.transform);
            TMP_InputField inputField = input.AddComponent<TMP_InputField>();
            
            // Background for input
            Image inputBg = input.AddComponent<Image>();
            inputBg.color = Color.white;
            
            RectTransform inputRect = input.GetComponent<RectTransform>();
            inputRect.anchorMin = new Vector2(0.4f, 0);
            inputRect.anchorMax = new Vector2(1, 1);
            inputRect.sizeDelta = Vector2.zero;
            
            // Text area for input
            GameObject textArea = new GameObject("Text Area");
            textArea.transform.SetParent(input.transform);
            RectTransform textAreaRect = textArea.AddComponent<RectTransform>();
            textAreaRect.anchorMin = Vector2.zero;
            textAreaRect.anchorMax = Vector2.one;
            textAreaRect.sizeDelta = new Vector2(-10, -10);
            
            GameObject placeholder = CreateTextObject("Placeholder", textArea.transform, Vector2.zero, "Enter meaning...");
            placeholder.GetComponent<TextMeshProUGUI>().color = new Color(0.5f, 0.5f, 0.5f);
            
            GameObject text = CreateTextObject("Text", textArea.transform, Vector2.zero, "");
            
            inputField.textComponent = text.GetComponent<TextMeshProUGUI>();
            inputField.placeholder = placeholder.GetComponent<TextMeshProUGUI>();
            
            // Add TranslationWordInput component
            TranslationWordInput inputComponent = root.AddComponent<TranslationWordInput>();
            var type = typeof(TranslationWordInput);
            type.GetField("riddleWordLabel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(inputComponent, label.GetComponent<TextMeshProUGUI>());
            type.GetField("translationInput", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(inputComponent, inputField);
            
            // Save
            string path = "Assets/Prefabs/UI/TranslationWordInput.prefab";
            PrefabUtility.SaveAsPrefabAsset(root, path);
            DestroyImmediate(root);
            
            Debug.Log($"Created Translation Word Input prefab at {path}");
            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<GameObject>(path));
        }
        
        private static void CreatePuzzleSelectButtonPrefab()
        {
            CreateDirectories();
            
            // Create button
            GameObject root = new GameObject("PuzzleSelectButton");
            RectTransform rootRect = root.AddComponent<RectTransform>();
            rootRect.sizeDelta = new Vector2(250, 60);
            
            Button btn = root.AddComponent<Button>();
            Image btnImage = root.AddComponent<Image>();
            btnImage.color = new Color(0.7f, 0.7f, 0.9f);
            
            // Add text
            GameObject text = CreateTextObject("Text", root.transform, Vector2.zero, "Puzzle Name");
            text.GetComponent<TextMeshProUGUI>().fontSize = 18;
            text.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
            
            // Save
            string path = "Assets/Prefabs/UI/PuzzleSelectButton.prefab";
            PrefabUtility.SaveAsPrefabAsset(root, path);
            DestroyImmediate(root);
            
            Debug.Log($"Created Puzzle Select Button prefab at {path}");
            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<GameObject>(path));
        }
        
        private static GameObject CreateTextObject(string name, Transform parent, Vector2 position, string text)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent);
            
            TextMeshProUGUI tmp = obj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = 16;
            tmp.color = Color.black;
            tmp.alignment = TextAlignmentOptions.Center;
            
            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = new Vector2(100, 30);
            
            return obj;
        }
        
        private static void CreateDirectories()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
                AssetDatabase.CreateFolder("Assets", "Prefabs");
            
            if (!AssetDatabase.IsValidFolder("Assets/Prefabs/UI"))
                AssetDatabase.CreateFolder("Assets/Prefabs", "UI");
        }
    }
}

