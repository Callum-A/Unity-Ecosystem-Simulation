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
    public int nutrition { get; protected set; }

    private Tile tile;
    /// <summary>
    /// The Tile that the food occupies.
    /// </summary>
    public Tile Tile
    {
        get { return tile; }
        set { tile = value; }
    }

    private bool isOccupied;

    public bool IsOccupied
    {
        get { return isOccupied; }
        set { isOccupied = value; }
    }


    /// <summary>
    /// This is used to determine the Initial sprout rate of the food, 1 in x where x is the initial sprout rate.
    /// </summary>
    public static int InitialSproutRate { get; protected set; } = 100;

    /// <summary>
    /// Action to perform to deal with sprites when the food is exhasted.
    /// </summary>
    private Action<Food> OnFoodExhausted;

    /// <summary>
    /// Action to change sprite rendering based on nutrition.
    /// </summary>
    private Action<Food> OnFoodChanged;

    /// <summary>
    /// Constuctor used to create a food object.
    /// </summary>
    /// <param name="tile">The tile that this food object occupies</param>
    public Food(Tile tile) 
    {
        this.tile = tile;
        nutrition = 6;
        this.isOccupied = false;
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
                OnFoodExhausted(this);
            }
        }
        else
        {
            if (OnFoodChanged != null)
            {
                OnFoodChanged(this);
            }
        }
    }

    public void Drown() 
    {
        OnFoodExhausted(this);
    }

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

    /// <summary>
    /// Register food changed callback to update sprite.
    /// </summary>
    /// <param name="cb"></param>
    public void RegisterOnFoodChangedCallback(Action<Food> cb)
    {
        OnFoodChanged += cb;
    }
}
