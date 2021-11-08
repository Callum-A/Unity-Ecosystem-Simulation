using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpriteController : SpriteController<Food>
{
    public Sprite FoodSprite;

    private void Start()
    {
        LoadSprites("Images/Tile");
    }

    /// <summary>
    /// Keeps track of the number of food sprites for testing.
    /// </summary>

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
        }
    }

    /// <summary>
    /// Change the food sprite based on how much nutri it has left, also handles food clean up when it hits 0.
    /// </summary>
    /// <param name="food">Food that has changed.</param>
    public void OnFoodChanged(Food food)
    {
        GameObject go = GetGameObjectByInstance(food);
        SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
        string b = "Bushes32x32_02";
        Sprite s;
        if (food.nutrition == 6)
        {
            s = GetSpriteByName(b);
        }
        else
        {
            s = GetSpriteByName(b + "_" + food.nutrition);
        }
        sr.sprite = s;
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

        
    }
}
