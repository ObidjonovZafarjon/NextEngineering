using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.IO;

public class QuizEditor : MonoBehaviour
{
    [Header("Yangi savol kiritish")]
    public TMP_InputField questionInput;
    public TMP_InputField correctInput;
    public TMP_InputField wrong1Input;
    public TMP_InputField wrong2Input;
    public TMP_InputField wrong3Input;
    public Button addButton;

    [Header("Scroll View")]
    public Transform contentPanel;       // ScrollView → Content
    public GameObject questionItemPrefab; // Prefab ichida: 5 ta TMP_InputField + Save/Delete tugmalari

    private List<QuizManager.Question> questionList = new List<QuizManager.Question>();
    private string jsonPath;

    // JSON uchun to'g'ri Wrapper (sizning faylda "Items" katta harf bilan!)
    [System.Serializable]
    public class QuestionListWrapper
    {
        public List<QuizManager.Question> Items = new List<QuizManager.Question>();
    }

    void Start()
    {
        jsonPath = Path.Combine(Application.dataPath, "Resources/questions.json");
        LoadQuestions();
        addButton.onClick.AddListener(AddNewQuestionUI);
    }

    void LoadQuestions()
    {
        questionList.Clear();

        // Eski elementlarni tozalash
        foreach (Transform child in contentPanel)
            Destroy(child.gameObject);

        if (!File.Exists(jsonPath))
        {
            Debug.LogWarning("questions.json topilmadi: " + jsonPath);
            return;
        }

        string json = File.ReadAllText(jsonPath);
        try
        {
            QuestionListWrapper wrapper = JsonUtility.FromJson<QuestionListWrapper>(json);
            if (wrapper?.Items != null)
            {
                questionList = new List<QuizManager.Question>(wrapper.Items);
                Debug.Log($"{questionList.Count} ta savol yuklandi.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("JSON o'qishda xato: " + e.Message);
        }

        // Har bir savolni UI ga qo'shish
        foreach (var q in questionList)
            CreateQuestionItem(q);
    }

    void CreateQuestionItem(QuizManager.Question q)
    {
        GameObject item = Instantiate(questionItemPrefab, contentPanel);
        TMP_InputField[] inputs = item.GetComponentsInChildren<TMP_InputField>(true);

        // Tartib: 0-savol, 1-to'g'ri javob, 2-3-4-xato javoblar
        inputs[0].text = q.question;
        inputs[1].text = q.correctAnswer;
        for (int i = 0; i < 3; i++)
            inputs[2 + i].text = q.wrongAnswers[i];

        Button saveBtn = item.transform.Find("Save")?.GetComponent<Button>();
        Button deleteBtn = item.transform.Find("Delete")?.GetComponent<Button>();

        if (saveBtn != null)
        {
            saveBtn.onClick.RemoveAllListeners();
            saveBtn.onClick.AddListener(() =>
            {
                q.question = inputs[0].text;
                q.correctAnswer = inputs[1].text;
                q.wrongAnswers = new string[]
                {
                    inputs[2].text,
                    inputs[3].text,
                    inputs[4].text
                };
                SaveToJson();
            });
        }

        if (deleteBtn != null)
        {
            deleteBtn.onClick.RemoveAllListeners();
            deleteBtn.onClick.AddListener(() =>
            {
                questionList.Remove(q);
                Destroy(item);
                SaveToJson();
            });
        }
    }

    void AddNewQuestionUI()
    {
        if (string.IsNullOrWhiteSpace(questionInput.text))
        {
            Debug.LogWarning("Savol matni bo'sh!");
            return;
        }

        QuizManager.Question newQ = new QuizManager.Question
        {
            question = questionInput.text,
            correctAnswer = correctInput.text,
            wrongAnswers = new string[]
            {
                wrong1Input.text ?? "",
                wrong2Input.text ?? "",
                wrong3Input.text ?? ""
            }
        };

        questionList.Add(newQ);
        CreateQuestionItem(newQ);
        SaveToJson();

        // Inputlarni tozalash
        questionInput.text = "";
        correctInput.text = "";
        wrong1Input.text = "";
        wrong2Input.text = "";
        wrong3Input.text = "";

        questionInput.Select();
    }

    void SaveToJson()
    {
        QuestionListWrapper wrapper = new QuestionListWrapper();
        wrapper.Items = questionList;

        string json = JsonUtility.ToJson(wrapper, true);

        string dir = Path.GetDirectoryName(jsonPath);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

        File.WriteAllText(jsonPath, json);
        Debug.Log($"JSON saqlandi: {questionList.Count} ta savol → {jsonPath}");
    }

    // Editor yopilganda ham saqlaydi
    private void OnApplicationQuit() => SaveToJson();
    private void OnApplicationPause(bool pause) { if (pause) SaveToJson(); }
}