using System.Collections;
using TMPro;
using UnityEngine;

namespace UI
{
    public class InfoTextPopup : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textField;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float floatSpeed = 30f;
        [SerializeField] private float fadeDuration = 1f;
        [SerializeField] private RectTransform rectTransform;
        
        private string _poolKey;

        public void Initialize(string message, string poolKey)
        {
            _poolKey = poolKey;
            rectTransform.anchoredPosition = Vector2.zero;
            textField.text = message;
            canvasGroup.alpha = 1f;

            gameObject.SetActive(true);
            StartCoroutine(AnimateAndReturn());
        }

        private IEnumerator AnimateAndReturn()
        {
            yield return new WaitForSeconds(0.5f);
            float elapsed = 0f;
            Vector3 start = transform.position;

            while (elapsed < fadeDuration)
            {
                float t = elapsed / fadeDuration;
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
                transform.position = start + Vector3.up * (floatSpeed * t / 100f);
                elapsed += Time.deltaTime;
                yield return null;
            }

            canvasGroup.alpha = 0f;
            gameObject.SetActive(false);
            Managers.ObjectPoolManager.Instance.Return(_poolKey, gameObject);
        }
    }
}