using Core.Health;
using Core.Interfaces;
using GridSystem;
using UnityEngine;

namespace Data.Buildings
{
    public class BuildingHealth : HealthBase, IAttackable
    {
        private BaseBuildingData _data;

        public override int MaxHealth { get; protected set; }
        public override int CurrentHealth { get; protected set; }

        protected override void Awake()
        {
            base.Awake();
        }

        public void Initialize(BaseBuildingData data)
        {
            _data = data;
            MaxHealth = data.health;
            CurrentHealth = MaxHealth;
            RaiseHealthChanged(CurrentHealth, MaxHealth);
        }

        protected override void HandleDeath()
        {
            RaiseHealthChanged(0, MaxHealth);
            RaiseDeath();

            if (_data != null)
                GridManager.Instance.FreeArea(transform.position, _data.size);

            Destroy(gameObject);
        }

        public Vector3 GetClosestPoint(Vector3 fromPosition)
        {
            Vector3 myPos = transform.position;
            Vector2Int size = _data.size;
            Vector3Int centerCell = GridManager.Instance.LayoutGrid.WorldToCell(myPos);
            Vector3Int bottomLeft = GridManager.Instance.GetBottomLeftCell(centerCell, size);

            float minX = bottomLeft.x;
            float minY = bottomLeft.y;
            float maxX = minX + size.x;
            float maxY = minY + size.y;

            return new Vector3(
                Mathf.Clamp(fromPosition.x, minX, maxX),
                Mathf.Clamp(fromPosition.y, minY, maxY),
                0f
            );
        }

        public float GetCollisionRadius() => Mathf.Max(_data.size.x, _data.size.y) / 2f;
        public Vector3 GetPosition() => transform.position;
    }
}