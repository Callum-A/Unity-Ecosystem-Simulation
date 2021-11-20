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
        //SpriteRenderer sr = animalGo.GetComponent<SpriteRenderer>();
        //string b = a.AnimalType == AnimalType.Prey ? "rabbit" : "fox";
        //Sprite s = null;
        //switch (a.CurrentState)
        //{
        //    case AnimalState.Idle:
        //    case AnimalState.Wandering:
        //        s = GetSpriteByName(b);
        //        break;
        //    case AnimalState.SeekFood:
        //    case AnimalState.SeekWater:
        //        s = GetSpriteByName(b + "_search");
        //        break;
        //    case AnimalState.FoundFood:
        //    case AnimalState.Hungry:
        //    case AnimalState.Eating:
        //        s = GetSpriteByName(b + "_food");
        //        break;
        //    case AnimalState.FoundWater:
        //    case AnimalState.Thirsty:
        //    case AnimalState.Drinking:
        //        s = GetSpriteByName(b + "_water");
        //        break;
        //}
        //sr.sprite = s;

        stateSprite.ChangeAnimalSprite(a, animalGo);
    }
}
