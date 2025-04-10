using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Data.Buildings
{
    public class BuildingCatalog : MonoBehaviour
    {
        private List<BaseBuildingData> allBuildings;

        public List<BaseBuildingData> GetAllBuildings()
        {
            if (allBuildings == null)
                LoadAllBuildings();

            return allBuildings;
        }

        private void LoadAllBuildings()
        {
            allBuildings = Resources.LoadAll<BaseBuildingData>($"ScriptableObjects/Buildings").ToList();
        }
    }
}