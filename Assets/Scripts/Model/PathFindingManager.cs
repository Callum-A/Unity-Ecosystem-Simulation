using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFindingManager
{

    public PathAStar path;

    public PathFindingManager()
    {
        path = new PathAStar();
    }
    public Queue<Tile> SolvePath(World world, Tile currentTile, Tile destinationTile)
    {
        return path.SolvePath(world, currentTile, destinationTile);
    }

    /// <summary>
    /// Returns a tile to use as a destination for wandering/searching. Is weighted to go in a similar direction to where they came from. Cheaper than an A* call.
    /// </summary>
    /// <param name="CurrentTile"> The current tile the animal occupies</param>
    /// <param name="LastTile">The last tile the animal occupied</param>
    /// <returns></returns>
    public Tile GetWanderTile(Tile CurrentTile, Tile LastTile)
    {
        Vector2 forwardOffset = new Vector2(CurrentTile.X - LastTile.X, CurrentTile.Y - LastTile.Y);

        // try to return forward tile, if not water
        if (Random.Range(0, 10) <= 1) // 20% chance to go in the same direction
        {
            Tile forwardTile = CurrentTile.World.GetTileAt(CurrentTile.X + (int)forwardOffset.x, CurrentTile.Y + (int)forwardOffset.y);
            if (forwardTile != null && forwardTile.Type != TileType.Water)
            {
                return forwardTile;
            }
        }

        var neighbours = CurrentTile.GetWalkableNeighboursIncludingDiagonal();

        if (neighbours.Count != 0)
        {
            if (neighbours.Count == 1)
            {
                return neighbours[0];
            }
            else
            {
                Vector2 direction = new Vector2(forwardOffset.x, forwardOffset.y).normalized;

                float bestScore = float.MinValue;
                Tile bestNeighbour = CurrentTile;

                for (int i = 0; i < 3; i++)
                {
                    Tile neighbour = neighbours[Random.Range(0, neighbours.Count)];
                    Vector2 offset = new Vector2(neighbour.X - CurrentTile.X, neighbour.Y - CurrentTile.Y);
                    float score = Vector2.Dot(offset.normalized, direction); // returns value between 1 and -1
                    if (score > bestScore)
                    {
                        bestNeighbour = neighbour;
                        bestScore = score;
                    }
                }

                return bestNeighbour;
            }
        }
        else
        {
            return CurrentTile; //we are surrounded by water.
        }
    }


    public Tile GetFollowMotherTile(Tile CurrentTile, Tile MotherTile)
    {
        Vector2 forwardOffset = new Vector2(CurrentTile.X - MotherTile.X, CurrentTile.Y - MotherTile.Y);

        var neighbours = CurrentTile.GetWalkableNeighboursIncludingDiagonal();

        if (neighbours.Count != 0)
        {
            if (neighbours.Count == 1)
            {
                return neighbours[0];
            }
            else
            {
                Vector2 direction = new Vector2(forwardOffset.x, forwardOffset.y).normalized;

                float bestScore = float.MinValue;
                Tile bestNeighbour = CurrentTile;

                for (int i = 0; i < 8; i++)
                {
                    Tile neighbour = neighbours[Random.Range(0, neighbours.Count)];
                    Vector2 offset = new Vector2(neighbour.X - CurrentTile.X, neighbour.Y - CurrentTile.Y);
                    float score = Vector2.Dot(offset.normalized, direction); // returns value between 1 and -1
                    if (score > bestScore)
                    {
                        bestNeighbour = neighbour;
                        bestScore = score;
                    }
                }

                return bestNeighbour;
            }
        }
        else
        {
            return CurrentTile; // we are surrounded by water.
        }
    }
}
