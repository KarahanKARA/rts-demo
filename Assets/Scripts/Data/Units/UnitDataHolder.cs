using Core.Health;
using Core.Interfaces;
using UnityEngine;

namespace Data.Units
{
    public class UnitDataHolder : MonoBehaviour, IDisplayInfoProvider
    {
        public UnitData Data;
        public UnitHealth Health;

        public string DisplayName => Data.unitName;
        public Sprite Icon => Data.icon;
        public int? AttackValue => Data.damage;
        HealthBase IDisplayInfoProvider.Health => Health;
    }
}