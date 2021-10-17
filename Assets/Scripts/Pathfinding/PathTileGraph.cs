using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathTileGraph
{
    public Dictionary<Tile, PathNode<Tile>> nodes;
    public PathTileGraph(World world)
    {
        // Creates graph from world
        // Loop through every tile, create a node
        // Do we create nodes for non floor tiles? No.
        // Do we create nodes for unwalkable tiles? No.
        // Loop again form edges
        nodes = new Dictionary<Tile, PathNode<Tile>>();
        for (int i = 0; i < world.Width; i++)
        {
            for (int j = 0; j < world.Height; j++)
            {
                Tile t = world.GetTileAt(i, j);
                // if (t.movementCost > 0) // Ignore impassable
                // {
                    PathNode<Tile> n = new PathNode<Tile>();
                    n.data = t;
                    nodes.Add(t, n);
                // }
            }
        }

        int edgeCount = 0;
        foreach (KeyValuePair<Tile, PathNode<Tile>> pair in nodes)
        {
            // Get list of neighbours, then create edges if neighbour is walkable
            Tile tile = pair.Key;
            PathNode<Tile> n = pair.Value;
            List<PathEdge<Tile>> edges = new List<PathEdge<Tile>>();
            Tile[] neighbours = tile.GetNeighbours(); // diag okay note some spots can be null
            for (int i = 0; i < neighbours.Length; i++)
            {
                Tile neighbour = neighbours[i];
                if (neighbour != null && neighbour.MovementCost > 0)
                {
                    // Make sure we aren't clipping a diagonal or squeeze inappropriately
                    if (IsClippingCorner(tile, neighbour))
                    {
                        continue;
                    }
                    // This neighbour exists and is walkable create an edge
                    PathEdge<Tile> e = new PathEdge<Tile>();
                    e.cost = neighbour.MovementCost;
                    e.node = nodes[neighbour];
                    edges.Add(e);
                }
            }

            edgeCount += edges.Count;
            n.edges = edges.ToArray();
        }
        Debug.Log($"Graph created with {nodes.Count} nodes and {edgeCount} edges");
    }

    bool IsClippingCorner(Tile curr, Tile neighbour)
    {
        // If movement from curr to neigh is diagonal e.g. NE
        // Then check to make sure we aren't clipping e.g. N and E are both walkable
        if (Mathf.Abs(curr.X - neighbour.X) + Mathf.Abs(curr.Y - neighbour.Y) == 2)
        {
            // We are diagonal
            int dX = curr.X - neighbour.X;
            int dY = curr.Y - neighbour.Y;
            if (curr.World.GetTileAt(curr.X - dX, curr.Y).MovementCost == 0)
            {
                // East or West is unwalkable so is a clipped movement
                return true;
            }
            
            if (curr.World.GetTileAt(curr.X, curr.Y - dY).MovementCost == 0)
            {
                // North or South is unwalkable so is a clipped movement
                return true;
            }
        }

        return false; // Not clipping corner
    }
}
