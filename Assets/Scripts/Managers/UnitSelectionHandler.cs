using System;
using System.Collections.Generic;
using Core.Input;
using Core.Interfaces;
using UnityEngine;

namespace Managers
{
    public class UnitSelectionHandler : MonoBehaviour
    {
        public bool IsDragging { get; private set; }
        public bool HasDragged { get; private set; }

        [Header("Unit Settings")]
        [SerializeField] private LayerMask unitLayer;

        [Header("World Selection Visual")]
        [SerializeField] private LineRenderer lineRenderer;

        private Vector3 dragStartWorld;
        private Vector3 dragEndWorld;

        private const float DragThreshold = 0.2f;

        public event Action<int> OnMultipleSelection;

        private Camera _cam;
        private readonly List<ISelectable> _currentlySelected = new();

        private void Start()
        {
            _cam = Camera.main;

            ClickInputRouter.Instance.OnLeftClickDown += OnClickStart;
            ClickInputRouter.Instance.OnLeftClickUp += OnClickEnd;

        }


        private void Update()
        {
            if (!IsDragging) return;

            dragEndWorld = _cam.ScreenToWorldPoint(Input.mousePosition);
            dragEndWorld.z = 0;

            float dragDistance = Vector2.Distance(dragStartWorld, dragEndWorld);
            HasDragged = dragDistance > DragThreshold;

            if (HasDragged)
            {
                Vector3[] corners = new Vector3[5];
                corners[0] = new Vector3(dragStartWorld.x, dragStartWorld.y, 0);
                corners[1] = new Vector3(dragStartWorld.x, dragEndWorld.y, 0);
                corners[2] = new Vector3(dragEndWorld.x, dragEndWorld.y, 0);
                corners[3] = new Vector3(dragEndWorld.x, dragStartWorld.y, 0);
                corners[4] = corners[0];

                lineRenderer.SetPositions(corners);
                lineRenderer.enabled = true;
            }
        }

        private void OnClickStart(Vector3 worldPos)
        {
            BeginDrag();
            dragStartWorld = worldPos;
            dragStartWorld.z = 0;
        }

        private void OnClickEnd(Vector3 worldPos)
        {
            EndDrag();
            dragEndWorld = worldPos;
            dragEndWorld.z = 0;

            lineRenderer.enabled = false;

            if (!HasDragged) return;

            SelectUnitsInBox();
        }

        private void BeginDrag()
        {
            IsDragging = true;
            HasDragged = false;
        }

        private void EndDrag()
        {
            IsDragging = false;
        }

        private void SelectUnitsInBox()
        {
            DeselectAll();

            Vector3 screenStart = _cam.WorldToScreenPoint(dragStartWorld);
            Vector3 screenEnd = _cam.WorldToScreenPoint(dragEndWorld);

            Vector2 min = Vector2.Min(screenStart, screenEnd);
            Vector2 max = Vector2.Max(screenStart, screenEnd);

            foreach (var unit in Utilities.UnitRegistry.AllUnits)
            {
                if (unit == null || !unit.activeInHierarchy) continue;

                Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(_cam, unit.transform.position);
                if (screenPos.x >= min.x && screenPos.x <= max.x &&
                    screenPos.y >= min.y && screenPos.y <= max.y)
                {
                    Select(unit);
                }
            }

            if (_currentlySelected.Count == 1 && _currentlySelected[0] is MonoBehaviour mb)
            {
                SelectionManager.Instance.SelectObject(mb.gameObject);
            }
            else if (_currentlySelected.Count > 1)
            {
                OnMultipleSelection?.Invoke(_currentlySelected.Count);
            }
        }

        private void Select(GameObject go)
        {
            if (go.TryGetComponent<ISelectable>(out var selectable))
            {
                if (!_currentlySelected.Contains(selectable))
                {
                    _currentlySelected.Add(selectable);
                }

                selectable.OnSelect();
            }
        }

        public void RemoveFromSelection(ISelectable unit)
        {
            if (_currentlySelected.Contains(unit))
            {
                _currentlySelected.Remove(unit);
            }
        }

        private void DeselectAll()
        {
            _currentlySelected.RemoveAll(s => s == null);

            foreach (var selectable in _currentlySelected)
            {
                selectable?.OnDeselect();
            }

            _currentlySelected.Clear();
        }

        public void DeselectAllPublic() => DeselectAll();

        public void SelectSingle(GameObject go)
        {
            DeselectAll();

            if (go.TryGetComponent<ISelectable>(out var selectable))
            {
                _currentlySelected.Add(selectable);
                selectable.OnSelect();
            }

            SelectionManager.Instance.SelectObject(go);
        }

        public IReadOnlyList<ISelectable> GetSelected() => _currentlySelected;
    }
}
