using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Lexicon.Managers;
using Lexicon.Systems;

namespace Lexicon.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button puzzleSelectButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;
        
        [Header("Progress Display")]
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private Slider progressBar;
        [SerializeField] private TextMeshProUGUI totalScoreText;
        
        [Header("Panels")]
        [SerializeField] private GameObject puzzleSelectPanel;
        [SerializeField] private GameObject settingsPanel;
        
        [Header("Puzzle Select")]
        [SerializeField] private Transform puzzleButtonsContainer;
        [SerializeField] private GameObject puzzleButtonPrefab;
        
        private ProgressionSystem progressionSystem;
        
        private void Start()
        {
            progressionSystem = FindObjectOfType<ProgressionSystem>();
            if (progressionSystem == null)
            {
                GameObject go = new GameObject("ProgressionSystem");
                progressionSystem = go.AddComponent<ProgressionSystem>();
            }
            
            // Setup button listeners
            if (newGameButton != null)
                newGameButton.onClick.AddListener(OnNewGame);
            
            if (continueButton != null)
                continueButton.onClick.AddListener(OnContinue);
            
            if (puzzleSelectButton != null)
                puzzleSelectButton.onClick.AddListener(OnPuzzleSelect);
            
            if (settingsButton != null)
                settingsButton.onClick.AddListener(OnSettings);
            
            if (quitButton != null)
                quitButton.onClick.AddListener(OnQuit);
            
            // Hide panels
            if (puzzleSelectPanel != null)
                puzzleSelectPanel.SetActive(false);
            
            if (settingsPanel != null)
                settingsPanel.SetActive(false);
            
            UpdateProgressDisplay();
            UpdateContinueButton();
        }
        
        private void OnNewGame()
        {
            // Confirm if player has progress
            int currentPuzzle = PlayerPrefs.GetInt("CurrentPuzzleIndex", 0);
            if (currentPuzzle > 0)
            {
                // TODO: Show confirmation dialog
                Debug.Log("Warning: This will reset your progress!");
            }
            
            GameManager.Instance.ResetProgress();
            GameManager.Instance.StartNewGame();
        }
        
        private void OnContinue()
        {
            GameManager.Instance.ContinueGame();
        }
        
        private void OnPuzzleSelect()
        {
            if (puzzleSelectPanel != null)
            {
                puzzleSelectPanel.SetActive(true);
                PopulatePuzzleSelect();
            }
        }
        
        private void OnSettings()
        {
            if (settingsPanel != null)
            {
                settingsPanel.SetActive(true);
            }
        }
        
        private void OnQuit()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
        
        private void UpdateProgressDisplay()
        {
            if (progressionSystem != null)
            {
                float progress = progressionSystem.GetOverallProgress();
                
                if (progressBar != null)
                    progressBar.value = progress;
                
                if (progressText != null)
                    progressText.text = progressionSystem.GetProgressDescription();
            }
            
            if (totalScoreText != null)
            {
                int totalScore = GameManager.Instance.TotalScore;
                totalScoreText.text = $"Total Score: {totalScore}";
            }
        }
        
        private void UpdateContinueButton()
        {
            if (continueButton != null)
            {
                int currentPuzzle = PlayerPrefs.GetInt("CurrentPuzzleIndex", 0);
                continueButton.interactable = currentPuzzle > 0 || PlayerPrefs.HasKey("UnlockedPuzzles");
            }
        }
        
        private void PopulatePuzzleSelect()
        {
            if (puzzleButtonsContainer == null || puzzleButtonPrefab == null)
                return;
            
            // Clear existing buttons
            foreach (Transform child in puzzleButtonsContainer)
            {
                Destroy(child.gameObject);
            }
            
            // Create button for each puzzle
            if (GameManager.Instance.Database != null)
            {
                for (int i = 0; i < GameManager.Instance.Database.TotalPuzzles; i++)
                {
                    int puzzleIndex = i;
                    var puzzle = GameManager.Instance.Database.GetPuzzle(i);
                    bool isUnlocked = GameManager.Instance.IsPuzzleUnlocked(i);
                    
                    GameObject btnObj = Instantiate(puzzleButtonPrefab, puzzleButtonsContainer);
                    Button btn = btnObj.GetComponent<Button>();
                    TextMeshProUGUI btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();
                    
                    if (btnText != null)
                    {
                        btnText.text = isUnlocked ? puzzle.PuzzleName : "???";
                    }
                    
                    if (btn != null)
                    {
                        btn.interactable = isUnlocked;
                        btn.onClick.AddListener(() => OnPuzzleSelected(puzzleIndex));
                    }
                    
                    // Visual indication of locked/unlocked
                    Image btnImage = btnObj.GetComponent<Image>();
                    if (btnImage != null)
                    {
                        btnImage.color = isUnlocked ? Color.white : new Color(0.5f, 0.5f, 0.5f);
                    }
                }
            }
        }
        
        private void OnPuzzleSelected(int puzzleIndex)
        {
            GameManager.Instance.SetCurrentPuzzle(puzzleIndex);
            GameManager.Instance.LoadPuzzleScene();
        }
        
        public void ClosePuzzleSelect()
        {
            if (puzzleSelectPanel != null)
                puzzleSelectPanel.SetActive(false);
        }
        
        public void CloseSettings()
        {
            if (settingsPanel != null)
                settingsPanel.SetActive(false);
        }
    }
}

