using GridSystem;
using UnityEngine;

namespace Utilities
{
    public static class GridUtility
    {
        public static bool IsValidCell(Vector3Int cell, Vector2Int size)
        {
            var grid = GridManager.Instance;
            return cell.x >= 0 && cell.x < grid.GridWidth &&
                   cell.y >= 0 && cell.y < grid.GridHeight &&
                   grid.IsAreaFree(cell, size);
        }
    }
}