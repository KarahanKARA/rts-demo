using System.Collections.Generic;
using Core.Input;
using Core.Interfaces;
using UnityEngine;

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

        private void Start()
        {
            _cam = Camera.main;
            selectionBox.gameObject.SetActive(false);

            ClickInputRouter.Instance.OnLeftClickDown += OnClickStart;
            ClickInputRouter.Instance.OnLeftClickUp += OnClickEnd;
            ClickInputRouter.Instance.OnRightClickDown += OnCommandIssued;
        }

        private void Update()
        {
            if (!selectionBox.gameObject.activeSelf) return;

            Vector2 currentMouse = Input.mousePosition;
            Vector2 size = currentMouse - _startPos;

            selectionBox.sizeDelta = new Vector2(Mathf.Abs(size.x), Mathf.Abs(size.y));
            selectionBox.anchoredPosition = _startPos + new Vector2(
                size.x < 0 ? size.x : 0,
                size.y < 0 ? size.y : 0
            );
        }
        
        private void OnClickStart(Vector3 worldPos)
        {
            _startPos = Input.mousePosition;
            selectionBox.sizeDelta = Vector2.zero;
            selectionBox.gameObject.SetActive(true);
        }
        

        private void OnClickEnd(Vector3 worldPos)
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

        private void OnCommandIssued(Vector3 worldPos)
        {
            foreach (var selectable in _currentlySelected)
            {
                if (selectable is MonoBehaviour mb && mb.TryGetComponent<IControllable>(out var ctrl))
                {
                    ctrl.MoveTo(worldPos);
                }
            }
        }

        private void SelectUnitsInBox()
        {
            DeselectAll();

            Vector2 min = Vector2.Min(_startPos, _endPos);
            Vector2 max = Vector2.Max(_startPos, _endPos);

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
                SelectionManager.Instance.SelectObject(go);
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
                _currentlySelected.Add(selectable);
                SelectionManager.Instance.SelectObject(go); 
            }
        }
    }
}
