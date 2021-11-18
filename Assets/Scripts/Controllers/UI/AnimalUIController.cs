using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AnimalUIController : MonoBehaviour
{
    public Text NameText;

    public Text StateText;

    public Text AgeText;

    public Text AgeStageText;

    public Text GenderText;

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
            AgeText.text = $"Age: {currentlySelected.Age}";
            AgeStageText.text = $"Age Stage: {currentlySelected.lifeStage}";
            GenderText.text = $"Gender: {currentlySelected.AnimalSex}";
        }
        else
        {
            NameText.text = "Name: N/A";
            StateText.text = "Current State: N/A";
            AgeText.text = "Age: N/A";
            AgeStageText.text = "Age Stage: N/A";
            GenderText.text = "Gender: N/A";
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

    public void CameraLockToSelectedAnimal()
    {
        if (currentlySelected != null)
        {
            mouseController.LockToAnimal(currentlySelected);
        }
    }
}
