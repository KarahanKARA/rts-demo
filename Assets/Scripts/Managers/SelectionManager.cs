using System;
using Core.Input;
using Core.Interfaces;
using UnityEngine;
using Utilities;

namespace Managers
{
    public class SelectionManager : MonoBehaviour
    {
        public static SelectionManager Instance { get; private set; }

        public event Action<GameObject> OnSelectedChanged;
        public GameObject SelectedObject => _selectedObject;

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
            ClickInputRouter.Instance.OnLeftClickDown += HandleClick;
        }

        private void HandleClick(Vector3 worldPos)
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
                    SelectObject(hit.gameObject);
                }
            }
            else
            {
                Deselect();
            }
        }

        public void SelectObject(GameObject go)
        {
            if (_selectedObject == go) return;

            if (_selectedObject != null && _selectedObject.TryGetComponent<ISelectable>(out var oldSelectable))
                oldSelectable.OnDeselect();

            _selectedObject = go;

            if (_selectedObject != null &&  _selectedObject.TryGetComponent<ISelectable>(out var newSelectable))
                newSelectable.OnSelect();

            OnSelectedChanged?.Invoke(_selectedObject);
        }

        public void Deselect()
        {
            if ( _selectedObject != null && _selectedObject.TryGetComponent<ISelectable>(out var selectable))
                selectable.OnDeselect();

            _selectedObject = null;
            OnSelectedChanged?.Invoke(null);
        }
    }
}
