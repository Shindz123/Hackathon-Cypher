using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Lexicon.Managers;

namespace Lexicon.UI
{
    public class WordMappingPanel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject mappingEntryPrefab;
        [SerializeField] private Transform mappingContainer;
        [SerializeField] private Button addMappingButton;
        
        [Header("Add Mapping UI")]
        [SerializeField] private GameObject addMappingPanel;
        [SerializeField] private TMP_InputField riddleWordInput;
        [SerializeField] private TMP_InputField actualMeaningInput;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;
        
        private PuzzleManager puzzleManager;
        private List<GameObject> mappingEntries = new List<GameObject>();
        
        private void Start()
        {
            puzzleManager = FindObjectOfType<PuzzleManager>();
            
            if (addMappingButton != null)
                addMappingButton.onClick.AddListener(ShowAddMappingPanel);
            
            if (confirmButton != null)
                confirmButton.onClick.AddListener(ConfirmAddMapping);
            
            if (cancelButton != null)
                cancelButton.onClick.AddListener(HideAddMappingPanel);
            
            if (addMappingPanel != null)
                addMappingPanel.SetActive(false);
        }
        
        public void ShowAddMappingPanel()
        {
            if (addMappingPanel != null)
            {
                addMappingPanel.SetActive(true);
                riddleWordInput.text = "";
                actualMeaningInput.text = "";
                riddleWordInput.Select();
            }
        }
        
        public void HideAddMappingPanel()
        {
            if (addMappingPanel != null)
                addMappingPanel.SetActive(false);
        }
        
        public void ConfirmAddMapping()
        {
            string riddleWord = riddleWordInput.text.Trim();
            string actualMeaning = actualMeaningInput.text.Trim();
            
            if (string.IsNullOrEmpty(riddleWord) || string.IsNullOrEmpty(actualMeaning))
            {
                Debug.LogWarning("Both fields must be filled");
                return;
            }
            
            AddMapping(riddleWord, actualMeaning);
            HideAddMappingPanel();
        }
        
        public void AddMapping(string riddleWord, string actualMeaning)
        {
            if (puzzleManager != null)
            {
                puzzleManager.AddPlayerMapping(riddleWord, actualMeaning);
            }
            
            // Check if mapping already exists in UI
            foreach (var entry in mappingEntries)
            {
                WordMappingEntry mappingEntry = entry.GetComponent<WordMappingEntry>();
                if (mappingEntry != null && mappingEntry.RiddleWord.ToLower() == riddleWord.ToLower())
                {
                    // Update existing entry
                    mappingEntry.UpdateMapping(riddleWord, actualMeaning);
                    CheckMappingCorrectness(mappingEntry);
                    return;
                }
            }
            
            // Create new entry
            if (mappingEntryPrefab != null && mappingContainer != null)
            {
                GameObject entryObj = Instantiate(mappingEntryPrefab, mappingContainer);
                WordMappingEntry entry = entryObj.GetComponent<WordMappingEntry>();
                
                if (entry != null)
                {
                    entry.Initialize(riddleWord, actualMeaning, this);
                    CheckMappingCorrectness(entry);
                    mappingEntries.Add(entryObj);
                }
            }
        }
        
        public void RemoveMapping(GameObject entryObj)
        {
            if (mappingEntries.Contains(entryObj))
            {
                mappingEntries.Remove(entryObj);
                Destroy(entryObj);
            }
        }
        
        private void CheckMappingCorrectness(WordMappingEntry entry)
        {
            if (puzzleManager != null)
            {
                bool isCorrect = puzzleManager.IsMappingCorrect(entry.RiddleWord, entry.ActualMeaning);
                entry.SetCorrectness(isCorrect);
            }
        }
        
        public Dictionary<string, string> GetAllMappings()
        {
            Dictionary<string, string> mappings = new Dictionary<string, string>();
            
            foreach (var entryObj in mappingEntries)
            {
                WordMappingEntry entry = entryObj.GetComponent<WordMappingEntry>();
                if (entry != null)
                {
                    mappings[entry.RiddleWord] = entry.ActualMeaning;
                }
            }
            
            return mappings;
        }
        
        public void ClearAllMappings()
        {
            foreach (var entry in mappingEntries)
            {
                Destroy(entry);
            }
            mappingEntries.Clear();
        }
    }
    
    public class WordMappingEntry : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI riddleWordText;
        [SerializeField] private TextMeshProUGUI actualMeaningText;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Button deleteButton;
        
        [Header("Colors")]
        [SerializeField] private Color unknownColor = new Color(0.8f, 0.8f, 0.8f);
        [SerializeField] private Color correctColor = new Color(0.5f, 1f, 0.5f);
        [SerializeField] private Color incorrectColor = new Color(1f, 0.5f, 0.5f);
        
        private string riddleWord;
        private string actualMeaning;
        private WordMappingPanel parentPanel;
        private bool? isCorrect = null;
        
        public string RiddleWord => riddleWord;
        public string ActualMeaning => actualMeaning;
        
        public void Initialize(string riddle, string meaning, WordMappingPanel panel)
        {
            riddleWord = riddle;
            actualMeaning = meaning;
            parentPanel = panel;
            
            UpdateDisplay();
            
            if (deleteButton != null)
                deleteButton.onClick.AddListener(OnDeleteClicked);
        }
        
        public void UpdateMapping(string riddle, string meaning)
        {
            riddleWord = riddle;
            actualMeaning = meaning;
            isCorrect = null;
            UpdateDisplay();
        }
        
        private void UpdateDisplay()
        {
            if (riddleWordText != null)
                riddleWordText.text = riddleWord;
            
            if (actualMeaningText != null)
                actualMeaningText.text = actualMeaning;
            
            UpdateBackgroundColor();
        }
        
        public void SetCorrectness(bool correct)
        {
            isCorrect = correct;
            UpdateBackgroundColor();
        }
        
        private void UpdateBackgroundColor()
        {
            if (backgroundImage == null)
                return;
            
            if (isCorrect == null)
                backgroundImage.color = unknownColor;
            else if (isCorrect.Value)
                backgroundImage.color = correctColor;
            else
                backgroundImage.color = incorrectColor;
        }
        
        private void OnDeleteClicked()
        {
            if (parentPanel != null)
                parentPanel.RemoveMapping(gameObject);
        }
    }
}

