using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private QuizManager quizManager;
    private TriviaManager triviaManager;

    private bool isGamePaused;
    public int score;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "SampleScene")
        {
            quizManager = FindObjectOfType<QuizManager>();
            triviaManager = FindObjectOfType<TriviaManager>();
            triviaManager = TriviaManager.Instance;

            if (quizManager == null)
            {
                Debug.LogError("QuizManager not found in SampleScene!");
            }

            if (triviaManager == null)
            {
                Debug.LogError("TriviaManager not found in SampleScene!");
            }

            ResumeGame();
        }
    }

    public void StartGame(string difficulty)
    {
        Debug.Log("Game started with difficulty: " + difficulty);

        if (TriviaManager.Instance == null)
        {
            Debug.LogError("TriviaManager.Instance is not initialized!");
            return;
        }

        TriviaManager.Instance.SetDifficulty(difficulty);

        // Make sure SceneSwitcher.Instance is properly implemented and accessible
        if (SceneSwitcher.Instance != null)
        {
            SceneSwitcher.Instance.SwitchScene("SampleScene");
        }
        else
        {
            Debug.LogError("SceneSwitcher.Instance is not initialized!");
        }
    }
    /*
    private IEnumerator LoadTriviaQuestionsAndStart()
    {
        if (triviaManager == null)
        {
            Debug.LogError("TriviaManager is not assigned in GameManager or found in the scene!");
            yield break;
        }

        // Check if trivia questions are already cached
        if (triviaManager.TriviaQuestions.Count == 0)
        {
            yield return StartCoroutine(triviaManager.GetTriviaQuestions());
        }

        if (quizManager != null)
        {
            quizManager.SetTriviaQuestions(triviaManager.TriviaQuestions);
            SceneManager.LoadScene("SampleScene");
        }
        else
        {
            Debug.LogError("QuizManager is not assigned in GameManager!");
        }
    }
    */

    public void Question(Player player, Enemies enemy)
    {
        if (quizManager == null)
        {
            Debug.LogError("QuizManager is not assigned in GameManager or found in the scene!");
            return;
        }

        quizManager.ShowQuestion(player, enemy);
        PauseGame();
    }

    public void PauseGame()
    {
        Debug.Log("Pausing game...");
        Time.timeScale = 0f;
        isGamePaused = true;
    }

    public void Endgame()
    {
        Debug.Log("End game");
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Debug.Log("Resuming game...");
        Time.timeScale = 1f;
        isGamePaused = false;
    }

    public void RestartGame()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);

        // Shuffle the questions when restarting the game
        if (quizManager != null)
        {
            quizManager.ShuffleQuestions();
        }
        else
        {
            Debug.LogError("QuizManager is not initialized!");
        }
    }

    public void AddScore(int points)
    {
        score += points;
    }
    public void ResetState()
    {
        score = 0;
        Debug.Log("Game state has been reset.");
    }
}
