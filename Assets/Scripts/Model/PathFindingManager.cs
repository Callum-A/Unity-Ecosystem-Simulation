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
}
