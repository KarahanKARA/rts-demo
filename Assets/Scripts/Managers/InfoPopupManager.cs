using Core.Enums;
using UI;
using UnityEngine;

namespace Managers
{
    public class InfoPopupManager : MonoBehaviour
    {
        [SerializeField] private GameObject popupTextPrefab;
        [SerializeField] private RectTransform parentRectTransform;
        private const string PopupKey = "InfoPopupTextPrefab";
        
        public static InfoPopupManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }
        
        private void Start()
        {
            ObjectPoolManager.Instance.Preload(popupTextPrefab, 5, parentRectTransform);
        }

        public void ShowPopup(InfoPopupType type)
        {
            string message = GetMessage(type);

            GameObject go = ObjectPoolManager.Instance.Get(PopupKey, popupTextPrefab, parentRectTransform);
            go.SetActive(true);

            var popup = go.GetComponent<InfoTextPopup>();
            popup.Initialize(message, PopupKey);
        }

        private string GetMessage(InfoPopupType type)
        {
            return type switch
            {
                InfoPopupType.InvalidSpawnPoint => "Spawn point cannot be placed here.",
                InfoPopupType.ObstructedSpawnArea => "A unit cannot be spawned at this location. Please choose a valid spawn point.",
                _ => "Unknown popup message."
            };
        }
    }
}