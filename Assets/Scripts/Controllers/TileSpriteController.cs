using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSpriteController : SpriteController<Tile>
{
    public Sprite GroundSprite;
    public Sprite FoodSprite;
    public Sprite WaterSprite;
    private World world => WorldController.Instance.World;
    // Start is called before the first frame update
    void Start()
    {
        for (int x = 0; x < world.Width; x++)
        {
            for (int y = 0; y < world.Height; y++)
            {
                Tile t = world.GetTileAt(x, y);
                OnTileCreated(t);
                t.RegisterOnTileTypeChangedCallback(OnTileTypeChanged);
                if (x % 10 == 0)
                {
                    t.Type = TileType.Water;
                }
            }
        }
    }

    void OnTileTypeChanged(Tile t)
    {
        GameObject tileGameObject = GetGameObjectByInstance(t);
        SpriteRenderer sr = tileGameObject.GetComponent<SpriteRenderer>();
        if (t.Type == TileType.Ground)
        {
            sr.sprite = GroundSprite;
        }
        else if (t.Type == TileType.Water)
        {
            sr.sprite = WaterSprite;
        }
        else if (t.Type == TileType.Plant)
        {
            sr.sprite = FoodSprite;
        }
        else
        {
            Debug.LogError("Unreachable unrecognised type " + t.Type);
        }
    }


    void OnTileCreated(Tile t)
    {
        GameObject tileGameObject = new GameObject();
        tileGameObject.name = $"Tile_{t.X}_{t.Y}";
        tileGameObject.transform.position = new Vector3(t.X, t.Y, 0);
        tileGameObject.transform.SetParent(transform, true);
        SpriteRenderer sr = tileGameObject.AddComponent<SpriteRenderer>();
        if (t.Type == TileType.Ground)
        {
            sr.sprite = GroundSprite;
        } 
        else if (t.Type == TileType.Water)
        {
            sr.sprite = WaterSprite;
        } 
        else if (t.Type == TileType.Plant)
        {
            sr.sprite = FoodSprite;
        } 
        else
        {
            Debug.LogError("Unreachable unrecognised type " + t.Type);
        }
        AddGameObject(t, tileGameObject);
    }
}
