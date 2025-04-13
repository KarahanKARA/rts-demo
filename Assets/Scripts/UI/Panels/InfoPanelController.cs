using Core.Health;
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
        [SerializeField] private GameObject scrollView;
        [SerializeField] private TextMeshProUGUI informationText;
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private Slider healthSlider;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI attackText;

        private HealthBase _currentHealth;
        private GameObject _selected;

        private void Start()
        {
            SelectionManager.Instance.OnSelectedChanged += OnSelectionChanged;
            SelectionManager.Instance.UnitSelector.OnMultipleSelection += HandleMultipleSelection;

            panelRoot.SetActive(false);
        }

        private void OnSelectionChanged(GameObject selected)
        {
            if (_currentHealth != null)
            {
                _currentHealth.OnHealthChanged.RemoveListener(UpdateHealthBar);
                _currentHealth = null;
            }

            _selected = selected;

            var selectedUnits = SelectionManager.Instance.UnitSelector.GetSelected();

            if (selectedUnits.Count > 1)
            {
                ShowMultipleSelectionInfo(selectedUnits.Count);
                return;
            }

            if (selected == null)
            {
                ClosePanel();
                return;
            }

            ShowSingleSelectionInfo(selected);
        }

        private void HandleMultipleSelection(int count)
        {
            ShowMultipleSelectionInfo(count);
        }

        private void ShowMultipleSelectionInfo(int unitCount)
        {
            panelRoot.SetActive(true);
            scrollView.SetActive(false);
            iconImage.enabled = false;
            nameText.text = "";
            attackText.gameObject.SetActive(false);
            healthText.text = "";
            healthSlider.value = 0;

            informationText.text = $"{unitCount} soldier(s) selected";
            informationText.gameObject.SetActive(true);
        }

        private void ShowSingleSelectionInfo(GameObject selected)
        {
            scrollView.SetActive(true);
            iconImage.enabled = true;
            informationText.gameObject.SetActive(false);
            attackText.gameObject.SetActive(false);

            if (_currentHealth != null)
            {
                _currentHealth.OnHealthChanged.RemoveListener(UpdateHealthBar);
                _currentHealth.OnDied.RemoveListener(OnSelectedDied);
                _currentHealth = null;
            }

            Sprite icon = null;
            string nameStr = "";
            _currentHealth = null;

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

                attackText.text = $"ATK: {unitData.Data.damage}";
                attackText.gameObject.SetActive(true);
            }
            else
            {
                ClosePanel();
                return;
            }

            if (_currentHealth == null)
            {
                ClosePanel();
                return;
            }

            iconImage.sprite = icon;
            nameText.text = nameStr;
            healthSlider.maxValue = _currentHealth.MaxHealth;
            healthSlider.value = _currentHealth.CurrentHealth;
            healthText.text = $"{_currentHealth.CurrentHealth} / {_currentHealth.MaxHealth}";

            _currentHealth.OnHealthChanged.AddListener(UpdateHealthBar);
            _currentHealth.OnDied.AddListener(OnSelectedDied);

            panelRoot.SetActive(true);
        }

        private void UpdateHealthBar(int current, int max)
        {
            var selectedUnits = SelectionManager.Instance.UnitSelector.GetSelected();
            if (selectedUnits.Count > 1 || 
                (SelectionManager.Instance.SelectedObject != null && 
                 SelectionManager.Instance.SelectedObject != _selected))
            {
                if (_currentHealth != null)
                {
                    _currentHealth.OnHealthChanged.RemoveListener(UpdateHealthBar);
                    _currentHealth = null;
                }
                return;
            }

            if (_currentHealth == null || _selected == null)
                return;

            if (!_selected.TryGetComponent(out HealthBase currentSelectedHealth))
                return;

            if (currentSelectedHealth != _currentHealth)
                return;

            healthSlider.value = current;
            healthText.text = $"{current} / {max}";
        }

        private void OnSelectedDied()
        {
            if (_selected != null && SelectionManager.Instance.SelectedObject == _selected)
            {
                ClosePanel();
                SelectionManager.Instance.Deselect();
            }
        }

        private void ClosePanel()
        {
            panelRoot.SetActive(false);
            scrollView.SetActive(true);
            iconImage.enabled = true;
            informationText.text = "";
            _currentHealth = null;
            _selected = null;
        }

        public void OnClickDestroy()
        {
            var selectedUnits = SelectionManager.Instance.UnitSelector.GetSelected();

            if (selectedUnits.Count > 1)
            {
                foreach (var selectable in selectedUnits)
                {
                    if (selectable is MonoBehaviour mb &&
                        mb.TryGetComponent<UnitHealth>(out var unitHealth))
                    {
                        unitHealth.TakeDamage(unitHealth.MaxHealth);
                    }
                }

                SelectionManager.Instance.UnitSelector.DeselectAllPublic();
                SelectionManager.Instance.Deselect();
                ClosePanel();
                return;
            }

            var obj = SelectionManager.Instance.SelectedObject;

            if (obj == null)
            {
                ClosePanel();
                return;
            }

            if (obj.TryGetComponent<UnitHealth>(out var unit))
            {
                unit.TakeDamage(unit.MaxHealth);
            }
            else if (obj.TryGetComponent<BuildingHealth>(out var buildingHealth))
            {
                buildingHealth.DestroyBuilding();
            }
            else
            {
                Destroy(obj);
            }

            SelectionManager.Instance.Deselect();
            ClosePanel();
        }
    }
}
