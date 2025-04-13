using Core.Health;
using UnityEngine;

namespace Core.Interfaces
{
    public interface IDisplayInfoProvider
    {
        string DisplayName { get; }
        Sprite Icon { get; }
        int? AttackValue { get; }
        HealthBase Health { get; }
    }
}