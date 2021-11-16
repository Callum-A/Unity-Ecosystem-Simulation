using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFindingManager
{

    public PathAStar patsh;

    public PathFindingManager()
    {
        patsh = new PathAStar();
    }
    public Queue<Tile> SolvePath(World world, Tile currentTile, Tile destinationTile)
    {
        return patsh.doShit(world, currentTile, destinationTile);
    }
}
