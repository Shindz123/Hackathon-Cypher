using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lexicon.Data
{
    [CreateAssetMenu(fileName = "NewPuzzle", menuName = "Lexicon/Puzzle Data")]
    public class PuzzleData : ScriptableObject
    {
        [Header("Puzzle Info")]
        [SerializeField] private string puzzleName;
        [SerializeField] private int difficultyLevel = 1;
        [SerializeField, TextArea(2, 4)] private string riddleSentence;
        [SerializeField, TextArea(2, 4)] private string targetTranslation;
        
        [Header("Word Mappings")]
        [SerializeField] private List<WordMapping> wordMappings = new List<WordMapping>();
        
        [Header("Scoring")]
        [SerializeField] private int maxQuestionsForPerfect = 5;
        [SerializeField] private int maxQuestionsForGood = 10;
        [SerializeField] private int maxQuestionsForPass = 15;
        
        [Header("AI Character")]
        [SerializeField] private string characterProjectId;
        [SerializeField, TextArea(3, 6)] private string systemPromptTemplate;
        
        public string PuzzleName => puzzleName;
        public int DifficultyLevel => difficultyLevel;
        public string RiddleSentence => riddleSentence;
        public string TargetTranslation => targetTranslation;
        public List<WordMapping> WordMappings => wordMappings;
        public int MaxQuestionsForPerfect => maxQuestionsForPerfect;
        public int MaxQuestionsForGood => maxQuestionsForGood;
        public int MaxQuestionsForPass => maxQuestionsForPass;
        public string CharacterProjectId => characterProjectId;
        
        public Dictionary<string, string> GetMappingsDictionary()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var mapping in wordMappings)
            {
                dict[mapping.riddleWord.ToLower()] = mapping.actualMeaning.ToLower();
            }
            return dict;
        }
        
        public string GetSystemPrompt()
        {
            // Use custom prompt if configured
            if (!string.IsNullOrEmpty(systemPromptTemplate))
                return systemPromptTemplate;
            
            // If no custom prompt, show warning
            Debug.LogWarning($"Puzzle '{puzzleName}' has no System Prompt Template configured! Please set one in the Inspector.");
            
            // Return minimal fallback
            return "You are Lexara, a mystical oracle. Answer questions poetically and cryptically.";
        }
        
        /// <summary>
        /// Helper method to generate a reference prompt showing all mappings
        /// Use this as a starting point to customize in the Inspector
        /// </summary>
        [ContextMenu("Generate Default Prompt")]
        public void GenerateDefaultPromptToConsole()
        {
            string mappingsString = "";
            foreach (var mapping in wordMappings)
            {
                mappingsString += $"  '{mapping.riddleWord}' = '{mapping.actualMeaning}'\n";
            }
            
            string defaultPrompt = $@"You are Lexara, a mystical AI oracle who speaks only in riddles and poetic language.

PUZZLE: {puzzleName}

SECRET WORD MAPPINGS FOR THIS PUZZLE:
{mappingsString}

IMPORTANT RULES:
1. When the player asks questions, answer based on the ACTUAL meaning, not the riddle word
2. Never directly reveal the mappings - always respond poetically and cryptically
3. Give clues through metaphors, questions, and indirect descriptions
4. If asked about properties (like 'Does the leaf fly?'), consider the actual meaning (e.g., bird)
5. Be mysterious, whimsical, and encouraging
6. Keep responses concise (2-3 sentences max)
7. Use vivid imagery and nature metaphors

EXAMPLES:
Q: 'Does the leaf have wings?'
A: 'Indeed, what you name leaf knows the freedom of flight, soaring on currents of wind.'

Q: 'Can the sun be seen at night?'
A: 'The orb I call sun graces the darkness, waxing and waning through its celestial dance.'

Remember: Help them discover truth through thoughtful hints, not direct revelation.";

            Debug.Log("=== DEFAULT PROMPT FOR COPY/PASTE ===");
            Debug.Log(defaultPrompt);
            Debug.Log("=== Copy this and paste into System Prompt Template field ===");
        }
        
        public int CalculateScore(int correctMappings, int questionsAsked)
        {
            int totalMappings = wordMappings.Count;
            float accuracyPercent = (float)correctMappings / totalMappings;
            
            int baseScore = Mathf.RoundToInt(accuracyPercent * 1000);
            
            // Bonus for fewer questions
            int questionBonus = 0;
            if (questionsAsked <= maxQuestionsForPerfect)
                questionBonus = 500;
            else if (questionsAsked <= maxQuestionsForGood)
                questionBonus = 300;
            else if (questionsAsked <= maxQuestionsForPass)
                questionBonus = 100;
            
            // Perfect bonus
            int perfectBonus = (correctMappings == totalMappings) ? 250 : 0;
            
            return baseScore + questionBonus + perfectBonus;
        }
        
        public ScoreTier GetScoreTier(int questionsAsked)
        {
            if (questionsAsked <= maxQuestionsForPerfect)
                return ScoreTier.Perfect;
            else if (questionsAsked <= maxQuestionsForGood)
                return ScoreTier.Good;
            else if (questionsAsked <= maxQuestionsForPass)
                return ScoreTier.Pass;
            else
                return ScoreTier.NeedsImprovement;
        }
    }
    
    [Serializable]
    public class WordMapping
    {
        public string riddleWord;
        public string actualMeaning;
        [TextArea(1, 3)] public string hint;
    }
    
    public enum ScoreTier
    {
        Perfect,
        Good,
        Pass,
        NeedsImprovement
    }
}

