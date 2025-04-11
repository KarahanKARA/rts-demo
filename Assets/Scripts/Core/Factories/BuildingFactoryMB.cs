using Data.Buildings;
using UnityEngine;

namespace Core.Factories
{
    public class BuildingFactoryMB : MonoBehaviour, IBuildingFactory
    {
        public GameObject CreateBuilding(BaseBuildingData data, Vector3 worldPosition)
        {
            return Instantiate(data.prefab, worldPosition, Quaternion.identity);
        }
    }
}