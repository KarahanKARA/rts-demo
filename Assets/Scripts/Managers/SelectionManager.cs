using System;
using UnityEngine;
using Utilities;

namespace Managers
{
    public class SelectionManager : MonoBehaviour
    {
        public static SelectionManager Instance { get; private set; }

        public event Action<GameObject> OnSelectedChanged;

        public GameObject SelectedObject => _selectedObject;
        private Camera _mainCamera;
        private GameObject _selectedObject;
        private SpriteRenderer _selectedRenderer;
        private Color _originalColor;

        [SerializeField] private Color selectionColor = new(0.65f, 0.8f, 1f, 1f);

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
            if (Input.GetMouseButtonDown(0))
            {
                if (UIUtility.IsPointerOverUIObject()) return;

                var mouseWorld = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
                mouseWorld.z = 0;

                var hit = Physics2D.OverlapPoint(mouseWorld);
                if (hit != null)
                {
                    SelectObject(hit.gameObject);
                }
                else
                {
                    Deselect();
                }
            }
        }

        public void SelectObject(GameObject go)
        {
            if (_selectedObject == go) return;

            if (_selectedRenderer != null)
                _selectedRenderer.color = _originalColor;

            _selectedObject = go;

            _selectedRenderer = _selectedObject.GetComponentInChildren<SpriteRenderer>();
            if (_selectedRenderer != null)
            {
                _originalColor = _selectedRenderer.color;
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
