using UnityEngine;
using Data.Buildings;

namespace Core.Factories
{
    public class BuildingFactory : IBuildingFactory
    {
        public GameObject CreateBuilding(BaseBuildingData data, Vector3 worldPosition)
        {
            var building = GameObject.Instantiate(data.prefab, worldPosition, Quaternion.identity);
            building.name = data.buildingName;
            return building;
        }
    }
}