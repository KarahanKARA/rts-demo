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

            // 🧠 Mevcut occupied grid'i kopyala
            bool[,] gridCopy = (bool[,])GridManager.Instance.GetOccupiedGrid().Clone();

            // 🧼 Spawn hücresini "yürünebilir" olarak işaretle
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
            Debug.Log($"{gameObject.name} moveTo called. World pos: {position}");

            Vector3Int startCell = GridManager.Instance.LayoutGrid.WorldToCell(transform.position);
            Vector3Int targetCell = GridManager.Instance.LayoutGrid.WorldToCell(position);
            Debug.Log($"StartCell: {startCell}, TargetCell: {targetCell}");

            // 🧠 Mevcut occupied grid'i kopyala (sadece binalar için)
            bool[,] gridCopy = (bool[,])GridManager.Instance.GetOccupiedGrid().Clone();

            var pathfinder = new Pathfinder(gridCopy);
            var cellPath = pathfinder.FindPath(startCell, targetCell);

            if (cellPath == null || cellPath.Count == 0)
            {
                Debug.LogWarning($"Path not found! Start: {startCell}, Target: {targetCell}");
                Debug.Log($"Is start walkable: {IsWithinBounds(startCell) && !GridManager.Instance.IsCellOccupied(startCell)}");
                Debug.Log($"Is target walkable: {IsWithinBounds(targetCell) && !GridManager.Instance.IsCellOccupied(targetCell)}");
                return;
            }

            _path = new List<Vector3>();
            foreach (var cell in cellPath)
            {
                _path.Add(GridManager.Instance.LayoutGrid.CellToWorld(cell) + new Vector3(0.5f, 0.5f, 0));
            }

            _pathIndex = 0;
            Debug.Log($"Path found with {cellPath.Count} steps!");
        }

    }
}
