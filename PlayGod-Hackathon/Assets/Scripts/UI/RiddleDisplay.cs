using UnityEngine;
using TMPro;
using Lexicon.Managers;
using Lexicon.Data;
using System.Collections.Generic;

namespace Lexicon.UI
{
    /// <summary>
    /// Displays the puzzle riddle with word mappings highlighted in a customizable color
    /// </summary>
    public class RiddleDisplay : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI riddleText;
        
        [Header("Highlight Settings")]
        [SerializeField] private Color highlightColor = new Color(1f, 0.84f, 0f); // Gold by default
        [SerializeField] private bool boldHighlightedWords = true;
        [SerializeField] private bool underlineHighlightedWords = false;
        [SerializeField] private float highlightSizeMultiplier = 1.1f;
        
        [Header("Animation")]
        [SerializeField] private bool animateOnDisplay = true;
        [SerializeField] private float fadeInDuration = 1f;
        
        private PuzzleManager puzzleManager;
        private string originalRiddle;
        private List<string> highlightWords = new List<string>();
        
        private void Start()
        {
            // Delay to ensure PuzzleManager has loaded
            StartCoroutine(InitializeDisplay());
        }
        
        private System.Collections.IEnumerator InitializeDisplay()
        {
            // Wait for PuzzleManager to initialize
            yield return new WaitForSeconds(0.2f);
            
            puzzleManager = FindFirstObjectByType<PuzzleManager>();
            
            if (puzzleManager == null)
            {
                Debug.LogError("RiddleDisplay: PuzzleManager not found in scene! Make sure GameManager has PuzzleManager component.");
                yield break;
            }
            
            // Wait for puzzle to load (try a few times)
            for (int i = 0; i < 10; i++)
            {
                if (puzzleManager.CurrentPuzzle != null)
                {
                    Debug.Log($"RiddleDisplay: Found puzzle '{puzzleManager.CurrentPuzzle.PuzzleName}'");
                    DisplayRiddle();
                    yield break;
                }
                
                yield return new WaitForSeconds(0.1f);
            }
            
            Debug.LogError("RiddleDisplay: CurrentPuzzle is still null after waiting! Make sure:\n" +
                "1. GameManager GameObject exists in scene\n" +
                "2. GameManager has PuzzleDatabase assigned in Inspector\n" +
                "3. PuzzleDatabase asset has puzzles in it");
        }
        
        public void DisplayRiddle()
        {
            if (riddleText == null)
            {
                Debug.LogError("RiddleDisplay: Riddle Text (TextMeshProUGUI) is not assigned in Inspector!");
                return;
            }
            
            if (puzzleManager == null || puzzleManager.CurrentPuzzle == null)
            {
                Debug.LogError("Cannot display riddle: PuzzleManager or CurrentPuzzle is null");
                return;
            }
            
            PuzzleData puzzle = puzzleManager.CurrentPuzzle;
            originalRiddle = puzzle.RiddleSentence;
            
            Debug.Log($"Original Riddle: {originalRiddle}");
            
            // Get all words that need highlighting
            highlightWords.Clear();
            foreach (var mapping in puzzle.WordMappings)
            {
                highlightWords.Add(mapping.riddleWord.ToLower());
                Debug.Log($"Word to highlight: '{mapping.riddleWord}'");
            }
            
            // Create highlighted version
            string highlightedRiddle = HighlightWords(originalRiddle, highlightWords);
            
            Debug.Log($"Highlighted Riddle: {highlightedRiddle}");
            
            if (riddleText != null)
            {
                // Enable rich text if not already enabled
                riddleText.richText = true;
                
                if (animateOnDisplay)
                {
                    StartCoroutine(AnimateRiddleDisplay(highlightedRiddle));
                }
                else
                {
                    riddleText.text = highlightedRiddle;
                }
                
                Debug.Log("Riddle text updated successfully!");
            }
        }
        
        private string HighlightWords(string sentence, List<string> wordsToHighlight)
        {
            string result = sentence;
            string colorHex = ColorUtility.ToHtmlStringRGB(highlightColor);
            
            // Process each word to highlight
            foreach (string word in wordsToHighlight)
            {
                // Create the highlight tags
                string openTags = $"<color=#{colorHex}>";
                
                if (boldHighlightedWords)
                    openTags += "<b>";
                
                if (underlineHighlightedWords)
                    openTags += "<u>";
                
                if (highlightSizeMultiplier != 1f)
                {
                    int sizePercent = Mathf.RoundToInt(highlightSizeMultiplier * 100);
                    openTags += $"<size={sizePercent}%>";
                }
                
                // Create close tags (reverse order)
                string closeTags = "";
                
                if (highlightSizeMultiplier != 1f)
                    closeTags += "</size>";
                
                if (underlineHighlightedWords)
                    closeTags = "</u>" + closeTags;
                
                if (boldHighlightedWords)
                    closeTags = "</b>" + closeTags;
                
                closeTags = "</color>" + closeTags;
                
                // Replace word with highlighted version (case-insensitive)
                // Use word boundaries to avoid partial matches
                result = System.Text.RegularExpressions.Regex.Replace(
                    result,
                    $@"\b{word}\b",
                    $"{openTags}{word}{closeTags}",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase
                );
            }
            
            return result;
        }
        
        private System.Collections.IEnumerator AnimateRiddleDisplay(string text)
        {
            if (riddleText == null)
                yield break;
            
            riddleText.text = text;
            
            // Fade in animation
            Color startColor = riddleText.color;
            startColor.a = 0;
            riddleText.color = startColor;
            
            float elapsed = 0f;
            while (elapsed < fadeInDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
                Color newColor = riddleText.color;
                newColor.a = alpha;
                riddleText.color = newColor;
                yield return null;
            }
            
            // Ensure fully visible
            Color finalColor = riddleText.color;
            finalColor.a = 1f;
            riddleText.color = finalColor;
        }
        
        /// <summary>
        /// Updates the riddle display with a new puzzle
        /// </summary>
        public void RefreshRiddle()
        {
            DisplayRiddle();
        }
        
        /// <summary>
        /// Change highlight color at runtime
        /// </summary>
        public void SetHighlightColor(Color newColor)
        {
            highlightColor = newColor;
            DisplayRiddle(); // Refresh display with new color
        }
        
        /// <summary>
        /// Show only the original riddle without highlights
        /// </summary>
        public void ShowPlainRiddle()
        {
            if (riddleText != null && !string.IsNullOrEmpty(originalRiddle))
            {
                riddleText.text = originalRiddle;
            }
        }
        
        /// <summary>
        /// Toggle highlights on/off
        /// </summary>
        public void ToggleHighlights(bool show)
        {
            if (show)
                DisplayRiddle();
            else
                ShowPlainRiddle();
        }
    }
}

