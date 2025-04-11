using Data.Buildings;
using Data.Units;
using Managers;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Panels
{
    public class ProductionPanelController : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private RectTransform contentParent;
        [SerializeField] private GameObject buttonPrefab;

        private readonly List<ProductionButtonUI> _buttons = new();

        private void Start()
        {
            SelectionManager.Instance.OnSelectedChanged += OnSelectedChanged;
            panelRoot.SetActive(false);
            CreateAllButtons();
        }

        private void CreateAllButtons()
        {
            var unitList = Resources.LoadAll<UnitData>("ScriptableObjects/Units");

            foreach (var unit in unitList)
            {
                var go = Instantiate(buttonPrefab, contentParent);
                var buttonUI = go.GetComponent<ProductionButtonUI>();
                buttonUI.Setup(unit, OnProduceClicked);
                go.SetActive(false); 
                _buttons.Add(buttonUI);
            }
        }

        private void OnSelectedChanged(GameObject selected)
        {
            bool canProduce = selected != null &&
                              selected.TryGetComponent(out BuildingDataHolder holder) &&
                              holder.Data is BarracksData;

            panelRoot.SetActive(canProduce);

            foreach (var btn in _buttons)
            {
                btn.gameObject.SetActive(canProduce);
            }
        }

        private void OnProduceClicked(UnitData data)
        {
            if (!SelectionManager.Instance.SelectedObject.TryGetComponent(out UnitProducer producer))
            {
                return;
            }

            producer.Produce(data);
        }
    }
}