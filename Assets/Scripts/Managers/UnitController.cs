using Data.Units;
using GridSystem;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class UnitController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float personalSpaceRadius = 0.3f;
        [SerializeField] private LayerMask unitLayer;

        private List<Vector3> path;
        private int pathIndex;

        public void Initialize(UnitData data, Vector3 target)
        {
            Vector3Int startCell = GridManager.Instance.LayoutGrid.WorldToCell(transform.position);
            Vector3Int endCell = GridManager.Instance.LayoutGrid.WorldToCell(target);

            bool[,] occupancy = GridManager.Instance.GetOccupiedGrid();

            Pathfinder pathfinder = new Pathfinder(occupancy);
            var cellPath = pathfinder.FindPath(startCell, endCell);

            if (cellPath == null || cellPath.Count == 0)
            {
                Debug.Log($"{name} için yol bulunamadı.");
                return;
            }

            path = new List<Vector3>();
            foreach (var cell in cellPath)
            {
                path.Add(GridManager.Instance.LayoutGrid.CellToWorld(cell) + new Vector3(0.5f, 0.5f));
            }

            pathIndex = 0;
        }

        private void Update()
        {
            if (path == null || pathIndex >= path.Count)
                return;

            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, personalSpaceRadius, unitLayer);
            if (hits.Length > 1) return; 

            Vector3 targetPos = path[pathIndex];
            Vector3 direction = (targetPos - transform.position).normalized;

            transform.position += direction * (moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPos) < 0.1f)
                pathIndex++;
        }
    }
}
