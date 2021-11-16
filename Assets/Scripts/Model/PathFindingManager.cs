using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFindingManager
{
    public Queue<Tile> SolvePath(World world, Tile currentTile, Tile destinationTile)
    {
        return new PathAStar(world, currentTile, destinationTile).path;
    }
}
