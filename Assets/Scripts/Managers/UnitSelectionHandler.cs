using System.Collections.Generic;
using Core.Interfaces;
using UnityEngine;

namespace Managers
{
    public class UnitSelectionHandler : MonoBehaviour
    {
        [SerializeField] private RectTransform selectionBox;
        [SerializeField] private LayerMask unitLayer;

        private Vector2 _startPos;
        private Camera _cam;

        private readonly List<ISelectable> _currentlySelected = new();

        private void Start()
        {
            _cam = Camera.main;
            selectionBox.gameObject.SetActive(false);
        }

        private void Update()
        {
            HandleLeftClick();
            HandleRightClick();
        }

        private void HandleLeftClick()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _startPos = Input.mousePosition;
                selectionBox.gameObject.SetActive(true);
            }

            if (Input.GetMouseButton(0))
            {
                Vector2 currentMouse = Input.mousePosition;
                UpdateSelectionBox(_startPos, currentMouse);
            }

            if (Input.GetMouseButtonUp(0))
            {
                SelectUnitsInBox();
                selectionBox.gameObject.SetActive(false);
            }
        }

        private void HandleRightClick()
        {
            if (!Input.GetMouseButtonDown(1)) return;

            Vector3 mouseWorld = _cam.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0;

            foreach (var selectable in _currentlySelected)
            {
                if (selectable is MonoBehaviour mb && mb.TryGetComponent<IControllable>(out var ctrl))
                {
                    ctrl.MoveTo(mouseWorld);
                }
            }
        }

        private void UpdateSelectionBox(Vector2 start, Vector2 end)
        {
            Vector2 size = end - start;

            selectionBox.anchoredPosition = start;
            selectionBox.sizeDelta = new Vector2(Mathf.Abs(size.x), Mathf.Abs(size.y));
            selectionBox.anchoredPosition += new Vector2(size.x > 0 ? 0 : size.x, size.y > 0 ? 0 : size.y);
        }

        private void SelectUnitsInBox()
        {
            DeselectAll();

            Vector2 min = selectionBox.anchoredPosition;
            Vector2 max = min + selectionBox.sizeDelta;

            foreach (var unit in Utilities.UnitRegistry.AllUnits)
            {
                Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(_cam, unit.transform.position);
                if (screenPos.x >= min.x && screenPos.x <= max.x &&
                    screenPos.y >= min.y && screenPos.y <= max.y)
                {
                    Select(unit.gameObject);
                }
            }
        }

        private void Select(GameObject go)
        {
            if (go.TryGetComponent<ISelectable>(out var selectable))
            {
                selectable.OnSelect();
                _currentlySelected.Add(selectable);
            }
        }

        private void DeselectAll()
        {
            foreach (var selectable in _currentlySelected)
            {
                selectable?.OnDeselect();
            }

            _currentlySelected.Clear();
        }

        public void SelectSingle(GameObject go)
        {
            DeselectAll();

            if (go.TryGetComponent<ISelectable>(out var selectable))
            {
                selectable.OnSelect();
                _currentlySelected.Add(selectable);
            }
        }
    }
}
