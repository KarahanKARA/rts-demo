using UnityEngine;

namespace Data.Buildings
{
    public class BuildingDataHolder : MonoBehaviour
    {
        public BaseBuildingData Data;

        private BuildingHealth _health;

        private void Awake()
        {
            _health = GetComponent<BuildingHealth>();
        }

        public BuildingHealth Health => _health;
    }
}