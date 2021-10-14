using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Food
{
    private int nutrition;

    private Tile tile;

    public Tile Tile
    {
        get { return tile; }
        set { tile = value; }
    }

    private Action<Food> OnFoodExhausted;

    public Food(Tile tile) 
    {
        this.tile = tile;
        nutrition = 6;
    }

    public void Consume() 
    {
        nutrition -= 1;

        if (nutrition <= 0) 
        {
            if (OnFoodExhausted != null)
            {
                OnFoodExhausted(this);
            }
        }
    }

    public void Spread() 
    {
        foreach (Tile t in tile.GetNeighbours()) 
        {
            if (t != null)
            {
                t.Sprout();
            }
        }
    }

    public void RegisterOnFoodExhaustedCallback(Action<Food> cb)
    {
        OnFoodExhausted += cb;
    }

    public void UnregisterOnFoodExhaustedCallback(Action<Food> cb)
    {
        OnFoodExhausted -= cb;
    }
}
