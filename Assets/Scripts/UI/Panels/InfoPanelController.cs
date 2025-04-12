using Data.Buildings;
using Data.Units;
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
        [SerializeField] private TextMeshProUGUI attackText;

        private Core.Health.HealthBase _currentHealth;

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

            Sprite icon = null;
            string nameStr = "";

            attackText.gameObject.SetActive(false);

            if (selected.TryGetComponent(out BuildingDataHolder buildingData))
            {
                icon = buildingData.Data.icon;
                nameStr = buildingData.Data.buildingName;
                _currentHealth = buildingData.Health;
            }
            else if (selected.TryGetComponent(out UnitDataHolder unitData))
            {
                icon = unitData.Data.icon;
                nameStr = unitData.Data.unitName;
                _currentHealth = unitData.Health;

                attackText.text = "ATK:" + unitData.Data.damage;
                attackText.gameObject.SetActive(true);
            }
            else
            {
                ClosePanel();
                return;
            }

            iconImage.sprite = icon;
            nameText.text = nameStr;
            healthSlider.maxValue = _currentHealth.MaxHealth;
            healthSlider.value = _currentHealth.CurrentHealth;
            healthText.text = $"{_currentHealth.CurrentHealth} / {_currentHealth.MaxHealth}";

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

            if (obj.TryGetComponent(out BuildingHealth buildingHealth))
                buildingHealth.DestroyBuilding();
            else
                Destroy(obj);

            SelectionManager.Instance.Deselect();
            panelRoot.SetActive(false);
        }
    }
}
