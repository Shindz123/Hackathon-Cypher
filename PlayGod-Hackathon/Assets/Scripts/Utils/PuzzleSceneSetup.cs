using UnityEngine;
using Neocortex;
using Lexicon.Managers;
using Lexicon.AI;
using Lexicon.Data;

namespace Lexicon.Utils
{
    /// <summary>
    /// Attach this to your PuzzleScene to automatically wire up components.
    /// </summary>
    public class PuzzleSceneSetup : MonoBehaviour
    {
        [Header("Auto-Setup on Start")]
        [SerializeField] private bool autoSetupOnStart = true;
        
        [Header("References (will auto-find if not set)")]
        [SerializeField] private PuzzleManager puzzleManager;
        [SerializeField] private LexiconAIController aiController;
        [SerializeField] private NeocortexSmartAgent smartAgent;
        
        private void Start()
        {
            if (autoSetupOnStart)
            {
                SetupPuzzleScene();
            }
        }
        
        [ContextMenu("Setup Puzzle Scene")]
        public void SetupPuzzleScene()
        {
            // Find components if not assigned
            if (puzzleManager == null)
                puzzleManager = FindFirstObjectByType<PuzzleManager>();
            
            if (aiController == null)
                aiController = FindFirstObjectByType<LexiconAIController>();
            
            if (smartAgent == null)
                smartAgent = FindFirstObjectByType<NeocortexSmartAgent>();
            
            // Get current puzzle
            PuzzleData currentPuzzle = GameManager.Instance?.GetCurrentPuzzle();
            
            if (currentPuzzle == null)
            {
                Debug.LogError("No current puzzle found! Make sure GameManager is initialized with a PuzzleDatabase.");
                return;
            }
            
            // Set the project ID on the smart agent
            if (smartAgent != null && !string.IsNullOrEmpty(currentPuzzle.CharacterProjectId))
            {
                var field = typeof(NeocortexSmartAgent).GetField("projectId", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    field.SetValue(smartAgent, currentPuzzle.CharacterProjectId);
                    Debug.Log($"Set SmartAgent Project ID to: {currentPuzzle.CharacterProjectId}");
                }
            }
            
            Debug.Log($"Puzzle Scene setup complete for: {currentPuzzle.PuzzleName}");
        }
        
        private void OnValidate()
        {
            // Auto-find components in editor
            if (puzzleManager == null)
                puzzleManager = FindFirstObjectByType<PuzzleManager>();
            
            if (aiController == null)
                aiController = FindFirstObjectByType<LexiconAIController>();
            
            if (smartAgent == null)
                smartAgent = FindFirstObjectByType<NeocortexSmartAgent>();
        }
    }
}

