using System.Collections.Generic;
using System.Linq;
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

        private SpriteRenderer _renderer;
        private List<Vector3> _path;
        private int _pathIndex;
        private IAttackable _pendingTarget;
        private GameObject _pendingTargetGO;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            UnitRegistry.Register(gameObject);
        }

        private void OnDestroy()
        {
            UnitRegistry.Unregister(gameObject);

            if (SelectionManager.Instance != null &&
                SelectionManager.Instance.UnitSelector.GetSelected().Contains(this))
            {
                SelectionManager.Instance.UnitSelector.DeselectAllPublic();
            }
        }

        public void Initialize(UnitData data, Vector3 target)
        {
            if (TryGetComponent(out UnitHealth health))
                health.Initialize(data.health);

            if (TryGetComponent(out UnitAttackController attackController))
                attackController.Initialize(data);

            Vector3Int spawnCell = GridManager.Instance.LayoutGrid.WorldToCell(target);
            if (!GridManager.Instance.IsAreaFree(spawnCell, Vector2Int.one))
                spawnCell = SpawnPointUtility.FindNearestFreeCell(spawnCell, Vector2Int.one);

            TryMoveToCell(spawnCell);
        }

        private void TryMoveToCell(Vector3Int targetCell)
        {
            var startCell = GridManager.Instance.LayoutGrid.WorldToCell(transform.position);
            var pathfinder = new Pathfinder(GridManager.Instance.GetOccupiedGrid());
            var cellPath = pathfinder.FindPath(startCell, targetCell);

            if (cellPath == null || cellPath.Count == 0) return;

            _path = new List<Vector3>();

            for (int i = 1; i < cellPath.Count; i++)
            {
                var cell = cellPath[i];
                _path.Add(GridManager.Instance.LayoutGrid.CellToWorld(cell) + new Vector3(0.5f, 0.5f));
            }

            _pathIndex = 0;
        }


        private void Update()
        {
            if (_path != null && _pathIndex < _path.Count)
            {
                Vector3 targetPos = _path[_pathIndex];
                Vector3 direction = (targetPos - transform.position).normalized;
                transform.position += direction * (moveSpeed * Time.deltaTime);

                if (Vector3.Distance(transform.position, targetPos) < 0.1f)
                    _pathIndex++;
            }

            if (_pendingTarget != null && TryGetComponent<UnitAttackController>(out var attacker))
            {
                if (_pendingTargetGO == null || _pendingTargetGO.Equals(null))
                {
                    _pendingTarget = null;
                    _pendingTargetGO = null;
                    return;
                }

                float distance = Vector3.Distance(transform.position, _pendingTarget.GetPosition());
                float adjustedDistance = distance - _pendingTarget.GetCollisionRadius();

                if (adjustedDistance <= attacker.AttackRange)
                {
                    attacker.Attack(_pendingTarget);
                    _pendingTarget = null;
                    _pendingTargetGO = null;
                }
            }
        }

        public void MoveTo(Vector3 worldPos)
        {
            if (TryGetComponent<UnitAttackController>(out var attackController))
                attackController.StopAttack();

            var targetCell = GridManager.Instance.LayoutGrid.WorldToCell(worldPos);
            var startCell = GridManager.Instance.LayoutGrid.WorldToCell(transform.position);

            if (!GridUtility.IsValidCell(targetCell, Vector2Int.one)) return;

            var gridCopy = (bool[,])GridManager.Instance.GetOccupiedGrid().Clone();
            float originalDistance = Vector3Int.Distance(startCell, targetCell);
            var nearby = new List<Vector3Int>();
            int searchRadius = Mathf.Max(GridManager.Instance.GridWidth, GridManager.Instance.GridHeight);

            for (int x = -searchRadius; x <= searchRadius; x++)
            {
                for (int y = -searchRadius; y <= searchRadius; y++)
                {
                    var candidate = new Vector3Int(targetCell.x + x, targetCell.y + y, 0);
                    if (IsWithinBounds(candidate) && !gridCopy[candidate.x, candidate.y])
                        nearby.Add(candidate);
                }
            }

            nearby.Sort((a, b) => Vector3Int.Distance(a, targetCell).CompareTo(Vector3Int.Distance(b, targetCell)));

            foreach (var cell in nearby)
            {
                if (Vector3Int.Distance(startCell, cell) > originalDistance) continue;

                var pathfinder = new Pathfinder(gridCopy);
                var cellPath = pathfinder.FindPath(startCell, cell);

                if (cellPath != null && cellPath.Count > 0)
                {
                    _path = new List<Vector3>();
                    for (int i = 1; i < cellPath.Count; i++)
                        _path.Add(GridManager.Instance.LayoutGrid.CellToWorld(cellPath[i]) + new Vector3(0.5f, 0.5f));

                    _pathIndex = 0;
                    return;
                }
            }
        }

        public void AttackTarget(GameObject target)
        {
            if (target.TryGetComponent<IAttackable>(out var attackable))
            {
                _pendingTarget = attackable;
                _pendingTargetGO = target;

                Vector3 closest = attackable.GetClosestPoint(transform.position);

                var targetCell = GridManager.Instance.LayoutGrid.WorldToCell(closest);
                if (GridManager.Instance.IsCellOccupied(targetCell))
                {
                    var nearest = SpawnPointUtility.FindNearestFreeCell(targetCell, Vector2Int.one);
                    MoveTo(GridManager.Instance.LayoutGrid.CellToWorld(nearest));
                }
                else
                {
                    MoveTo(closest);
                }
            }
        }


        private bool IsWithinBounds(Vector3Int cell)
        {
            return cell.x >= 0 && cell.x < GridManager.Instance.GridWidth &&
                   cell.y >= 0 && cell.y < GridManager.Instance.GridHeight;
        }

        public void OnSelect() => _renderer.color = SelectionColors.SelectedColor;
        public void OnDeselect() => _renderer.color = SelectionColors.DeselectedColor;
        public void TakeDamage(int amount) { }
    }
}
