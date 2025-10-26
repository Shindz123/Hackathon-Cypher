using UnityEngine;
using Lexicon.Managers;
using Lexicon.Data;

namespace Lexicon.Utils
{
    /// <summary>
    /// Helper component to initialize the game properly.
    /// Attach this to a GameObject in your first scene (MainMenu or initial scene).
    /// </summary>
    public class GameInitializer : MonoBehaviour
    {
        [Header("Required References")]
        [SerializeField] private PuzzleDatabase puzzleDatabase;
        [SerializeField] private bool createGameManagerIfMissing = true;
        
        private void Awake()
        {
            InitializeGame();
        }
        
        private void InitializeGame()
        {
            // Check if GameManager already exists in scene
            GameManager existingGM = FindObjectOfType<GameManager>();
            
            if (existingGM != null)
            {
                Debug.Log("GameManager found in scene.");
                
                // If we have a database reference, try to help set it up
                if (puzzleDatabase != null && existingGM.Database == null)
                {
                    Debug.LogWarning("GameManager found but has no PuzzleDatabase. Please assign it manually in the Inspector.");
                }
            }
            else if (createGameManagerIfMissing)
            {
                Debug.LogWarning("No GameManager found in scene! Please create one manually:\n" +
                    "1. Create Empty GameObject named 'GameManager'\n" +
                    "2. Add 'GameManager' component\n" +
                    "3. Add 'PuzzleManager' component\n" +
                    "4. Assign PuzzleDatabase in Inspector");
            }
            
            // Initialize PlayerPrefs if needed
            if (!PlayerPrefs.HasKey("UnlockedPuzzles"))
            {
                PlayerPrefs.SetInt("UnlockedPuzzles", 1);
                PlayerPrefs.Save();
                Debug.Log("Initialized player progress");
            }
        }
        
        [ContextMenu("Reset All Progress")]
        public void ResetAllProgress()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetInt("UnlockedPuzzles", 1);
            PlayerPrefs.Save();
            Debug.Log("All progress reset!");
        }
        
        [ContextMenu("Unlock All Puzzles")]
        public void UnlockAllPuzzles()
        {
            if (puzzleDatabase != null)
            {
                PlayerPrefs.SetInt("UnlockedPuzzles", puzzleDatabase.TotalPuzzles + 1);
                PlayerPrefs.Save();
                Debug.Log($"All {puzzleDatabase.TotalPuzzles} puzzles unlocked!");
            }
        }
    }
}

