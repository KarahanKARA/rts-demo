using System;
using UnityEngine;

namespace Core.Input
{
    public class ClickInputRouter : MonoBehaviour
    {
        public static ClickInputRouter Instance { get; private set; }

        public event Action<Vector3> OnLeftClickDown;
        public event Action<Vector3> OnLeftClickUp;
        public event Action<Vector3> OnRightClickDown;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Update()
        {
            if (UnityEngine.EventSystems.EventSystem.current != null &&
                UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                return;

            Vector3 worldPos = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
            worldPos.z = 0;

            if (UnityEngine.Input.GetMouseButtonDown(0))
                OnLeftClickDown?.Invoke(worldPos);

            if (UnityEngine.Input.GetMouseButtonUp(0))
                OnLeftClickUp?.Invoke(worldPos);

            if (UnityEngine.Input.GetMouseButtonDown(1))
                OnRightClickDown?.Invoke(worldPos);
        }
    }
}