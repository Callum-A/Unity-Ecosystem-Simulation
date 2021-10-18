using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpriteController : SpriteController<Animal>
{
    private World world => WorldController.Instance.World;
    public Sprite PreySprite;
    public Sprite PredatorSprite;

    // Start is called before the first frame update
    void Start()
    {
        world.AnimalManager.RegisterOnAnimalCreatedCallback(OnAnimalCreated);
        world.AnimalManager.RegisterOnAnimalDestroyedCallback(OnAnimalDestroyed);
    }

    public void OnAnimalDestroyed(Animal a)
    {
        GameObject go = GetGameObjectByInstance(a);
        Destroy(go);
    }

    public void OnAnimalCreated(Animal a)
    {
        // Create a game object linked to this data
        GameObject objGameObject = new GameObject();
        AddGameObject(a, objGameObject);
        objGameObject.name = a.AnimalType == AnimalType.Prey ? "Prey_" + a.ID : "Predator_" + +a.ID; // TODO: make this determined by character class
        objGameObject.transform.position = new Vector3(a.X, a.Y, 0);
        objGameObject.transform.SetParent(transform, true);
        SpriteRenderer sr = objGameObject.AddComponent<SpriteRenderer>();
        sr.sortingLayerName = "Animals";
        sr.sprite = a.AnimalType == AnimalType.Prey ? PreySprite : PredatorSprite;
        a.RegisterOnAnimalChangedCallback(OnAnimalChanged);
    }

    public void OnAnimalChanged(Animal a)
    {
        GameObject characterGameObject = GetGameObjectByInstance(a);
        characterGameObject.transform.position = new Vector3(a.X, a.Y, 0);
    }
}
