using UnityEngine;

namespace Core.Interfaces
{
    public interface IAttackable
    {
        void TakeDamage(int amount);
        Vector3 GetPosition();
        float GetCollisionRadius();
    }
}