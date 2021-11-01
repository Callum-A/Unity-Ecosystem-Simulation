using UnityEngine;
using UnityEditor;

public class TerrainEditorWindow : EditorWindow
{
    #region GUI

    bool advNoiseEnabled;

    bool advLayersEnabled;

    bool showNoise = true;
    bool showLayers = true;

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
        WorldController.Instance.World.TerrainGenerator.Seed = seed;
        WorldController.Instance.World.TerrainGenerator.Scale = scale;
        WorldController.Instance.World.TerrainGenerator.Octaves = octaves;
        WorldController.Instance.World.TerrainGenerator.Persistance = persistance;
        WorldController.Instance.World.TerrainGenerator.Lacunarity = lacunarity;
        //WorldController.Instance.World.TerrainGenerator.Offset = offset;

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
        WorldController.Instance.World.TerrainGenerator.WaterHeight = waterHeight;
        WorldController.Instance.World.TerrainGenerator.SandHeight = sandHeight;
        WorldController.Instance.World.TerrainGenerator.GrassHeight = grassHeight;

        WorldController.Instance.World.GenerateTerrain();
    }

    private void resetLayers()
    {
        waterHeight = waterHeightInitial;
        sandHeight = sandHeightInitial;
        grassHeight = grassHeightInitial;

        updateLayers();
    }

    private void initalise()
    {
        //ugly variable setting hidden away in a function

        seed = WorldController.Instance.World.TerrainGenerator.Seed;
        scale = WorldController.Instance.World.TerrainGenerator.Scale;
        octaves = WorldController.Instance.World.TerrainGenerator.Octaves;
        persistance = WorldController.Instance.World.TerrainGenerator.Persistance;
        lacunarity = WorldController.Instance.World.TerrainGenerator.Lacunarity;
        offset = WorldController.Instance.World.TerrainGenerator.Offset;

        seedInitial = WorldController.Instance.World.TerrainGenerator.Seed;
        scaleInitial = WorldController.Instance.World.TerrainGenerator.Scale;
        octavesInitial = WorldController.Instance.World.TerrainGenerator.Octaves;
        persistanceInitial = WorldController.Instance.World.TerrainGenerator.Persistance;
        lacunarityInitial = WorldController.Instance.World.TerrainGenerator.Lacunarity;
        offsetInitial = WorldController.Instance.World.TerrainGenerator.Offset;

        waterHeight = WorldController.Instance.World.TerrainGenerator.WaterHeight;
        sandHeight = WorldController.Instance.World.TerrainGenerator.SandHeight;
        grassHeight = WorldController.Instance.World.TerrainGenerator.GrassHeight;

        waterHeightInitial = WorldController.Instance.World.TerrainGenerator.WaterHeight;
        sandHeightInitial = WorldController.Instance.World.TerrainGenerator.SandHeight;
        grassHeightInitial = WorldController.Instance.World.TerrainGenerator.GrassHeight;
    }
}
