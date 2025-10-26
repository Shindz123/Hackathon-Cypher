using UnityEngine;
using Neocortex;
using Neocortex.Data;
using Lexicon.Data;
using Lexicon.Managers;

namespace Lexicon.AI
{
    [RequireComponent(typeof(NeocortexSmartAgent))]
    public class LexiconAIController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PuzzleManager puzzleManager;
        [SerializeField] private AICharacterSettings characterSettings;
        
        private NeocortexSmartAgent smartAgent;
        private PuzzleData currentPuzzle;
        private bool isProcessing = false;
        
        public bool IsProcessing => isProcessing;
        
        private void Awake()
        {
            smartAgent = GetComponent<NeocortexSmartAgent>();
        }
        
        private void Start()
        {
            if (puzzleManager == null)
                puzzleManager = FindFirstObjectByType<PuzzleManager>();
            
            // Auto-load character settings if not assigned
            if (characterSettings == null)
            {
                characterSettings = Resources.Load<AICharacterSettings>("AICharacterSettings");
                if (characterSettings == null)
                {
                    Debug.LogWarning("AICharacterSettings not found! Using default behavior.");
                }
            }
            
            // Subscribe to smart agent events
            smartAgent.OnChatResponseReceived.AddListener(OnAIResponseReceived);
            smartAgent.OnRequestFailed.AddListener(OnRequestFailed);
            
            // Load current puzzle
            LoadPuzzle();
        }
        
        public void LoadPuzzle()
        {
            currentPuzzle = GameManager.Instance.GetCurrentPuzzle();
            
            if (currentPuzzle != null)
            {
                Debug.Log($"AI Controller loaded puzzle: {currentPuzzle.PuzzleName}");
                
                // Send initial greeting
                SendInitialGreeting();
            }
        }
        
        private void SendInitialGreeting()
        {
            if (currentPuzzle == null)
            {
                Debug.LogWarning("Cannot send initial greeting: No puzzle loaded");
                return;
            }
            
            // Get greeting from settings or use default
            string greeting;
            if (characterSettings != null)
            {
                greeting = characterSettings.GetInitialGreeting(currentPuzzle.RiddleSentence);
            }
            else
            {
                greeting = $"Greetings, seeker of truth. I am Lexara, keeper of riddles.\n\nPonder this mystery:\n\n\"{currentPuzzle.RiddleSentence}\"\n\nAsk your questions wisely, and you may uncover what lies beneath the veil of words.";
            }
            
            // Broadcast as initial message without calling API (safely)
            try
            {
                BroadcastAIMessage(greeting);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Could not broadcast initial greeting (chat UI may not be set up): {e.Message}");
                Debug.Log($"Initial Greeting (not displayed): {greeting}");
            }
        }
        
        public void AskQuestion(string question)
        {
            if (isProcessing)
            {
                Debug.LogWarning("AI is still processing previous question");
                return;
            }
            
            if (currentPuzzle == null)
            {
                Debug.LogError("No puzzle loaded! Make sure GameManager has a PuzzleDatabase assigned.");
                return;
            }
            
            if (puzzleManager == null)
            {
                Debug.LogError("PuzzleManager not found! Make sure it exists in the scene.");
                puzzleManager = FindFirstObjectByType<PuzzleManager>();
                if (puzzleManager == null)
                    return;
            }
            
            // Check for special commands
            if (question.ToLower().StartsWith("/hint"))
            {
                HandleHintCommand();
                return;
            }
            
            if (question.ToLower().StartsWith("/riddle"))
            {
                HandleRiddleCommand();
                return;
            }
            
            // Increment question counter
            puzzleManager.IncrementQuestionCount();
            
            // Build contextual prompt
            string contextualPrompt = BuildPromptWithPuzzleContext(question);
            
            // Send to Neocortex
            isProcessing = true;
            smartAgent.TextToText(contextualPrompt);
        }
        
        private string BuildPromptWithPuzzleContext(string userQuestion)
        {
            string systemPrompt = currentPuzzle.GetSystemPrompt();
            
            // Combine system prompt with user question
            string fullPrompt = $@"{systemPrompt}

PLAYER'S QUESTION: {userQuestion}

Remember: Answer cryptically based on the ACTUAL meanings, not the riddle words. Be poetic and mysterious.";
            
            return fullPrompt;
        }
        
        private void OnAIResponseReceived(ChatResponse response)
        {
            isProcessing = false;
            
            string message = response.message;
            
            if (!string.IsNullOrEmpty(message))
            {
                Debug.Log($"AI Response: {message}");
                // The UI will handle displaying this through the event system
            }
            
            // Handle emotion if provided
            if (response.emotion != Emotions.Neutral)
            {
                Debug.Log($"AI Emotion: {response.emotion}");
                // UI can use this to change portrait
            }
            
            // Handle action if provided
            if (!string.IsNullOrEmpty(response.action))
            {
                Debug.Log($"AI Action: {response.action}");
            }
        }
        
        private void OnRequestFailed(string error)
        {
            isProcessing = false;
            Debug.LogError($"AI request failed: {error}");
            
            // Provide fallback response
            string fallbackMessage = "The cosmic winds are turbulent... Perhaps rephrase your inquiry?";
            BroadcastAIMessage(fallbackMessage);
        }
        
        private void HandleHintCommand()
        {
            if (puzzleManager == null)
            {
                BroadcastAIMessage("*The cosmic winds are silent... Try again in a moment.*");
                return;
            }
            
            string hint = puzzleManager.GetHint();
            
            // Use settings for hint message format
            string hintMessage;
            if (characterSettings != null)
            {
                hintMessage = characterSettings.GetHintMessage(hint);
            }
            else
            {
                hintMessage = $"*Lexara whispers a hint*: {hint}";
            }
            
            BroadcastAIMessage(hintMessage);
        }
        
        private void HandleRiddleCommand()
        {
            if (currentPuzzle == null)
            {
                BroadcastAIMessage("*The riddle fades from memory...*");
                return;
            }
            
            // Use settings for riddle repeat message
            string riddleMessage;
            if (characterSettings != null)
            {
                riddleMessage = characterSettings.GetRiddleRepeatMessage(currentPuzzle.RiddleSentence);
            }
            else
            {
                riddleMessage = $"The riddle you seek to unravel:\n\n\"{currentPuzzle.RiddleSentence}\"";
            }
            
            BroadcastAIMessage(riddleMessage);
        }
        
        public AICharacterSettings GetCharacterSettings()
        {
            return characterSettings;
        }
        
        private void BroadcastAIMessage(string message)
        {
            // Create a ChatResponse for the greeting
            ChatResponse greetingResponse = new ChatResponse
            {
                message = message,
                emotion = Emotions.Neutral,
                action = ""
            };
            
            // Broadcast through the smart agent's event
            smartAgent.OnChatResponseReceived.Invoke(greetingResponse);
        }
        
        public string GetCurrentRiddle()
        {
            return currentPuzzle?.RiddleSentence ?? "";
        }
        
        public NeocortexSmartAgent GetSmartAgent()
        {
            return smartAgent;
        }
    }
}

