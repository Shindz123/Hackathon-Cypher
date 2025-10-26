using UnityEngine;
using Neocortex;
using Neocortex.Data;
using Lexicon.AI;
using Lexicon.Managers;

namespace Lexicon.UI
{
    public class LexiconChatController : MonoBehaviour
    {
        [Header("Neocortex Components")]
        [SerializeField] private NeocortexTextChatInput chatInput;
        [SerializeField] private NeocortexChatPanel chatPanel;
        [SerializeField] private NeocortexThinkingIndicator thinkingIndicator;
        
        [Header("Custom Components")]
        [SerializeField] private LexiconAIController aiController;
        [SerializeField] private VisualNovelUI visualNovelUI;
        [SerializeField] private PuzzleManager puzzleManager;
        [SerializeField] private SentenceSubmissionPanel sentenceSubmissionPanel;
        [SerializeField] private TranslationPanel translationPanel; // Legacy (optional)
        
        private void Start()
        {
            // Find components if not assigned
            if (aiController == null)
                aiController = FindFirstObjectByType<LexiconAIController>();
            
            if (visualNovelUI == null)
                visualNovelUI = FindFirstObjectByType<VisualNovelUI>();
            
            if (puzzleManager == null)
                puzzleManager = FindFirstObjectByType<PuzzleManager>();
            
            if (sentenceSubmissionPanel == null)
                sentenceSubmissionPanel = FindFirstObjectByType<SentenceSubmissionPanel>();
            
            if (translationPanel == null)
                translationPanel = FindFirstObjectByType<TranslationPanel>();
            
            // Setup event listeners
            if (chatInput != null)
            {
                chatInput.OnSendButtonClicked.AddListener(OnPlayerMessage);
            }
            
            if (aiController != null)
            {
                var smartAgent = aiController.GetSmartAgent();
                if (smartAgent != null)
                {
                    smartAgent.OnChatResponseReceived.AddListener(OnAIResponse);
                }
            }
            
            if (puzzleManager != null)
            {
                puzzleManager.OnQuestionAsked.AddListener(OnQuestionAsked);
                puzzleManager.OnMappingDiscovered.AddListener(OnMappingDiscovered);
            }
            
            // Display initial riddle (delayed to ensure puzzle is loaded)
            StartCoroutine(DisplayInitialRiddle());
        }
        
        private void OnPlayerMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;
            
            // Add player message to chat
            if (chatPanel != null)
            {
                chatPanel.AddMessage(message, true);
            }
            
            // Check for special commands
            if (message.ToLower().StartsWith("/solve") || message.ToLower().StartsWith("/translate"))
            {
                HandleSolveCommand();
                return;
            }
            
            if (message.ToLower().StartsWith("/hint"))
            {
                HandleHintCommand();
                return;
            }
            
            if (message.ToLower().StartsWith("/riddle"))
            {
                HandleRiddleCommand();
                return;
            }
            
            // Show thinking indicator
            if (thinkingIndicator != null)
            {
                thinkingIndicator.Display(true);
            }
            
            // Send to AI
            if (aiController != null)
            {
                aiController.AskQuestion(message);
            }
        }
        
        private void OnAIResponse(ChatResponse response)
        {
            // Hide thinking indicator
            if (thinkingIndicator != null)
            {
                thinkingIndicator.Display(false);
            }
            
            // Add to chat panel
            if (chatPanel != null)
            {
                chatPanel.AddMessage(response.message, false);
            }
            
            // Update visual novel UI
            if (visualNovelUI != null)
            {
                visualNovelUI.DisplayMessage(response.message, response.emotion);
            }
        }
        
        private void OnQuestionAsked(int questionCount)
        {
            // Update question counter in UI
            if (visualNovelUI != null)
            {
                visualNovelUI.UpdateQuestionsCount(questionCount);
            }
            
            // Update score tier
            if (puzzleManager != null)
            {
                var tier = puzzleManager.GetCurrentScoreTier();
                if (visualNovelUI != null)
                {
                    visualNovelUI.UpdateScoreTier(tier);
                }
            }
        }
        
        private void OnMappingDiscovered(string riddleWord, string actualMeaning)
        {
            // Show celebration message
            string celebrationMsg = $"✨ You discovered a truth! '{riddleWord}' = '{actualMeaning}'";
            
            if (chatPanel != null)
            {
                chatPanel.AddMessage(celebrationMsg, false);
            }
        }
        
        private void HandleSolveCommand()
        {
            // Use new sentence submission panel if available
            if (sentenceSubmissionPanel != null)
            {
                sentenceSubmissionPanel.ShowPanel();
                
                if (chatPanel != null)
                {
                    chatPanel.AddMessage("Ready to solve? Type the decoded sentence and submit!", false);
                }
            }
            // Fallback to old translation panel
            else if (translationPanel != null)
            {
                translationPanel.ShowPanel();
                
                if (chatPanel != null)
                {
                    chatPanel.AddMessage("Opening translation panel... Good luck, seeker!", false);
                }
            }
            else
            {
                if (chatPanel != null)
                {
                    chatPanel.AddMessage("Submission panel not found! Please set up SentenceSubmissionPanel.", false);
                }
            }
        }
        
        private void HandleHintCommand()
        {
            if (puzzleManager != null)
            {
                string hint = puzzleManager.GetHint();
                
                if (chatPanel != null)
                {
                    chatPanel.AddMessage($"💡 Hint: {hint}", false);
                }
            }
        }
        
        private void HandleRiddleCommand()
        {
            if (puzzleManager != null && puzzleManager.CurrentPuzzle != null)
            {
                string riddle = puzzleManager.CurrentPuzzle.RiddleSentence;
                
                if (chatPanel != null)
                {
                    chatPanel.AddMessage($"The riddle:\n\"{riddle}\"", false);
                }
            }
        }
        
        public void OnSkipDialogueClicked()
        {
            if (visualNovelUI != null)
            {
                visualNovelUI.SkipTypewriter();
            }
        }
        
        private System.Collections.IEnumerator DisplayInitialRiddle()
        {
            // Wait a frame to ensure PuzzleManager has loaded
            yield return new WaitForEndOfFrame();
            
            // Try a few times with delays
            for (int i = 0; i < 5; i++)
            {
                if (puzzleManager != null && puzzleManager.CurrentPuzzle != null)
                {
                    if (visualNovelUI != null)
                    {
                        visualNovelUI.SetRiddleText(puzzleManager.CurrentPuzzle.RiddleSentence);
                    }
                    Debug.Log("Riddle displayed successfully!");
                    yield break;
                }
                
                Debug.LogWarning($"Waiting for puzzle to load... Attempt {i + 1}/5");
                yield return new WaitForSeconds(0.2f);
            }
            
            // If we got here, puzzle still not loaded
            Debug.LogError("Failed to load puzzle after multiple attempts! Check:\n" +
                "1. GameManager has PuzzleDatabase assigned\n" +
                "2. PuzzleManager component exists on GameManager\n" +
                "3. PuzzleDatabase has puzzles in it");
        }
    }
}

