using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Lexicon.Data;

namespace Lexicon.Managers
{
    public class PuzzleManager : MonoBehaviour
    {
        [Header("Events")]
        public UnityEvent<int> OnQuestionAsked;
        public UnityEvent<int, int, int> OnPuzzleCompleted; // correctMappings, totalMappings, score
        public UnityEvent<string, string> OnMappingDiscovered; // riddleWord, actualMeaning
        
        private PuzzleData currentPuzzle;
        private int questionsAsked = 0;
        private Dictionary<string, string> playerMappings = new Dictionary<string, string>();
        private Dictionary<string, string> correctMappings = new Dictionary<string, string>();
        
        public PuzzleData CurrentPuzzle => currentPuzzle;
        public int QuestionsAsked => questionsAsked;
        public Dictionary<string, string> PlayerMappings => playerMappings;
        
        private void Start()
        {
            LoadPuzzle();
        }
        
        public void LoadPuzzle()
        {
            currentPuzzle = GameManager.Instance.GetCurrentPuzzle();
            
            if (currentPuzzle != null)
            {
                questionsAsked = 0;
                playerMappings.Clear();
                correctMappings = currentPuzzle.GetMappingsDictionary();
                
                Debug.Log($"Loaded puzzle: {currentPuzzle.PuzzleName}");
                Debug.Log($"Riddle: {currentPuzzle.RiddleSentence}");
            }
            else
            {
                Debug.LogError("Failed to load puzzle!");
            }
        }
        
        public void IncrementQuestionCount()
        {
            questionsAsked++;
            OnQuestionAsked?.Invoke(questionsAsked);
            
            Debug.Log($"Question #{questionsAsked} asked");
        }
        
        public void AddPlayerMapping(string riddleWord, string guessedMeaning)
        {
            string normalizedRiddle = riddleWord.ToLower().Trim();
            string normalizedGuess = guessedMeaning.ToLower().Trim();
            
            playerMappings[normalizedRiddle] = normalizedGuess;
            
            // Check if correct
            if (correctMappings.ContainsKey(normalizedRiddle) && 
                correctMappings[normalizedRiddle] == normalizedGuess)
            {
                OnMappingDiscovered?.Invoke(riddleWord, guessedMeaning);
                Debug.Log($"Correct mapping discovered: {riddleWord} = {guessedMeaning}");
            }
        }
        
        public bool IsMappingCorrect(string riddleWord, string guessedMeaning)
        {
            string normalizedRiddle = riddleWord.ToLower().Trim();
            string normalizedGuess = guessedMeaning.ToLower().Trim();
            
            return correctMappings.ContainsKey(normalizedRiddle) && 
                   correctMappings[normalizedRiddle] == normalizedGuess;
        }
        
        public int CountCorrectMappings()
        {
            int correct = 0;
            foreach (var mapping in playerMappings)
            {
                if (correctMappings.ContainsKey(mapping.Key) && 
                    correctMappings[mapping.Key] == mapping.Value)
                {
                    correct++;
                }
            }
            return correct;
        }
        
        public ValidationResult ValidateTranslation(Dictionary<string, string> submittedMappings)
        {
            ValidationResult result = new ValidationResult();
            result.totalMappings = correctMappings.Count;
            result.correctMappings = 0;
            result.incorrectMappings = new List<string>();
            result.missingMappings = new List<string>();
            
            // Check submitted mappings
            foreach (var mapping in submittedMappings)
            {
                string normalizedRiddle = mapping.Key.ToLower().Trim();
                string normalizedGuess = mapping.Value.ToLower().Trim();
                
                if (correctMappings.ContainsKey(normalizedRiddle))
                {
                    if (correctMappings[normalizedRiddle] == normalizedGuess)
                    {
                        result.correctMappings++;
                    }
                    else
                    {
                        result.incorrectMappings.Add(mapping.Key);
                    }
                }
            }
            
            // Check for missing mappings
            foreach (var correctMapping in correctMappings)
            {
                bool found = false;
                foreach (var submitted in submittedMappings)
                {
                    if (submitted.Key.ToLower().Trim() == correctMapping.Key)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    result.missingMappings.Add(correctMapping.Key);
                }
            }
            
            result.questionsUsed = questionsAsked;
            result.score = currentPuzzle.CalculateScore(result.correctMappings, questionsAsked);
            result.scoreTier = currentPuzzle.GetScoreTier(questionsAsked);
            result.isPerfect = result.correctMappings == result.totalMappings;
            
            return result;
        }
        
        public void CompletePuzzle(ValidationResult result)
        {
            OnPuzzleCompleted?.Invoke(result.correctMappings, result.totalMappings, result.score);
            GameManager.Instance.CompletePuzzle(result.score);
        }
        
        public string GetHint()
        {
            // Find a mapping the player hasn't discovered yet
            foreach (var mapping in currentPuzzle.WordMappings)
            {
                string riddleWord = mapping.riddleWord.ToLower();
                if (!playerMappings.ContainsKey(riddleWord) || 
                    playerMappings[riddleWord] != correctMappings[riddleWord])
                {
                    if (!string.IsNullOrEmpty(mapping.hint))
                        return mapping.hint;
                    else
                        return $"Consider what '{mapping.riddleWord}' might truly represent...";
                }
            }
            
            return "You're on the right path. Trust your instincts.";
        }
        
        public ScoreTier GetCurrentScoreTier()
        {
            return currentPuzzle.GetScoreTier(questionsAsked);
        }
    }
    
    public class ValidationResult
    {
        public int correctMappings;
        public int totalMappings;
        public List<string> incorrectMappings;
        public List<string> missingMappings;
        public int questionsUsed;
        public int score;
        public ScoreTier scoreTier;
        public bool isPerfect;
    }
}

