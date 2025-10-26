using UnityEngine;
using UnityEngine.Events;
using Lexicon.Data;
using Lexicon.Managers;

namespace Lexicon.Systems
{
    public class ProgressionSystem : MonoBehaviour
    {
        [Header("Events")]
        public UnityEvent<int> OnPuzzleUnlocked;
        public UnityEvent OnAllPuzzlesCompleted;
        public UnityEvent<string> OnAchievementUnlocked;
        
        [Header("Achievement Thresholds")]
        [SerializeField] private int perfectScoreThreshold = 1500;
        [SerializeField] private int speedRunQuestionsMax = 5;
        
        private void Start()
        {
            CheckAchievements();
        }
        
        public void UnlockNextPuzzle(int currentPuzzleIndex)
        {
            int nextIndex = currentPuzzleIndex + 1;
            int unlockedPuzzles = PlayerPrefs.GetInt("UnlockedPuzzles", 1);
            
            if (nextIndex >= unlockedPuzzles)
            {
                PlayerPrefs.SetInt("UnlockedPuzzles", nextIndex + 1);
                PlayerPrefs.Save();
                
                OnPuzzleUnlocked?.Invoke(nextIndex);
                
                // Check if all puzzles completed
                if (GameManager.Instance.Database != null &&
                    nextIndex >= GameManager.Instance.Database.TotalPuzzles)
                {
                    OnAllPuzzlesCompleted?.Invoke();
                    UnlockAchievement("Riddle Master");
                }
            }
        }
        
        public void CheckPuzzleCompletion(int correctMappings, int totalMappings, int questionsAsked, int score)
        {
            // Perfect puzzle achievement
            if (correctMappings == totalMappings)
            {
                IncrementStat("PerfectPuzzles");
                
                if (GetStat("PerfectPuzzles") == 1)
                {
                    UnlockAchievement("First Perfect");
                }
                
                if (GetStat("PerfectPuzzles") >= 5)
                {
                    UnlockAchievement("Perfectionist");
                }
            }
            
            // Speed run achievement
            if (questionsAsked <= speedRunQuestionsMax && correctMappings == totalMappings)
            {
                UnlockAchievement("Speed Reader");
            }
            
            // High score achievement
            if (score >= perfectScoreThreshold)
            {
                UnlockAchievement("Score Master");
            }
            
            // Track total questions asked
            IncrementStat("TotalQuestions", questionsAsked);
            
            // Track total puzzles completed
            IncrementStat("PuzzlesCompleted");
        }
        
        public void UnlockAchievement(string achievementName)
        {
            string key = $"Achievement_{achievementName}";
            
            if (!PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.SetInt(key, 1);
                PlayerPrefs.Save();
                
                OnAchievementUnlocked?.Invoke(achievementName);
                Debug.Log($"Achievement Unlocked: {achievementName}!");
            }
        }
        
        public bool HasAchievement(string achievementName)
        {
            return PlayerPrefs.GetInt($"Achievement_{achievementName}", 0) == 1;
        }
        
        private void IncrementStat(string statName, int amount = 1)
        {
            int current = PlayerPrefs.GetInt($"Stat_{statName}", 0);
            PlayerPrefs.SetInt($"Stat_{statName}", current + amount);
            PlayerPrefs.Save();
        }
        
        private int GetStat(string statName)
        {
            return PlayerPrefs.GetInt($"Stat_{statName}", 0);
        }
        
        public float GetOverallProgress()
        {
            if (GameManager.Instance.Database == null)
                return 0f;
            
            int unlockedPuzzles = PlayerPrefs.GetInt("UnlockedPuzzles", 1);
            int totalPuzzles = GameManager.Instance.Database.TotalPuzzles;
            
            // Unlocked puzzles includes the next one to play, so subtract 1 for completed
            int completedPuzzles = Mathf.Max(0, unlockedPuzzles - 1);
            
            return (float)completedPuzzles / totalPuzzles;
        }
        
        public string GetProgressDescription()
        {
            float progress = GetOverallProgress() * 100f;
            int completed = Mathf.FloorToInt(progress / 100f * GameManager.Instance.Database.TotalPuzzles);
            int total = GameManager.Instance.Database.TotalPuzzles;
            
            return $"{completed}/{total} Puzzles Completed ({progress:F0}%)";
        }
        
        private void CheckAchievements()
        {
            // Check for any milestone achievements based on current stats
            int totalQuestions = GetStat("TotalQuestions");
            
            if (totalQuestions >= 100)
            {
                UnlockAchievement("Curious Mind");
            }
            
            if (totalQuestions >= 500)
            {
                UnlockAchievement("Questioner Extraordinaire");
            }
        }
    }
}

