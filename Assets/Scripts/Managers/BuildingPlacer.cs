using Core.Factories;
using Core.Interfaces;
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
                    Destroy(_ghost);
                    _ghost = null;
                    _currentData = null;

                    SelectionManager.Instance.Deselect(); 
                    return;
                }

                var go = _buildingFactory.CreateBuilding(_currentData, snapped);

                if (!go.TryGetComponent(out UnitSpawnPointHolder spawnHolder))
                    spawnHolder = go.AddComponent<UnitSpawnPointHolder>();

                Vector3Int centerCell = GridManager.Instance.LayoutGrid.WorldToCell(snapped);
                GridManager.Instance.OccupyArea(cell, _currentData.size);

                Vector3Int defaultSpawnCell = SpawnPointUtility.FindNearestFreeCell(centerCell, _currentData.size);
                spawnHolder.SetSpawnCell(defaultSpawnCell);

                go.GetComponent<BuildingHealth>().Initialize(_currentData.health);

                if (go.TryGetComponent(out UnitProducer producer))
                {
                    producer.SetFactory(UnitFactoryMB.Instance); 
                }

                SelectionManager.Instance.SelectObject(go);
                PushUnitsAwayFromBuilding(snapped, _currentData.size);
                Destroy(_ghost);
                _ghost = null;
                _currentData = null;
            }
        }
        
        private void PushUnitsAwayFromBuilding(Vector3 centerWorldPos, Vector2Int size)
        {
            Vector3Int centerCell = GridManager.Instance.LayoutGrid.WorldToCell(centerWorldPos);
            Vector3Int bottomLeft = GridManager.Instance.GetBottomLeftCell(centerCell, size);
            Vector3 sizeOffset = new Vector3(0.5f, 0.5f);

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    Vector3Int cell = new Vector3Int(bottomLeft.x + x, bottomLeft.y + y, 0);
                    Vector3 worldPos = GridManager.Instance.LayoutGrid.CellToWorld(cell) + sizeOffset;

                    Collider2D[] hits = Physics2D.OverlapCircleAll(worldPos, 0.4f);
                    foreach (var hit in hits)
                    {
                        if (hit.CompareTag("Unit"))
                        {
                            Vector3Int freeCell = SpawnPointUtility.FindNearestFreeCell(cell, Vector2Int.one);
                            hit.transform.position = GridManager.Instance.LayoutGrid.CellToWorld(freeCell) + sizeOffset;
                        }
                    }
                }
            }
        }

        public void StartPlacing(BaseBuildingData data)
        {
            if (_ghost != null) Destroy(_ghost);

            SelectionManager.Instance.Deselect();
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
