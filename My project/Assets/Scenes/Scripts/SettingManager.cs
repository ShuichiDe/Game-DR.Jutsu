using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingManager : MonoBehaviour
{
    public GameObject settingPanel;

    void Start()
    {
        settingPanel.SetActive(false);
    }

    public void OpenSettings()
    {
        Time.timeScale = 0f;
        settingPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        Time.timeScale = 1f;
        settingPanel.SetActive(false);
    }

    public void GoHome()
    {  
        SceneManager.LoadScene("StartGameScene");
    }

    public void ResetGame()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
