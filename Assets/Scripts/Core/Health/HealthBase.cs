using UnityEngine;
using UnityEngine.Events;

namespace Core.Health
{
    public abstract class HealthBase : MonoBehaviour
    {
        public UnityEvent<int, int> OnHealthChanged = new();
        public UnityEvent OnDied = new();

        public abstract int MaxHealth { get; }
        public abstract int CurrentHealth { get; }

        public abstract void TakeDamage(int amount);

        protected void Die()
        {
            OnDied?.Invoke();
        }
    }
}