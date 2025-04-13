using Core.Health;
using UnityEngine;

namespace Data.Buildings
{
    public class BuildingDataHolder : SelectableDataHolder
    {
        public BaseBuildingData Data;
        public BuildingHealth buildingHealth;

        public override string DisplayName => Data.buildingName;
        public override Sprite Icon => Data.icon;
        public override int? AttackValue => null;
        public override HealthBase Health => buildingHealth;
    }

}