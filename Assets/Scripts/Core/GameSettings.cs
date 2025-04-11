using UnityEngine;
using UnityEngine.Tilemaps;

namespace Core
{
    [CreateAssetMenu(menuName = "Config/Game Settings")]
    public class GameSettings : ScriptableObject
    {
        [Header("Grid Settings")]
        public int gridWidth = 20;
        public int gridHeight = 10;

        [Header("Tilemap Settings")]
        public TileBase gridTile;
        public TileBase outlineTile;
    }
}
