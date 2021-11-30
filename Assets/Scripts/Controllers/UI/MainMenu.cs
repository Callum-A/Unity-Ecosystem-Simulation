using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Text PreyCount;
    public Text PredatorCount;

    public Dropdown WorldTypeDropdown;
    public InputField InputSeed;
    public Slider WaterSlider;
    public Slider AriditySlider;
    public Slider SpreadSlider;
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
        int length = Random.Range(4,12);
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < length; i++)
        {
            sb.Append(Random.Range(0, 9));
        }

        InputSeed.text = sb.ToString();
        OnValueSeedChanged();
    }
}
