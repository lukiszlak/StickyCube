using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {

    private float level;
    private string levelName;
    private Scene currentScene;
    private Text levelDisplay;

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            levelDisplay = GameObject.Find("LevelName").GetComponent<Text>();
            levelDisplay.text = SceneManager.GetActiveScene().name; 
        }
    }

    public void NewGame()
    {
        SceneManager.LoadScene("1.01");
    }

    public void Continue()
    {

    }

    public void NextLevel()
    {
        //TODO fix this hax
        level = float.Parse(SceneManager.GetActiveScene().name, new CultureInfo("en-US", false));
        if (level < 1.12f || (level > 2 && level < 2.10))
        {
            level += 0.01f;
        }

        if (level == 1.12f)
        {
            level = 2.01f;
        }
        else if (level == 2.10f)
        {
            level = 1.12f;
        }

        levelName = level.ToString(new CultureInfo("en-US", false));

        if (level == 1.1f || level == 2.1f)
        {
            levelName += "0";
        }

        Time.timeScale = 1;
        SceneManager.LoadScene(levelName);
    }

    public void Restart()
    {
        currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void Options()
    {

    }

    public void Exit()
    {
        Application.Quit();
    }

    public void ExitToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    //Temp Functions to delete

    public void LoadTempLevel()
    {
        SceneManager.LoadScene("TwoPC_1");
    }

    public void ChangeToTwoPCLevel2()
    {
        SceneManager.LoadScene("TwoPC_2");
    }

}
