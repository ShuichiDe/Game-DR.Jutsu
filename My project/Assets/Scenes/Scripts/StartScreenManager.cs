using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartScreenManager : MonoBehaviour
{
    public static StartScreenManager Instance { get; private set; }

    public GameObject startPanel;
    public Button startButton;
    public TMP_Text welcomeText;
    public Button volumeButton; // Thêm nút âm lượng
    public AudioSource audioSource; // Thêm AudioSource để quản lý âm thanh
    public GameObject difficultyPanel; // Thêm biến công khai cho difficultyPanel
    public Button easyButton;
    public Button mediumButton;
    public Button hardButton;

    private bool isMuted = false; // Biến lưu trạng thái âm thanh

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
    }

    // Start is called before the first frame update
    void Start()
    {
        if (startPanel == null || startButton == null || welcomeText == null || difficultyPanel == null || easyButton == null || mediumButton == null || hardButton == null)
        {
            Debug.LogError("Some UI components are not assigned in StartScreenManager.");
            return;
        }
        ShowStartScreen();
        startButton.onClick.AddListener(OnStartButtonClicked);
        volumeButton.onClick.AddListener(OnVolumeButtonClicked);
        easyButton.onClick.AddListener(() => OnDifficultySelected("easy"));
        mediumButton.onClick.AddListener(() => OnDifficultySelected("medium"));
        hardButton.onClick.AddListener(() => OnDifficultySelected("hard"));
    }

    private void OnStartButtonClicked()
    {
        startButton.gameObject.SetActive(false); // Ẩn panel bắt đầu
        ShowDifficultySelection();   // Hiển thị panel chọn độ khó
    }

    private void OnDifficultySelected(string difficulty)
    {
        difficultyPanel.SetActive(false);
        GameManager.Instance.StartGame(difficulty);
        SceneSwitcher.Instance.SwitchScene("SampleScene");
    }

    private void ShowDifficultySelection()
    {
        difficultyPanel.SetActive(true); // Hiển thị panel chọn độ khó
    }

    public void ShowStartScreen()
    {
        startPanel.SetActive(true);
    }

    private void OnVolumeButtonClicked()
    {
        isMuted = !isMuted; // Chuyển đổi trạng thái âm thanh
        audioSource.mute = isMuted; // Tắt hoặc bật âm thanh
        UpdateVolumeButtonText(); // Cập nhật văn bản nút âm lượng nếu cần
    }

    private void UpdateVolumeButtonText()
    {
        // Cập nhật văn bản trên nút âm lượng (nếu có)
        volumeButton.GetComponentInChildren<TMP_Text>().text = isMuted ? "Unmute" : "Mute";
    }
}
