using UnityEngine;
using UnityEngine.UI;
using Neocortex.Data;

namespace Lexicon.UI
{
    public class CharacterPortraitController : MonoBehaviour
    {
        [Header("Portrait Images")]
        [SerializeField] private Image portraitImage;
        [SerializeField] private Sprite neutralSprite;
        [SerializeField] private Sprite happySprite;
        [SerializeField] private Sprite sadSprite;
        [SerializeField] private Sprite angrySprite;
        [SerializeField] private Sprite surprisedSprite;
        [SerializeField] private Sprite mysteriousSprite;
        
        [Header("Animation")]
        [SerializeField] private float transitionSpeed = 0.3f;
        [SerializeField] private bool useFade = true;
        
        private Emotions currentEmotion = Emotions.Neutral;
        
        private void Start()
        {
            SetEmotion(Emotions.Neutral);
        }
        
        public void SetEmotion(Emotions emotion)
        {
            if (emotion == currentEmotion)
                return;
            
            currentEmotion = emotion;
            
            Sprite targetSprite = GetSpriteForEmotion(emotion);
            
            if (targetSprite != null && portraitImage != null)
            {
                if (useFade)
                {
                    StartCoroutine(FadeToSprite(targetSprite));
                }
                else
                {
                    portraitImage.sprite = targetSprite;
                }
            }
        }
        
        private Sprite GetSpriteForEmotion(Emotions emotion)
        {
            switch (emotion)
            {
                case Emotions.Happy:
                case Emotions.Pleased:
                case Emotions.Amazed:
                    return happySprite ?? neutralSprite;
                    
                case Emotions.Disappointed:
                case Emotions.Upset:
                case Emotions.Concerned:
                    return sadSprite ?? neutralSprite;
                    
                case Emotions.Angry:
                case Emotions.Annoyed:
                case Emotions.Alarmed:
                    return angrySprite ?? neutralSprite;
                    
                case Emotions.Fascinated:
                case Emotions.Impressed:
                case Emotions.Confident:
                    return surprisedSprite ?? neutralSprite;
                    
                case Emotions.Curious:
                case Emotions.Confused:
                case Emotions.Reassured:
                    return mysteriousSprite ?? neutralSprite;
                    
                case Emotions.Scared:
                    return sadSprite ?? neutralSprite;
                    
                default:
                    return neutralSprite;
            }
        }
        
        private System.Collections.IEnumerator FadeToSprite(Sprite newSprite)
        {
            float elapsed = 0f;
            Color startColor = portraitImage.color;
            
            // Fade out
            while (elapsed < transitionSpeed / 2)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsed / (transitionSpeed / 2));
                portraitImage.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
                yield return null;
            }
            
            // Change sprite
            portraitImage.sprite = newSprite;
            
            // Fade in
            elapsed = 0f;
            while (elapsed < transitionSpeed / 2)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(0f, 1f, elapsed / (transitionSpeed / 2));
                portraitImage.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
                yield return null;
            }
            
            portraitImage.color = startColor;
        }
        
        public void SetPortraitSprite(Sprite sprite)
        {
            if (portraitImage != null && sprite != null)
                portraitImage.sprite = sprite;
        }
    }
}

