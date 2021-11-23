using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlantEditorWindow : EditorWindow
{
    private FoodManager fm => WorldController.Instance.World.FoodManager;
    
    [MenuItem("Window/Plant Editor")]
    public static void init()
    {
        PlantEditorWindow window = (PlantEditorWindow)EditorWindow.GetWindow(typeof(PlantEditorWindow));
        window.Show();
    }

    private void OnGUI()
    {
        // GUILayout.Label("Test", EditorStyles.boldLabel);
        GUILayout.Space(5f);
        
        if (GUILayout.Button("Kill All Plants"))
        {
            int index = 0;
            while (index < fm.FoodTiles.Count)
            {
                Tile tile = fm.FoodTiles[index];
                tile.DrownTile();
            }
            // world.AnimalManager.SpawnPrey(world.GetTileAt(50, 50));
        }
    }
}
