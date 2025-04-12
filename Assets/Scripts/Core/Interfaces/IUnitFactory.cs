using Data.Units;
using UnityEngine;

namespace Core.Interfaces
{
    public interface IUnitFactory
    {
        GameObject CreateUnit(UnitData data, Vector3 position);
    }
}