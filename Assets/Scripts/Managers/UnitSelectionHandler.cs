using System.Collections.Generic;
using Core.Input;
using Core.Interfaces;
using Data.Units;
using UnityEngine;
using Utilities;

namespace Managers
{
    public class UnitSelectionHandler : MonoBehaviour
    {
        [SerializeField] private RectTransform selectionBox;
        [SerializeField] private LayerMask unitLayer;

        private Vector2 _startPos;
        private Vector2 _endPos;
        private Camera _cam;

        private readonly List<ISelectable> _currentlySelected = new();

        public IReadOnlyList<ISelectable> GetSelected() => _currentlySelected;

        private void Start()
        {
            _cam = Camera.main;
            selectionBox.gameObject.SetActive(false);

            ClickInputRouter.Instance.OnLeftClickDown += OnClickStart;
            ClickInputRouter.Instance.OnLeftClickUp += OnClickEnd;
        }

        private void Update()
        {
            if (selectionBox.gameObject.activeSelf)
            {
                _endPos = Input.mousePosition;
                UpdateSelectionBox(_startPos, _endPos);
            }
        }

        private void UpdateSelectionBox(Vector2 start, Vector2 end)
        {
            Vector2 size = end - start;

            selectionBox.anchoredPosition = start;
            selectionBox.sizeDelta = new Vector2(Mathf.Abs(size.x), Mathf.Abs(size.y));
            selectionBox.anchoredPosition += new Vector2(size.x > 0 ? 0 : size.x, size.y > 0 ? 0 : size.y);
        }

        private void OnClickStart(Vector3 _)
        {
            _startPos = Input.mousePosition;
            selectionBox.sizeDelta = Vector2.zero;
            selectionBox.gameObject.SetActive(true);
        }

        private void OnClickEnd(Vector3 _)
        {
            _endPos = Input.mousePosition;

            if (Vector2.Distance(_startPos, _endPos) < 10f)
            {
                selectionBox.gameObject.SetActive(false);
                return;
            }

            SelectUnitsInBox();
            selectionBox.gameObject.SetActive(false);
        }

        private void SelectUnitsInBox()
        {
            DeselectAll();

            Vector2 min = Vector2.Min(_startPos, _endPos);
            Vector2 max = Vector2.Max(_startPos, _endPos);

            foreach (var unit in UnitRegistry.AllUnits)
            {
                if (unit == null) continue;

                Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(_cam, unit.transform.position);
                if (screenPos.x >= min.x && screenPos.x <= max.x &&
                    screenPos.y >= min.y && screenPos.y <= max.y)
                {
                    Select(unit);
                }
            }
        }

        private void Select(GameObject go)
        {
            if (go.TryGetComponent<ISelectable>(out var selectable))
            {
                _currentlySelected.Add(selectable);
                selectable.OnSelect();

                if (go.TryGetComponent<UnitHealth>(out var health))
                {
                    health.OnDied -= HandleUnitDeath;
                    health.OnDied += HandleUnitDeath;
                }
            }
        }

        private void HandleUnitDeath(GameObject dead)
        {
            if (dead.TryGetComponent<ISelectable>(out var selectable) && _currentlySelected.Contains(selectable))
                _currentlySelected.Remove(selectable);

            if (SelectionManager.Instance.SelectedObject == dead)
                SelectionManager.Instance.Deselect();
        }

        public void DeselectAllPublic() => DeselectAll();

        private void DeselectAll()
        {
            foreach (var selectable in _currentlySelected)
                selectable?.OnDeselect();

            _currentlySelected.Clear();
        }

        public void SelectSingle(GameObject go)
        {
            DeselectAll();

            if (go.TryGetComponent<ISelectable>(out var selectable))
            {
                _currentlySelected.Add(selectable);
                selectable.OnSelect();

                if (go.TryGetComponent<UnitHealth>(out var health))
                {
                    health.OnDied -= HandleUnitDeath;
                    health.OnDied += HandleUnitDeath;
                }
            }

            SelectionManager.Instance.SelectObject(go);
        }
    }
}
