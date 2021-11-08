using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpriteController : SpriteController<Animal>
{
    private World world => WorldController.Instance.World;
    public Sprite PreySprite;
    public Sprite PreySearchSprite;
    public Sprite PreyDrinkSprite;
    public Sprite PreyEatSprite;
    public Sprite PredatorSprite;
    public Sprite PredatorSearchSprite;
    public Sprite PredatorDrinkSprite;
    public Sprite PredatorEatSprite;

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
        if (a.AnimalType == AnimalType.Prey)
        {
            switch (a.CurrentState)
            {
                case AnimalState.Idle:
                case AnimalState.Wandering:
                    sr.sprite = PreySprite;
                    break;
                case AnimalState.SeekFood:
                case AnimalState.SeekWater:
                    sr.sprite = PreySearchSprite;
                    break;
                case AnimalState.FoundFood:
                case AnimalState.Hungry:
                    sr.sprite = PreyEatSprite;
                    break;
                case AnimalState.FoundWater:
                case AnimalState.Thirsty:
                    sr.sprite = PreyDrinkSprite;
                    break;
            }
        }
        else
        {
            switch (a.CurrentState)
            {
                case AnimalState.Idle:
                case AnimalState.Wandering:
                    sr.sprite = PredatorSprite;
                    break;
                case AnimalState.SeekFood:
                case AnimalState.SeekWater:
                    sr.sprite = PredatorSearchSprite;
                    break;
                case AnimalState.FoundFood:
                case AnimalState.Hungry:
                    sr.sprite = PredatorEatSprite;
                    break;
                case AnimalState.FoundWater:
                case AnimalState.Thirsty:
                    sr.sprite = PredatorDrinkSprite;
                    break;
            }
        }
    }
}
