using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class TriviaManager : MonoBehaviour
{
    private const string triviaApiUrl = "https://opentdb.com/api.php?amount=50&difficulty=easy&type=boolean";
    private List<Question> triviaQuestions = new List<Question>();
    private const int MaxRetryAttempts = 2;
    private const float RetryDelaySeconds = 3f;

    // Cached questions
    private static List<Question> cachedTriviaQuestions = new List<Question>();

    public List<Question> TriviaQuestions
    {
        get { return triviaQuestions; }
    }

    private static TriviaManager _instance;

    public static TriviaManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<TriviaManager>();

                if (_instance == null)
                {
                    Debug.LogError("TriviaManager is not found in the scene.");
                }
            }
            return _instance;
        }
    }

    [System.Serializable]
    public class Question
    {
        public string questionText;
        public bool isTrue;
    }

    void Start()
    {
        // Check the cache before making an API call
        if (cachedTriviaQuestions.Count > 0)
        {
            triviaQuestions = new List<Question>(cachedTriviaQuestions);
            Debug.Log("Loaded questions from cache.");
        }
        else
        {
            StartCoroutine(GetTriviaQuestions());
        }
    }

    public IEnumerator GetTriviaQuestions()
    {
        int retryCount = 0;

        while (retryCount < MaxRetryAttempts)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(triviaApiUrl))
            {
                yield return webRequest.SendWebRequest();

                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Failed to get trivia questions: " + webRequest.error);

                    // Wait before retrying
                    yield return new WaitForSeconds(RetryDelaySeconds);
                    retryCount++;
                }
                else
                {
                    string json = webRequest.downloadHandler.text;
                    ProcessTriviaJson(json);
                    yield break; // Exit coroutine on success
                }
            }
        }

        Debug.LogError("Max retry attempts reached. Unable to fetch trivia questions.");
    }

    void ProcessTriviaJson(string json)
    {
        try
        {
            TriviaResponse response = JsonUtility.FromJson<TriviaResponse>(json);

            if (response == null || response.results == null || response.results.Count == 0)
            {
                Debug.LogError("No questions received from API.");
                return;
            }

            triviaQuestions.Clear(); // Clear previous questions if any
            cachedTriviaQuestions.Clear(); // Clear the cache

            foreach (var result in response.results)
            {
                Question newQuestion = new Question();
                newQuestion.questionText = result.question;
                newQuestion.isTrue = (result.correct_answer.ToLower() == "true");

                triviaQuestions.Add(newQuestion);
                cachedTriviaQuestions.Add(newQuestion); // Cache the question
            }

            Debug.Log("Successfully loaded " + triviaQuestions.Count + " trivia questions.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error processing trivia JSON: " + e.Message);
        }
    }

    [System.Serializable]
    public class TriviaResponse
    {
        public List<TriviaQuestion> results;
    }

    [System.Serializable]
    public class TriviaQuestion
    {
        public string question;
        public string correct_answer;
    }
}
