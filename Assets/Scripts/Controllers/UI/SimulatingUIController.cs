using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class SimulatingUIController : MonoBehaviour
{
    public Text SimulatingText;
    private int secondsBetweenDots = 1;
    private float currentTime = 0f;
    private int numberOfDots = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime >= secondsBetweenDots)
        {
            currentTime = 0f;
            numberOfDots++;
            if (numberOfDots > 3)
            {
                numberOfDots = 0;
            }
            string dots = new String('.', numberOfDots);
            SimulatingText.text = "Simulating" + dots;
        }
    }
}
