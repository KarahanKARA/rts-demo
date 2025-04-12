using System.Collections.Generic;
using Core.Interfaces;
using Data.Units;
using GridSystem;
using UnityEngine;
using Utilities;

namespace Managers
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class UnitController : MonoBehaviour, ISelectable, IDamageable, IControllable
    {
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private LayerMask unitLayer;

        private SpriteRenderer _renderer;
        private List<Vector3> _path;
        private int _pathIndex;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            UnitRegistry.Register(gameObject);
        }

        private void OnDestroy()
        {
           UnitRegistry.Unregister(gameObject);
        }


        public void Initialize(UnitData data, Vector3 target)
        {
            Vector3Int spawnCell = GridManager.Instance.LayoutGrid.WorldToCell(target);

            if (!GridManager.Instance.IsAreaFree(spawnCell, Vector2Int.one))
            {
                spawnCell = SpawnPointUtility.FindNearestFreeCell(spawnCell, Vector2Int.one);
            }

            bool[,] gridCopy = (bool[,])GridManager.Instance.GetOccupiedGrid().Clone();

            if (IsWithinBounds(spawnCell))
            {
                gridCopy[spawnCell.x, spawnCell.y] = true;
            }

            var pathfinder = new Pathfinder(gridCopy);
            var cellPath = pathfinder.FindPath(
                GridManager.Instance.LayoutGrid.WorldToCell(transform.position),
                spawnCell
            );

            if (cellPath == null || cellPath.Count == 0) return;

            _path = new List<Vector3>();
            foreach (var cell in cellPath)
            {
                _path.Add(GridManager.Instance.LayoutGrid.CellToWorld(cell) + new Vector3(0.5f, 0.5f));
            }

            _pathIndex = 0;
        }

        private void Update()
        {
            if (_path == null || _pathIndex >= _path.Count) return;

            Vector3Int currentCell = GridManager.Instance.LayoutGrid.WorldToCell(transform.position);
            if (!IsWithinBounds(currentCell))
            {
                _path = null;
                return;
            }

            Vector3 targetPos = _path[_pathIndex];

            Vector3 dir = (targetPos - transform.position).normalized;
            transform.position += dir * (moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPos) < 0.1f)
            {
                _pathIndex++;
            }
        }

        private bool IsWithinBounds(Vector3Int cell)
        {
            return cell.x >= 0 && cell.x < GridManager.Instance.GridWidth &&
                   cell.y >= 0 && cell.y < GridManager.Instance.GridHeight;
        }

        public void OnSelect()
        {
            if (_renderer != null)
                _renderer.color = SelectionColors.SelectedColor;
        }

        public void OnDeselect()
        {
            if (_renderer != null)
                _renderer.color = SelectionColors.DeselectedColor;
        }


        public void TakeDamage(int amount)
        {
            Debug.Log($"Unit {gameObject.name} took {amount} damage!");
            // TODO:  İleride health eklersen burada düşürürsün
        }
        
        private void Die()
        {
            string key = gameObject.name.Replace("(Clone)", "").Trim();
            if (ObjectPoolManager.Instance != null)
            {
                ObjectPoolManager.Instance.Release(key, gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        public void MoveTo(Vector3 position)
        {
            Vector3Int startCell = GridManager.Instance.LayoutGrid.WorldToCell(transform.position);
            Vector3Int targetCell = GridManager.Instance.LayoutGrid.WorldToCell(position);

            bool[,] gridCopy = (bool[,])GridManager.Instance.GetOccupiedGrid().Clone();

            float originalDistance = Vector3Int.Distance(startCell, targetCell);

            List<Vector3Int> surroundingCells = new List<Vector3Int>();
            int searchRadius = Mathf.Max(GridManager.Instance.GridWidth, GridManager.Instance.GridHeight);

            for (int x = -searchRadius; x <= searchRadius; x++)
            {
                for (int y = -searchRadius; y <= searchRadius; y++)
                {
                    Vector3Int cell = new Vector3Int(targetCell.x + x, targetCell.y + y, 0);
                    if (IsWithinBounds(cell) && !gridCopy[cell.x, cell.y])
                    {
                        surroundingCells.Add(cell);
                    }
                }
            }

            surroundingCells.Sort((a, b) => 
                Vector3Int.Distance(a, targetCell).CompareTo(Vector3Int.Distance(b, targetCell)));

            foreach (var cell in surroundingCells)
            {
                float distanceToTarget = Vector3Int.Distance(cell, targetCell);
                float distanceFromStart = Vector3Int.Distance(startCell, cell);

                if (distanceFromStart > originalDistance)
                {
                    continue;
                }

                var pathfinder = new Pathfinder(gridCopy);
                var cellPath = pathfinder.FindPath(startCell, cell);

                if (cellPath != null && cellPath.Count > 0)
                {
                    _path = new List<Vector3>();
                    foreach (var pathCell in cellPath)
                    {
                        _path.Add(GridManager.Instance.LayoutGrid.CellToWorld(pathCell) + new Vector3(0.5f, 0.5f, 0));
                    }

                    _pathIndex = 0;
                    return;
                }
            }
        }

        private Vector3Int FindNearestWalkableCell(Vector3Int targetCell, bool[,] grid)
        {
            int maxDistance = Mathf.Max(GridManager.Instance.GridWidth, GridManager.Instance.GridHeight);
            
            for (int distance = 1; distance < maxDistance; distance++)
            {
                for (int x = -distance; x <= distance; x++)
                {
                    for (int y = -distance; y <= distance; y++)
                    {
                        if (Mathf.Abs(x) != distance && Mathf.Abs(y) != distance)
                            continue;

                        Vector3Int testCell = new Vector3Int(targetCell.x + x, targetCell.y + y, 0);
                        
                        if (IsWithinBounds(testCell) && !grid[testCell.x, testCell.y])
                        {
                            return testCell;
                        }
                    }
                }
            }

            return GridManager.Instance.LayoutGrid.WorldToCell(transform.position);
        }

    }
}
