using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.IO;

public class QuestionEditor : MonoBehaviour
{
    [Header("UI References")]
    public Transform questionsContainer; // ScrollView ning Content obyekti
    public GameObject questionPrefab; // Har bir savol uchun prefab (InputField'lar bilan)
    public Button saveButton;
    public Button startQuizButton;

    [System.Serializable]
    public class Question
    {
        public string question;
        public string correctAnswer;
        public string[] wrongAnswers = new string[3]; // 3 ta noto'g'ri javob
    }

    private List<Question> questions = new List<Question>();
    private string jsonPath = "Assets/Resources/questions.json"; // JSON fayl joyi

    void Start()
    {
        LoadQuestions();
        DisplayQuestionsUI();
        saveButton.onClick.AddListener(SaveQuestions);
        startQuizButton.onClick.AddListener(StartQuiz);
    }

    void LoadQuestions()
    {
        string json = File.ReadAllText(jsonPath); // Faylni o'qish
        questions = JsonUtility.FromJson<QuestionWrapper>(json).questions; // Wrapper klass orqali
        if (questions == null || questions.Count == 0)
        {
            // Agar fayl bo'sh bo'lsa, default savollarni yuklash (sizning kodingizdan)
            questions = GetDefaultQuestions();
        }
    }

    void DisplayQuestionsUI()
    {
        // Eski UI'larni tozalash
        foreach (Transform child in questionsContainer)
        {
            Destroy(child.gameObject);
        }

        // Har bir savol uchun UI yaratish
        for (int i = 0; i < questions.Count; i++)
        {
            GameObject qObj = Instantiate(questionPrefab, questionsContainer);
            // Prefab'dagi InputField'larga qiymatlarni yuklash (masalan, index bo'yicha)
            // Misol: qObj.transform.Find("QuestionInput").GetComponent<TMP_InputField>().text = questions[i].question;
            // To'g'ri javob va wrongAnswers uchun ham shunday.
        }
    }

    void SaveQuestions()
    {
        // UI'dan savollarni o'qish (prefab'lardan InputField'larni topib)
        // Misol: questions[i].question = ... .text;
        // Keyin JSON ga saqlash
        QuestionWrapper wrapper = new QuestionWrapper { questions = questions };
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(jsonPath, json);
        Debug.Log("Savollar saqlandi!");
    }

    void StartQuiz()
    {
        SceneManager.LoadScene("QuizScene");
    }

    // Default savollar (sizning kodingizdan)
    List<Question> GetDefaultQuestions()
    {
        return new List<Question>
        {
            new Question { question = "Himoya gaz ostida dastaki payvandlashda asosiy himoya vositasi nima?", correctAnswer = "Gaz oqimi", wrongAnswers = new string[] { "Elektr tok", "Elektrod qoplamasi", "Suv oqimi" } },
            // ... boshqa savollarni qo'shing (jami 30+ ta)
        };
    }

    // JSON uchun wrapper (JsonUtility array'larni to'g'ri ishlatish uchun)
    [System.Serializable]
    private class QuestionWrapper
    {
        public List<Question> questions;
    }
}