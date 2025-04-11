using Core.Factories;
using Data.Buildings;
using GridSystem;
using UnityEngine;

namespace Managers
{
    public class BuildingPlacer : MonoBehaviour
    {
        [SerializeField] private Color validColor = new(0f, 1f, 0f, 0.4f);
        [SerializeField] private Color invalidColor = new(1f, 0f, 0f, 0.4f);

        private BaseBuildingData _currentData;
        private GameObject _ghost;
        private SpriteRenderer _ghostRenderer;
        private int _originalSortingOrder;
        private Camera _cam;
        
        [SerializeField] private MonoBehaviour factorySource;
        private IBuildingFactory _buildingFactory;

        private void Awake()
        {
            _cam = Camera.main;
            _buildingFactory = factorySource as IBuildingFactory;
        }

        private void Update()
        {
            if (_currentData == null || _ghost == null) return;

            var mouseWorld = _cam.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0;

            var cell = GridManager.Instance.LayoutGrid.WorldToCell(mouseWorld);
            var snapped = GridManager.Instance.GetSnappedPosition(cell, _currentData.size);

            _ghost.transform.position = snapped;

            var isValid = GridManager.Instance.IsAreaFree(cell, _currentData.size);
            _ghostRenderer.color = isValid ? validColor : invalidColor;

            if (Input.GetMouseButtonDown(0))
            {
                if (isValid)
                {
                    var go = _buildingFactory.CreateBuilding(_currentData, snapped);
                    go.GetComponent<BuildingHealth>().Initialize(_currentData.health);
                    GridManager.Instance.OccupyArea(cell, _currentData.size);
                    SelectionManager.Instance.SelectObject(go);
                }

                Destroy(_ghost);
                _ghost = null;
                _currentData = null;
            }

        }

        public void StartPlacing(BaseBuildingData data)
        {
            if (_ghost != null) Destroy(_ghost);

            _currentData = data;
            _ghost = Instantiate(data.prefab);
            _ghostRenderer = _ghost.GetComponentInChildren<SpriteRenderer>();
            _originalSortingOrder = _ghostRenderer.sortingOrder;
            _ghostRenderer.sortingOrder = _originalSortingOrder + 1;
            _ghostRenderer.color = invalidColor;

            var mouseWorld = _cam.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0;

            var cell = GridManager.Instance.LayoutGrid.WorldToCell(mouseWorld);
            _ghost.transform.position = GridManager.Instance.GetSnappedPosition(cell, data.size);
        }
    }
}
