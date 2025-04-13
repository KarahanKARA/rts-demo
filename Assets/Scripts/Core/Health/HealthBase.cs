using System;
using UnityEngine;

namespace Core.Health
{
    public abstract class HealthBase : MonoBehaviour
    {
        public event Action<int, int> OnHealthChanged;
        public event Action OnDied;

        public abstract int MaxHealth { get; }
        public abstract int CurrentHealth { get; }

        public abstract void TakeDamage(int amount);

        protected void RaiseHealthChanged(int current, int max)
        {
            OnHealthChanged?.Invoke(current, max);
        }

        protected void RaiseDeath()
        {
            OnDied?.Invoke();
        }
    }
}