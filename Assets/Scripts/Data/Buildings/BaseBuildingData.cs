using UnityEngine;

namespace Data.Buildings
{
    public abstract class BaseBuildingData : ScriptableObject
    {
        public string buildingName;
        public Sprite icon;
        public GameObject prefab;
        public Vector2Int size = Vector2Int.one;
        public int health = 100;

        public virtual bool CanProduceUnits => false;
        public virtual bool IsObstacle => false;
    }
}