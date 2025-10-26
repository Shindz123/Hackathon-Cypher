using UnityEngine;
using UnityEditor;
using Lexicon.Data;
using System.IO;

namespace Lexicon.Editor
{
    public class PuzzleDataCreator : EditorWindow
    {
        [MenuItem("Lexicon/Create Sample Puzzles")]
        public static void CreateSamplePuzzles()
        {
            string puzzlesPath = "Assets/Data/Puzzles";
            
            // Create directories if they don't exist
            if (!Directory.Exists("Assets/Data"))
                AssetDatabase.CreateFolder("Assets", "Data");
            
            if (!Directory.Exists(puzzlesPath))
                AssetDatabase.CreateFolder("Assets/Data", "Puzzles");
            
            // Create 5 sample puzzles
            CreatePuzzle1(puzzlesPath);
            CreatePuzzle2(puzzlesPath);
            CreatePuzzle3(puzzlesPath);
            CreatePuzzle4(puzzlesPath);
            CreatePuzzle5(puzzlesPath);
            
            // Create puzzle database
            CreatePuzzleDatabase(puzzlesPath);
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log("Sample puzzles created successfully!");
        }
        
        private static void CreatePuzzle1(string path)
        {
            PuzzleData puzzle = ScriptableObject.CreateInstance<PuzzleData>();
            
            // Use reflection to set private fields
            var type = typeof(PuzzleData);
            type.GetField("puzzleName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(puzzle, "The Dawn Song");
            type.GetField("difficultyLevel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(puzzle, 1);
            type.GetField("riddleSentence", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(puzzle, "The leaf sings when the sun sleeps.");
            type.GetField("targetTranslation", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(puzzle, "The bird flies when the moon rises.");
            type.GetField("maxQuestionsForPerfect", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(puzzle, 5);
            type.GetField("maxQuestionsForGood", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(puzzle, 10);
            type.GetField("maxQuestionsForPass", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(puzzle, 15);
            
            var mappings = new System.Collections.Generic.List<WordMapping>
            {
                new WordMapping { riddleWord = "leaf", actualMeaning = "bird", hint = "It has wings but grows not on trees" },
                new WordMapping { riddleWord = "sings", actualMeaning = "flies", hint = "It moves through the air" },
                new WordMapping { riddleWord = "sun", actualMeaning = "moon", hint = "It lights the night sky" },
                new WordMapping { riddleWord = "sleeps", actualMeaning = "rises", hint = "When darkness comes, it awakens" }
            };
            
            type.GetField("wordMappings", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(puzzle, mappings);
            
            AssetDatabase.CreateAsset(puzzle, $"{path}/Puzzle01_DawnSong.asset");
        }
        
        private static void CreatePuzzle2(string path)
        {
            PuzzleData puzzle = ScriptableObject.CreateInstance<PuzzleData>();
            
            var type = typeof(PuzzleData);
            type.GetField("puzzleName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(puzzle, "The Silent Ocean");
            type.GetField("difficultyLevel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(puzzle, 2);
            type.GetField("riddleSentence", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(puzzle, "The whisper dances through the stone garden where shadows drink.");
            type.GetField("targetTranslation", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(puzzle, "The wind flows through the desert where cacti grow.");
            type.GetField("maxQuestionsForPerfect", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(puzzle, 6);
            type.GetField("maxQuestionsForGood", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(puzzle, 12);
            type.GetField("maxQuestionsForPass", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(puzzle, 18);
            
            var mappings = new System.Collections.Generic.List<WordMapping>
            {
                new WordMapping { riddleWord = "whisper", actualMeaning = "wind", hint = "An invisible force of nature" },
                new WordMapping { riddleWord = "dances", actualMeaning = "flows", hint = "Moves smoothly and continuously" },
                new WordMapping { riddleWord = "stone", actualMeaning = "desert", hint = "A vast, dry landscape" },
                new WordMapping { riddleWord = "garden", actualMeaning = "where", hint = "A connecting word, a place indicator" },
                new WordMapping { riddleWord = "shadows", actualMeaning = "cacti", hint = "Desert plants with spines" },
                new WordMapping { riddleWord = "drink", actualMeaning = "grow", hint = "What plants do" }
            };
            
            type.GetField("wordMappings", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(puzzle, mappings);
            
            AssetDatabase.CreateAsset(puzzle, $"{path}/Puzzle02_SilentOcean.asset");
        }
        
        private static void CreatePuzzle3(string path)
        {
            PuzzleData puzzle = ScriptableObject.CreateInstance<PuzzleData>();
            
            var type = typeof(PuzzleData);
            type.GetField("puzzleName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(puzzle, "The Frozen Flame");
            type.GetField("difficultyLevel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(puzzle, 3);
            type.GetField("riddleSentence", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(puzzle, "The ancient tower weeps silver tears beneath the emerald veil.");
            type.GetField("targetTranslation", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(puzzle, "The old mountain sheds white snow beneath the blue sky.");
            type.GetField("maxQuestionsForPerfect", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(puzzle, 7);
            type.GetField("maxQuestionsForGood", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(puzzle, 14);
            type.GetField("maxQuestionsForPass", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(puzzle, 20);
            
            var mappings = new System.Collections.Generic.List<WordMapping>
            {
                new WordMapping { riddleWord = "ancient", actualMeaning = "old", hint = "A simpler word for aged" },
                new WordMapping { riddleWord = "tower", actualMeaning = "mountain", hint = "A natural tall formation" },
                new WordMapping { riddleWord = "weeps", actualMeaning = "sheds", hint = "Releases from itself" },
                new WordMapping { riddleWord = "silver", actualMeaning = "white", hint = "A color, not a metal" },
                new WordMapping { riddleWord = "tears", actualMeaning = "snow", hint = "Frozen water from the sky" },
                new WordMapping { riddleWord = "emerald", actualMeaning = "blue", hint = "The color of the sky" },
                new WordMapping { riddleWord = "veil", actualMeaning = "sky", hint = "What covers everything above" }
            };
            
            type.GetField("wordMappings", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(puzzle, mappings);
            
            AssetDatabase.CreateAsset(puzzle, $"{path}/Puzzle03_FrozenFlame.asset");
        }
        
        private static void CreatePuzzle4(string path)
        {
            PuzzleData puzzle = ScriptableObject.CreateInstance<PuzzleData>();
            
            var type = typeof(PuzzleData);
            type.GetField("puzzleName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(puzzle, "The Eternal Riddle");
            type.GetField("difficultyLevel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(puzzle, 4);
            type.GetField("riddleSentence", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(puzzle, "When the guardian's crimson eye closes, the wanderers seek shelter in crystal caves.");
            type.GetField("targetTranslation", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(puzzle, "When the lighthouse's red light turns off, the sailors seek safety in ice harbors.");
            type.GetField("maxQuestionsForPerfect", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(puzzle, 8);
            type.GetField("maxQuestionsForGood", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(puzzle, 16);
            type.GetField("maxQuestionsForPass", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(puzzle, 22);
            
            var mappings = new System.Collections.Generic.List<WordMapping>
            {
                new WordMapping { riddleWord = "guardian", actualMeaning = "lighthouse", hint = "A tower that guides ships" },
                new WordMapping { riddleWord = "crimson", actualMeaning = "red", hint = "A deep red color" },
                new WordMapping { riddleWord = "eye", actualMeaning = "light", hint = "What a lighthouse emits" },
                new WordMapping { riddleWord = "closes", actualMeaning = "turns off", hint = "Stops shining" },
                new WordMapping { riddleWord = "wanderers", actualMeaning = "sailors", hint = "People who travel the seas" },
                new WordMapping { riddleWord = "shelter", actualMeaning = "safety", hint = "Protection from danger" },
                new WordMapping { riddleWord = "crystal", actualMeaning = "ice", hint = "Frozen water" },
                new WordMapping { riddleWord = "caves", actualMeaning = "harbors", hint = "Safe places for ships" }
            };
            
            type.GetField("wordMappings", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(puzzle, mappings);
            
            AssetDatabase.CreateAsset(puzzle, $"{path}/Puzzle04_EternalRiddle.asset");
        }
        
        private static void CreatePuzzle5(string path)
        {
            PuzzleData puzzle = ScriptableObject.CreateInstance<PuzzleData>();
            
            var type = typeof(PuzzleData);
            type.GetField("puzzleName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(puzzle, "The Cosmic Paradox");
            type.GetField("difficultyLevel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(puzzle, 5);
            type.GetField("riddleSentence", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(puzzle, "The scholar's quill inscribes forgotten symphonies upon the breathing canvas while the eternal flame devours memories.");
            type.GetField("targetTranslation", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(puzzle, "The artist's brush paints silent music upon the living paper while the endless fire consumes thoughts.");
            type.GetField("maxQuestionsForPerfect", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(puzzle, 10);
            type.GetField("maxQuestionsForGood", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(puzzle, 18);
            type.GetField("maxQuestionsForPass", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(puzzle, 25);
            
            var mappings = new System.Collections.Generic.List<WordMapping>
            {
                new WordMapping { riddleWord = "scholar", actualMeaning = "artist", hint = "A creator of visual works" },
                new WordMapping { riddleWord = "quill", actualMeaning = "brush", hint = "A painting tool" },
                new WordMapping { riddleWord = "inscribes", actualMeaning = "paints", hint = "Creates visual art" },
                new WordMapping { riddleWord = "forgotten", actualMeaning = "silent", hint = "Without sound" },
                new WordMapping { riddleWord = "symphonies", actualMeaning = "music", hint = "Harmonious sounds" },
                new WordMapping { riddleWord = "breathing", actualMeaning = "living", hint = "Alive" },
                new WordMapping { riddleWord = "canvas", actualMeaning = "paper", hint = "Surface for art" },
                new WordMapping { riddleWord = "eternal", actualMeaning = "endless", hint = "Never-ending" },
                new WordMapping { riddleWord = "flame", actualMeaning = "fire", hint = "Burning element" },
                new WordMapping { riddleWord = "devours", actualMeaning = "consumes", hint = "Eats up" },
                new WordMapping { riddleWord = "memories", actualMeaning = "thoughts", hint = "Mental processes" }
            };
            
            type.GetField("wordMappings", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(puzzle, mappings);
            
            AssetDatabase.CreateAsset(puzzle, $"{path}/Puzzle05_CosmicParadox.asset");
        }
        
        private static void CreatePuzzleDatabase(string path)
        {
            PuzzleDatabase database = ScriptableObject.CreateInstance<PuzzleDatabase>();
            
            // Load all puzzles
            var puzzle1 = AssetDatabase.LoadAssetAtPath<PuzzleData>($"{path}/Puzzle01_DawnSong.asset");
            var puzzle2 = AssetDatabase.LoadAssetAtPath<PuzzleData>($"{path}/Puzzle02_SilentOcean.asset");
            var puzzle3 = AssetDatabase.LoadAssetAtPath<PuzzleData>($"{path}/Puzzle03_FrozenFlame.asset");
            var puzzle4 = AssetDatabase.LoadAssetAtPath<PuzzleData>($"{path}/Puzzle04_EternalRiddle.asset");
            var puzzle5 = AssetDatabase.LoadAssetAtPath<PuzzleData>($"{path}/Puzzle05_CosmicParadox.asset");
            
            var type = typeof(PuzzleDatabase);
            var puzzlesList = new System.Collections.Generic.List<PuzzleData> { puzzle1, puzzle2, puzzle3, puzzle4, puzzle5 };
            type.GetField("puzzles", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(database, puzzlesList);
            
            AssetDatabase.CreateAsset(database, "Assets/Data/PuzzleDatabase.asset");
        }
    }
}

