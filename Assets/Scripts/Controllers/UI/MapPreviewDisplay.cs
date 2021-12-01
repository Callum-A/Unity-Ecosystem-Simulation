using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapPreviewDisplay : MonoBehaviour
{
    public RawImage MapDisplay;

    // Hardcoded Variables for menu. Don't sue //
    //-----------------------------------------//
    private int width = 100;
    private int height = 100;
    private float scale = 44;
    private int octaves = 5;
    private float persistence = 0.229f;
    private float lacunarity = 3;
    private Vector2 offset = new Vector2(0,0);
    //-----------------------------------------//

    // Call this to draw!
    public void DrawMap(float[,] noiseMap)
    {

        Color[] colourMap = new Color[width*height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float currentHeight = noiseMap[x, y];
                if (currentHeight <= WorldController.WaterLevel)
                {
                    colourMap[y * width + x] = new Color(0.45f, 0.69f, 0.9f, 1);
                }
                else if (currentHeight <= Mathf.Lerp(Mathf.Min(WorldController.WaterLevel + 0.05f, 1), 1, WorldController.AridityLevel)) // this is not very nice, but this is the easiest way to emulate how the map looks in this class
                {
                    colourMap[y * width + x] = new Color(0.95f, 0.85f, 0.41f, 1);
                }
                else if (currentHeight <= 1)
                {
                    colourMap[y * width + x] = new Color(0.49f, 0.75f, 0.24f, 1);
                }
            }
        }

        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colourMap);
        texture.Apply();

        MapDisplay.texture = texture;
    }


    public void UpdateMap() 
    {
        DrawMap(GenerateNoiseMap());
    }

    public void Start()
    {
        UpdateMap();
    }

    public float[,] GenerateNoiseMap()
    {
        float[,] noisemap = new float[width, height];

        System.Random rng = new System.Random(WorldController.WorldSeed);
        Vector2[] octaveOffset = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = rng.Next(-100000, 100000) + offset.x;
            float offsetY = rng.Next(-100000, 100000) + offset.y;
            octaveOffset[i] = new Vector2(offsetX, offsetY);
        }

        //prevents division by 0 errors.
        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = width / 2f;
        float halfHeight = height / 2f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth) / scale * frequency + octaveOffset[i].x;
                    float sampleY = (y - halfHeight) / scale * frequency + octaveOffset[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1; // range -1 to 1
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                switch (WorldController.WorldType)
                {
                    case 1:

                        noiseHeight = noiseHeight + (float)Mathf.Sqrt((halfWidth - x) * (halfWidth - x) + (halfHeight - y) * (halfHeight - y)) / 50;
                        break;

                    case 2:

                        noiseHeight = noiseHeight - (float)Mathf.Sqrt((halfWidth - x) * (halfWidth - x) + (halfHeight - y) * (halfHeight - y)) / 25;
                        break;
                }


                // update min and max noise values;
                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }

                noisemap[x, y] = noiseHeight;
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                noisemap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noisemap[x, y]);
            }
        }

        return noisemap;

    }

}
