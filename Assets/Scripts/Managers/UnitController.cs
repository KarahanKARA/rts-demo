using System.Collections.Generic;
using Data.Units;
using GridSystem;
using UnityEngine;
using Utilities;

namespace Managers
{
    public class UnitController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float personalSpaceRadius = 0.3f;
        [SerializeField] private LayerMask unitLayer;

        private List<Vector3> _path;
        private int _pathIndex;

        public void Initialize(UnitData data, Vector3 target)
        {
            Vector3Int spawnCell = GridManager.Instance.LayoutGrid.WorldToCell(target);

            if (!GridManager.Instance.IsAreaFree(spawnCell, Vector2Int.one))
            {
                spawnCell = SpawnPointUtility.FindNearestFreeCell(spawnCell, Vector2Int.one);
            }

            var pathfinder = new Pathfinder(GridManager.Instance.GetOccupiedGrid());
            var cellPath = pathfinder.FindPath(
                GridManager.Instance.LayoutGrid.WorldToCell(transform.position),
                spawnCell
            );

            if (cellPath == null || cellPath.Count == 0)
            {
                return;
            }

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

            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, personalSpaceRadius, unitLayer);
            if (hits.Length > 1) return; 

            Vector3 targetPos = _path[_pathIndex];
            Vector3 dir = (targetPos - transform.position).normalized;

            transform.position += dir * (moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPos) < 0.1f)
            {
                _pathIndex++;
            }
        }
    }
}
