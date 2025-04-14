using System;
using System.Collections;
using UnityEngine;

namespace Core.Health
{
    public abstract class HealthBase : MonoBehaviour
    {
        public event Action<int, int> OnHealthChanged;
        public event Action OnDied;

        public abstract int MaxHealth { get; protected set; }
        public abstract int CurrentHealth { get; protected set; }

        protected Coroutine HitEffectCoroutine;
        protected SpriteRenderer SpriteRenderer;

        protected virtual void Awake()
        {
            SpriteRenderer = GetComponent<SpriteRenderer>();
        }

        public virtual void TakeDamage(int amount)
        {
            CurrentHealth -= amount;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
            RaiseHealthChanged(CurrentHealth, MaxHealth);

            if (CurrentHealth <= 0)
            {
                HandleDeath();
                return;
            }

            if (HitEffectCoroutine != null)
                StopCoroutine(HitEffectCoroutine);

            HitEffectCoroutine = StartCoroutine(HitEffectRoutine());
        }


        protected virtual IEnumerator HitEffectRoutine()
        {
            if (SpriteRenderer != null)
            {
                SpriteRenderer.color = Utilities.AlphaColors.HitColor;
                yield return new WaitForSeconds(0.05f);
                SpriteRenderer.color = Utilities.AlphaColors.DeselectedColor;
            }
            HitEffectCoroutine = null;
        }

        protected void RaiseHealthChanged(int current, int max) => OnHealthChanged?.Invoke(current, max);
        protected void RaiseDeath() => OnDied?.Invoke();

        protected abstract void HandleDeath(); 
    }
}