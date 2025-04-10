using UnityEngine;

namespace Data.Buildings
{
    [CreateAssetMenu(menuName = "Buildings/Obstacle")]
    public class ObstacleData : BaseBuildingData
    {
        public override bool IsObstacle => true;
    }
}