using UnityEngine;
using GridSystem;
using Data.Buildings;
using Utilities;

namespace Managers
{
    public class SpawnPointController : MonoBehaviour
    {
        [SerializeField] private GameObject flagSprite;

        private BaseBuildingData _currentBuilding;
        private Vector3Int _spawnCell;
        private UnitSpawnPointHolder _currentSpawnHolder;
        
        private void Start()
        {
            if (flagSprite != null)
                flagSprite.SetActive(false);

            if (SelectionManager.Instance != null)
                SelectionManager.Instance.OnSelectedChanged += OnSelectionChanged;
        }

        private void Update()
        {
            if (_currentBuilding == null || !_currentBuilding.CanProduceUnits)
                return;

            if (Input.GetMouseButtonDown(1))
            {
                var world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                world.z = 0;
                Vector3Int cell = GridManager.Instance.LayoutGrid.WorldToCell(world);

                if (!IsCellValid(cell)) return;

                _spawnCell = cell;

                _currentSpawnHolder?.SetSpawnCell(_spawnCell);

                UpdateFlagPosition();

            }
        }

        private void OnSelectionChanged(GameObject selected)
        {
            if (selected == null || !selected.TryGetComponent(out BuildingDataHolder holder))
            {
                _currentBuilding = null;
                _currentSpawnHolder = null;
                flagSprite?.SetActive(false);
                return;
            }

            if (!holder.Data.CanProduceUnits)
            {
                _currentBuilding = null;
                _currentSpawnHolder = null;
                flagSprite?.SetActive(false);
                return;
            }

            _currentBuilding = holder.Data;

            if (!selected.TryGetComponent(out UnitSpawnPointHolder spawnHolder))
            {
                spawnHolder = selected.AddComponent<UnitSpawnPointHolder>();

                Vector3Int centerCell = GridManager.Instance.LayoutGrid.WorldToCell(selected.transform.position);
                Vector3Int fallbackSpawnCell = SpawnPointUtility.FindNearestFreeCell(centerCell, holder.Data.size);
                spawnHolder.SetSpawnCell(fallbackSpawnCell);
            }

            _currentSpawnHolder = spawnHolder;
            _spawnCell = _currentSpawnHolder.SpawnCell;

            UpdateFlagPosition();
            flagSprite?.SetActive(true);
        }



        private void UpdateFlagPosition()
        {
            if (flagSprite == null) return;
            Vector3 world = GridManager.Instance.LayoutGrid.CellToWorld(_spawnCell) + new Vector3(0.5f, 0.5f, 0);
            flagSprite.transform.position = world;
        }

        public Vector3 GetSpawnWorldPosition()
        {
            return GridManager.Instance.LayoutGrid.CellToWorld(_spawnCell) + new Vector3(0.5f, 0.5f, 0);
        }

        private Vector3Int FindNearestFreeCell(Vector3Int center, Vector2Int size)
        {
            int range = 1;
            while (range < 10)
            {
                for (int dx = -range; dx <= range; dx++)
                {
                    for (int dy = -range; dy <= range; dy++)
                    {
                        if (Mathf.Abs(dx) != range && Mathf.Abs(dy) != range)
                            continue;

                        Vector3Int testCell = new Vector3Int(center.x + dx, center.y + dy, 0);
                        if (!IsCellValid(testCell)) continue;

                        return testCell;
                    }
                }
                range++;
            }

            return Vector3Int.one * -1;
        }

        private bool IsCellValid(Vector3Int cell)
        {
            return cell.x >= 0 && cell.x < GridManager.Instance.GridWidth &&
                   cell.y >= 0 && cell.y < GridManager.Instance.GridHeight &&
                   GridManager.Instance.IsAreaFree(cell, Vector2Int.one);
        }
    }
}
