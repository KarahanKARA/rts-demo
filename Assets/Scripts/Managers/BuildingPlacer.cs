using Core.Factories;
using Data.Buildings;
using GridSystem;
using UnityEngine;
using Utilities;

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
                if (!isValid)
                {
                    SelectionManager.Instance.Deselect();
                    Destroy(_ghost);
                    _ghost = null;
                    _currentData = null;
                    return;
                }

                var go = _buildingFactory.CreateBuilding(_currentData, snapped);

                Vector3Int gridCell = GridManager.Instance.LayoutGrid.WorldToCell(snapped);
                GridManager.Instance.OccupyArea(gridCell, _currentData.size);

                if (!go.TryGetComponent(out UnitSpawnPointHolder spawnHolder))
                    spawnHolder = go.AddComponent<UnitSpawnPointHolder>();

                Vector3Int centerCell = GridManager.Instance.LayoutGrid.WorldToCell(snapped);
                Vector3Int defaultSpawnCell = SpawnPointUtility.FindNearestFreeCell(centerCell, _currentData.size);
                spawnHolder.SetSpawnCell(defaultSpawnCell);

                go.GetComponent<BuildingHealth>().Initialize(_currentData.health);

                SelectionManager.Instance.SelectObject(go);

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
