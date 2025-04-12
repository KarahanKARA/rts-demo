using Data.Buildings;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels
{
    public class InfoPanelController : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private Slider healthSlider;
        [SerializeField] private TextMeshProUGUI healthText;

        private BuildingHealth _currentHealth;

        private void Start()
        {
            SelectionManager.Instance.OnSelectedChanged += OnSelectionChanged;
            panelRoot.SetActive(false);
        }

        private void OnSelectionChanged(GameObject selected)
        {
            if (selected == null)
            {
                ClosePanel();
                return;
            }

            if (!selected.TryGetComponent(out BuildingDataHolder holder))
            {
                ClosePanel();
                return;
            }

            var data = holder.Data;
            _currentHealth = holder.Health;

            iconImage.sprite = data.icon;
            nameText.text = data.buildingName;
            healthSlider.maxValue = _currentHealth.MaxHealth;
            healthSlider.value = _currentHealth.CurrentHealth;

            UpdateHealthBar(_currentHealth.CurrentHealth, _currentHealth.MaxHealth);

            _currentHealth.OnHealthChanged.RemoveListener(UpdateHealthBar);
            _currentHealth.OnHealthChanged.AddListener(UpdateHealthBar);

            panelRoot.SetActive(true);
        }

        private void UpdateHealthBar(int current, int max)
        {
            healthSlider.value = current;
            healthText.text = $"{current} / {max}";
        }

        private void ClosePanel()
        {
            panelRoot.SetActive(false);
            _currentHealth = null;
        }
        
        public void OnClickDestroy()
        {
            if (SelectionManager.Instance.SelectedObject == null)
                return;

            var obj = SelectionManager.Instance.SelectedObject;

            if (obj.TryGetComponent(out BuildingHealth health))
            {
                health.DestroyBuilding();
            }
            else
            {
                Destroy(obj);
            }

            SelectionManager.Instance.Deselect(); 
            panelRoot.SetActive(false);
        }
    }
}
