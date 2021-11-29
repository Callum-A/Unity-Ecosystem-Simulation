using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpriteController : SpriteController<Animal>
{
    private bool gameObjectsShown = true;
    public Sprite PreySprite;
    public Sprite PredatorSprite;

    public AnimalStateSpriteController stateSprite;
    public GameObject simulatingOverlay;

    private void Start()
    {
        LoadSprites("Images/Animals");
    }

    public void TurnOffGameObjects()
    {
        ToggleGameObjects(false);
    }

    public void TurnOnGameObjects()
    {
        ToggleGameObjects(true);
    }

    private void ToggleGameObjects(bool toggle)
    {
        if (!gameObjectsShown == toggle)
        {
            foreach (KeyValuePair<Animal, GameObject> pair in gameObjectMap)
            {
                GameObject go = pair.Value;
                SpriteRenderer[] childSrs = go.GetComponentsInChildren<SpriteRenderer>();
                foreach (SpriteRenderer childSr in childSrs)
                {
                    childSr.enabled = toggle;
                }
                SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
                sr.enabled = toggle;
                go.SetActive(toggle);
            }
            simulatingOverlay.SetActive(!toggle);
            gameObjectsShown = !gameObjectsShown;
        }
    }

    public void OnAnimalDestroyed(Animal a)
    {
        GameObject go = GetGameObjectByInstance(a);
        RemoveGameObject(a);
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
        if (!gameObjectsShown)
        {
            objGameObject.SetActive(false);
        }
        SpriteRenderer sr = objGameObject.AddComponent<SpriteRenderer>();
        sr.sortingLayerName = "Animals";
        sr.sprite = a.AnimalType == AnimalType.Prey ? PreySprite : PredatorSprite;

        stateSprite.OnAnimalCreated(objGameObject);

        a.RegisterOnAnimalChangedCallback(OnAnimalChanged);
    }

    public void OnAnimalChanged(Animal a)
    {
        GameObject animalGameObject = GetGameObjectByInstance(a);
        if (animalGameObject != null)
        {
            ChangeAnimalSprite(a, animalGameObject);
            animalGameObject.transform.position = new Vector3(a.X, a.Y, 0);
        }
    }

    private void ChangeAnimalSprite(Animal a, GameObject animalGo)
    {
        if (animalGo != null)
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
}
