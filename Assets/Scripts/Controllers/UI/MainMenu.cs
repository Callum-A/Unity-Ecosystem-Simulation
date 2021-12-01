using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Text PreyCount;
    public Text PreyBreedingRate;
    public Text PredatorCount;
    public Text PredatorBreedingRate;

    public Dropdown WorldTypeDropdown;
    public InputField InputSeed;
    public Slider WaterSlider;
    public Slider AriditySlider;
    public Slider SpreadSlider;
    public Slider PreyBreedingRateSlider;
    public Slider PredatorBreedingRateSlider;
    public Text WaterLevel;
    public Text AridityLevel;
    public Text SpreadRate;

    public void PlayGame() 
    {
        SceneManager.LoadScene("SampleScene"); 
    }

    public void OnValuePreyChanged(float value) 
    {
        WorldController.PreyCount = (int)value;
        PreyCount.text = value.ToString();
    }

    public void OnPreyBreedingValuechange() 
    {
        PreyBreedingRate.text = PreyBreedingRateSlider.value.ToString("0.00") + "x";
        WorldController.PreyBreedingRate = 2 - PreyBreedingRateSlider.value;
    }

    public void OnPredatorBreedingValuechange()
    {
        PredatorBreedingRate.text = PredatorBreedingRateSlider.value.ToString("0.00") + "x";
        WorldController.PredatorBreedingRate = 2 - PredatorBreedingRateSlider.value;
    }

    public void OnValuePredatorChanged(float value) 
    {
        WorldController.PredatorCount = (int)value;
        PredatorCount.text = value.ToString();
    }

    public void OnValueSeedChanged()
    {
        if (InputSeed.text.Length > 0)
        {
            WorldController.WorldSeed = int.Parse(InputSeed.text);
        }
    }

    public void OnWaterLevelChange(float value) 
    {
        WaterLevel.text = WaterSlider.value.ToString() + "%";
        WorldController.WaterLevel = WaterSlider.value/100; 
    }

    public void OnSpreadRateChange(float value) 
    {
        SpreadRate.text = SpreadSlider.value.ToString() + "%";
        WorldController.SpreadRate = SpreadSlider.value;
    }

    public void OnAridityLevelChange(float value)
    {
        AridityLevel.text = AriditySlider.value.ToString() + "%";
        WorldController.AridityLevel = AriditySlider.value/100;
    }

    public void OnIslandTypeChanged() 
    {
        WorldController.WorldType = WorldTypeDropdown.value;
    }

    public void getRandomSeed() 
    {
        int length = UnityEngine.Random.Range(4,10);
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < length; i++)
        {
            sb.Append(UnityEngine.Random.Range(0, 9));
        }

        InputSeed.text = sb.ToString();
        OnValueSeedChanged();
    }
}
