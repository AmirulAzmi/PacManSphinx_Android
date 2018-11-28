using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChooseScene : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SceneManager.GetActiveScene().name == "mainmenu")
                Quit();
            else MainMenu();
        }
    }

    public void PlayGame()
    {
        PlayerPrefs.SetInt("score", 0);
        PlayerPrefs.SetInt("life", 3);
        SceneManager.LoadScene("level", LoadSceneMode.Single);
    }
    public void TestCommand()
    {
        SceneManager.LoadScene("commandtest", LoadSceneMode.Single);
    }
    public void MainMenu()
    {
        SceneManager.LoadScene("mainmenu", LoadSceneMode.Single);
    }
    public void Quit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
		Application.Quit();
        #endif
    }
}
