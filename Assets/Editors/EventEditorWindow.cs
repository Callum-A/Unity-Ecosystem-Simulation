using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EventEditorWindow : EditorWindow
{
    private World world => WorldController.Instance.World;
    [MenuItem("Window/Event Editor")]
    public static void init()
    {
        EventEditorWindow window = (EventEditorWindow)EditorWindow.GetWindow(typeof(EventEditorWindow));
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Space(5f);
        if (WorldController.Instance != null)
        {
            bool eventOnGoing = world.EventManager.IsEventHappening();
            if (eventOnGoing)
            {
                if (GUILayout.Button("Next Event Day"))
                {
                    world.EventManager.OnNewDay(world);
                }
            }
            else
            {
                if (GUILayout.Button("Flood Event"))
                {
                    world.EventManager.OnNewDay(world, new FloodEvent());
                }

                if (GUILayout.Button("Drought Event"))
                {
                    world.EventManager.OnNewDay(world, new DroughtEvent());
                }

                if (GUILayout.Button("Famine Event"))
                {
                    world.EventManager.OnNewDay(world, new FamineEvent());
                }

                if (GUILayout.Button("Sprout Event"))
                {
                    world.EventManager.OnNewDay(world, new SproutEvent());
                }

                if (GUILayout.Button("Disease Event"))
                {
                    world.EventManager.OnNewDay(world, new DiseaseEvent());
                }

                if (GUILayout.Button("Migration Event"))
                {
                    world.EventManager.OnNewDay(world, new MigrationEvent());
                }
            }
        }
    }
}
