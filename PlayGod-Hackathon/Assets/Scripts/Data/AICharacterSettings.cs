using UnityEngine;

namespace Lexicon.Data
{
    /// <summary>
    /// Centralized AI character configuration for Lexara
    /// Configure the AI's name, personality, greetings, and behavior here
    /// </summary>
    [CreateAssetMenu(fileName = "AICharacterSettings", menuName = "Lexicon/AI Character Settings")]
    public class AICharacterSettings : ScriptableObject
    {
        [Header("Character Identity")]
        [SerializeField] private string characterName = "Lexara";
        [SerializeField, TextArea(2, 4)] private string characterTitle = "Keeper of Riddles";
        [SerializeField, TextArea(3, 6)] private string characterDescription = "A mystical AI oracle who speaks only in riddles and poetic language.";
        
        [Header("Greetings")]
        [SerializeField, TextArea(4, 8)] private string initialGreeting = "Greetings, seeker of truth. I am {NAME}, {TITLE}.\n\nPonder this mystery:\n\n\"{RIDDLE}\"\n\nAsk your questions wisely, and you may uncover what lies beneath the veil of words.";
        [SerializeField, TextArea(2, 4)] private string puzzleCompleteGreeting = "Well done, seeker! You have unraveled my riddle.";
        [SerializeField, TextArea(2, 4)] private string newPuzzleGreeting = "A new challenge awaits. Let us see if your wisdom has grown...";
        
        [Header("Response Style")]
        [SerializeField, Range(1, 10)] private int mysteryLevel = 5;
        [Tooltip("1 = Very direct and helpful, 10 = Extremely cryptic")]
        [SerializeField] private bool useEmotionalResponses = true;
        [SerializeField] private bool encouragePlayerWhenStuck = true;
        [SerializeField] private int questionsBeforeExtraHint = 10;
        
        [Header("Visual Style")]
        [SerializeField] private Color nameColor = new Color(1f, 0.84f, 0f); // Gold
        [SerializeField] private float typewriterSpeed = 0.05f;
        
        [Header("Special Messages")]
        [SerializeField, TextArea(2, 3)] private string hintMessage = "*{NAME} whispers a hint*: {HINT}";
        [SerializeField, TextArea(2, 3)] private string riddleRepeatMessage = "The riddle you seek to unravel:\n\n\"{RIDDLE}\"";
        [SerializeField, TextArea(2, 3)] private string stuckMessage = "I sense you are struggling, seeker. Perhaps approach from a different angle?";
        
        // Public accessors
        public string CharacterName => characterName;
        public string CharacterTitle => characterTitle;
        public string CharacterDescription => characterDescription;
        public Color NameColor => nameColor;
        public float TypewriterSpeed => typewriterSpeed;
        public int MysteryLevel => mysteryLevel;
        public bool UseEmotionalResponses => useEmotionalResponses;
        public bool EncouragePlayerWhenStuck => encouragePlayerWhenStuck;
        public int QuestionsBeforeExtraHint => questionsBeforeExtraHint;
        
        public string GetInitialGreeting(string riddle)
        {
            return initialGreeting
                .Replace("{NAME}", characterName)
                .Replace("{TITLE}", characterTitle)
                .Replace("{RIDDLE}", riddle);
        }
        
        public string GetPuzzleCompleteGreeting()
        {
            return puzzleCompleteGreeting.Replace("{NAME}", characterName);
        }
        
        public string GetNewPuzzleGreeting()
        {
            return newPuzzleGreeting.Replace("{NAME}", characterName);
        }
        
        public string GetHintMessage(string hint)
        {
            return hintMessage
                .Replace("{NAME}", characterName)
                .Replace("{HINT}", hint);
        }
        
        public string GetRiddleRepeatMessage(string riddle)
        {
            return riddleRepeatMessage
                .Replace("{NAME}", characterName)
                .Replace("{RIDDLE}", riddle);
        }
        
        public string GetStuckMessage()
        {
            return stuckMessage.Replace("{NAME}", characterName);
        }
        
        public string GetMysteryLevelDescription()
        {
            if (mysteryLevel <= 3)
                return "Very Helpful - Clear hints and descriptions";
            else if (mysteryLevel <= 5)
                return "Balanced - Poetic but guessable";
            else if (mysteryLevel <= 7)
                return "Cryptic - Abstract metaphors";
            else
                return "Extremely Mysterious - Philosophical riddles";
        }
    }
}

