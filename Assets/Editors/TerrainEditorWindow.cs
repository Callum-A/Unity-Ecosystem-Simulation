using UnityEngine;
using UnityEditor;

public class TerrainEditorWindow : EditorWindow
{
    #region GUI

    bool advNoiseEnabled;

    bool advLayersEnabled;

   // bool showNoise = true;
  // bool showLayers = true;

    #endregion

    #region noiseVars

    int seed = 0;
    float scale = 0.0001f;
    int octaves = 0;
    float persistance = 0f;
    float lacunarity = 0;
    Vector2 offset = new Vector2(0, 0);

    int seedInitial = 0;
    float scaleInitial = 0.0001f;
    int octavesInitial = 0;
    float persistanceInitial = 0f;
    float lacunarityInitial = 0;
    Vector2 offsetInitial = new Vector2(0, 0);

    bool isIsland = false;

    #endregion

    #region tileLayers

    private float waterHeight = 0f;
    private float sandHeight = 0f;
    private float grassHeight = 0f;

    private float waterHeightInitial = 0f;
    private float sandHeightInitial = 0f;
    private float grassHeightInitial = 0f;
    #endregion


    [MenuItem("Window/Terrain Editor")]
    public static void init()
    {
        TerrainEditorWindow window = (TerrainEditorWindow)EditorWindow.GetWindow(typeof(TerrainEditorWindow));
        window.Show();
    }

    void OnGUI()
    {

        GUILayout.Label("Noise Settings", EditorStyles.boldLabel);
        GUILayout.Space(5f);

        seed = EditorGUILayout.IntField("Seed", seed);

        isIsland = EditorGUILayout.Toggle("Island Generation", isIsland);

        advNoiseEnabled = EditorGUILayout.BeginToggleGroup("Advanced Settings", advNoiseEnabled);

        scale = EditorGUILayout.FloatField("Scale", scale);
        octaves = EditorGUILayout.IntField("Octaves", octaves);
        persistance = EditorGUILayout.Slider("Persistance", persistance, 0, 1);
        lacunarity = EditorGUILayout.FloatField("Lacunarity", lacunarity);

        EditorGUILayout.EndToggleGroup();

        if (GUILayout.Button("Update Noise"))
        {
            updateNoise();
        }

        if (GUILayout.Button("Reset Noise"))
        {
            resetNoise();
        }

        GUILayout.Space(10f);
        GUILayout.Label("Tile Levels", EditorStyles.boldLabel);
        GUILayout.Space(5f);

        if (GUILayout.Button("Increment Water Level"))
        {
            WorldController.Instance.World.ChangeWaterLevel(0.1f);
            waterHeight = WorldController.Instance.World.TerrainGenerator.WaterHeight;
            sandHeight = WorldController.Instance.World.TerrainGenerator.SandHeight;
            grassHeight = WorldController.Instance.World.TerrainGenerator.GrassHeight;
            updateLayers();
        }

        if (GUILayout.Button("Decrement Water Level"))
        {
            WorldController.Instance.World.ChangeWaterLevel(-0.1f);
            waterHeight = WorldController.Instance.World.TerrainGenerator.WaterHeight;
            sandHeight = WorldController.Instance.World.TerrainGenerator.SandHeight;
            grassHeight = WorldController.Instance.World.TerrainGenerator.GrassHeight;
            updateLayers();
        }


        advLayersEnabled = EditorGUILayout.BeginToggleGroup("Advanced Settings", advLayersEnabled);

        waterHeight = EditorGUILayout.Slider("Water Height", waterHeight, 0, 1);
        sandHeight = EditorGUILayout.Slider("Sand Height", sandHeight, 0, 1);
        grassHeight = EditorGUILayout.Slider("Grass Height", grassHeight, 0, 1);

        EditorGUILayout.EndToggleGroup();

        if (GUILayout.Button("Update Layers"))
        {
            updateLayers();
        }

        if (GUILayout.Button("Reset Layers"))
        {
            resetLayers();
        }


    }

    public void Update()
    {

    }

    public void Awake()
    {
        initalise();
    }

    private void updateNoise()
    {
        WorldController.Instance.World.TerrainGenerator.IsIsland = isIsland;

        WorldController.Instance.World.GenerateTerrain();
    }

    private void resetNoise()
    {
        seed = seedInitial;
        scale = scaleInitial;
        octaves = octavesInitial;
        persistance = persistanceInitial;
        lacunarity = lacunarityInitial;
        //offset = offsetInitial;

        updateNoise();
    }

    private void updateLayers()
    {
        WorldController.Instance.World.Data.WaterHeight = waterHeight;
        WorldController.Instance.World.Data.SandHeight = sandHeight;
        WorldController.Instance.World.Data.GrassHeight = grassHeight;

        WorldController.Instance.World.UpdateTerrain();
    }

    private void resetLayers()
    {
        waterHeight = WorldController.Instance.World.Data.WaterHeightInitial;
        sandHeight = WorldController.Instance.World.Data.SandHeightInitial;
        grassHeight = WorldController.Instance.World.Data.GrassHeightInitial;

        updateLayers();
    }

    private void initalise()
    {
        //ugly variable setting hidden away in a function TODO: add iitial variables to WorldData.

        seed = WorldController.Instance.World.Data.Seed;
        scale = WorldController.Instance.World.Data.Scale;
        octaves = WorldController.Instance.World.Data.Octaves;
        persistance = WorldController.Instance.World.Data.Persistence;
        lacunarity = WorldController.Instance.World.Data.Lacunarity;
        offset = WorldController.Instance.World.Data.Offset;

        isIsland = WorldController.Instance.World.TerrainGenerator.IsIsland;

        seedInitial = WorldController.Instance.World.Data.Seed;
        scaleInitial = WorldController.Instance.World.Data.Scale;
        octavesInitial = WorldController.Instance.World.Data.Octaves;
        persistanceInitial = WorldController.Instance.World.Data.Persistence;
        lacunarityInitial = WorldController.Instance.World.Data.Lacunarity;
        offsetInitial = WorldController.Instance.World.Data.Offset;

        waterHeight = WorldController.Instance.World.TerrainGenerator.WaterHeight;
        sandHeight = WorldController.Instance.World.TerrainGenerator.SandHeight;
        grassHeight = WorldController.Instance.World.TerrainGenerator.GrassHeight;

        waterHeightInitial = WorldController.Instance.World.TerrainGenerator.WaterHeight;
        sandHeightInitial = WorldController.Instance.World.TerrainGenerator.SandHeight;
        grassHeightInitial = WorldController.Instance.World.TerrainGenerator.GrassHeight;
    }
}
