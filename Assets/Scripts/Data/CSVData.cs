using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using System.IO;

public static class CSVData
{
    public static List<int> PreyPopulation = new List<int>();
    public static List<int> PredatorPopulation = new List<int>();
    public static List<int> NutritionTotal = new List<int>();
    public static List<int> NumFoodTiles = new List<int>();
    public static List<int> NumWaterTiles = new List<int>();

    private static int daysProcessed = 0;
    private static readonly string fileName = "data.csv";
    private static bool hasWritten = false;

    public async static void CollectData(World world)
    {
        PreyPopulation.Add(world.AnimalManager.Prey.Count);
        PredatorPopulation.Add(world.AnimalManager.Predators.Count);
        NutritionTotal.Add(world.getTotalNutritionOnMap());
        NumFoodTiles.Add(world.getFoodTiles().Count);
        NumWaterTiles.Add(world.getWaterTiles().Count);
        WriteCSV();
    }

    private async static void WriteCSV()
    {
        string path = Application.streamingAssetsPath + "/" + fileName;
        if (!hasWritten)
        {
            // Uses a static boolean to create a file on each start
            using (StreamWriter sw = File.CreateText(path))
            {
                // Write the header
                sw.WriteLine("Day,PreyPop,PredatorPop,TotalPop,Nutrition,NumFoodTiles,NumWaterTiles");
            }
            hasWritten = true;
        }
        
        // We assume all the lists are the same length
        using (StreamWriter sw = File.AppendText(path))
        {
            sw.WriteLine($"{daysProcessed},{PreyPopulation[daysProcessed]},{PredatorPopulation[daysProcessed]}," +
                $"{PreyPopulation[daysProcessed] + PredatorPopulation[daysProcessed]},{NutritionTotal[daysProcessed]}," +
                $"{NumFoodTiles[daysProcessed]},{NumWaterTiles[daysProcessed]}");
        }
        daysProcessed++;
    }
}
