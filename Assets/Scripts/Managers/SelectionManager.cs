using System;
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
        [SerializeField] private Color selectionColor = new(0.65f, 0.8f, 1f, 1f);

        private Camera _mainCamera;
        private GameObject _selectedObject;
        private SpriteRenderer _selectedRenderer;
        private Color _originalColor;

        private bool _selectionHandledThisFrame = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            // Bu frame içinde seçim yapıldıysa Deselect çalışmasın
            if (_selectionHandledThisFrame)
            {
                _selectionHandledThisFrame = false;
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (UIUtility.IsPointerOverUIObject()) return;

                var mouseWorld = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
                mouseWorld.z = 0;

                var hit = Physics2D.OverlapPoint(mouseWorld);
                if (hit != null)
                {
                    if (hit.TryGetComponent<ISelectable>(out var selectable))
                    {
                        if (hit.CompareTag("Unit"))
                        {
                            unitSelector.SelectSingle(hit.gameObject);
                            _selectionHandledThisFrame = true;
                        }
                        else
                        {
                            SelectObject(hit.gameObject);
                            _selectionHandledThisFrame = true;
                        }
                    }
                }
                else
                {
                    Deselect();
                }
            }
        }

        public void SelectObject(GameObject go)
        {
            if (_selectedRenderer != null)
                _selectedRenderer.color = _originalColor;

            _selectedObject = go;

            if (_selectedObject.TryGetComponent(out SpriteRenderer renderer))
            {
                _selectedRenderer = renderer;
                _originalColor = renderer.color;
                _selectedRenderer.color = selectionColor;
            }

            OnSelectedChanged?.Invoke(_selectedObject);
        }

        public void Deselect()
        {
            if (_selectedRenderer != null)
                _selectedRenderer.color = _originalColor;

            _selectedObject = null;
            _selectedRenderer = null;
            OnSelectedChanged?.Invoke(null);
        }
    }
}
