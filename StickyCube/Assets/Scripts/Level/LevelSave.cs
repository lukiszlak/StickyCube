using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using SimpleJSON;

public class LevelSave : MonoBehaviour {

    //ToDelete
    public int howLong;

    public int floorContainerChildCount;
    public GameObject FloorContainer;
    //public PlayerData[] playerData; // TODO check if it does anything
    JSONArray levelData;

    private float MessageStartingTime = -5.0f;
    private string debugMessage;


    //[System.Serializable]
    //public class PlayerData
    //{
    //    public string name;
    //    public int x;
    //    public int y;
    //}

	void Awake ()
    {
        FloorContainer = GameObject.Find("FloorContainer");
        levelData = new JSONArray();

    }

    private void Start()
    {
        
    }

    private void Update()
    {
        floorContainerChildCount = FloorContainer.transform.childCount;
        if (Input.GetKeyDown(KeyCode.K))
        {
            Save();
            MessageStartingTime = Time.time;
            debugMessage = "Saved";
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Load();
            print("Loaded");
            MessageStartingTime = Time.time;
            debugMessage = "Loaded";
        }
    }

    private void OnGUI()
    {
        // TODO maybe move it to Menu controller? 
        if ((MessageStartingTime + 2) > Time.time)
        {
            GUI.Label(new Rect(Screen.width / 2, Screen.height / 4, 200f, 200f), debugMessage);
        }
    }

    public void Load()
    {
        string jsonString = File.ReadAllText("Assets/Levels/SavedLevels.json");
        print(jsonString);
        levelData = (JSONArray)JSON.Parse(jsonString);
        howLong =  levelData.Count;
        for (int i = 0; i < howLong; i++)
        {
            int x = levelData.AsArray[i].AsObject["x"];
            int y = levelData.AsArray[i].AsObject["y"];
            gameObject.transform.GetComponentInParent<LevelGenerator>().GenerateTheCube(x,y);
        }
    }

    void Save()
    {
        // G:/!!!Unity/StickyCube/StickyCube/
        if (File.ReadAllText("Assets/Levels/SavedLevels.json") != null)
        {
            for (int i = 0; i < FloorContainer.transform.childCount; i++)
            {
                JSONObject cubeNames = new JSONObject();

                //cubeNames.Add("name", FloorContainer.transform.GetChild(i).name);
                cubeNames.Add("x", FloorContainer.transform.GetChild(i).GetComponent<FloorValuesHolder>().x);
                cubeNames.Add("y", FloorContainer.transform.GetChild(i).GetComponent<FloorValuesHolder>().y);
                levelData.Add(cubeNames);
            }

            print("This is JSON " + levelData.ToString());
            File.WriteAllText("Assets/Levels/SavedLevels.json", levelData.ToString());
        }
    }
}