using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpriteController : SpriteController<Animal>
{
    private World world => WorldController.Instance.World;
    public Sprite PreySprite;
    public Sprite PredatorSprite;

    public AnimalStateSpriteController stateSprite;

    private void Start()
    {
        LoadSprites("Images/Animals");
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
        objGameObject.name = a.AnimalType == AnimalType.Prey ? "Prey_" + a.ID : "Predator_" + + a.ID;
        objGameObject.transform.position = new Vector3(a.X, a.Y, 0);
        objGameObject.transform.SetParent(transform, true);
        SpriteRenderer sr = objGameObject.AddComponent<SpriteRenderer>();
        sr.sortingLayerName = "Animals";
        sr.sprite = a.AnimalType == AnimalType.Prey ? PreySprite : PredatorSprite;

        stateSprite.OnAnimalCreated(objGameObject);

        a.RegisterOnAnimalChangedCallback(OnAnimalChanged);
    }

    public void OnAnimalChanged(Animal a)
    {
        GameObject animalGameObject = GetGameObjectByInstance(a);
        ChangeAnimalSprite(a, animalGameObject);
        animalGameObject.transform.position = new Vector3(a.X, a.Y, 0);
    }

    private void ChangeAnimalSprite(Animal a, GameObject animalGo)
    {
        SpriteRenderer sr = animalGo.GetComponent<SpriteRenderer>();
        string b = a.AnimalType == AnimalType.Prey ? "rabbit" : "fox";
        Sprite s = null;

        switch (a.lifeStage)
        {
            case LifeStage.Child:
                s = GetSpriteByName(b + "_child");
                break;
            case LifeStage.Adult:
                s = GetSpriteByName(b);
                break;
            case LifeStage.Elder:
                s = GetSpriteByName(b + "_elder");
                break;
            default:
                s = GetSpriteByName(b);
                break;
        }
        sr.sprite = s;

        stateSprite.ChangeAnimalSprite(a, animalGo);
    }
}
