using Core.Health;
using Core.Interfaces;
using GridSystem;
using UnityEngine;

namespace Data.Buildings
{
    public class BuildingHealth : HealthBase, IAttackable
    {
        [SerializeField] private int maxHealth = 100;
        private int currentHealth;
        private BaseBuildingData _data;

        public override int MaxHealth => maxHealth;
        public override int CurrentHealth => currentHealth;

        public void Initialize(BaseBuildingData data)
        {
            _data = data;
            maxHealth = data.health;
            currentHealth = maxHealth;
            RaiseHealthChanged(currentHealth, maxHealth);
        }

        public override void TakeDamage(int amount)
        {
            currentHealth -= amount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            RaiseHealthChanged(currentHealth, maxHealth);

            if (currentHealth <= 0)
                DestroySelf();
        }

        public void DestroySelf()
        {
            RaiseHealthChanged(0, maxHealth);
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
            float maxX = bottomLeft.x + size.x;
            float maxY = bottomLeft.y + size.y;

            Vector3 clamped = new Vector3(
                Mathf.Clamp(fromPosition.x, minX, maxX),
                Mathf.Clamp(fromPosition.y, minY, maxY),
                0f
            );

            return clamped;
        }

        
        public float GetCollisionRadius() => Mathf.Max(_data.size.x, _data.size.y) / 2f;
        public Vector3 GetPosition() => transform.position;
    }
}