using System;
using UnityEngine;

public static class Persistence
{
    public static Data data = new();

    public static void Initialize()
    {
        Load();
        ApplyDisplay();
    }

    public static void Load()
    {
        data = ES3.Load("myData", data);
    }

    public static void Save()
    {
        ES3.Save("myData", data);
    }

    public static void ApplyDisplay()
    {
        if (data.display.width == -1) data.display.width = Screen.currentResolution.width;
        if (data.display.height == -1) data.display.height = Screen.currentResolution.height;
        
        Screen.SetResolution(data.display.width, data.display.height, FullScreenMode.ExclusiveFullScreen);
    }
}

[Serializable]
public class Data
{
    public Display display = new();
}

[Serializable]
public class Display
{
    public int width = -1;
    public int height = -1;
}