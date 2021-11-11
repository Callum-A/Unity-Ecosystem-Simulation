using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileUIController : MonoBehaviour
{
    public Text CoordinateText;

    public Text TileTypeText;
    public Text NutritionText;
    public Text FoodOccupiedText;

    private MouseController mouseController;
    
    // Start is called before the first frame update
    void Start()
    {
        mouseController = FindObjectOfType<MouseController>();
        if (mouseController == null || CoordinateText == null || TileTypeText == null || FoodOccupiedText == null)
        {
            enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Tile mouseOverTile = mouseController.GetMouseoverTile();
        if (mouseOverTile != null)
        {
            CoordinateText.text = $"Coordinate: ({mouseOverTile.X}, {mouseOverTile.Y})";
            TileTypeText.text = $"Tile Type: {mouseOverTile.Type}";
            if (mouseOverTile.HasFood())
            {
                NutritionText.text = $"Nutrition: {mouseOverTile.food.nutrition}";
                FoodOccupiedText.text = "Is Food Occupied: " + (mouseOverTile.isFoodOccupied() ? "True" : "False");
            }
            else
            {
                NutritionText.text = "Nutrition: N/A";
                FoodOccupiedText.text = "Is Food Occupied: N/A";
            }
        }
        else
        {
            CoordinateText.text = "Coordinate: N/A";
            TileTypeText.text = "Tile Type: N/A";
            NutritionText.text = "Nutrition: N/A";
            FoodOccupiedText.text = "Is Food Occupied: N/A";
        }
    }
}
