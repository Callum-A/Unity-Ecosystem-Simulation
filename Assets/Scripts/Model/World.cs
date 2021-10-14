using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class World
{
    public int Width { get; protected set; }
    public int Height { get; protected set; }

    public Tile[,] tiles { get; protected set; }

    private TerrainGenerator terrainGenerator;

    public World(int w, int h)
    {
        Width = w;
        Height = h;
        tiles = new Tile[Width, Height];
        terrainGenerator = new TerrainGenerator();
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                //Debug.Log($"Creating tile at {x}, {y}");
                tiles[x, y] = new Tile(x, y, this);
            }
        }

        //Debug.Log(GetTileAt(0, 0));
        //Debug.Log(GetTileAt(new Vector3(0, 0, 0)));
        //Debug.Log(GetTileAt(new Vector2(0, 0)));

    }

    public void GenerateTerrain() 
    {
        terrainGenerator.GenerateTerrain(tiles);

        foreach (Tile tile in tiles)
        {
            tile.InitialSprout();
        }
    }

    public Tile GetTileAt(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
        {
            return null;
        }

        return tiles[x, y];
    }

    public Tile GetTileAt(Vector3 coord)
    {
        return GetTileAt(Mathf.FloorToInt(coord.x), Mathf.FloorToInt(coord.y));
    }
}
