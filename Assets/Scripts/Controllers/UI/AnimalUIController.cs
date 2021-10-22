using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimalUIController : MonoBehaviour
{
    public Text NameText;

    public Text HungerText;

    public Text ThirstText;

    public Text StateText;

    private MouseController mouseController;

    private Animal currentlySelected;
    
    // Start is called before the first frame update
    void Start()
    {
        mouseController = FindObjectOfType<MouseController>();
        currentlySelected = null;
        if (mouseController == null || NameText == null || HungerText == null || ThirstText == null ||
            StateText == null)
        {
            enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            currentlySelected = mouseController.GetMouseoverAnimal();
        }
        
        if (currentlySelected != null)
        {
            NameText.text = "Name: " + currentlySelected.ToString();
            HungerText.text = $"Hunger: {currentlySelected.Hunger}";
            ThirstText.text = $"Thirst: {currentlySelected.Thirst}";
            StateText.text = $"Current State: {currentlySelected.CurrentState}";
        }
        else
        {
            NameText.text = "Name: N/A";
            HungerText.text = "Hunger: N/A";
            ThirstText.text = "Thirst: N/A";
            StateText.text = "Current State: N/A";
        }
    }
}
