using UnityEngine;
using UnityEditor;
using Lexicon.Data;
using System.Text;

namespace Lexicon.Editor
{
    public class SystemPromptGenerator : EditorWindow
    {
        private PuzzleDatabase database;
        private string generatedPrompt = "";
        private Vector2 scrollPosition;
        
        [MenuItem("Lexicon/Generate Master System Prompt")]
        public static void ShowWindow()
        {
            GetWindow<SystemPromptGenerator>("System Prompt Generator");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("Master System Prompt Generator", EditorStyles.boldLabel);
            GUILayout.Space(10);
            
            // Database selection
            database = (PuzzleDatabase)EditorGUILayout.ObjectField(
                "Puzzle Database", 
                database, 
                typeof(PuzzleDatabase), 
                false
            );
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("Generate Prompt with ALL Puzzles", GUILayout.Height(40)))
            {
                GenerateMasterPrompt();
            }
            
            GUILayout.Space(10);
            
            if (!string.IsNullOrEmpty(generatedPrompt))
            {
                GUILayout.Label("Generated Prompt (Copy this):", EditorStyles.boldLabel);
                
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(400));
                EditorGUILayout.TextArea(generatedPrompt, GUILayout.ExpandHeight(true));
                GUILayout.EndScrollView();
                
                GUILayout.Space(10);
                
                if (GUILayout.Button("Copy to Clipboard", GUILayout.Height(30)))
                {
                    EditorGUIUtility.systemCopyBuffer = generatedPrompt;
                    Debug.Log("✅ Prompt copied to clipboard! Paste it into your puzzle's System Prompt Template field.");
                }
                
                if (GUILayout.Button("Apply to All Puzzles", GUILayout.Height(30)))
                {
                    ApplyToAllPuzzles();
                }
            }
            
            GUILayout.Space(10);
            GUILayout.Label("Instructions:", EditorStyles.helpBox);
            GUILayout.Label("1. Select your PuzzleDatabase\n2. Click 'Generate Prompt'\n3. Copy the prompt\n4. Paste into each puzzle's 'System Prompt Template' field\n5. Customize as needed!", EditorStyles.wordWrappedLabel);
        }
        
        private void GenerateMasterPrompt()
        {
            if (database == null)
            {
                EditorUtility.DisplayDialog("Error", "Please select a PuzzleDatabase first!", "OK");
                return;
            }
            
            if (database.Puzzles == null || database.Puzzles.Count == 0)
            {
                EditorUtility.DisplayDialog("Error", "PuzzleDatabase has no puzzles!", "OK");
                return;
            }
            
            StringBuilder sb = new StringBuilder();
            
            // Header
            sb.AppendLine("You are Lexara, a mystical AI oracle who speaks only in riddles and poetic language.");
            sb.AppendLine();
            sb.AppendLine("You possess complete knowledge of ALL the riddles across multiple challenges.");
            sb.AppendLine();
            sb.AppendLine("=== COMPLETE WORD DICTIONARY (All Puzzles) ===");
            sb.AppendLine();
            
            // Add all puzzles
            for (int i = 0; i < database.Puzzles.Count; i++)
            {
                PuzzleData puzzle = database.Puzzles[i];
                if (puzzle == null) continue;
                
                sb.AppendLine($"--- Puzzle {i + 1}: {puzzle.PuzzleName} (Level {puzzle.DifficultyLevel}) ---");
                sb.AppendLine($"Riddle: \"{puzzle.RiddleSentence}\"");
                sb.AppendLine($"Answer: \"{puzzle.TargetTranslation}\"");
                sb.AppendLine("Mappings:");
                
                foreach (var mapping in puzzle.WordMappings)
                {
                    sb.AppendLine($"  '{mapping.riddleWord}' = '{mapping.actualMeaning}'");
                }
                
                sb.AppendLine();
            }
            
            sb.AppendLine("=== YOUR ROLE ===");
            sb.AppendLine();
            sb.AppendLine("IMPORTANT RULES:");
            sb.AppendLine("1. When answering questions, think about the ACTUAL meaning, not the riddle word");
            sb.AppendLine("2. Never directly reveal mappings - always respond poetically and cryptically");
            sb.AppendLine("3. Give helpful clues through metaphors and vivid descriptions");
            sb.AppendLine("4. Describe real-world properties of the ACTUAL meaning");
            sb.AppendLine("   Example: If 'leaf'=bird, describe that birds fly, have feathers, build nests");
            sb.AppendLine("5. Be encouraging and guide seekers to discover truth");
            sb.AppendLine("6. Keep responses concise (2-3 sentences maximum)");
            sb.AppendLine("7. Use vivid imagery and nature metaphors");
            sb.AppendLine();
            sb.AppendLine("ANSWER STYLE EXAMPLES:");
            sb.AppendLine();
            sb.AppendLine("Q: 'Does the leaf have wings?'");
            sb.AppendLine("A: 'Indeed, what you name leaf knows the freedom of flight, soaring high above earthly bounds with grace.'");
            sb.AppendLine();
            sb.AppendLine("Q: 'Can the sun be seen at night?'");
            sb.AppendLine("A: 'The celestial body I call sun reveals itself when darkness falls, a silver guardian waxing and waning.'");
            sb.AppendLine();
            sb.AppendLine("Q: 'What does sings mean?'");
            sb.AppendLine("A: 'The action named sings speaks of movement through air, a graceful dance on invisible currents.'");
            sb.AppendLine();
            sb.AppendLine("=== CUSTOMIZE BELOW THIS LINE ===");
            sb.AppendLine();
            sb.AppendLine("Add your own personality traits, special behaviors, or difficulty adjustments here.");
            sb.AppendLine("You can make the AI more helpful (easier) or more cryptic (harder).");
            
            generatedPrompt = sb.ToString();
            
            Debug.Log("✅ Master prompt generated! You can now customize it and paste into puzzle assets.");
        }
        
        private void ApplyToAllPuzzles()
        {
            if (string.IsNullOrEmpty(generatedPrompt))
            {
                EditorUtility.DisplayDialog("Error", "Generate a prompt first!", "OK");
                return;
            }
            
            if (!EditorUtility.DisplayDialog(
                "Apply to All Puzzles?",
                $"This will set the System Prompt Template for all {database.Puzzles.Count} puzzles.\n\nYou can customize each one afterwards.",
                "Apply", 
                "Cancel"))
            {
                return;
            }
            
            foreach (var puzzle in database.Puzzles)
            {
                if (puzzle == null) continue;
                
                SerializedObject so = new SerializedObject(puzzle);
                SerializedProperty prop = so.FindProperty("systemPromptTemplate");
                
                if (prop != null)
                {
                    prop.stringValue = generatedPrompt;
                    so.ApplyModifiedProperties();
                }
            }
            
            AssetDatabase.SaveAssets();
            Debug.Log($"✅ Applied master prompt to all {database.Puzzles.Count} puzzles!");
            EditorUtility.DisplayDialog("Success", "Prompt applied to all puzzles!\n\nYou can now customize each puzzle individually in the Inspector.", "OK");
        }
    }
}

