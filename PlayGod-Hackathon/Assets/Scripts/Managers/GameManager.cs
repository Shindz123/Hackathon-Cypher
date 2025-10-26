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
            
            // Validate setup
            if (puzzleDatabase == null)
            {
                Debug.LogError("GameManager: PuzzleDatabase is not assigned! Please assign it in the Inspector.");
            }
            
            LoadProgress();
            
            Debug.Log("GameManager initialized successfully.");
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
            SceneManager.LoadScene("PuzzleScene");
        }
        
        public void LoadMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }
        
        public void LoadResultsScene()
        {
            SceneManager.LoadScene("ResultsScene");
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
        }
    }
}

