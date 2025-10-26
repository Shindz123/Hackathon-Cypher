using UnityEngine;
using UnityEngine.SceneManagement;
using Lexicon.Data;

namespace Lexicon.Managers
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance;
        public static GameManager Instance => instance;
        
        [Header("References")]
        [SerializeField] private PuzzleDatabase puzzleDatabase;
        
        [Header("Development Settings")]
        [SerializeField] private bool alwaysStartAtLevel1InEditor = true;
        
        private int currentPuzzleIndex = 0;
        private int totalScore = 0;
        
        public PuzzleDatabase Database => puzzleDatabase;
        public int CurrentPuzzleIndex => currentPuzzleIndex;
        public int TotalScore => totalScore;
        
        private void Awake()
        {
            // Singleton pattern - only one GameManager can exist
            if (instance != null && instance != this)
            {
                Debug.LogWarning("Multiple GameManagers detected! Destroying duplicate.");
                Destroy(gameObject);
                return;
            }
            
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Try to auto-load PuzzleDatabase if not assigned
            if (puzzleDatabase == null)
            {
                Debug.LogWarning("GameManager: PuzzleDatabase not assigned, attempting to auto-load...");
                puzzleDatabase = Resources.Load<PuzzleDatabase>("PuzzleDatabase");
                
                if (puzzleDatabase == null)
                {
                    // Try alternate path
                    var databases = Resources.LoadAll<PuzzleDatabase>("");
                    if (databases != null && databases.Length > 0)
                    {
                        puzzleDatabase = databases[0];
                        Debug.Log($"✅ Auto-loaded PuzzleDatabase: {puzzleDatabase.name}");
                    }
                }
            }
            
            // Final validation
            if (puzzleDatabase == null)
            {
                Debug.LogError("❌ CRITICAL: GameManager has no PuzzleDatabase!");
                Debug.LogError("SOLUTION 1: Select GameManager → Drag PuzzleDatabase from Assets/Data/ to Inspector");
                Debug.LogError("SOLUTION 2: Move PuzzleDatabase.asset to Assets/Resources/ folder");
                Debug.LogError("SOLUTION 3: Use LexiconSceneManager component to auto-assign it");
            }
            else
            {
                Debug.Log($"✅ GameManager initialized with {puzzleDatabase.TotalPuzzles} puzzles");
            }
            
            LoadProgress();
            
            // Development feature: Always start at Level 1 when pressing Play in Unity Editor
            #if UNITY_EDITOR
            if (alwaysStartAtLevel1InEditor)
            {
                currentPuzzleIndex = 0;
                Debug.Log("🎮 [EDITOR MODE] Starting at Level 1 (alwaysStartAtLevel1InEditor is enabled)");
            }
            #endif
        }
        
        public void StartNewGame()
        {
            currentPuzzleIndex = 0;
            totalScore = 0;
            SaveProgress();
            LoadPuzzleScene();
        }
        
        public void ContinueGame()
        {
            LoadProgress();
            LoadPuzzleScene();
        }
        
        public void LoadPuzzleScene()
        {
            try
            {
                Debug.Log($"Loading PuzzleScene for puzzle index: {currentPuzzleIndex}");
                SceneManager.LoadScene("PuzzleScene");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load PuzzleScene! Make sure 'PuzzleScene' is added to Build Settings.\nError: {e.Message}");
            }
        }
        
        public void LoadMainMenu()
        {
            try
            {
                SceneManager.LoadScene("MainMenu");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load MainMenu! Error: {e.Message}");
            }
        }
        
        public void LoadResultsScene()
        {
            try
            {
                SceneManager.LoadScene("ResultsScene");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load ResultsScene! Error: {e.Message}");
            }
        }
        
        public PuzzleData GetCurrentPuzzle()
        {
            if (puzzleDatabase == null)
                return null;
            return puzzleDatabase.GetPuzzle(currentPuzzleIndex);
        }
        
        public void CompletePuzzle(int puzzleScore)
        {
            totalScore += puzzleScore;
            
            // Unlock next puzzle
            int unlockedPuzzles = PlayerPrefs.GetInt("UnlockedPuzzles", 1);
            if (currentPuzzleIndex + 1 >= unlockedPuzzles)
            {
                PlayerPrefs.SetInt("UnlockedPuzzles", currentPuzzleIndex + 2);
            }
            
            SaveProgress();
        }
        
        public void AdvanceToNextPuzzle()
        {
            if (puzzleDatabase != null && !puzzleDatabase.IsLastPuzzle(currentPuzzleIndex))
            {
                currentPuzzleIndex++;
                SaveProgress();
            }
        }
        
        public bool HasNextPuzzle()
        {
            if (puzzleDatabase == null)
                return false;
            return !puzzleDatabase.IsLastPuzzle(currentPuzzleIndex);
        }
        
        public bool IsPuzzleUnlocked(int index)
        {
            int unlockedPuzzles = PlayerPrefs.GetInt("UnlockedPuzzles", 1);
            return index < unlockedPuzzles;
        }
        
        public void SetCurrentPuzzle(int index)
        {
            if (IsPuzzleUnlocked(index))
            {
                currentPuzzleIndex = index;
                SaveProgress();
            }
        }
        
        public float GetCompletionPercentage()
        {
            if (puzzleDatabase == null || puzzleDatabase.TotalPuzzles == 0)
                return 0f;
            
            int unlockedPuzzles = PlayerPrefs.GetInt("UnlockedPuzzles", 1);
            // -1 because unlocked includes the next puzzle to play
            int completedPuzzles = Mathf.Max(0, unlockedPuzzles - 1);
            return (float)completedPuzzles / puzzleDatabase.TotalPuzzles * 100f;
        }
        
        private void SaveProgress()
        {
            PlayerPrefs.SetInt("CurrentPuzzleIndex", currentPuzzleIndex);
            PlayerPrefs.SetInt("TotalScore", totalScore);
            PlayerPrefs.Save();
        }
        
        private void LoadProgress()
        {
            currentPuzzleIndex = PlayerPrefs.GetInt("CurrentPuzzleIndex", 0);
            totalScore = PlayerPrefs.GetInt("TotalScore", 0);
            
            // Ensure at least first puzzle is unlocked
            if (!PlayerPrefs.HasKey("UnlockedPuzzles"))
            {
                PlayerPrefs.SetInt("UnlockedPuzzles", 1);
            }
        }
        
        public void ResetProgress()
        {
            currentPuzzleIndex = 0;
            totalScore = 0;
            PlayerPrefs.DeleteKey("CurrentPuzzleIndex");
            PlayerPrefs.DeleteKey("TotalScore");
            PlayerPrefs.DeleteKey("UnlockedPuzzles");
            PlayerPrefs.SetInt("UnlockedPuzzles", 1);
            PlayerPrefs.Save();
            
            Debug.Log("Progress reset - starting from Level 1");
        }
        
        public void ReplayFromLevel1()
        {
            ResetProgress();
            LoadPuzzleScene();
            Debug.Log("Replaying from Level 1!");
        }
    }
}

