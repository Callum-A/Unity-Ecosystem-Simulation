using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Class that represents food for the prey (Vegitation).
/// </summary>
public class Food
{
    /// <summary>
    /// The Nutrition remaining on the food.
    /// </summary>
    private int nutrition;

    private Tile tile;
    /// <summary>
    /// The Tile that the food occupies.
    /// </summary>
    public Tile Tile
    {
        get { return tile; }
        set { tile = value; }
    }

    /// <summary>
    /// This is used to determine the Initial sprout rate of the food, 1 in x where x is the initial sprout rate.
    /// </summary>
    public static int InitialSproutRate { get; protected set; } = 100;

    /// <summary>
    /// This is used to determine the Initial sprout rate of the food, 1 in x where x is the spread rate.
    /// </summary>
    public static int SpreadRate { get; protected set; } = 25;

    /// <summary>
    /// Action to perform to deal with sprites when the food is exhasted.
    /// </summary>
    private Action<Food> OnFoodExhausted;

    /// <summary>
    /// Constuctor used to create a food object.
    /// </summary>
    /// <param name="tile">The tile that this food object occupies</param>
    public Food(Tile tile) 
    {
        this.tile = tile;
        nutrition = 6;
    }

    /// <summary>
    /// Called when an animal consumes a nutrition point of food.
    /// </summary>
    public void Consume() 
    {
        nutrition -= 1;

        if (nutrition <= 0) 
        {
            if (OnFoodExhausted != null)
            {
                WorldController.Instance.World.FoodTiles.Remove(tile);
                OnFoodExhausted(this);
            }
        }
    }

    /// <summary>
    /// Each cardinal dirction from this tile has a chance to sprout food, in otehr words spreads food.
    /// </summary>
    //public void Spread() 
    //{

    //    foreach (Tile t in tile.GetNeighbours()) 
    //    {
    //        if (t != null)
    //        {
    //           t.Sprout();
    //        }
    //    }
    //}

    /// <summary>
    /// Register the callback for when a food supply is Exhasted.
    /// </summary>
    /// <param name="cb"></param>
    public void RegisterOnFoodExhaustedCallback(Action<Food> cb)
    {
        OnFoodExhausted += cb;
    }

    /// <summary>
    /// DeRegister the callback for when a supply is exhasted.
    /// </summary>
    /// <param name="cb"></param>
    public void UnregisterOnFoodExhaustedCallback(Action<Food> cb)
    {
        OnFoodExhausted -= cb;
    }
}
