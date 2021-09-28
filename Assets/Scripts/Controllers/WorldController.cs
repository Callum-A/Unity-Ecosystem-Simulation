using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour
{
    public int Width;
    public int Height;
    public static WorldController Instance { get; protected set; }
    public World World { get; protected set; }
    private void OnEnable()
    {
        if (Instance != null)
        {
            Debug.LogError("This shouldnt be reachable");
        }
        Instance = this;
        World = new World(Width, Height);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
