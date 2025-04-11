using Data.Buildings;
using UnityEngine;

namespace Core.Factories
{
    public interface IBuildingFactory
    {
        GameObject CreateBuilding(BaseBuildingData data, Vector3 worldPosition);
    }
}