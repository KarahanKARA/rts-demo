using GridSystem;
using UnityEngine;

namespace Utilities
{
    public static class SpawnPointUtility
    {
        public static Vector3Int FindNearestFreeCell(Vector3Int center, Vector2Int size)
        {
            int range = 1;
            while (range < 10)
            {
                for (int dx = -range; dx <= range; dx++)
                {
                    for (int dy = -range; dy <= range; dy++)
                    {
                        if (Mathf.Abs(dx) != range && Mathf.Abs(dy) != range)
                            continue;

                        Vector3Int testCell = new Vector3Int(center.x + dx, center.y + dy, 0);
                        if (!IsCellValid(testCell)) continue;

                        return testCell;
                    }
                }
                range++;
            }

            return Vector3Int.one * -1;
        }

        private static bool IsCellValid(Vector3Int cell)
        {
            return cell.x >= 0 && cell.x < GridManager.Instance.GridWidth &&
                   cell.y >= 0 && cell.y < GridManager.Instance.GridHeight &&
                   GridManager.Instance.IsAreaFree(cell, Vector2Int.one);
        }
    }
}