using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuController : MonoBehaviour {

    private GameObject levelEndScreen;
    private GameObject controlButtons;
    private GameObject restartButtons;

    private void Awake()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName != "MainMenu" && sceneName != "1.12")
        {
            TextMeshProUGUI levelDisplay = GameObject.Find("LevelName").GetComponent<TextMeshProUGUI>();
            levelDisplay.text = SceneManager.GetActiveScene().name; // TODO check if It's not started every level, if yes then we can save active scene name to a variable
            levelEndScreen = GameObject.Find("LevelEnd");
            controlButtons = GameObject.Find("Controls");
            restartButtons = GameObject.Find("PauseButtons");
            levelEndScreen.SetActive(false);
        }
    }

    public void NewGame()
    {
        SceneManager.LoadScene("1.01");
    }

    public void Continue()
    {

    }

    public void LevelEnd()
    {
        if (levelEndScreen)
            levelEndScreen.SetActive(true);
        if (restartButtons)
            restartButtons.SetActive(false);
        if (controlButtons) 
            controlButtons.SetActive(false);
        Time.timeScale = 0;
    }

    public void NextLevel()
    {
        //TODO fix this hax
        float level = float.Parse(SceneManager.GetActiveScene().name, new CultureInfo("en-US", false));
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

        string levelName = level.ToString(new CultureInfo("en-US", false));

        if (level == 1.1f || level == 2.1f)
        {
            levelName += "0";
        }

        Time.timeScale = 1;
        SceneManager.LoadScene(levelName);
    }

    public void Restart()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
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
