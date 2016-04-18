using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Pathfinding
{
    public static List<Tile> FindPath(Tile startTile, Tile goalTile, bool includePlayer)
    {

        Dictionary<Tile, Tile> predecessors = new Dictionary<Tile, Tile>();
        Dictionary<Tile, float> costs = new Dictionary<Tile, float>();
        SortedList<ScoredTile, float> priorityQueue = new SortedList<ScoredTile, float>();

        predecessors.Add(startTile, null);
        costs[startTile] = 0;
        priorityQueue.Add(new ScoredTile(startTile, 0), 0);

        int numIter = 0; ;
        while (true)
        {
            // Pop lowest-cost node from priority queue.
            Tile currentTile = priorityQueue.Keys[0].EnclosedTile;
            priorityQueue.RemoveAt(0);

            // Add neighbors if a new lowest-cost path can be made through them.
            foreach (Tile neighbor in GetNeighbors(currentTile, goalTile, includePlayer))
            {
                float newCost = costs[currentTile] + 1 + GetDistance(currentTile, neighbor);
                if (!(costs.ContainsKey(neighbor) && costs[neighbor] <= newCost))
                {
                    // Remove old cost in priorityQueue if necessary.
                    if (costs.ContainsKey(neighbor))
                    {
                        ScoredTile oldNeighbor = new ScoredTile(neighbor, costs[neighbor]);
                        if (priorityQueue.ContainsKey(oldNeighbor))
                        {
                            priorityQueue.Remove(oldNeighbor);
                        }
                    }
                    costs[neighbor] = newCost;
                    predecessors[neighbor] = currentTile;
                    priorityQueue.Add(new ScoredTile(neighbor, newCost), newCost);
                }
            }

            // Ending condition.
            if (costs.ContainsKey(goalTile))
            {
                float goalCost = costs[goalTile];
                foreach (float cost in priorityQueue.Values)
                {
                    if (cost < goalCost)
                    {
                        continue;
                    }
                }
                break;
            }

            numIter++;
            if (priorityQueue.Count == 0 || numIter > 1000)
            {
                // There is no path. :(
                return null;
            }
        }
        // Adds everything, including the start tile, to the path.
        List<Tile> tracedPath = new List<Tile>();
        Tile traceTile = goalTile;
        while (traceTile != null)
        {
            tracedPath.Add(traceTile);
            traceTile = predecessors[traceTile];
        }
        tracedPath.Reverse();
        return tracedPath;
    }

    public static List<Tile> GetNeighbors(Tile fromMe, Tile goalTile, bool includePlayer)
    {
        List<Tile> neighbors = new List<Tile>(4);

        Tile right = new Tile(fromMe.X + 1, fromMe.Y);
        if (IsViable(right, goalTile, includePlayer)) { neighbors.Add(right); }

        Tile up = new Tile(fromMe.X, fromMe.Y + 1);
        if (IsViable(up, goalTile, includePlayer)) { neighbors.Add(up); }

        Tile left = new Tile(fromMe.X - 1, fromMe.Y);
        if (IsViable(left, goalTile, includePlayer)) { neighbors.Add(left); }

        Tile down = new Tile(fromMe.X, fromMe.Y - 1);
        if (IsViable(down, goalTile, includePlayer)) { neighbors.Add(down); }
        return neighbors;
    }

    public static bool IsViable(Tile tile, Tile goalTile, bool includePlayer)
    {
        if (tile.Equals(goalTile))
        {
            return true;
        }
        else if (TileItem.GetObjectsAtPosition<FurnitureItem>(tile.X, tile.Y).Count > 0)
        {
            return false;
        }
        else if (TileItem.GetObjectsAtPosition<Wall>(tile.X, tile.Y).Count > 0)
        {
            return false;
        }
        else if (includePlayer && GetPlayerTile().Equals(tile))
        {
            return false;
        }
        return true;
    }

    public static Tile GetPlayerTile()
    {
        GameObject player = GameObject.FindObjectOfType<PlayerController>().gameObject;
        if (player == null)
        {
            return null;
        }
        return new Tile(
            TileItem.GlobalToTilePosition(player.transform.position.x),
            TileItem.GlobalToTilePosition(player.transform.position.y));
    }

    public static float GetDistance(Tile tile1, Tile tile2)
    {
        float xDist = tile1.X - tile2.X;
        float yDist = tile1.Y - tile2.Y;
        return (float)Math.Sqrt((xDist * xDist) + (yDist * yDist));
    }
}
