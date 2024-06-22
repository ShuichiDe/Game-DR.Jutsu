using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    private static QuizManager _instance;
    private GameManager _gameManager;

    public static QuizManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<QuizManager>();

                if (_instance == null)
                {
                    Debug.LogError("QuizManager is not found in the scene.");
                }
            }
            return _instance;
        }
    }

    public GameObject questionPanel;
    public TMP_Text questionText;
    public Button answerButton1;
    public Button answerButton2;
    public TMP_Text scoreText;
    public GameObject losePanel;
    public TMP_Text loseText;
    public Button restartButton;
    public GameObject smokePrefab;
    public TMP_Text scoreTextFinal;

    private List<TriviaManager.Question> triviaQuestions = new List<TriviaManager.Question>();

    private Player playerScript;
    private Enemies enemyScript;

    private void Start()
    {
        if (questionPanel == null || questionText == null || answerButton1 == null || answerButton2 == null || scoreText == null || restartButton == null)
        {
            Debug.LogError("Some UI components are not assigned in QuizManager.");
            return;
        }

        if (losePanel != null)
            losePanel.SetActive(false);

        questionPanel.SetActive(false);
        restartButton.gameObject.SetActive(false);
        restartButton.onClick.AddListener(RestartGame);
        scoreText.gameObject.SetActive(false);
        scoreTextFinal.gameObject.SetActive(false);

        // Ensure we have the latest trivia questions
        FetchTriviaQuestions();
    }

    private void FetchTriviaQuestions()
    {
        TriviaManager triviaManager = TriviaManager.Instance as TriviaManager;

        if (triviaManager != null)
        {
            triviaQuestions = triviaManager.TriviaQuestions;
        }
        else
        {
            Debug.LogError("TriviaManager is not found or Instance is not of type TriviaManager.");
        }
    }
    public void SetTriviaQuestions(List<TriviaManager.Question> questions)
    {
        triviaQuestions = questions;
    }

    public void ShowQuestion(Player player, Enemies enemy)
    {
        if (questionPanel == null || questionText == null || answerButton1 == null || answerButton2 == null || scoreText == null)
        {
            Debug.LogError("Some UI components are not assigned in QuizManager.");
            return;
        }

        if (triviaQuestions == null || triviaQuestions.Count == 0)
        {
            Debug.LogWarning("No questions available.");
            return;
        }

        DisplayQuestion(player, enemy);
    }

    private void DisplayQuestion(Player player, Enemies enemy)
    {
        if (questionPanel == null || questionText == null || answerButton1 == null || answerButton2 == null || scoreText == null)
        {
            Debug.LogError("Some UI components are not assigned in QuizManager.");
            return;
        }

        if (triviaQuestions.Count == 0)
        {
            Debug.LogWarning("No questions available.");
            return;
        }

        playerScript = player;
        enemyScript = enemy;

        playerScript.StopMovement();
        enemyScript.StopMovement();

        TriviaManager.Question currentQuestion = triviaQuestions[0];
        triviaQuestions.RemoveAt(0);

        questionText.text = currentQuestion.questionText;

        TMP_Text answerText1 = answerButton1.GetComponentInChildren<TMP_Text>();
        TMP_Text answerText2 = answerButton2.GetComponentInChildren<TMP_Text>();

        if (answerText1 == null || answerText2 == null)
        {
            Debug.LogError("Answer buttons do not have TMP_Text components.");
            return;
        }

        answerText1.text = "True";
        answerText2.text = "False";

        answerButton1.onClick.RemoveAllListeners();
        answerButton2.onClick.RemoveAllListeners();

        answerButton1.onClick.AddListener(() => OnAnswerSelected(true, currentQuestion.isTrue));
        answerButton2.onClick.AddListener(() => OnAnswerSelected(false, currentQuestion.isTrue));

        questionPanel.SetActive(true);
    }

    private void OnAnswerSelected(bool selectedAnswer, bool correctAnswer)
    {
        _gameManager = GameManager.Instance;
        Debug.Log("Answer selected: " + selectedAnswer);

        if (selectedAnswer == correctAnswer)
        {
            Debug.Log("Correct answer!");
            _gameManager.AddScore(1);
            HideEnemy();
            playerScript.TriggerAttackAnimation();
            scoreText.gameObject.SetActive(true);
            questionPanel.SetActive(false);
            scoreText.text = "Score: " + _gameManager.score;
            scoreText.gameObject.SetActive(true);
        }
        else
        {
            scoreTextFinal.text = "Total: " + _gameManager.score;
            Debug.Log("Wrong answer!");
            questionPanel.SetActive(false);
            losePanel.SetActive(true);
            loseText.gameObject.SetActive(true);
            loseText.text = "Game Over!";
            restartButton.gameObject.SetActive(true);
            scoreTextFinal.gameObject.SetActive(true);
            _gameManager.score = 0;
            _gameManager.PauseGame();
            
        }

        playerScript.ResumeMovement();
        enemyScript.ResumeMovement();
        _gameManager.ResumeGame();

        if (triviaQuestions.Count == 0)
        {
            Debug.Log("No questions left, fetching more questions.");
        }
        else
        {
            Debug.Log($"Questions remaining: {triviaQuestions.Count}");
        }
    }

    private void RestartGame()
    {
        //_gameManager.re
        triviaQuestions.Clear();
        _gameManager.RestartGame();
    }

    private void HideEnemy()
    {
        if (enemyScript != null)
        {
            Instantiate(smokePrefab, enemyScript.transform.position, Quaternion.identity);
            enemyScript.gameObject.SetActive(false);
        }
    }
}
