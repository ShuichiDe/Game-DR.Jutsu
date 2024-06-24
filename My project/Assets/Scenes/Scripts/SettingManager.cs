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
        ResetTriviaQuestions();
        ResetGameState();
        SceneManager.LoadScene("StartGameScene");
    }

    public void ResetGame()
    {
        QuizManager quizManager = QuizManager.Instance;
        if (quizManager != null)
        {
            quizManager.ResetScore();
            quizManager.FetchTriviaQuestions();
            quizManager.ShuffleQuestions();
        }
        else
        {
            Debug.LogError("QuizManager is not found in the scene.");
        }

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    private void ResetTriviaQuestions()
    {
        TriviaManager triviaManager = TriviaManager.Instance;
        if (triviaManager != null)
        {
            triviaManager.ClearTriviaQuestions();
        }
        else
        {
            Debug.LogError("TriviaManager is not found in the scene.");
        }
    }

    private void ResetGameState()
    {
        GameManager gameManager = GameManager.Instance;
        if (gameManager != null)
        {
            gameManager.ResetState();
        }
        else
        {
            Debug.LogError("GameManager is not found in the scene.");
        }
    }
}
