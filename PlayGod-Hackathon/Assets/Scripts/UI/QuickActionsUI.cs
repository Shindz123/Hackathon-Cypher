using UnityEngine;
using UnityEngine.UI;
using Lexicon.Managers;

namespace Lexicon.UI
{
    /// <summary>
    /// Quick action buttons that are always visible during gameplay
    /// Includes: Pause, Restart from Level 1, Hint, etc.
    /// </summary>
    public class QuickActionsUI : MonoBehaviour
    {
        [Header("Action Buttons")]
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button replayFromLevel1Button;
        [SerializeField] private Button hintButton;
        [SerializeField] private Button mainMenuButton;
        
        [Header("References")]
        [SerializeField] private PauseMenuUI pauseMenu;
        [SerializeField] private PuzzleManager puzzleManager;
        
        [Header("Confirmation")]
        [SerializeField] private bool confirmBeforeReplay = true;
        
        private void Start()
        {
            if (puzzleManager == null)
                puzzleManager = FindFirstObjectByType<PuzzleManager>();
            
            // Setup button listeners
            if (pauseButton != null)
                pauseButton.onClick.AddListener(OnPauseClicked);
            
            if (replayFromLevel1Button != null)
                replayFromLevel1Button.onClick.AddListener(OnReplayFromLevel1Clicked);
            
            if (hintButton != null)
                hintButton.onClick.AddListener(OnHintClicked);
            
            if (mainMenuButton != null)
                mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        }
        
        private void OnPauseClicked()
        {
            if (pauseMenu != null)
            {
                pauseMenu.Pause();
            }
            else
            {
                Debug.LogWarning("PauseMenu not assigned!");
            }
        }
        
        private void OnReplayFromLevel1Clicked()
        {
            if (confirmBeforeReplay)
            {
                // Simple confirmation using Unity dialog
                #if UNITY_EDITOR
                if (UnityEditor.EditorUtility.DisplayDialog(
                    "Replay from Level 1?",
                    "This will reset your progress and start from the beginning.\n\nAre you sure?",
                    "Yes, Replay",
                    "Cancel"))
                {
                    PerformReplay();
                }
                #else
                // In build, just replay (or implement custom dialog)
                Debug.Log("Replaying from Level 1 (confirmation in editor only)");
                PerformReplay();
                #endif
            }
            else
            {
                PerformReplay();
            }
        }
        
        private void PerformReplay()
        {
            Debug.Log("Restarting game from Level 1...");
            GameManager.Instance.ReplayFromLevel1();
        }
        
        private void OnHintClicked()
        {
            if (puzzleManager != null)
            {
                string hint = puzzleManager.GetHint();
                Debug.Log($"HINT: {hint}");
                
                // You could also display this in a UI popup
                // For now, it shows in console
            }
        }
        
        private void OnMainMenuClicked()
        {
            GameManager.Instance.LoadMainMenu();
        }
    }
}

