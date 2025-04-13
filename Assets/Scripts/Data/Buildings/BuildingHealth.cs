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

        public float GetCollisionRadius() => Mathf.Max(_data.size.x, _data.size.y) / 2f;
        public Vector3 GetPosition() => transform.position;
    }
}