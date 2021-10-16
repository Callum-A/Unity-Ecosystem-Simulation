using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode<T>
{
    public T data;
    public PathEdge<T>[] edges;
}
