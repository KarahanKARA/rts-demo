using Data.Buildings;
using UnityEngine;

namespace Core.Interfaces
{
    public interface IBuildingFactory
    {
        GameObject CreateBuilding(BaseBuildingData data, Vector3 worldPosition);
    }
}