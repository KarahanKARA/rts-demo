using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace GridSystem
{
    public class Pathfinder
    {
        private readonly bool[,] _grid;
        private readonly int _width;
        private readonly int _height;

        private readonly Vector2Int[] _directions =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        public Pathfinder(bool[,] walkable)
        {
            _grid = walkable;
            _width = _grid.GetLength(0);
            _height = _grid.GetLength(1);
        }

        public List<Vector3Int> FindPath(Vector3Int start, Vector3Int goal)
        {
            if (!IsWalkable(start) || !IsWalkable(goal))
                return new List<Vector3Int>();

            var openSet = new PriorityQueue<Vector3Int>();
            var cameFrom = new Dictionary<Vector3Int, Vector3Int>();
            var gScore = new Dictionary<Vector3Int, int>();
            var fScore = new Dictionary<Vector3Int, int>();

            openSet.Enqueue(start, 0);
            gScore[start] = 0;
            fScore[start] = Heuristic(start, goal);

            while (openSet.Count > 0)
            {
                var current = openSet.Dequeue();

                if (current == goal)
                    return ReconstructPath(cameFrom, current);

                foreach (var dir in _directions)
                {
                    var neighbor = current + new Vector3Int(dir.x, dir.y, 0);
                    if (!IsWalkable(neighbor)) continue;

                    int tentativeG = gScore[current] + 1;

                    if (!gScore.ContainsKey(neighbor) || tentativeG < gScore[neighbor])
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeG;
                        fScore[neighbor] = tentativeG + Heuristic(neighbor, goal);
                        if (!openSet.Contains(neighbor))
                            openSet.Enqueue(neighbor, fScore[neighbor]);
                    }
                }
            }

            return new List<Vector3Int>();
        }

        private List<Vector3Int> ReconstructPath(Dictionary<Vector3Int, Vector3Int> cameFrom, Vector3Int current)
        {
            List<Vector3Int> totalPath = new() { current };
            while (cameFrom.ContainsKey(current) && cameFrom[current] != current)
            {
                current = cameFrom[current];
                totalPath.Insert(0, current);
            }
            return totalPath;
        }

        private int Heuristic(Vector3Int a, Vector3Int b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }

        private bool IsWalkable(Vector3Int cell)
        {
            return IsInBounds(cell) && _grid[cell.x, cell.y];
        }

        private bool IsInBounds(Vector3Int cell)
        {
            return cell.x >= 0 && cell.x < _width && cell.y >= 0 && cell.y < _height;
        }
    }
}
