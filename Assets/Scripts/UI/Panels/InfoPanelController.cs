using System.Linq;
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
            DetachHealthEvents();
            _selected = selected;

            var selectedUnits = SelectionManager.Instance.UnitSelector.GetSelected();
            if (selectedUnits.Count > 1)
            {
                if (selectedUnits.Count == 1 && selectedUnits[0] is MonoBehaviour mb)
                    ShowSingleSelectionInfo(mb.gameObject);
                else
                    ShowMultipleSelectionInfo(selectedUnits.Count);

                return;
            }

            if (_selected == null)
            {
                ClosePanel();
                return;
            }

            ShowSingleSelectionInfo(_selected);
        }

        private void HandleMultipleSelection(int count)
        {
            if (count == 1 && SelectionManager.Instance.UnitSelector.GetSelected()[0] is MonoBehaviour mb)
                ShowSingleSelectionInfo(mb.gameObject);
            else
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

            DetachHealthEvents();

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

            _currentHealth.OnHealthChanged += UpdateHealthBar;
            _currentHealth.OnDied += HandleDeath;

            panelRoot.SetActive(true);
        }

        private void UpdateHealthBar(int current, int max)
        {
            if (_currentHealth == null || _selected == null) return;
            if (SelectionManager.Instance.SelectedObject != _selected) return;

            healthSlider.value = current;
            healthText.text = $"{current} / {max}";
        }

        private void HandleDeath()
        {
            if (SelectionManager.Instance.SelectedObject == _selected)
            {
                SelectionManager.Instance.Deselect();
                ClosePanel();
            }
        }

        private void DetachHealthEvents()
        {
            if (_currentHealth != null)
            {
                _currentHealth.OnHealthChanged -= UpdateHealthBar;
                _currentHealth.OnDied -= HandleDeath;
            }

            _currentHealth = null;
        }

        private void ClosePanel()
        {
            panelRoot.SetActive(false);
            scrollView.SetActive(true);
            iconImage.enabled = true;
            informationText.text = "";
            DetachHealthEvents();
            _selected = null;
        }

        public void OnClickDestroy()
        {
            var selectedUnits = SelectionManager.Instance.UnitSelector.GetSelected();

            if (selectedUnits.Count > 1)
            {
                var toDestroy = selectedUnits.ToList();

                foreach (var selectable in toDestroy)
                {
                    if (selectable is MonoBehaviour mb && mb.TryGetComponent<UnitHealth>(out var unitHealth))
                        unitHealth.TakeDamage(unitHealth.MaxHealth);
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
                unit.TakeDamage(unit.MaxHealth);
            else if (obj.TryGetComponent<BuildingHealth>(out var building))
                building.DestroySelf();
            else
                Destroy(obj);

            SelectionManager.Instance.Deselect();
            ClosePanel();
        }

    }
}
