using System.Collections.Generic;
using UnityEngine;

namespace GridSystem
{
    public class Pathfinder
    {
        private readonly bool[,] _grid;
        private readonly int _width;
        private readonly int _height;

        private readonly Vector2Int[] _directions = new Vector2Int[]
        {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
        };

        public Pathfinder(bool[,] walkable)
        {
            _grid = walkable;
            _width = _grid.GetLength(0);
            _height = _grid.GetLength(1);
        }

        public List<Vector3Int> FindPath(Vector3Int start, Vector3Int goal)
        {
            Queue<Vector3Int> queue = new();
            Dictionary<Vector3Int, Vector3Int> cameFrom = new();

            queue.Enqueue(start);
            cameFrom[start] = start;

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (current == goal)
                    break;

                foreach (var dir in _directions)
                {
                    Vector3Int neighbor = current + new Vector3Int(dir.x, dir.y, 0);

                    if (!IsInBounds(neighbor)) continue;
                    if (!_grid[neighbor.x, neighbor.y]) continue;
                    if (cameFrom.ContainsKey(neighbor)) continue;

                    queue.Enqueue(neighbor);
                    cameFrom[neighbor] = current;
                }
            }

            if (!cameFrom.ContainsKey(goal))
                return new(); 

            List<Vector3Int> path = new();
            Vector3Int step = goal;

            while (step != start)
            {
                path.Insert(0, step);
                step = cameFrom[step];
            }

            return path;
        }

        private bool IsInBounds(Vector3Int cell)
        {
            return cell.x >= 0 && cell.x < _width && cell.y >= 0 && cell.y < _height;
        }
    }
}