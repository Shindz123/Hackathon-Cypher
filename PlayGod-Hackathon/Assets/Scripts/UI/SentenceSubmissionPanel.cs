using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Lexicon.Managers;
using Lexicon.Data;
using System.Collections;

namespace Lexicon.UI
{
    /// <summary>
    /// Simple sentence-based puzzle submission system
    /// Player types the entire decoded sentence and submits
    /// </summary>
    public class SentenceSubmissionPanel : MonoBehaviour
    {
        [Header("Display Mode")]
        [SerializeField] private bool alwaysVisible = true;
        [SerializeField] private bool showCancelButton = false;
        
        [Header("UI References")]
        [SerializeField] private GameObject submissionPanel;
        [SerializeField] private TextMeshProUGUI riddleDisplayText;
        [SerializeField] private TMP_InputField answerInputField;
        [SerializeField] private Button submitButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private TextMeshProUGUI feedbackText;
        
        [Header("Feedback Settings")]
        [SerializeField] private Color correctColor = new Color(0.5f, 1f, 0.5f);
        [SerializeField] private Color incorrectColor = new Color(1f, 0.5f, 0.5f);
        [SerializeField] private float feedbackDisplayTime = 2f;
        [SerializeField] private float autoAdvanceDelay = 1.5f;
        
        [Header("Validation Settings")]
        [SerializeField] private bool ignorePunctuation = true;
        [SerializeField] private bool ignoreCase = true;
        [SerializeField] private bool allowPartialMatch = false;
        [SerializeField] [Range(0f, 1f)] private float partialMatchThreshold = 0.8f;
        
        private PuzzleManager puzzleManager;
        private PuzzleData currentPuzzle;
        private bool isProcessing = false;
        
        private void Start()
        {
            puzzleManager = FindFirstObjectByType<PuzzleManager>();
            
            if (submitButton != null)
                submitButton.onClick.AddListener(OnSubmitAnswer);
            
            if (cancelButton != null)
            {
                cancelButton.onClick.AddListener(HidePanel);
                // Hide cancel button if not needed in always visible mode
                if (alwaysVisible && !showCancelButton)
                    cancelButton.gameObject.SetActive(false);
            }
            
            if (feedbackText != null)
                feedbackText.text = "";
            
            // Set panel visibility based on mode
            if (alwaysVisible)
            {
                if (submissionPanel != null)
                    submissionPanel.SetActive(true);
                
                // Initialize with current puzzle
                StartCoroutine(InitializeWithDelay());
            }
            else
            {
                if (submissionPanel != null)
                    submissionPanel.SetActive(false);
            }
        }
        
        private IEnumerator InitializeWithDelay()
        {
            // Wait for puzzle to load
            yield return new WaitForSeconds(0.3f);
            
            if (puzzleManager != null && puzzleManager.CurrentPuzzle != null)
            {
                RefreshPuzzle();
            }
        }
        
        public void ShowPanel()
        {
            if (!alwaysVisible && submissionPanel != null)
                submissionPanel.SetActive(true);
            
            RefreshPuzzle();
        }
        
        public void RefreshPuzzle()
        {
            if (puzzleManager == null || puzzleManager.CurrentPuzzle == null)
            {
                Debug.LogWarning("Cannot refresh submission panel: No puzzle loaded!");
                return;
            }
            
            currentPuzzle = puzzleManager.CurrentPuzzle;
            
            // Display the riddle
            if (riddleDisplayText != null)
            {
                if (alwaysVisible)
                    riddleDisplayText.text = currentPuzzle.RiddleSentence;
                else
                    riddleDisplayText.text = $"Original Riddle:\n\"{currentPuzzle.RiddleSentence}\"";
            }
            
            // Clear previous input
            if (answerInputField != null)
            {
                answerInputField.text = "";
                answerInputField.interactable = true;
                
                var placeholder = answerInputField.placeholder.GetComponent<TextMeshProUGUI>();
                if (placeholder != null)
                    placeholder.text = "Type the decoded sentence here...";
            }
            
            // Clear feedback
            if (feedbackText != null)
                feedbackText.text = "";
            
            // Reset processing state
            isProcessing = false;
            
            Debug.Log($"Submission panel refreshed for puzzle: {currentPuzzle.PuzzleName}");
        }
        
        public void HidePanel()
        {
            // Only hide if not in always visible mode
            if (!alwaysVisible && submissionPanel != null)
                submissionPanel.SetActive(false);
        }
        
        private void OnSubmitAnswer()
        {
            if (isProcessing)
                return;
            
            if (answerInputField == null || string.IsNullOrWhiteSpace(answerInputField.text))
            {
                ShowFeedback("Please enter your answer!", incorrectColor, false);
                return;
            }
            
            string playerAnswer = answerInputField.text.Trim();
            string correctAnswer = currentPuzzle.TargetTranslation;
            
            bool isCorrect = ValidateAnswer(playerAnswer, correctAnswer);
            
            if (isCorrect)
            {
                HandleCorrectAnswer();
            }
            else
            {
                HandleIncorrectAnswer();
            }
        }
        
