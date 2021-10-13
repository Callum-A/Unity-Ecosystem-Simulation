using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour
{
    public int Width;
    public int Height;

    public TileSpriteController TileSpriteController;
    public static WorldController Instance { get; protected set; }
    public World World { get; protected set; }
    private void OnEnable()
    {
        if (Instance != null)
        {
            Debug.LogError("This shouldnt be reachable");
        }
        Instance = this;

        InitialiseTiles();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InitialiseTiles() 
    {
        World = new World(Width, Height);

        for (int x = 0; x < World.Width; x++)
        {
            for (int y = 0; y < World.Height; y++)
            {
                Tile t = World.GetTileAt(x, y);

                TileSpriteController.OnTileCreated(t);
                t.RegisterOnTileTypeChangedCallback(TileSpriteController.OnTileTypeChanged);

            }
        }

        World.GenerateTerrain();
    }
}
