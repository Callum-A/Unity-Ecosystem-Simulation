using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AnimalUIController : MonoBehaviour
{
    public Text NameText;

    public Text StateText;

    public Slider HungerSlider;

    public Slider ThirstSlider;

    private MouseController mouseController;

    private Animal currentlySelected;
    
    // Start is called before the first frame update
    void Start()
    {
        mouseController = FindObjectOfType<MouseController>();
        currentlySelected = null;
        if (mouseController == null || NameText == null || StateText == null)
        {
            enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                currentlySelected = mouseController.GetMouseoverAnimal();
            }
        }
        
        if (currentlySelected != null)
        {
            NameText.text = "Name: " + currentlySelected.ToString();
            HungerSlider.value = currentlySelected.Hunger;
            ThirstSlider.value = currentlySelected.Thirst;
            StateText.text = $"Current State: {currentlySelected.CurrentState}";
        }
        else
        {
            NameText.text = "Name: N/A";
            StateText.text = "Current State: N/A";
        }
    }

    public void SetCurrentlySelectedToHungry()
    {
        if (currentlySelected != null)
        {
            currentlySelected.Hunger = 0.31f;
        }
    }

    public void SetCurrentlySelectedToThirsty()
    {
        if (currentlySelected != null)
        {
            currentlySelected.Thirst = 0.31f;
        }
    }
}
