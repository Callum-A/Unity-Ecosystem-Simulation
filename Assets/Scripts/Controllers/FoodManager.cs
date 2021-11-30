using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Controllers
{
    public class FoodManager
    {
        public List<Tile> FoodTiles { get; protected set; }

        private Action<Food> OnFoodSproutedCallback;
        private Action<Food> OnFoodExhaustedCallback;


        public FoodManager() 
        {
            FoodTiles = new List<Tile>();
            RegisterOnFoodExhaustedCallback(FoodExhausted);
        }

        public void SpreadFoodAcrossMap() 
        {
            foreach (Tile tile in FoodTiles)
            {
                Spread(tile);
            }
        }

        public void SproutInitialFood(Tile[,] tiles)
        {
            foreach (Tile tile in tiles)
            {
                if (InitialSprout(tile))
                {
                    FoodTiles.Add(tile);
                }
            }
        }

        public void FoodExhausted(Food food) 
        {
            FoodTiles.Remove(food.Tile);
            food.Tile.RemoveFood();
        }

        /// <summary>
        /// Rolls to determine whether food will sprout on this tile, used only once on instantiation.
        /// </summary>
        private bool InitialSprout(Tile tile)
        {
            if (tile.Type == TileType.Ground && !tile.HasFood())
            {
                if (UnityEngine.Random.Range(0, Food.InitialSproutRate) == 0)
                {
                    Food newFood = new Food(tile);
                    newFood.RegisterOnFoodExhaustedCallback(OnFoodExhaustedCallback);
                    tile.addFood(newFood);
                    
                    OnFoodSproutedCallback(tile.food);

                    return true;
                }

                return false;

            }

            return false;
        }

        private bool Sprout(Tile tile) 
        {
            if (tile.Type == TileType.Ground && !tile.HasFood())
            {
                if (UnityEngine.Random.Range(0, 99) < (WorldController.SpreadRate/4))
                {
                    Food newFood = new Food(tile);
                    newFood.RegisterOnFoodExhaustedCallback(OnFoodExhaustedCallback);
                    tile.addFood(newFood);
                    OnFoodSproutedCallback(tile.food);
                    FoodTiles.Add(tile);


                    return true;
                }

                return false;

            }

            return false;
        }

        private void Spread(Tile tile) 
        {
            foreach (Tile t in tile.GetNeighbours())
            {
                if (t != null)
                {
                    Sprout(t);
                }
            }
        }

        public void RegisterOnFoodSproutedCallback(Action<Food> cb)
        {
            OnFoodSproutedCallback += cb;
        }

        public void RegisterOnFoodExhaustedCallback(Action<Food> cb)
        {
            OnFoodExhaustedCallback += cb;
        }
    }
}
