using Core.Health;
using UnityEngine;

namespace Data.Units
{
    public class UnitHealth : HealthBase
    {
        [SerializeField] private int maxHealth = 10;
        private int currentHealth;

        public override int MaxHealth => maxHealth;
        public override int CurrentHealth => currentHealth;

        private void Awake()
        {
            currentHealth = maxHealth;
        }

        public void Initialize(int healthFromData)
        {
            maxHealth = healthFromData;
            currentHealth = maxHealth;
            OnHealthChanged.Invoke(currentHealth, maxHealth);
        }

        public override void TakeDamage(int amount)
        {
            currentHealth -= amount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            OnHealthChanged.Invoke(currentHealth, maxHealth);

            if (currentHealth <= 0)
                Die();
        }

        private void Die()
        {
            string key = gameObject.name.Replace("(Clone)", "").Trim();
            if (Managers.ObjectPoolManager.Instance != null)
                Managers.ObjectPoolManager.Instance.Release(key, gameObject);
            else
                Destroy(gameObject);
        }
    }
}