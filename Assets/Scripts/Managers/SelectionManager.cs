using System;
using Core.Input;
using Core.Interfaces;
using UnityEngine;

namespace Managers
{
    /// <summary>
    /// Handles high-level selection logic between buildings and units.
    /// Integrates with UnitSelectionHandler for multiple selection support.
    /// </summary>

    public class SelectionManager : MonoBehaviour
    {
        public static SelectionManager Instance { get; private set; }

        public event Action<GameObject> OnSelectedChanged;
        public GameObject SelectedObject => _selectedObject;
        public UnitSelectionHandler UnitSelector => unitSelector;

        [SerializeField] private UnitSelectionHandler unitSelector;

        private GameObject _selectedObject;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            ClickInputRouter.Instance.OnLeftClickUp += HandleClickUp;
        }

        private void HandleClickUp(Vector3 worldPos)
        {
            if (!UnitSelector.HasDragged)
            {
                var hit = Physics2D.OverlapPoint(worldPos);

                if (hit != null && hit.TryGetComponent<ISelectable>(out var selectable))
                {
                    if (hit.CompareTag("Unit"))
                    {
                        unitSelector.SelectSingle(hit.gameObject);
                    }
                    else
                    {
                        unitSelector.DeselectAllPublic();
                        SelectObject(hit.gameObject);
                    }
                }
                else
                {
                    unitSelector.DeselectAllPublic();
                    Deselect();
                }
            }
        }

        public void SelectObject(GameObject go)
        {
            if (_selectedObject == go) return;

            if (_selectedObject != null && _selectedObject.TryGetComponent<ISelectable>(out var oldSelectable))
                oldSelectable.OnDeselect();

            _selectedObject = go;

            if (_selectedObject != null && _selectedObject.TryGetComponent<ISelectable>(out var newSelectable))
                newSelectable.OnSelect();

            OnSelectedChanged?.Invoke(_selectedObject);
        }

        public void Deselect()
        {
            if (_selectedObject != null && _selectedObject.TryGetComponent<ISelectable>(out var selectable))
                selectable.OnDeselect();

            _selectedObject = null;
            OnSelectedChanged?.Invoke(null);
        }
    }
}
