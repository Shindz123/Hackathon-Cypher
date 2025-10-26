using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Neocortex.Data;
using Lexicon.Managers;
using Lexicon.Data;

namespace Lexicon.UI
{
    public class VisualNovelUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private AICharacterSettings characterSettings;
        
        [Header("Portrait")]
        [SerializeField] private CharacterPortraitController portraitController;
        
        [Header("Dialogue")]
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private TextMeshProUGUI characterNameText;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private float typewriterSpeed = 0.05f;
        
        [Header("Stats")]
        [SerializeField] private TextMeshProUGUI questionsCountText;
        [SerializeField] private TextMeshProUGUI scoreTierText;
        
        [Header("Riddle Display")]
        [SerializeField] private RiddleDisplay riddleDisplay;
        [SerializeField] private TextMeshProUGUI riddleText; // Legacy fallback
        
        private bool isTyping = false;
        private string fullText = "";
        private Coroutine typewriterCoroutine;
        
        private void Start()
        {
            // Auto-load character settings if not assigned
            if (characterSettings == null)
            {
                characterSettings = Resources.Load<AICharacterSettings>("AICharacterSettings");
            }
            
            // Set character name
            if (characterNameText != null)
            {
                if (characterSettings != null)
                {
                    characterNameText.text = characterSettings.CharacterName;
                    characterNameText.color = characterSettings.NameColor;
                }
                else
                {
                    characterNameText.text = "Lexara";
                }
            }
            
            // Set typewriter speed from settings
            if (characterSettings != null)
            {
                typewriterSpeed = characterSettings.TypewriterSpeed;
            }
            
            // Initialize display
            UpdateQuestionsCount(0);
            UpdateScoreTier(ScoreTier.Perfect);
        }
        
        public void DisplayMessage(string message, Emotions emotion = Emotions.Neutral)
        {
            // Update portrait based on emotion
            if (portraitController != null)
                portraitController.SetEmotion(emotion);
            
            // Display text with typewriter effect
            if (dialoguePanel != null)
                dialoguePanel.SetActive(true);
            
            fullText = message;
            
            if (typewriterCoroutine != null)
                StopCoroutine(typewriterCoroutine);
            
            typewriterCoroutine = StartCoroutine(TypewriterEffect());
        }
        
        private System.Collections.IEnumerator TypewriterEffect()
        {
            isTyping = true;
            dialogueText.text = "";
            
            foreach (char c in fullText)
            {
                dialogueText.text += c;
                yield return new WaitForSeconds(typewriterSpeed);
            }
            
            isTyping = false;
        }
        
        public void SkipTypewriter()
        {
            if (isTyping && typewriterCoroutine != null)
            {
                StopCoroutine(typewriterCoroutine);
                dialogueText.text = fullText;
                isTyping = false;
            }
        }
        
        public void UpdateQuestionsCount(int count)
        {
            if (questionsCountText != null)
                questionsCountText.text = $"Questions: {count}";
        }
        
        public void UpdateScoreTier(ScoreTier tier)
        {
            if (scoreTierText != null)
            {
                string tierText = "";
                Color tierColor = Color.white;
                
                switch (tier)
                {
                    case ScoreTier.Perfect:
                        tierText = "Perfect Path";
                        tierColor = new Color(1f, 0.84f, 0f); // Gold
                        break;
                    case ScoreTier.Good:
                        tierText = "Good Progress";
                        tierColor = new Color(0.75f, 0.75f, 1f); // Light blue
                        break;
                    case ScoreTier.Pass:
                        tierText = "Acceptable";
                        tierColor = new Color(0.5f, 1f, 0.5f); // Light green
                        break;
                    case ScoreTier.NeedsImprovement:
                        tierText = "Needs Focus";
                        tierColor = new Color(1f, 0.5f, 0.5f); // Light red
                        break;
                }
                
                scoreTierText.text = tierText;
                scoreTierText.color = tierColor;
            }
        }
        
        public void SetRiddleText(string riddle)
        {
            // Use RiddleDisplay if available (with highlighting)
            if (riddleDisplay != null)
            {
                riddleDisplay.RefreshRiddle();
            }
            // Fallback to plain text display
            else if (riddleText != null)
            {
                riddleText.text = riddle;
            }
        }
        
        public void HideDialoguePanel()
        {
            if (dialoguePanel != null)
                dialoguePanel.SetActive(false);
        }
        
        public void ShowDialoguePanel()
        {
            if (dialoguePanel != null)
                dialoguePanel.SetActive(true);
        }
    }
}

