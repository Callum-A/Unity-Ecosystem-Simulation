using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum TileType
{
    Empty,
    Ground,
    Water,
    Sand,
    Plant
}

public class Tile
{
    public World World { get; protected set; }
    public int X { get; protected set; }
    public int Y { get; protected set; }

    private TileType _type;

    public TileType Type
    {
        get => _type;
        set
        {
            TileType oldValue = _type;
            _type = value;
            if (oldValue != _type)
            {
                OnTileTypeChangedCallback(this);
            }
        }
    }

    private Action<Tile> OnTileTypeChangedCallback;

    //private Action<Tile> OnTileCreated;

    public Tile(int x, int y, World world)
    {
        X = x;
        Y = y;
        World = world;
    }

    public Tile North()
    {
        return World.GetTileAt(X, Y + 1);
    }

    public Tile East()
    {
        return World.GetTileAt(X + 1, Y);
    }

    public Tile South()
    {
        return World.GetTileAt(X, Y - 1);
    }

    public Tile West()
    {
        return World.GetTileAt(X - 1, Y);
    }

    public Tile[] GetNeighbours()
    {
        Tile[] ns = new Tile[4];
        ns[0] = North();
        ns[1] = East();
        ns[2] = South();
        ns[3] = West();
        return ns;
    }

    public void RegisterOnTileTypeChangedCallback(Action<Tile> cb)
    {
        OnTileTypeChangedCallback += cb;
    }

    public void UnregisterOnTileTypeChangedCallback(Action<Tile> cb)
    {
        OnTileTypeChangedCallback -= cb;
    }

    //public void RegisterOnTileTypeCreatedCallback(Action<Tile> cb)
    //{
    //    OnTileCreated += cb;
    //}

    //public void UnregisterOnTileCreatedCallback(Action<Tile> cb)
    //{
    //    OnTileCreated -= cb;
    //}
}
