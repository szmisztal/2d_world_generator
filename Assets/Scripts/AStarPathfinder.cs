using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class AStarPathfinder
{
    public static List<Vector3Int> FindPath(Tilemap tilemap, Vector3Int start, Vector3Int goal, HashSet<TileBase> walkableTiles)
    {
        var openSet = new PriorityQueue<Vector3Int, int>();
        var cameFrom = new Dictionary<Vector3Int, Vector3Int>();

        var gScore = new Dictionary<Vector3Int, int>();
        gScore[start] = 0;

        var fScore = new Dictionary<Vector3Int, int>();
        fScore[start] = Heuristic(start, goal);

        openSet.Enqueue(start, fScore[start]);

        while (openSet.Count > 0)
        {
            var current = openSet.Dequeue();
            if (current.Equals(goal))
            {
                return ReconstructPath(cameFrom, current);
            }

            foreach (var neighbor in GetNeighbors(tilemap, current, walkableTiles))
            {
                var tentativeGScore = gScore.TryGetValue(current, out int currentGScore) ? currentGScore + 1 : int.MaxValue;

                if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor, goal);

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Enqueue(neighbor, fScore[neighbor]);
                    }
                }
            }
        }

        return new List<Vector3Int>();
    }

    private static int Heuristic(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private static List<Vector3Int> ReconstructPath(Dictionary<Vector3Int, Vector3Int> cameFrom, Vector3Int current)
    {
        var totalPath = new List<Vector3Int> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Add(current);
        }
        totalPath.Reverse();
        return totalPath;
    }

    private static IEnumerable<Vector3Int> GetNeighbors(Tilemap tilemap, Vector3Int node, HashSet<TileBase> walkableTiles)
    {
        var directions = new Vector3Int[] {
        new Vector3Int(1, 0, 0), new Vector3Int(-1, 0, 0),
        new Vector3Int(0, 1, 0), new Vector3Int(0, -1, 0)
    };

        foreach (var direction in directions)
        {
            Vector3Int neighbor = node + direction;
            if (tilemap.cellBounds.Contains(neighbor) && walkableTiles.Contains(tilemap.GetTile(neighbor)))
            {
                yield return neighbor;
            }
        }
    }
}