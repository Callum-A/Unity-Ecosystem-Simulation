using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode<T>
{
    public T data;
    public PathEdge<T>[] edges;

    //public override bool Equals(object obj)
    //{
    //    return data.Equals(obj);
    //}

    //public override int GetHashCode()
    //{
    //    return data.GetHashCode();
    //}
}
 
