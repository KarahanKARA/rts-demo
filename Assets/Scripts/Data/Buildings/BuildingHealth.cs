using System.Collections;
using GridSystem;
using UnityEngine;
using UnityEngine.Events;

namespace Data.Buildings
{
    public class BuildingHealth : MonoBehaviour
    {
        public UnityEvent<int, int> OnHealthChanged;
        public UnityEvent OnDestroyed;

        private int _maxHealth;
        private int _currentHealth;
        

        public void Initialize(int hp)
        {
            _maxHealth = hp;
            _currentHealth = hp;
        }

        public void TakeDamage(int amount)
        {
            _currentHealth -= amount;
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);

            if (_currentHealth <= 0)
            {
                OnDestroyed?.Invoke();
                Destroy(gameObject);
            }
        }
        
        private void OnDestroy()
        {
            if (TryGetComponent(out BuildingDataHolder holder))
            {
                if (GridManager.Instance != null)
                {
                    GridManager.Instance.FreeArea(transform.position, holder.Data.size);
                }
            }
        }


        public int CurrentHealth => _currentHealth;
        public int MaxHealth => _maxHealth;
    }
}