using UnityEngine;
using Lexicon.Managers;
using Lexicon.Data;

namespace Lexicon.Utils
{
    /// <summary>
    /// Diagnostic tool to check if scene is set up correctly
    /// </summary>
    public class SceneDiagnostic : MonoBehaviour
    {
        [Header("Run Diagnostic")]
        [SerializeField] private bool runOnStart = true;
        
        private void Start()
        {
            if (runOnStart)
            {
                Invoke(nameof(RunDiagnostic), 0.5f);
            }
        }
        
        [ContextMenu("Run Scene Diagnostic")]
        public void RunDiagnostic()
        {
            Debug.Log("=== LEXICON SCENE DIAGNOSTIC ===");
            
            bool allGood = true;
            
            // Check 1: GameManager
            GameManager gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager == null)
            {
                Debug.LogError("❌ CRITICAL: No GameManager found in scene!");
                Debug.LogError("   FIX: Create GameObject → Add GameManager component");
                allGood = false;
            }
            else
            {
                Debug.Log("✅ GameManager found");
                
                // Check PuzzleDatabase
                if (gameManager.Database == null)
                {
                    Debug.LogError("❌ CRITICAL: GameManager has no PuzzleDatabase assigned!");
                    Debug.LogError("   FIX: Select GameManager → Assign PuzzleDatabase in Inspector");
                    allGood = false;
                }
                else
                {
                    Debug.Log($"✅ PuzzleDatabase assigned ({gameManager.Database.TotalPuzzles} puzzles)");
                    
                    if (gameManager.Database.TotalPuzzles == 0)
                    {
                        Debug.LogError("❌ PuzzleDatabase has 0 puzzles!");
                        Debug.LogError("   FIX: Open PuzzleDatabase asset → Add puzzle references");
                        allGood = false;
                    }
                }
            }
            
            // Check 2: PuzzleManager
            PuzzleManager puzzleManager = FindFirstObjectByType<PuzzleManager>();
            if (puzzleManager == null)
            {
                Debug.LogError("❌ CRITICAL: No PuzzleManager found in scene!");
                Debug.LogError("   FIX: Select GameManager → Add Component → PuzzleManager");
                allGood = false;
            }
            else
            {
                Debug.Log("✅ PuzzleManager found");
                
                if (puzzleManager.CurrentPuzzle == null)
                {
                    Debug.LogWarning("⚠️ PuzzleManager.CurrentPuzzle is null (may load after Start)");
                }
                else
                {
                    Debug.Log($"✅ Current Puzzle loaded: {puzzleManager.CurrentPuzzle.PuzzleName}");
                }
            }
            
            // Check 3: AIController
            var aiController = FindFirstObjectByType<Lexicon.AI.LexiconAIController>();
            if (aiController == null)
            {
                Debug.LogWarning("⚠️ No LexiconAIController found (AI won't respond)");
                Debug.LogWarning("   FIX: Create AIController GameObject → Add LexiconAIController + NeocortexSmartAgent");
            }
            else
            {
                Debug.Log("✅ LexiconAIController found");
            }
            
            // Check 4: Neocortex
            var smartAgent = FindFirstObjectByType<Neocortex.NeocortexSmartAgent>();
            if (smartAgent == null)
            {
                Debug.LogWarning("⚠️ No NeocortexSmartAgent found (AI won't respond)");
            }
            else
            {
                Debug.Log("✅ NeocortexSmartAgent found");
            }
            
            // Check 5: UI Components
            var chatController = FindFirstObjectByType<Lexicon.UI.LexiconChatController>();
            if (chatController == null)
            {
                Debug.LogWarning("⚠️ No LexiconChatController found");
            }
            else
            {
                Debug.Log("✅ LexiconChatController found");
            }
            
            // Check 6: EventSystem
            var eventSystem = FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>();
            if (eventSystem == null)
            {
                Debug.LogError("❌ No EventSystem found (UI won't work)");
                Debug.LogError("   FIX: Right-click Hierarchy → UI → Event System");
                allGood = false;
            }
            else
            {
                Debug.Log("✅ EventSystem found");
            }
            
            // Summary
            Debug.Log("=================================");
            if (allGood)
            {
                Debug.Log("✅ ALL CRITICAL CHECKS PASSED!");
                Debug.Log("Scene should work correctly.");
            }
            else
            {
                Debug.LogError("❌ CRITICAL ISSUES FOUND!");
                Debug.LogError("Fix the errors above before running the game.");
            }
            Debug.Log("=================================");
        }
    }
}

