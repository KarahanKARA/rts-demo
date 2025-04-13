using System;
using System.Linq;
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

        public event Action<GameObject> OnUnitDied;

        private void Awake()
        {
            currentHealth = maxHealth;
        }

        public void Initialize(int health)
        {
            maxHealth = health;
            currentHealth = maxHealth;
            RaiseHealthChanged(currentHealth, maxHealth);
        }

        public override void TakeDamage(int amount)
        {
            currentHealth -= amount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            RaiseHealthChanged(currentHealth, maxHealth);

            if (currentHealth <= 0)
                Die();
        }

        private void Die()
        {
            RaiseDeath();
            OnUnitDied?.Invoke(gameObject);

            if (SelectionManager.Instance != null &&
                SelectionManager.Instance.UnitSelector.GetSelected().Contains(GetComponent<ISelectable>()))
            {
                SelectionManager.Instance.UnitSelector.DeselectAllPublic();
                SelectionManager.Instance.Deselect();
            }

            string key = gameObject.name.Replace("(Clone)", "").Trim();
            if (ObjectPoolManager.Instance != null)
                ObjectPoolManager.Instance.Release(key, gameObject);
            else
                Destroy(gameObject);
            
            if (SelectionManager.Instance != null)
            {
                var selectable = GetComponent<ISelectable>();
                SelectionManager.Instance.UnitSelector.RemoveFromSelection(selectable);
            }
        }
        
        public Vector3 GetClosestPoint(Vector3 fromPosition)
        {
            return transform.position;
        }

        public float GetCollisionRadius() => 0.5f;
        public Vector3 GetPosition() => transform.position;
    }
}