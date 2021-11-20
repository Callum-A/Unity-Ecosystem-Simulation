using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalStateSpriteController : SpriteController<Animal>
{
    public Sprite FoodState;
    public Sprite WaterState;
    public Sprite SearchState;
    public Sprite BreedingState;

    public void OnAnimalCreated(GameObject go)
    {
        GameObject state = new GameObject();
        state.name = "State_Overlay";
        state.transform.position = new Vector3(go.transform.position.x, go.transform.position.y, -1); // z = -1 so that the sprites remain on the animal layer but still render infront.
        state.transform.SetParent(go.transform, true);

        SpriteRenderer sr = state.AddComponent<SpriteRenderer>();
        sr.sortingLayerName = "Animals";
        sr.sprite = null;
    }

    public void ChangeAnimalSprite(Animal a, GameObject animalGo)
    {
        SpriteRenderer sr = animalGo.transform.GetChild(0).GetComponent<SpriteRenderer>();
        //string b = a.AnimalType == AnimalType.Prey ? "rabbit" : "fox";
        Sprite s = null;
        switch (a.CurrentState)
        {
            case AnimalState.Idle:
            case AnimalState.Wandering:
                s = null;
                break;
            case AnimalState.SeekFood:
            case AnimalState.SeekWater:
                s = SearchState;
                break;
            case AnimalState.FoundFood:
            case AnimalState.Hungry:
            case AnimalState.Eating:
                s = FoodState;
                break;
            case AnimalState.FoundWater:
            case AnimalState.Thirsty:
            case AnimalState.Drinking:
                s = WaterState;
                break;
            case AnimalState.Breeding:
                s = BreedingState;
                break;
            s = null;
        }
        sr.sprite = s;
    }


}
