using Data.Buildings;
using UnityEngine;

namespace Core.Factories
{
    public class BuildingFactory : IBuildingFactory
    {
        public GameObject CreateBuilding(BaseBuildingData data, Vector3 worldPosition)
        {
            return Object.Instantiate(data.prefab, worldPosition, Quaternion.identity);
        }
    }
}