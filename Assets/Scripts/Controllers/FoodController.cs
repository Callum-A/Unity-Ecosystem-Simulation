using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodController : SpriteController<Food>
{
    public Sprite FoodSprite;
    public int FoodCount { get; protected set; }

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

    public void OnFoodExhausted(Food food) 
    {
        //Destroy Game Object
        GameObject go = GetGameObjectByInstance(food);
        Destroy(go);
        food.Tile.RemoveFood();
        FoodCount--;
    }
}
