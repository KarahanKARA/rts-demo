using Data.Units;
using UnityEngine;

namespace Core.Factories
{
    public interface IUnitFactory
    {
        GameObject CreateUnit(UnitData data, Vector3 position);
    }
}