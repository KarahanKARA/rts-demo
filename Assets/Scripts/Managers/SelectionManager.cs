using System;
using UnityEngine;
using Utilities;

namespace Managers
{
    public class SelectionManager : MonoBehaviour
    {
        public static SelectionManager Instance { get; private set; }

        public event Action<GameObject> OnSelectedChanged;

        private Camera _mainCamera;
        
        private GameObject _selectedObject;
        
        private void Awake()
        {
            if (Instance != null) Destroy(gameObject);
            Instance = this;
            
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (UIUtility.IsPointerOverUIObject())
                    return;

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
            _selectedObject = go;
            OnSelectedChanged?.Invoke(_selectedObject);
        }

        public void Deselect()
        {
            _selectedObject = null;
            OnSelectedChanged?.Invoke(null);
        }
    }
}