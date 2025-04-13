using Core.Health;
using Core.Interfaces;
using UnityEngine;

namespace Data.Buildings
{
    public class BuildingDataHolder : MonoBehaviour, IDisplayInfoProvider
    {
        public BaseBuildingData Data;
        public BuildingHealth Health;

        public string DisplayName => Data.buildingName;
        public Sprite Icon => Data.icon;
        public int? AttackValue => null; 
        HealthBase IDisplayInfoProvider.Health => Health;
    }

}