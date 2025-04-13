using System;
using System.Linq;
using Core.Health;
using Core.Interfaces;
using Managers;
using UnityEngine;

namespace Data.Units
{
    public class UnitHealth : HealthBase, IAttackable
    {
        [SerializeField] private int maxHealth = 10;
        public override int MaxHealth { get; protected set; }
        public override int CurrentHealth { get; protected set; }

        public event Action<GameObject> OnUnitDied;

        protected override void Awake()
        {
            base.Awake();
            MaxHealth = maxHealth;
            CurrentHealth = MaxHealth;
        }

        public void Initialize(int health)
        {
            MaxHealth = health;
            CurrentHealth = MaxHealth;
            RaiseHealthChanged(CurrentHealth, MaxHealth);
        }

        protected override void HandleDeath()
        {
            RaiseDeath();
            OnUnitDied?.Invoke(gameObject);

            var selectable = GetComponent<ISelectable>();
            var selector = SelectionManager.Instance?.UnitSelector;

            if (selector != null && selector.GetSelected().Contains(selectable))
            {
                selector.DeselectAllPublic();
                SelectionManager.Instance.Deselect();
            }

            string key = gameObject.name.Replace("(Clone)", "").Trim();

            if (Managers.ObjectPoolManager.Instance != null)
                Managers.ObjectPoolManager.Instance.Release(key, gameObject);
            else
                Destroy(gameObject);

            selector?.RemoveFromSelection(selectable);
        }

        public Vector3 GetClosestPoint(Vector3 fromPosition) => transform.position;
        public float GetCollisionRadius() => 0.5f;
        public Vector3 GetPosition() => transform.position;
    }
}