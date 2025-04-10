using UnityEngine;

namespace Data.Buildings
{
    [CreateAssetMenu(menuName = "Buildings/Power Plant")]
    public class PowerPlantData : BaseBuildingData
    {
        public override bool IsObstacle => true;
    }
}