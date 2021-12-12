using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public static class CSVData
{
    public static List<int> PreyPopulation = new List<int>();
    public static List<int> PredatorPopulation = new List<int>();
    public static List<int> NutritionTotal = new List<int>();
    public static List<int> NumFoodTiles = new List<int>();
    public static List<int> NumWaterTiles = new List<int>();
    public static List<int> NumGrassTiles = new List<int>();
    private static List<string> EventData = new List<string>();

    private static int daysProcessed = 0;
    private static readonly string fileName = "data.csv";
    private static bool hasWrittenWorldData = false;
    private static bool hasWrittenEventData = false;

    public async static void CollectData(World world)
    {
        PreyPopulation.Add(world.AnimalManager.Prey.Count);
        PredatorPopulation.Add(world.AnimalManager.Predators.Count);
        NutritionTotal.Add(world.getTotalNutritionOnMap());
        NumFoodTiles.Add(world.getFoodTiles().Count);
        NumWaterTiles.Add(world.getWaterTiles().Count);
        NumGrassTiles.Add(world.getGrassTiles().Count);
        world.getGrassTiles();
        //WriteCSV();

        await AsyncWriteCSV();

    }

    private static async Task AsyncWriteCSV()
    {


        string path = Application.streamingAssetsPath + "/" + fileName;
        if (!hasWrittenWorldData)
        {
            // Uses a static boolean to create a file on each start
            using (StreamWriter sw = File.CreateText(path))
            {
                // Write the header
                sw.WriteLine("Prey Population,Predator Population,Total Population,Nutrition,Total Food,Water Tiles, Grass Tiles");
            }
            hasWrittenWorldData = true;
        }

        // We assume all the lists are the same length
        using (StreamWriter sw = File.AppendText(path))
        {
            sw.WriteLine($"{PreyPopulation[daysProcessed]},{PredatorPopulation[daysProcessed]}," +
                $"{PreyPopulation[daysProcessed] + PredatorPopulation[daysProcessed]},{NutritionTotal[daysProcessed]}," +
                $"{NumFoodTiles[daysProcessed]},{NumWaterTiles[daysProcessed]}, {NumGrassTiles[daysProcessed]}");
        }
        daysProcessed++;

        return;
    }

    public static async Task WriteEventData(string eventData) 
    {
        string path = Application.streamingAssetsPath + "/" + "EventData.csv";

        if (!hasWrittenEventData)
        {
            // Uses a static boolean to create a file on each start
            using (StreamWriter sw = File.CreateText(path))
            {
                hasWrittenEventData = true;
            }
        }

        using (StreamWriter sw = File.AppendText(path))
        {
            sw.WriteLine("Day " + daysProcessed + ": " + eventData);
        }
    }

    public static void ClearData()
    {
        string path = Application.streamingAssetsPath + "/" + fileName;

        if (File.Exists(path))
        {
            File.Delete(path);
        }

        if (File.Exists(Application.streamingAssetsPath + "/" + "EventData.csv")) 
        {
            File.Delete(Application.streamingAssetsPath + "/" + "EventData.csv");
        }


        PreyPopulation.Clear();
        PredatorPopulation.Clear();
        NutritionTotal.Clear();
        NumFoodTiles.Clear();
        NumWaterTiles.Clear();
        NumGrassTiles.Clear();
        daysProcessed = 0;
        EventData.Clear();

        hasWrittenWorldData = false;
        hasWrittenEventData = false;
    }
}
