using Data.Units;
using UnityEngine;

namespace Core.Factories
{
    public class UnitFactoryMB : MonoBehaviour, IUnitFactory
    {
        [SerializeField] private Transform unitParent;

        public GameObject CreateUnit(UnitData data, Vector3 position)
        {
            return Instantiate(data.prefab, position, Quaternion.identity, unitParent);
        }
    }
}