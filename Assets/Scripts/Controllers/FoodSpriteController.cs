using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpriteController : SpriteController<Food>
{
    public Sprite FoodSprite;
    
    /// <summary>
    /// Keeps track of the number of food sprites for testing.
    /// </summary>
    public int FoodCount { get; protected set; }

    /// <summary>
    /// Called when food has been spawned, creates and sets up initial Sprite for them.
    /// </summary>
    /// <param name="food">Food object to handle</param>
    public void OnFoodSpawned(Food food) 
    {
        if (food != null) 
        {
            Tile tileData = food.Tile;

            GameObject gameObject = new GameObject();
            gameObject.name = "Food - " + tileData.X + "," + tileData.Y;
            gameObject.transform.position = new Vector3(tileData.X, tileData.Y);
            gameObject.transform.SetParent(transform, true);
            SpriteRenderer sr = gameObject.AddComponent<SpriteRenderer>();
            sr.sprite = FoodSprite;
            sr.sortingOrder = 1;

            AddGameObject(food, gameObject);

            FoodCount++;
        }
    }

    /// <summary>
    /// Called when food has been exhasted, destroys the game object and removes it from the tile.
    /// </summary>
    /// <param name="food">Food object to handle</param>
    public void OnFoodExhausted(Food food) 
    {
        //Destroy Game Object
        GameObject go = GetGameObjectByInstance(food);
        Destroy(go);
        food.Tile.RemoveFood();
        FoodCount--;
    }
}
