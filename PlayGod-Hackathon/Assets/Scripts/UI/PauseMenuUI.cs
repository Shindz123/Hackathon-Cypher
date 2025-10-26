using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Lexicon.Managers;

namespace Lexicon.UI
{
    /// <summary>
    /// In-game pause/options menu with restart and quit options
    /// </summary>
    public class PauseMenuUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button replayFromLevel1Button;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button quitButton;
        
        [Header("Settings")]
        [SerializeField] private KeyCode pauseKey = KeyCode.Escape;
        [SerializeField] private bool showConfirmationForReplay = true;
        
        [Header("Confirmation Dialog")]
        [SerializeField] private GameObject confirmationDialog;
        [SerializeField] private TextMeshProUGUI confirmationText;
        [SerializeField] private Button confirmYesButton;
        [SerializeField] private Button confirmNoButton;
        
        private bool isPaused = false;
        
        private void Start()
        {
            // Setup button listeners
            if (resumeButton != null)
                resumeButton.onClick.AddListener(Resume);
            
            if (replayFromLevel1Button != null)
                replayFromLevel1Button.onClick.AddListener(OnReplayFromLevel1Clicked);
            
            if (mainMenuButton != null)
                mainMenuButton.onClick.AddListener(OnMainMenu);
            
            if (quitButton != null)
                quitButton.onClick.AddListener(OnQuit);
            
            if (confirmYesButton != null)
                confirmYesButton.onClick.AddListener(ConfirmReplay);
            
            if (confirmNoButton != null)
                confirmNoButton.onClick.AddListener(CancelConfirmation);
            
            // Hide panels initially
            if (pausePanel != null)
                pausePanel.SetActive(false);
            
            if (confirmationDialog != null)
                confirmationDialog.SetActive(false);
        }
        
        private void Update()
        {
            // Toggle pause with Escape key
            if (Input.GetKeyDown(pauseKey))
            {
                if (isPaused)
                    Resume();
                else
                    Pause();
            }
        }
        
        public void Pause()
        {
            isPaused = true;
            
            if (pausePanel != null)
                pausePanel.SetActive(true);
            
            // Pause game time (optional)
            // Time.timeScale = 0;
            
            Debug.Log("Game paused");
        }
        
        public void Resume()
        {
            isPaused = false;
            
            if (pausePanel != null)
                pausePanel.SetActive(false);
            
            if (confirmationDialog != null)
                confirmationDialog.SetActive(false);
            
            // Resume game time (optional)
            // Time.timeScale = 1;
            
            Debug.Log("Game resumed");
        }
        
        private void OnReplayFromLevel1Clicked()
        {
            if (showConfirmationForReplay && confirmationDialog != null)
            {
                // Show confirmation
                confirmationDialog.SetActive(true);
                
                if (confirmationText != null)
                {
                    int currentPuzzle = GameManager.Instance.CurrentPuzzleIndex + 1;
                    confirmationText.text = $"Reset all progress and replay from Level 1?\n\n(You are currently on Level {currentPuzzle})";
                }
            }
            else
            {
                // Replay directly without confirmation
                ConfirmReplay();
            }
        }
        
        private void ConfirmReplay()
        {
            Debug.Log("Player confirmed: Replaying from Level 1");
            
            // Reset progress and load level 1
            GameManager.Instance.ReplayFromLevel1();
        }
        
        private void CancelConfirmation()
        {
            if (confirmationDialog != null)
                confirmationDialog.SetActive(false);
        }
        
        private void OnMainMenu()
        {
            Resume(); // Unpause before transitioning
            GameManager.Instance.LoadMainMenu();
        }
        
        private void OnQuit()
        {
            Resume(); // Unpause before quitting
            
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
        
        /// <summary>
        /// Public method to open pause menu from UI button
        /// </summary>
        public void OnPauseButtonClicked()
        {
            Pause();
        }
    }
}

