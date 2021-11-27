using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AnimalEditorWindow : EditorWindow
{
    private World world => WorldController.Instance.World;
    private AnimalUIController auc;
    
    [MenuItem("Window/Animal Editor")]
    public static void init()
    {
        AnimalEditorWindow window = (AnimalEditorWindow)EditorWindow.GetWindow(typeof(AnimalEditorWindow));
        window.Show();
        window.auc = FindObjectOfType<AnimalUIController>();
    }

    private void OnGUI()
    {
        // GUILayout.Label("Test", EditorStyles.boldLabel);
        GUILayout.Space(5f);
        
        if (GUILayout.Button("Spawn Prey"))
        {
            world.AnimalManager.SpawnPrey(world.GetTileAt(50, 50));
        }
        
        if (GUILayout.Button("Spawn Predator"))
        {
            world.AnimalManager.SpawnPredator(world.GetTileAt(50, 50), null);
        }
        
        if (GUILayout.Button("Set Currently Selected To Hungry"))
        {
            if (auc.currentlySelected != null)
            {
                auc.currentlySelected.Hunger = 0.31f;
            }
        }
        
        if (GUILayout.Button("Set Currently Selected To Thirsty"))
        {
            if (auc.currentlySelected != null)
            {
                auc.currentlySelected.Thirst = 0.31f;
            }
        }
        
        if (GUILayout.Button("Make Currently Selected Needs Met"))
        {
            if (auc.currentlySelected != null)
            {
                auc.currentlySelected.Hunger = 1f;
                auc.currentlySelected.Thirst = 1f;
            }
        }
    }
}
