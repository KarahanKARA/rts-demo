using Data.Buildings;
using GridSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Managers
{
    public class DragManager : MonoBehaviour
    {
        [SerializeField] private BuildingPlacer placer;
        [SerializeField] private Color dragColor = new(1f, 1f, 1f, 0.6f);

        private Camera _cam;
        private BaseBuildingData _draggingData;
        private GameObject _ghostObject;
        private SpriteRenderer _ghostRenderer;

        private void Start()
        {
            _cam = Camera.main;
        }

        private void Update()
        {
            if (_draggingData == null || _ghostObject == null) return;
            if (EventSystem.current.IsPointerOverGameObject()) return;

            var mouseWorld = _cam.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0;

            var cell = GridManager.Instance.LayoutGrid.WorldToCell(mouseWorld);
            var snapped = GridManager.Instance.GetSnappedPosition(cell, _draggingData.size);

            _ghostObject.transform.position = snapped;

            var isValid = GridManager.Instance.IsAreaFree(cell, _draggingData.size);
            _ghostRenderer.color = isValid ? dragColor : new Color(1f, 0.3f, 0.3f, dragColor.a);

            if (Input.GetMouseButtonUp(0))
            {
                if (isValid)
                {
                    Instantiate(_draggingData.prefab, snapped, Quaternion.identity);
                    GridManager.Instance.OccupyArea(cell, _draggingData.size);
                }

                Destroy(_ghostObject);
                _ghostObject = null;
                _draggingData = null;
            }
        }
    }
}