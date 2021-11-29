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
    float persistence = 0f;
    float lacunarity = 0;
    Vector2 offset = new Vector2(0, 0);

    int seedInitial = 0;
    float scaleInitial = 0.0001f;
    int octavesInitial = 0;
    float persistenceInitial = 0f;
    float lacunarityInitial = 0;
    Vector2 offsetInitial = new Vector2(0, 0);

    public TileRegion[] regions = new TileRegion[0];

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
        persistence = EditorGUILayout.Slider("Persistance", persistence, 0, 1);
        lacunarity = EditorGUILayout.FloatField("Lacunarity", lacunarity);

        EditorGUILayout.EndToggleGroup();

        if (GUILayout.Button("Update Noise"))
        {
            UpdateNoise();
        }

        if (GUILayout.Button("Reset Noise"))
        {
            ResetNoise();
        }

        GUILayout.Space(10f);
        GUILayout.Label("Tile Levels", EditorStyles.boldLabel);
        GUILayout.Space(5f);

        if (GUILayout.Button("Increment Water Level"))
        {
            WorldController.Instance.World.ChangeWaterLevel(0.05f);
            waterHeight = WorldController.Instance.World.Data.WaterHeight;
            sandHeight = WorldController.Instance.World.Data.SandHeight;
            grassHeight = WorldController.Instance.World.Data.GrassHeight;
            UpdateLayers();
        }

        if (GUILayout.Button("Decrement Water Level"))
        {
            WorldController.Instance.World.ChangeWaterLevel(-0.05f);
            waterHeight = WorldController.Instance.World.Data.WaterHeight;
            sandHeight = WorldController.Instance.World.Data.SandHeight;
            grassHeight = WorldController.Instance.World.Data.GrassHeight;
            UpdateLayers();
        }


        advLayersEnabled = EditorGUILayout.BeginToggleGroup("Advanced Settings", advLayersEnabled);

        waterHeight = EditorGUILayout.Slider("Water Height", waterHeight, 0, 1);
        sandHeight = EditorGUILayout.Slider("Sand Height", sandHeight, 0, 1);
        grassHeight = EditorGUILayout.Slider("Grass Height", grassHeight, 0, 1);


        EditorGUILayout.EndToggleGroup();

        if (GUILayout.Button("Update Layers"))
        {
            UpdateLayers();
        }

        if (GUILayout.Button("Reset Layers"))
        {
            ResetLayers();
        }


    }

    public void Update()
    {

    }

    public void Awake()
    {
        Initalise();
    }

    private void UpdateNoise()
    {
        WorldController.Instance.World.TerrainGenerator.IsIsland = isIsland;

        WorldController.Instance.World.GenerateTerrain(seed, scale, octaves, persistence, lacunarity, offset, waterHeight, sandHeight, grassHeight);
    }

    private void ResetNoise()
    {
        seed = seedInitial;
        scale = scaleInitial;
        octaves = octavesInitial;
        persistence = persistenceInitial;
        lacunarity = lacunarityInitial;
        offset = offsetInitial;

        UpdateNoise();
    }

    private void UpdateLayers()
    {
        WorldController.Instance.World.Data.WaterHeight = waterHeight;
        WorldController.Instance.World.Data.SandHeight = sandHeight;
        WorldController.Instance.World.Data.GrassHeight = grassHeight;

        WorldController.Instance.World.UpdateTerrain();
    }

    private void ResetLayers()
    {
        waterHeight = WorldController.Instance.World.Data.WaterHeightInitial;
        sandHeight = WorldController.Instance.World.Data.SandHeightInitial;
        grassHeight = WorldController.Instance.World.Data.GrassHeightInitial;

        UpdateLayers();
    }

    private void Initalise()
    {
        //ugly variable setting hidden away in a function TODO: add iitial variables to WorldData.

        seed = WorldController.Instance.World.Data.Seed;
        scale = WorldController.Instance.World.Data.Scale;
        octaves = WorldController.Instance.World.Data.Octaves;
        persistence = WorldController.Instance.World.Data.Persistence;
        lacunarity = WorldController.Instance.World.Data.Lacunarity;
        offset = WorldController.Instance.World.Data.Offset;

        isIsland = WorldController.Instance.World.TerrainGenerator.IsIsland;

        seedInitial = WorldController.Instance.World.Data.Seed;
        scaleInitial = WorldController.Instance.World.Data.Scale;
        octavesInitial = WorldController.Instance.World.Data.Octaves;
        persistenceInitial = WorldController.Instance.World.Data.Persistence;
        lacunarityInitial = WorldController.Instance.World.Data.Lacunarity;
        offsetInitial = WorldController.Instance.World.Data.Offset;

        waterHeight = WorldController.Instance.World.Data.WaterHeight;
        sandHeight = WorldController.Instance.World.Data.SandHeight;
        grassHeight = WorldController.Instance.World.Data.GrassHeight;

        waterHeightInitial = WorldController.Instance.World.Data.WaterHeightInitial;
        sandHeightInitial = WorldController.Instance.World.Data.SandHeightInitial;
        grassHeightInitial = WorldController.Instance.World.Data.GrassHeightInitial;
    }
}
