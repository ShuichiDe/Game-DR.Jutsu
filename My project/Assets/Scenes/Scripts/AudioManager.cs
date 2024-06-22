using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public AudioClip audio1;
    public AudioClip audio2;
    public AudioSource audioSource;

    void Awake()
    {
        // Đảm bảo chỉ có một AudioManager tồn tại trong tất cả các scene
        DontDestroyOnLoad(gameObject);
        audioSource = GetComponent<AudioSource>();
    }


    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlaySceneAudio(scene.name);
    }

    void PlaySceneAudio(string sceneName)
    {
        switch (sceneName)
        {
            case "SartGameScene":
                audioSource.clip = audio1;
                break;
            case "SampleScene":
                audioSource.clip = audio2;
                break;
        }

        audioSource.Play();
    }

}
