using UnityEngine;
using Lexicon.Data;

namespace Lexicon.Systems
{
    public class ScoringSystem : MonoBehaviour
    {
        [Header("Base Scoring")]
        [SerializeField] private int pointsPerCorrectMapping = 100;
        [SerializeField] private int perfectBonusPoints = 250;
        
        [Header("Question Efficiency Bonuses")]
        [SerializeField] private int perfectTierBonus = 500;
        [SerializeField] private int goodTierBonus = 300;
        [SerializeField] private int passTierBonus = 100;
        
        [Header("Pattern Recognition")]
        [SerializeField] private int patternRecognitionBonus = 150;
        [SerializeField] private int earlyDiscoveryBonus = 50;
        
        public int CalculateScore(int correctMappings, int totalMappings, int questionsAsked, ScoreTier tier, bool foundPatterns = false, int earlyDiscoveries = 0)
        {
            int score = 0;
            
            // Base points for correct mappings
            score += correctMappings * pointsPerCorrectMapping;
            
            // Perfect bonus
            if (correctMappings == totalMappings)
            {
                score += perfectBonusPoints;
            }
            
            // Tier bonus based on questions asked
            switch (tier)
            {
                case ScoreTier.Perfect:
                    score += perfectTierBonus;
                    break;
                case ScoreTier.Good:
                    score += goodTierBonus;
                    break;
                case ScoreTier.Pass:
                    score += passTierBonus;
                    break;
            }
            
            // Pattern recognition bonus
            if (foundPatterns)
            {
                score += patternRecognitionBonus;
            }
            
            // Early discovery bonus
            score += earlyDiscoveries * earlyDiscoveryBonus;
            
            return Mathf.Max(0, score);
        }
        
        public string GetScoreFeedback(int score, ScoreTier tier)
        {
            if (tier == ScoreTier.Perfect && score >= 1500)
            {
                return "Legendary! You are a true master of riddles!";
            }
            else if (tier == ScoreTier.Perfect)
            {
                return "Perfect! Your insight is remarkable!";
            }
            else if (tier == ScoreTier.Good)
            {
                return "Well done! You're becoming quite skilled at this!";
            }
            else if (tier == ScoreTier.Pass)
            {
                return "Good effort! You solved the riddle!";
            }
            else
            {
                return "Keep trying! Every puzzle teaches something new.";
            }
        }
        
        public Color GetScoreTierColor(ScoreTier tier)
        {
            switch (tier)
            {
                case ScoreTier.Perfect:
                    return new Color(1f, 0.84f, 0f); // Gold
                case ScoreTier.Good:
                    return new Color(0.75f, 0.75f, 1f); // Silver/Light Blue
                case ScoreTier.Pass:
                    return new Color(0.7f, 0.9f, 0.7f); // Light Green
                default:
                    return new Color(0.9f, 0.7f, 0.7f); // Light Red
            }
        }
    }
}

