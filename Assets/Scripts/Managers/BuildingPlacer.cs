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

        [SerializeField] private MonoBehaviour factorySource;

        private IBuildingFactory _buildingFactory;
        private BaseBuildingData _currentData;
        private GameObject _ghost;
        private SpriteRenderer _ghostRenderer;
        private int _originalSortingOrder;
        private Camera _cam;

        private void Awake()
        {
            _cam = Camera.main;
            _buildingFactory = factorySource as IBuildingFactory;
        }

        private void Update()
        {
            if (_currentData == null || _ghost == null) return;

            Vector3 mouseWorld = _cam.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0;

            Vector3Int cell = GridManager.Instance.LayoutGrid.WorldToCell(mouseWorld);
            Vector3 snapped = GridManager.Instance.GetSnappedPosition(cell, _currentData.size);

            _ghost.transform.position = snapped;

            bool isValid = GridManager.Instance.IsAreaFree(cell, _currentData.size);
            _ghostRenderer.color = isValid ? validColor : invalidColor;

            if (Input.GetMouseButtonDown(0))
            {
                if (!isValid)
                {
                    CleanupPlacement();
                    return;
                }

                GameObject go = _buildingFactory.CreateBuilding(_currentData, snapped);

                InitializeBuilding(go, cell, snapped);

                GridManager.Instance.OccupyArea(cell, _currentData.size);
                SelectionManager.Instance.SelectObject(go);

                CleanupPlacement();
            }
        }

        private void InitializeBuilding(GameObject go, Vector3Int cell, Vector3 snapped)
        {
            if (!go.TryGetComponent(out UnitSpawnPointHolder spawnHolder))
                spawnHolder = go.AddComponent<UnitSpawnPointHolder>();

            Vector3Int centerCell = GridManager.Instance.LayoutGrid.WorldToCell(snapped);
            Vector3Int defaultSpawnCell = SpawnPointUtility.FindNearestFreeCell(centerCell, _currentData.size);
            spawnHolder.SetSpawnCell(defaultSpawnCell);

            if (go.TryGetComponent(out BuildingHealth health))
                health.Initialize(_currentData.health);

            if (go.TryGetComponent(out UnitProducer producer))
            {
                producer.SetFactory(UnitFactoryMB.Instance);
            }
        }

        public void StartPlacing(BaseBuildingData data)
        {
            if (_ghost != null)
                Destroy(_ghost);

            _currentData = data;

            _ghost = Instantiate(data.prefab);
            _ghostRenderer = _ghost.GetComponentInChildren<SpriteRenderer>();
            _originalSortingOrder = _ghostRenderer.sortingOrder;
            _ghostRenderer.sortingOrder = _originalSortingOrder + 1;
            _ghostRenderer.color = invalidColor;

            Vector3 mouseWorld = _cam.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0;

            Vector3Int cell = GridManager.Instance.LayoutGrid.WorldToCell(mouseWorld);
            _ghost.transform.position = GridManager.Instance.GetSnappedPosition(cell, data.size);
        }

        private void CleanupPlacement()
        {
            if (_ghost != null)
                Destroy(_ghost);

            _ghost = null;
            _currentData = null;
        }
    }
}
