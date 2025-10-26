using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Lexicon.Managers;
using Lexicon.Data;

namespace Lexicon.UI
{
    public class TranslationPanel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject translationPanel;
        [SerializeField] private TextMeshProUGUI riddleDisplayText;
        [SerializeField] private Transform wordInputsContainer;
        [SerializeField] private GameObject wordInputPrefab;
        [SerializeField] private Button submitButton;
        [SerializeField] private Button cancelButton;
        
        [Header("Results")]
        [SerializeField] private GameObject resultsPanel;
        [SerializeField] private TextMeshProUGUI resultsText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI accuracyText;
        [SerializeField] private Button nextPuzzleButton;
        [SerializeField] private Button retryButton;
        [SerializeField] private Button mainMenuButton;
        
        private PuzzleManager puzzleManager;
        private WordMappingPanel wordMappingPanel;
        private List<TranslationWordInput> wordInputs = new List<TranslationWordInput>();
        
        private void Start()
        {
            puzzleManager = FindObjectOfType<PuzzleManager>();
            wordMappingPanel = FindObjectOfType<WordMappingPanel>();
            
            if (submitButton != null)
                submitButton.onClick.AddListener(OnSubmitTranslation);
            
            if (cancelButton != null)
                cancelButton.onClick.AddListener(HidePanel);
            
            if (nextPuzzleButton != null)
                nextPuzzleButton.onClick.AddListener(OnNextPuzzle);
            
            if (retryButton != null)
                retryButton.onClick.AddListener(OnRetry);
            
            if (mainMenuButton != null)
                mainMenuButton.onClick.AddListener(OnMainMenu);
            
            if (translationPanel != null)
                translationPanel.SetActive(false);
            
            if (resultsPanel != null)
                resultsPanel.SetActive(false);
        }
        
        public void ShowPanel()
        {
            if (puzzleManager == null || puzzleManager.CurrentPuzzle == null)
                return;
            
            if (translationPanel != null)
                translationPanel.SetActive(true);
            
            if (resultsPanel != null)
                resultsPanel.SetActive(false);
            
            SetupTranslationInputs();
        }
        
        public void HidePanel()
        {
            if (translationPanel != null)
                translationPanel.SetActive(false);
        }
        
        private void SetupTranslationInputs()
        {
            // Clear existing inputs
            foreach (var input in wordInputs)
            {
                if (input != null)
                    Destroy(input.gameObject);
            }
            wordInputs.Clear();
            
            // Display the riddle
            if (riddleDisplayText != null)
                riddleDisplayText.text = puzzleManager.CurrentPuzzle.RiddleSentence;
            
            // Create input for each word in the mappings
            foreach (var mapping in puzzleManager.CurrentPuzzle.WordMappings)
            {
                if (wordInputPrefab != null && wordInputsContainer != null)
                {
                    GameObject inputObj = Instantiate(wordInputPrefab, wordInputsContainer);
                    TranslationWordInput wordInput = inputObj.GetComponent<TranslationWordInput>();
                    
                    if (wordInput != null)
                    {
                        wordInput.Initialize(mapping.riddleWord);
                        
                        // Pre-fill with player's mapping if they have one
                        if (wordMappingPanel != null)
                        {
                            var playerMappings = wordMappingPanel.GetAllMappings();
                            if (playerMappings.ContainsKey(mapping.riddleWord))
                            {
                                wordInput.SetTranslation(playerMappings[mapping.riddleWord]);
                            }
                        }
                        
                        wordInputs.Add(wordInput);
                    }
                }
            }
        }
        
        private void OnSubmitTranslation()
        {
            // Collect all translations
            Dictionary<string, string> submittedMappings = new Dictionary<string, string>();
            
            foreach (var input in wordInputs)
            {
                if (input != null && !string.IsNullOrEmpty(input.Translation))
                {
                    submittedMappings[input.RiddleWord] = input.Translation;
                }
            }
            
            // Validate
            ValidationResult result = puzzleManager.ValidateTranslation(submittedMappings);
            
            // Complete puzzle
            puzzleManager.CompletePuzzle(result);
            
            // Show results
            ShowResults(result);
        }
        
        private void ShowResults(ValidationResult result)
        {
            if (resultsPanel != null)
            {
                resultsPanel.SetActive(true);
                translationPanel.SetActive(false);
                
                // Build results text
                string resultsMessage = "";
                
                if (result.isPerfect)
                {
                    resultsMessage = "Perfect! You've unraveled all the mysteries!\n\n";
                }
                else
                {
                    resultsMessage = $"You discovered {result.correctMappings} out of {result.totalMappings} mappings.\n\n";
                    
                    if (result.incorrectMappings.Count > 0)
                    {
                        resultsMessage += "Incorrect mappings: " + string.Join(", ", result.incorrectMappings) + "\n";
                    }
                    
                    if (result.missingMappings.Count > 0)
                    {
                        resultsMessage += "Missing mappings: " + string.Join(", ", result.missingMappings) + "\n";
                    }
                }
                
                if (resultsText != null)
                    resultsText.text = resultsMessage;
                
                if (scoreText != null)
                    scoreText.text = $"Score: {result.score}";
                
                if (accuracyText != null)
                {
                    float accuracy = (float)result.correctMappings / result.totalMappings * 100f;
                    accuracyText.text = $"Accuracy: {accuracy:F1}%\nQuestions Used: {result.questionsUsed}\nRank: {result.scoreTier}";
                }
                
                // Show/hide next puzzle button
                if (nextPuzzleButton != null)
                {
                    nextPuzzleButton.gameObject.SetActive(GameManager.Instance.HasNextPuzzle());
                }
            }
        }
        
        private void OnNextPuzzle()
        {
            GameManager.Instance.AdvanceToNextPuzzle();
            GameManager.Instance.LoadPuzzleScene();
        }
        
        private void OnRetry()
        {
            GameManager.Instance.LoadPuzzleScene();
        }
        
        private void OnMainMenu()
        {
            GameManager.Instance.LoadMainMenu();
        }
    }
    
    public class TranslationWordInput : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI riddleWordLabel;
        [SerializeField] private TMP_InputField translationInput;
        
        private string riddleWord;
        
        public string RiddleWord => riddleWord;
        public string Translation => translationInput?.text ?? "";
        
        public void Initialize(string word)
        {
            riddleWord = word;
            
            if (riddleWordLabel != null)
                riddleWordLabel.text = $"\"{word}\" means:";
            
            if (translationInput != null)
                translationInput.text = "";
        }
        
        public void SetTranslation(string translation)
        {
            if (translationInput != null)
                translationInput.text = translation;
        }
    }
}

