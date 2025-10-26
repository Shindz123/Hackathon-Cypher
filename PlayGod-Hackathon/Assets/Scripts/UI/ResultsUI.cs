using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Lexicon.Managers;

namespace Lexicon.UI
{
    public class ResultsUI : MonoBehaviour
    {
        [Header("Display")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI statsText;
        [SerializeField] private TextMeshProUGUI feedbackText;
        
        [Header("Buttons")]
        [SerializeField] private Button nextPuzzleButton;
        [SerializeField] private Button retryButton;
        [SerializeField] private Button mainMenuButton;
        
        [Header("Progress")]
        [SerializeField] private Slider progressBar;
        [SerializeField] private TextMeshProUGUI progressText;
        
        private void Start()
        {
            if (nextPuzzleButton != null)
                nextPuzzleButton.onClick.AddListener(OnNextPuzzle);
            
            if (retryButton != null)
                retryButton.onClick.AddListener(OnRetry);
            
            if (mainMenuButton != null)
                mainMenuButton.onClick.AddListener(OnMainMenu);
            
            DisplayResults();
        }
        
        private void DisplayResults()
        {
            // This would typically receive data from the puzzle completion
            // For now, we'll show placeholder information
            
            if (titleText != null)
                titleText.text = "Puzzle Complete!";
            
            // TODO: Get actual results from PuzzleManager or pass them via static/singleton
            
            UpdateProgressBar();
            
            // Show/hide next puzzle button based on availability
            if (nextPuzzleButton != null)
            {
                nextPuzzleButton.gameObject.SetActive(GameManager.Instance.HasNextPuzzle());
            }
        }
        
        private void UpdateProgressBar()
        {
            float completion = GameManager.Instance.GetCompletionPercentage();
            
            if (progressBar != null)
                progressBar.value = completion / 100f;
            
            if (progressText != null)
                progressText.text = $"Overall Progress: {completion:F0}%";
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
}

