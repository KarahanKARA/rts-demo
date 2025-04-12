using Core.Health;
using UnityEngine;
using UnityEngine.Events;

namespace Data.Buildings
{
    public class BuildingHealth : HealthBase
    {
        [SerializeField] private int maxHealth = 100;
        private int currentHealth;

        public override int MaxHealth => maxHealth;
        public override int CurrentHealth => currentHealth;

        public void Initialize(int health)
        {
            maxHealth = health;
            currentHealth = health;
            OnHealthChanged.Invoke(currentHealth, maxHealth);
        }

        public override void TakeDamage(int amount)
        {
            currentHealth -= amount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            OnHealthChanged.Invoke(currentHealth, maxHealth);

            if (currentHealth <= 0)
                DestroyBuilding();
        }

        public void DestroyBuilding()
        {
            // Existing destroy logic
        }
    }
}