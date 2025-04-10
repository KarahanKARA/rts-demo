using UnityEngine;
using Data.Buildings;

namespace Core.Factories
{
    public interface IBuildingFactory
    {
        public GameObject CreateBuilding(BaseBuildingData data, Vector3 worldPosition);
    }
}