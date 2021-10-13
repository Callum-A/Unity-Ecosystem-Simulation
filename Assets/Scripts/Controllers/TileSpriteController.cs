using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSpriteController : SpriteController<Tile>
{
    public Sprite GroundSprite;
    public Sprite FoodSprite;
    public Sprite WaterSprite;
    public Sprite SandSprite;

    // Start is called before the first frame update

    public void OnTileTypeChanged(Tile t)
    {
        GameObject tileGameObject = GetGameObjectByInstance(t);
        SpriteRenderer sr = tileGameObject.GetComponent<SpriteRenderer>();

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

    }

    public void OnTileCreated(Tile t)
    {
        GameObject tileGameObject = new GameObject();
        tileGameObject.name = $"Tile_{t.X}_{t.Y}";
        tileGameObject.transform.position = new Vector3(t.X, t.Y, 0);
        tileGameObject.transform.SetParent(transform, true);
        SpriteRenderer sr = tileGameObject.AddComponent<SpriteRenderer>();

        // is this needed?

        //switch (t.Type)
        //{
        //    case TileType.Water:
        //        sr.sprite = WaterSprite;
        //        break;

        //    case TileType.Sand:
        //        sr.sprite = SandSprite;
        //        break;

        //    case TileType.Ground:
        //        sr.sprite = GroundSprite;
        //        break;

        //    case TileType.Plant:
        //        sr.sprite = FoodSprite;
        //        break;

        //    case TileType.Empty:
        //        break;

        //    default:
        //        Debug.LogError("Unreachable unrecognised type " + t.Type);
        //        break;
        //}

        AddGameObject(t, tileGameObject);
    }
}
