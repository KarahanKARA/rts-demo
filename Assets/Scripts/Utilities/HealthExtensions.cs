using Core.Health;
using UnityEngine;

namespace Utilities
{
    public static class HealthExtensions
    {
        public static void Kill(this GameObject gameObject)
        {
            if (gameObject.TryGetComponent<HealthBase>(out var health))
            {
                health.TakeDamage(health.MaxHealth);
            }
            else
            {
                Object.Destroy(gameObject);
            }
        }
    }
}