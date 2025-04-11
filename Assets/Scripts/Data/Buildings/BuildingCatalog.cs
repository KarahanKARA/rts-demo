using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Data.Buildings
{
    public class BuildingCatalog : MonoBehaviour
    {
        private List<BaseBuildingData> _allBuildings;

        public List<BaseBuildingData> GetAllBuildings()
        {
            if (_allBuildings == null)
                LoadAllBuildings();

            return _allBuildings;
        }

        private void LoadAllBuildings()
        {
            _allBuildings = Resources.LoadAll<BaseBuildingData>($"ScriptableObjects/Buildings").ToList();
        }
    }
}