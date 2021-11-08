using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpriteController : SpriteController<Food>
{
    public Sprite FoodSprite;
    public Sprite FoodSprite5; // name is food sprite and then number of nutri it is displayed at so this is displayed when nutri = 5
    public Sprite FoodSprite4;
    public Sprite FoodSprite3;
    public Sprite FoodSprite2;
    public Sprite FoodSprite1;
    
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
        switch (food.nutrition)
        {
            case 6:
                sr.sprite = FoodSprite;
                break;
            case 5:
                sr.sprite = FoodSprite5;
                break;
            case 4:
                sr.sprite = FoodSprite4;
                break;
            case 3:
                sr.sprite = FoodSprite3;
                break;
            case 2:
                sr.sprite = FoodSprite2;
                break;
            case 1:
                sr.sprite = FoodSprite1;
                break;
            default:
                Debug.Log("REACHED UNREACHABLE IN FSC SWITCH");
                break;
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

        
    }
}
