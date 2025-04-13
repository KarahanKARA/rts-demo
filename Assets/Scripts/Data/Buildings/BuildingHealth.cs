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

        public override int MaxHealth => maxHealth;
        public override int CurrentHealth => currentHealth;

        private BaseBuildingData _data;

        public void Initialize(BaseBuildingData data)
        {
            _data = data;
            maxHealth = data.health;
            currentHealth = maxHealth;
        }

        public override void TakeDamage(int amount)
        {
            currentHealth -= amount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);

            if (currentHealth <= 0)
                DestroyBuilding();
        }

        public void DestroyBuilding()
        {
            if (_data != null)
            {
                GridManager.Instance.FreeArea(transform.position, _data.size);
            }

            Destroy(gameObject);
        }
        public float GetCollisionRadius()
        {
            return Mathf.Max(_data.size.x, _data.size.y) / 2f;
        }

        public Vector3 GetPosition() => transform.position;
    }
}