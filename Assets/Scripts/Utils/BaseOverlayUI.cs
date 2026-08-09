using EscapeDungeon.GamePlay;
using UnityEngine;

namespace EscapeDungeon.Ui
{
    public class BaseOverlayUI : MonoBehaviour
    {
        [Header("Base References")]
        [SerializeField] protected CanvasGroup canvasGroup;

        protected bool isVisible = false;
        protected float fadeDuration = 0.5f;

        protected virtual void Awake()
        {
            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();

            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            isVisible = false;
            gameObject.SetActive(false);
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);

            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            StopAllCoroutines();
            StartCoroutine(FadeIn());
        }

        public virtual void Hide()
        {
            StopAllCoroutines();
            StartCoroutine(FadeOut());
        }

        protected System.Collections.IEnumerator FadeIn()
        {
            isVisible = true;

            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                float k = t / fadeDuration;
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, k);
                yield return null;
            }

            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        protected System.Collections.IEnumerator FadeOut()
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;

            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                float k = t / fadeDuration;
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, k);
                yield return null;
            }

            canvasGroup.alpha = 0f;
            isVisible = false;
            gameObject.SetActive(false);
        }

        protected string FormatTime(float duration)
        {
            int mins = Mathf.FloorToInt(duration / 60);
            int secs = Mathf.FloorToInt(duration % 60);
            return $"{mins}m{secs}s";
        }
    }
}
