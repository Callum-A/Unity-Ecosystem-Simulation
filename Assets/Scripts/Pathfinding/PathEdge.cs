using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathEdge<T>
{
    public float cost; // cost to traverse the tile
    public PathNode<T> node;
}
