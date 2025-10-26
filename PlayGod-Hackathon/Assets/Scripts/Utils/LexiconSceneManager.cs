using UnityEngine;
using Lexicon.Managers;
using Lexicon.Data;

namespace Lexicon.Utils
{
    /// <summary>
    /// Master scene manager that ensures everything loads in correct order
    /// Attach this to a GameObject in your PuzzleScene for automatic setup
    /// </summary>
    public class LexiconSceneManager : MonoBehaviour
    {
        [Header("Required References - Assign These!")]
        [SerializeField] private PuzzleDatabase puzzleDatabase;
        
        [Header("Optional - Auto-Find if not assigned")]
        [SerializeField] private GameManager gameManager;
        [SerializeField] private PuzzleManager puzzleManager;
        
        [Header("Settings")]
        [SerializeField] private bool runDiagnosticOnStart = true;
        
        private void Awake()
        {
            // This runs BEFORE other Start() methods
            InitializeGameSystems();
        }
        
        private void Start()
        {
            if (runDiagnosticOnStart)
            {
                Invoke(nameof(RunDiagnostic), 0.5f);
            }
        }
        
        private void InitializeGameSystems()
        {
            Debug.Log("=== LEXICON SCENE INITIALIZING ===");
            
            // Step 1: Find or verify GameManager
            if (gameManager == null)
            {
                gameManager = FindFirstObjectByType<GameManager>();
            }
            
            if (gameManager == null)
            {
                Debug.LogError("❌ CRITICAL: No GameManager found in scene!");
                Debug.LogError("SOLUTION: Create GameObject named 'GameManager' → Add 'GameManager' component");
                return;
            }
            
            Debug.Log("✅ GameManager found");
            
            // Step 2: Assign PuzzleDatabase if we have one
            if (puzzleDatabase != null && gameManager.Database == null)
            {
                Debug.Log("Assigning PuzzleDatabase to GameManager...");
                
                // Use reflection to set the database
                var field = typeof(GameManager).GetField("puzzleDatabase", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    
                if (field != null)
                {
                    field.SetValue(gameManager, puzzleDatabase);
                    Debug.Log("✅ PuzzleDatabase assigned successfully!");
                }
            }
            else if (gameManager.Database == null)
            {
                Debug.LogError("❌ CRITICAL: GameManager has no PuzzleDatabase!");
                Debug.LogError("SOLUTION: Assign PuzzleDatabase in LexiconSceneManager or GameManager Inspector");
            }
            
            // Step 3: Verify PuzzleManager exists
            if (puzzleManager == null)
            {
                puzzleManager = FindFirstObjectByType<PuzzleManager>();
            }
            
            if (puzzleManager == null)
            {
                Debug.LogError("❌ CRITICAL: No PuzzleManager component found!");
                Debug.LogError("SOLUTION: Select GameManager GameObject → Add Component → PuzzleManager");
            }
            else
            {
                Debug.Log("✅ PuzzleManager found");
            }
            
            Debug.Log("=== INITIALIZATION COMPLETE ===");
        }
        
        private void RunDiagnostic()
        {
            Debug.Log("=== SCENE DIAGNOSTIC (Detailed) ===");
            
            // Check GameManager
            if (gameManager == null || GameManager.Instance == null)
            {
                Debug.LogError("❌ GameManager is NULL!");
                Debug.LogError("   Create: Empty GameObject → Add 'GameManager' component");
            }
            else
            {
                Debug.Log($"✅ GameManager Instance exists");
                
                if (gameManager.Database == null)
                {
                    Debug.LogError("❌ GameManager.Database is NULL!");
                    Debug.LogError("   Fix: Assign PuzzleDatabase in Inspector");
                }
                else
                {
                    Debug.Log($"✅ PuzzleDatabase assigned ({gameManager.Database.TotalPuzzles} puzzles)");
                    
                    var currentPuzzle = gameManager.GetCurrentPuzzle();
                    if (currentPuzzle == null)
                    {
                        Debug.LogError("❌ GetCurrentPuzzle() returned NULL!");
                        Debug.LogError($"   Current Index: {gameManager.CurrentPuzzleIndex}");
                        Debug.LogError($"   Total Puzzles: {gameManager.Database.TotalPuzzles}");
                    }
                    else
                    {
                        Debug.Log($"✅ Current Puzzle: {currentPuzzle.PuzzleName}");
                        Debug.Log($"   Riddle: {currentPuzzle.RiddleSentence}");
                        Debug.Log($"   Mappings: {currentPuzzle.WordMappings.Count} words");
                        Debug.Log($"   Neocortex ID: {currentPuzzle.CharacterProjectId}");
                    }
                }
            }
            
            // Check PuzzleManager
            if (puzzleManager == null)
            {
                Debug.LogError("❌ PuzzleManager is NULL!");
                Debug.LogError("   Fix: GameManager GameObject → Add Component → PuzzleManager");
            }
            else
            {
                Debug.Log($"✅ PuzzleManager exists");
                
                if (puzzleManager.CurrentPuzzle == null)
                {
                    Debug.LogError("❌ PuzzleManager.CurrentPuzzle is NULL!");
                }
                else
                {
                    Debug.Log($"✅ PuzzleManager.CurrentPuzzle: {puzzleManager.CurrentPuzzle.PuzzleName}");
                }
            }
            
            // Check UI Components
            var submissionPanel = FindFirstObjectByType<Lexicon.UI.SentenceSubmissionPanel>();
            if (submissionPanel != null)
            {
                Debug.Log("✅ SentenceSubmissionPanel found");
            }
            else
            {
                Debug.LogWarning("⚠️  SentenceSubmissionPanel not found");
            }
            
            var riddleDisplay = FindFirstObjectByType<Lexicon.UI.RiddleDisplay>();
            if (riddleDisplay != null)
            {
                Debug.Log("✅ RiddleDisplay found");
            }
            else
            {
                Debug.LogWarning("⚠️  RiddleDisplay not found");
            }
            
            Debug.Log("====================================");
        }
        
        [ContextMenu("Force Reload Puzzle")]
        public void ForceReloadPuzzle()
        {
            if (puzzleManager != null)
            {
                puzzleManager.LoadPuzzle();
            }
            else
            {
                Debug.LogError("PuzzleManager not found!");
            }
        }
    }
}

