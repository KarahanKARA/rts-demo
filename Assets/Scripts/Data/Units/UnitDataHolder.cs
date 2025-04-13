using Core.Health;
using UnityEngine;
using Data.Units;

namespace Data.Units
{
    public class UnitDataHolder : SelectableDataHolder
    {
        public UnitData Data;
        public UnitHealth unitHealth;

        public override string DisplayName => Data.unitName;
        public override Sprite Icon => Data.icon;
        public override int? AttackValue => Data.damage;
        public override HealthBase Health => unitHealth;
    }
}