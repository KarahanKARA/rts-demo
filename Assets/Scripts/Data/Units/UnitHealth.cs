using System;
using Core.Health;
using Core.Interfaces;
using UnityEngine;
using Managers;

namespace Data.Units
{
    public class UnitHealth : HealthBase, IAttackable
    {
        [SerializeField] private int maxHealth = 10;
        private int currentHealth;

        public override int MaxHealth => maxHealth;
        public override int CurrentHealth => currentHealth;

        public event Action<GameObject> OnDied;

        private void Awake()
        {
            currentHealth = maxHealth;
        }
        public void Initialize(int health)
        {
            maxHealth = health;
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
            OnDied?.Invoke(gameObject);

            string key = gameObject.name.Replace("(Clone)", "").Trim();
            if (ObjectPoolManager.Instance != null)
                ObjectPoolManager.Instance.Release(key, gameObject);
            else
                Destroy(gameObject);
        }
        
        public float GetCollisionRadius()
        {
            return 0.5f;
        }
        
        public Vector3 GetPosition() => transform.position;
    }
}