        private bool ValidateAnswer(string playerAnswer, string correctAnswer)
        {
            // Normalize answers
            string normalizedPlayer = NormalizeString(playerAnswer);
            string normalizedCorrect = NormalizeString(correctAnswer);
            
            // Exact match
            if (normalizedPlayer == normalizedCorrect)
                return true;
            
            // Partial match (if enabled)
            if (allowPartialMatch)
            {
                float similarity = CalculateSimilarity(normalizedPlayer, normalizedCorrect);
                return similarity >= partialMatchThreshold;
            }
            
            return false;
        }
        
        private string NormalizeString(string input)
        {
            string result = input;
            
            if (ignoreCase)
                result = result.ToLower();
            
            if (ignorePunctuation)
            {
                // Remove common punctuation
                result = result.Replace(".", "")
                              .Replace(",", "")
                              .Replace("!", "")
                              .Replace("?", "")
                              .Replace("'", "")
                              .Replace("\"", "");
            }
            
            // Normalize whitespace
            result = System.Text.RegularExpressions.Regex.Replace(result, @"\s+", " ");
            result = result.Trim();
            
            return result;
        }
        
        private float CalculateSimilarity(string s1, string s2)
        {
            // Simple word-based similarity
            string[] words1 = s1.Split(' ');
            string[] words2 = s2.Split(' ');
            
            int matchingWords = 0;
            foreach (string word1 in words1)
            {
                foreach (string word2 in words2)
                {
                    if (word1 == word2)
                    {
                        matchingWords++;
                        break;
                    }
                }
            }
            
            int totalWords = Mathf.Max(words1.Length, words2.Length);
            return totalWords > 0 ? (float)matchingWords / totalWords : 0f;
        }
        
        private void HandleCorrectAnswer()
        {
            isProcessing = true;
            
            // Calculate score
            int correctMappings = currentPuzzle.WordMappings.Count;
            int score = currentPuzzle.CalculateScore(correctMappings, puzzleManager.QuestionsAsked);
            
            // Complete puzzle
            puzzleManager.CompletePuzzle(new ValidationResult
            {
                correctMappings = correctMappings,
                totalMappings = correctMappings,
                incorrectMappings = new System.Collections.Generic.List<string>(),
                missingMappings = new System.Collections.Generic.List<string>(),
                questionsUsed = puzzleManager.QuestionsAsked,
                score = score,
                scoreTier = currentPuzzle.GetScoreTier(puzzleManager.QuestionsAsked),
                isPerfect = true
            });
            
            // Show success feedback
            ShowFeedback("✓ Correct!", correctColor, true);
            
            // Disable input
            if (answerInputField != null)
                answerInputField.interactable = false;
            
            Debug.Log($"Puzzle solved! Score: {score}, Questions: {puzzleManager.QuestionsAsked}");
            
            // Auto-advance to next puzzle
            StartCoroutine(AutoAdvanceToNextPuzzle());
        }
        
        private void HandleIncorrectAnswer()
        {
            // Show incorrect feedback
            ShowFeedback("✗ Incorrect - Try again!", incorrectColor, false);
            
            // Clear input or keep it for retry
            if (answerInputField != null)
            {
                answerInputField.text = "";
                answerInputField.Select();
                answerInputField.ActivateInputField();
            }
            
            Debug.Log("Incorrect answer submitted");
        }
        
        private void ShowFeedback(string message, Color color, bool success)
        {
            if (feedbackText != null)
            {
                feedbackText.text = message;
                feedbackText.color = color;
            }
            
            // Auto-clear feedback after delay (only if not success)
            if (!success)
            {
                StartCoroutine(ClearFeedbackAfterDelay());
            }
        }
        
        private IEnumerator ClearFeedbackAfterDelay()
        {
            yield return new WaitForSeconds(feedbackDisplayTime);
            
            if (feedbackText != null && !isProcessing)
            {
                feedbackText.text = "";
            }
        }
        
        private IEnumerator AutoAdvanceToNextPuzzle()
        {
            yield return new WaitForSeconds(autoAdvanceDelay);
            
            // Check if there's a next puzzle
            if (GameManager.Instance.HasNextPuzzle())
            {
                Debug.Log("Advancing to next puzzle...");
                GameManager.Instance.AdvanceToNextPuzzle();
                GameManager.Instance.LoadPuzzleScene();
            }
            else
            {
                Debug.Log("All puzzles completed!");
                // Could load results screen or main menu
                ShowFeedback("🎉 All Puzzles Complete!", correctColor, true);
                yield return new WaitForSeconds(2f);
                GameManager.Instance.LoadMainMenu();
            }
        }
        
        /// <summary>
        /// Manual method to show the correct answer (for debugging)
        /// </summary>
        [ContextMenu("Show Correct Answer")]
        public void ShowCorrectAnswer()
        {
            if (currentPuzzle != null)
            {
                Debug.Log($"Correct Answer: {currentPuzzle.TargetTranslation}");
                if (feedbackText != null)
                {
                    feedbackText.text = $"Hint: {currentPuzzle.TargetTranslation}";
                    feedbackText.color = Color.yellow;
                }
            }
        }
    }
}

