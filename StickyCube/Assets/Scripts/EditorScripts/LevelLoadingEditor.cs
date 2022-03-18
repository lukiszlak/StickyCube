using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using SimpleJSON;

public class LevelLoadingEditor : MonoBehaviour
{

    public int floorContainerChildCount;
    public GameObject FloorContainer;

    JSONArray levelData;
    private int howLong;

    void Awake()
    {
        FloorContainer = GameObject.Find("FloorContainer");
        levelData = new JSONArray();

    }
    
    public void Load()
    {
        string jsonString = File.ReadAllText("Assets/Levels/SavedLevels.json");
        print(jsonString);
        levelData = (JSONArray)JSON.Parse(jsonString);
        howLong = levelData.Count;
        for (int i = 0; i < howLong; i++)
        {
            int x = levelData.AsArray[i].AsObject["x"];
            int y = levelData.AsArray[i].AsObject["y"];
            gameObject.transform.GetComponentInParent<LevelGenerator>().CreateCheckerArray(x, y);
        }
    }
}
