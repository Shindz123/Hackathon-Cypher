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
        [SerializeField] private TranslationPanel translationPanel;
        
        private void Start()
        {
            // Find components if not assigned
            if (aiController == null)
                aiController = FindObjectOfType<LexiconAIController>();
            
            if (visualNovelUI == null)
                visualNovelUI = FindObjectOfType<VisualNovelUI>();
            
            if (puzzleManager == null)
                puzzleManager = FindObjectOfType<PuzzleManager>();
            
            if (translationPanel == null)
                translationPanel = FindObjectOfType<TranslationPanel>();
            
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
            
            // Display initial riddle
            if (visualNovelUI != null && puzzleManager != null && puzzleManager.CurrentPuzzle != null)
            {
                visualNovelUI.SetRiddleText(puzzleManager.CurrentPuzzle.RiddleSentence);
            }
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
            if (translationPanel != null)
            {
                translationPanel.ShowPanel();
            }
            
            if (chatPanel != null)
            {
                chatPanel.AddMessage("Opening translation panel... Good luck, seeker!", false);
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
    }
}

