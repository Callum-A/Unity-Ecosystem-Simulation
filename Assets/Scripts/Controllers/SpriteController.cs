using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpriteController<T> : MonoBehaviour
{
    private Dictionary<string, Sprite> spriteMap;
    private Dictionary<T, GameObject> gameObjectMap;

    /// <summary>
    /// Constructor setting up the sprite map and game object map
    /// </summary>
    protected SpriteController()
    {
        spriteMap = new Dictionary<string, Sprite>();
        gameObjectMap = new Dictionary<T, GameObject>();
    }
    
    /// <summary>
    /// Method called to load all sprites under the given path.
    /// </summary>
    /// <param name="path">Path to the sprite locations</param>
    protected void LoadSprites(string path)
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>(path);
        foreach (Sprite sprite in sprites)
        {
            spriteMap[sprite.name] = sprite;
        }
    }
    /// <summary>
    /// Method to get a sprite by its string name
    /// </summary>
    /// <param name="name">Name of the sprite</param>
    /// <returns>The sprite with the name or null if not found</returns>
    protected Sprite GetSpriteByName(string name)
    {
        if (!spriteMap.ContainsKey(name))
        {
            return null;
        }

        return spriteMap[name];
    }
    /// <summary>
    /// Method called to check the existence of a sprite with the specified name
    /// </summary>
    /// <param name="name">The name of the sprite</param>
    /// <returns>True if a sprite with that name exists false otherwise</returns>
    protected bool ContainsSpriteWithName(string name)
    {
        return spriteMap.ContainsKey(name);
    }

    /// <summary>
    /// Method to check the existence of a game object with the specified instance
    /// </summary>
    /// <param name="instance">The instance used as the key</param>
    /// <returns>True if a game object is present, false otherwise</returns>
    protected bool ContainsGameObjectWithInstance(T instance)
    {
        return gameObjectMap.ContainsKey(instance);
    }
    
    /// <summary>
    /// Method to get a gameobject by instance.
    /// </summary>
    /// <param name="instance">The instance we are trying to get the game object for</param>
    /// <returns>The gameobject for that instance or null if it does not exist</returns>
   protected GameObject GetGameObjectByInstance(T instance)
    {
        if (!gameObjectMap.ContainsKey(instance))
        {
            return null;
        }

        return gameObjectMap[instance];
    }

    /// <summary>
    /// Method called to add a game object to the map
    /// </summary>
    /// <param name="instance">The instance the game object is for</param>
    /// <param name="go">The game object</param>
    protected void AddGameObject(T instance, GameObject go)
    {
        gameObjectMap.Add(instance, go);
    }
}
