using UnityEngine;

namespace Data.Units
{
    public class UnitDataHolder : MonoBehaviour
    {
        [SerializeField] private UnitData data;
        [SerializeField] private UnitHealth health;

        public UnitData Data => data;
        public UnitHealth Health => health;
    }
}