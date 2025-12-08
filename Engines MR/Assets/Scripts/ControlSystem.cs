using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ControlSystem : MonoBehaviour
{
    [Header("Yig‘iladigan qismlar")]
    public List<DraggablePart> parts; // Barcha qismlar ro'yxati

    [Header("UI elementlari")]
    public Text taskTitleText;
    public Text attemptsText;
    public Slider progressSlider;
    public GameObject levelCompletePanel;
    public Text levelCompleteMessage;
    public AIAnalysisManager aiAnalysis;

    [Header("Ovozlar")]
    public AudioSource correctSound;
    public AudioSource wrongSound;

    private int totalParts;
    private int placedParts = 0;
    private int totalAttempts = 0;

    void Start()
    {
        totalParts = parts.Count;
        levelCompletePanel.SetActive(false);
        UpdateUI();
    }

    void Update()
    {
        // Qayta hisoblab turadi (modulli dizayn uchun foydali)
        CountPlacedParts();
        UpdateUI();

        // Barcha qismlar to‘g‘ri joylashtirilsa, daraja yakunlanadi
        if (placedParts == totalParts)
        {
            OnLevelComplete();
        }
    }

    void CountPlacedParts()
    {
        placedParts = 0;
        totalAttempts = 0;

        foreach (DraggablePart part in parts)
        {
            if (part.isPlaced)
                placedParts++;
            totalAttempts += part.attempts;
        }
    }

    void UpdateUI()
    {
        progressSlider.value = (float)placedParts / totalParts;
        attemptsText.text = "Xatoliklar soni: " + totalAttempts;
    }

    public void PlayCorrectSound()
    {
        if (correctSound != null) correctSound.Play();
    }

    public void PlayWrongSound()
    {
        if (wrongSound != null) wrongSound.Play();
    }

    void OnLevelComplete()
    {
    levelCompletePanel.SetActive(true);
    levelCompleteMessage.text = "Bosqich yakunlandi!\nXatoliklar: " + totalAttempts;

    // AI orqali natijani tahlil qilish
    aiAnalysis.totalParts = totalParts;
    aiAnalysis.placedParts = placedParts;
    aiAnalysis.totalAttempts = totalAttempts;
    aiAnalysis.totalTimeSpent = Time.timeSinceLevelLoad;

    aiAnalysis.Analyze();
    }

    void OnLevelComplete()
    {
        levelCompletePanel.SetActive(true);
        levelCompleteMessage.text = "Bosqich yakunlandi!\nXatoliklar: " + totalAttempts;
    }

    // Qayta boshlash tugmasi
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Keyingi bosqichga o‘tish (agar mavjud bo‘lsa)
    public void LoadNextLevel()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(nextSceneIndex);
    }
}
