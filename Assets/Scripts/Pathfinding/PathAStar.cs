using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Priority_Queue;

public class PathAStar
{
    public Queue<Tile> path;
    public PathAStar(World world, Tile startTile, Tile endTile)
    {
        if (world.tileGraph == null)
        {
            world.tileGraph = new PathTileGraph(world);
        }
        Dictionary<Tile, PathNode<Tile>> nodes = world.tileGraph.nodes;
        if (!nodes.ContainsKey(startTile))
        {
            Debug.LogError("Pathfinding nodes does not contain start tile");
            return;
        }
        if (!nodes.ContainsKey(endTile))
        {
            Debug.LogError("Pathfinding nodes does not contain end tile");
            return;
        }
        List<PathNode<Tile>> closedSet = new List<PathNode<Tile>>();
        SimplePriorityQueue<PathNode<Tile>> openSet = new SimplePriorityQueue<PathNode<Tile>>();
        PathNode<Tile> start = nodes[startTile];
        PathNode<Tile> goal = nodes[endTile];
        openSet.Enqueue(start, 0); // Priority 0 as it is dequeued, priority will usually be f score
        Dictionary<PathNode<Tile>, PathNode<Tile>> cameFrom = new Dictionary<PathNode<Tile>, PathNode<Tile>>();
        Dictionary<PathNode<Tile>, float> gScore = new Dictionary<PathNode<Tile>, float>();
        foreach (PathNode<Tile> node in nodes.Values)
        {
            gScore[node] = Mathf.Infinity;
        }
        gScore[nodes[startTile]] = 0;
        Dictionary<PathNode<Tile>, float> fScore = new Dictionary<PathNode<Tile>, float>();
        foreach (PathNode<Tile> node in nodes.Values)
        {
            fScore[node] = Mathf.Infinity;
        }
        fScore[nodes[startTile]] = HueristicCostEstimate(start, goal);

        while (openSet.Count > 0)
        {
            PathNode<Tile> current = openSet.Dequeue();
            if (current == goal)
            {
                // Populates path with the path
                ReconstructPath(cameFrom, current);
                return;
            }
            
            closedSet.Add(current);
            foreach (PathEdge<Tile> edge in current.edges)
            {
                if (closedSet.Contains(edge.node))
                {
                    continue;
                }

                float movementCostToNeighbour = edge.node.data.MovementCost * DistanceBetween(current, edge.node);
                float tentativeGScore = gScore[current] + movementCostToNeighbour;
                if (openSet.Contains(edge.node) && tentativeGScore >= gScore[edge.node])
                {
                    continue;
                }
                cameFrom[edge.node] = current;
                gScore[edge.node] = tentativeGScore;
                fScore[edge.node] = gScore[edge.node] + HueristicCostEstimate(edge.node, goal);
                if (!openSet.Contains(edge.node))
                {
                    openSet.Enqueue(edge.node, fScore[edge.node]);
                }
                else
                {
                    openSet.UpdatePriority(edge.node, fScore[edge.node]);
                }
            }
        }
        
        // If we get here we never reach goal, no path
    }

    void ReconstructPath(Dictionary<PathNode<Tile>, PathNode<Tile>> cameFrom, PathNode<Tile> current)
    {
        // We have a path, current is the goal, walk backwards through came from to get the path
        Queue<Tile> totalPath = new Queue<Tile>(); // this will be the path backwards
        totalPath.Enqueue(current.data);
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Enqueue(current.data);
        }

        path = new Queue<Tile>(totalPath.Reverse());
    }

    float HueristicCostEstimate(PathNode<Tile> a, PathNode<Tile> b)
    {
        return Mathf.Sqrt(Mathf.Pow(a.data.X - b.data.X, 2) + Mathf.Pow(a.data.Y - b.data.Y, 2));
    }

    float DistanceBetween(PathNode<Tile> a, PathNode<Tile> b)
    {
        // Can make assumptions due to being on a grid
        // May need to change later.
        if (Mathf.Abs(a.data.X - b.data.X) + Mathf.Abs(a.data.Y - b.data.Y) == 1)
        {
            return 1f;
        }

        if (Mathf.Abs(a.data.X - b.data.X) == 1 && Mathf.Abs(a.data.Y - b.data.Y) == 1)
        {
            return 1.41421356237f;
        }
        // Otherwise do manually
        return Mathf.Sqrt(Mathf.Pow(a.data.X - b.data.X, 2) + Mathf.Pow(a.data.Y - b.data.Y, 2));
    }

    public Tile Dequeue()
    {
        return path.Dequeue();
    }

    public int Length()
    {
        if (path == null) return 0;
        return path.Count;
    }
}
