using UnityEngine;

namespace Data.Units
{
    [CreateAssetMenu(menuName = "Units/Unit Data")]
    public class UnitData : ScriptableObject
    {
        public string unitName;
        public Sprite icon;
        public GameObject prefab;
        public int health = 10;
        public int damage = 1;
    }
}