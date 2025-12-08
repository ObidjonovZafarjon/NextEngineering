using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneMangment : MonoBehaviour
{
    public int CurrentLevel; // Bu endi faqat Inspector uchun, agar kerak bo'lsa
    public Text LevelText;
    private static bool Loded = false;

    private void Awake()
    {
        if (!Loded)
        {
            Loded = true;
            int SceneSaved = PlayerPrefs.GetInt("LastLevel", 1); // Dastlabki ochilishda 1-scene
            SceneManager.LoadScene(SceneSaved);
        }

        LevelText.text = "BOSQICH " + SceneManager.GetActiveScene().buildIndex;
    }

    public void NextSceneLoad()
    {
        StartCoroutine(waiting(0.25f));
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    public void BackButton()
    {
        SceneManager.LoadScene("MainScene");
    }

    IEnumerator waiting(float wait)
    {
        yield return new WaitForSeconds(wait);
        int nextScene = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextScene >= SceneManager.sceneCountInBuildSettings)
        {
            nextScene = 1; // Oxirgi sahna bo'lsa, 1-scenega qaytadi
        }
        SceneManager.LoadScene(nextScene);
        PlayerPrefs.SetInt("LastLevel", nextScene);
    }
}