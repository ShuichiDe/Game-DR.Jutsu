using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Net;

public class TriviaManager : MonoBehaviour
{
    private const string baseTriviaApiUrl = "https://opentdb.com/api.php?amount=50&difficulty={0}&type=boolean";
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
    { // Check the cache before making an API call
        if (cachedTriviaQuestions.Count > 0)
        {
            triviaQuestions = new List<Question>(cachedTriviaQuestions);
            Debug.Log("Loaded questions from cache.");
        }
        else
        {
            StartCoroutine(GetTriviaQuestions("easy")); // Default to easy if no difficulty is set
        }

        // Check if the list is empty and refetch questions if necessary
        CheckAndRefetchQuestionsIfEmpty("easy");
    }
    /*
    public void GetRandomQuestion()
    {
        if (triviaQuestions.Count > 0)
        {
            // Sinh ra một số ngẫu nhiên từ 0 đến triviaQuestions.Count - 1
            int randomIndex = UnityEngine.Random.Range(0, triviaQuestions.Count);

            // Lấy câu hỏi tại chỉ số ngẫu nhiên
            Question randomQuestion = triviaQuestions[randomIndex];

            // In câu hỏi ra để kiểm tra (có thể không cần in trong ứng dụng thực tế)
            Debug.Log("Random Question: " + randomQuestion.questionText);
        }
        else
        {
            Debug.LogWarning("Trivia questions list is empty. Cannot get random question.");
        }
    }
    */
    public IEnumerator GetTriviaQuestions(string difficulty)
    {
        string triviaApiUrl = string.Format(baseTriviaApiUrl, difficulty);
        int retryCount = 0;
        float delay = RetryDelaySeconds;

        while (retryCount < MaxRetryAttempts)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(triviaApiUrl))
            {
                yield return webRequest.SendWebRequest();

                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    if (webRequest.responseCode == 429)
                    {
                        Debug.LogWarning("Rate limit exceeded. Retrying after delay: " + delay);
                        yield return new WaitForSeconds(delay);
                        delay *= 2;
                    }
                    else
                    {
                        Debug.LogError("Failed to get trivia questions: " + webRequest.error);
                        yield return new WaitForSeconds(RetryDelaySeconds);
                    }
                    retryCount++;
                }
                else
                {
                    string json = webRequest.downloadHandler.text;
                    ProcessTriviaJson(json);
                    yield break;
                }
            }
        }
        Debug.LogError("Max retry attempts reached. Unable to fetch trivia questions.");
    }

    public void SetDifficulty(string difficulty)
    {
        StartCoroutine(GetTriviaQuestions(difficulty));

        // Check if the list is empty and refetch questions if necessary
        CheckAndRefetchQuestionsIfEmpty(difficulty);
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

            triviaQuestions.Clear();
            cachedTriviaQuestions.Clear();

            foreach (var result in response.results)
            {
                Question newQuestion = new Question();
                newQuestion.questionText = WebUtility.HtmlDecode(result.question); // Giải mã các ký tự HTML
                newQuestion.isTrue = (result.correct_answer.ToLower() == "true");

                triviaQuestions.Add(newQuestion);
                cachedTriviaQuestions.Add(newQuestion);
            }

            Debug.Log("Successfully loaded " + triviaQuestions.Count + " trivia questions.");
            CheckAndRefetchQuestionsIfEmpty("easy");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error processing trivia JSON: " + e.Message);
        }
    }

    void CheckAndRefetchQuestionsIfEmpty(string difficulty)
    {
        if (triviaQuestions.Count == 0)
        {
            Debug.LogWarning("Trivia questions list is empty. Refetching...");
            StartCoroutine(GetTriviaQuestions(difficulty));
        }
    }
    
    public void ClearTriviaQuestions()
    {
        triviaQuestions.Clear();
        cachedTriviaQuestions.Clear();
        Debug.Log("Trivia questions have been cleared.");
    }
    /*
    public void RefreshTriviaQuestions(string difficulty)
    {
        ClearTriviaQuestions();
        StartCoroutine(GetTriviaQuestions(difficulty));
    }
    */
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
