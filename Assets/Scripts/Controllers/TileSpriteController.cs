using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSpriteController : SpriteController<Tile>
{
    public Sprite GroundSprite;
    public Sprite FoodSprite;
    public Sprite WaterSprite;
    public Sprite SandSprite;
    public Sprite DefualtSprite;

   // private World world = WorldController.Instance.World;

    public DrawMode mode = DrawMode.Default;
    public Color[] HeatColours = new Color[] { Color.blue, Color.green, Color.yellow, Color.red };

    FoodSpriteController FoodController;

    float t1;

    public enum DrawMode
    {
        Default,
        Noise,
        HeatMap
    }

    public void ToggleHeatMap() 
    {
        mode = (mode == DrawMode.Default) ? DrawMode.HeatMap : DrawMode.Default;
        t1 = 0f;
    }

    public void OnTileTypeChanged(Tile t)
    {

        GameObject tileGameObject = GetGameObjectByInstance(t);
        SpriteRenderer sr = tileGameObject.GetComponent<SpriteRenderer>();

        switch (mode)
        {
            case DrawMode.Default:
                sr.color = new Color(1, 1, 1, 1); // reset sprite renderer colour
                switch (t.Type)
                {

                    case TileType.Water:
                        sr.sprite = WaterSprite;
                        break;

                    case TileType.Sand:
                        sr.sprite = SandSprite;
                        break;

                    case TileType.Ground:
                        sr.sprite = GroundSprite;
                        break;

                    case TileType.Plant:
                        sr.sprite = FoodSprite;
                        break;

                    case TileType.Empty:
                        Debug.LogError("Reached Empty tile type during runtime " + t.Type);
                        break;

                    default:
                        Debug.LogError("Unreachable unrecognised type " + t.Type);
                        break;
                }
                break;

            case DrawMode.Noise:
                
                sr.color = Color.Lerp(Color.black, Color.white, WorldController.Instance.World.Data.Noisemap[t.X, t.Y]);
 
                sr.sprite = DefualtSprite;

                break;

            case DrawMode.HeatMap:

                float scaledTime = (t.HeatCounter)/ (WorldController.Instance.World.Data.HeatMapMax) * (HeatColours.Length - 1);

                Color oldColor = HeatColours[(int)scaledTime];
                Color newColor = HeatColours[scaledTime < HeatColours.Length-1 ? (int)scaledTime + 1 : HeatColours.Length-1];
                float newT = scaledTime - Mathf.Floor(scaledTime);

                sr.color = Color.Lerp(oldColor, newColor, newT);
                sr.sprite = DefualtSprite;

                break;
        }

    }

    public void OnTileCreated(Tile t)
    {
        GameObject tileGameObject = new GameObject();
        tileGameObject.name = $"Tile_{t.X}_{t.Y}";
        tileGameObject.transform.position = new Vector3(t.X, t.Y, 0);
        tileGameObject.transform.SetParent(transform, true);
        SpriteRenderer sr = tileGameObject.AddComponent<SpriteRenderer>();

        AddGameObject(t, tileGameObject);
    }

    private void OnEnable()
    {
        t1 = Time.realtimeSinceStartup;
    }

    public void Update()
    {

        if (mode == TileSpriteController.DrawMode.HeatMap)
        {
            if (Time.realtimeSinceStartup - t1 > 2f)
            {
                WorldController.Instance.World.Data.updateHeatMap();
                WorldController.Instance.World.ForceTileUpdate();
                t1 = Time.realtimeSinceStartup;
            }
        }

        else 
        {
            WorldController.Instance.World.ForceTileUpdate();
        }
    }

}
