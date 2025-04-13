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

        protected Coroutine hitEffectCoroutine;
        protected SpriteRenderer spriteRenderer;

        protected virtual void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public virtual void TakeDamage(int amount)
        {
            CurrentHealth -= amount;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
            RaiseHealthChanged(CurrentHealth, MaxHealth);

            if (CurrentHealth <= 0)
                HandleDeath();

            if (hitEffectCoroutine != null)
                StopCoroutine(hitEffectCoroutine);

            hitEffectCoroutine = StartCoroutine(HitEffectRoutine());
        }

        protected virtual IEnumerator HitEffectRoutine()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Utilities.AlphaColors.HitColor;
                yield return new WaitForSeconds(0.05f);
                spriteRenderer.color = Utilities.AlphaColors.DeselectedColor;
            }
            hitEffectCoroutine = null;
        }

        protected void RaiseHealthChanged(int current, int max) => OnHealthChanged?.Invoke(current, max);
        protected void RaiseDeath() => OnDied?.Invoke();

        protected abstract void HandleDeath(); // her subclass kendine göre ölümü yönetecek
    }
}