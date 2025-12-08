using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Michsky.MUIP;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class QuizManager : MonoBehaviour
{
    [Header("UI Elementlar")]
    public TextMeshProUGUI questionText;
    public ButtonManager[] answerButtons; // 4 TA BO‘LISHI SHART!
    public ButtonManager backToMenuButton;
    public ButtonManager restartButton;
    public TextMeshProUGUI questionNumberText;

    [Header("Natija")]
    public float passingScore = 90f;

    private List<Question> questions = new List<Question>();
    private int currentQuestionIndex = 0;
    private int correctAnswers = 0;

    [System.Serializable]
    public class Question
    {
        public string question;
        public string correctAnswer;
        public string[] wrongAnswers;
    }

    [System.Serializable]
    private class QuestionListWrapper
    {
        public List<Question> Items;
    }

    // users.json uchun struktura
    [System.Serializable]
    public class UserResult
    {
        public string firstName;
        public string lastName;
        public float score;
        public int correct;
        public int total;
        public string date;
        public string time;
    }

    [System.Serializable]
    private class UsersWrapper
    {
        public List<UserResult> users = new List<UserResult>();
    }

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        LoadQuestions();
        SetupButtons();
        DisplayQuestion();
    }

    void LoadQuestions()
    {
        TextAsset jsonFile = LoadQuestionsFromResources();
        if (jsonFile == null)
        {
            questionText.text = "Savollar yuklanmadi!";
            return;
        }

        try
        {
            QuestionListWrapper wrapper = JsonUtility.FromJson<QuestionListWrapper>(jsonFile.text);
            if (wrapper?.Items != null && wrapper.Items.Count > 0)
            {
                questions = wrapper.Items;
                questions = questions.OrderBy(x => Random.value).Take(10).ToList();
                Debug.Log($"<color=green>{questions.Count} ta savol yuklandi!</color>");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("JSON xato: " + e.Message);
        }
    }

    TextAsset LoadQuestionsFromResources()
    {
        Resources.UnloadUnusedAssets();

#if UNITY_EDITOR
        string path = "Assets/Resources/questions.json";
        if (File.Exists(path))
        {
            string jsonText = File.ReadAllText(path);
            return new TextAsset(jsonText);
        }
#endif
        return Resources.Load<TextAsset>("questions");
    }

    void SetupButtons()
    {
        if (answerButtons == null || answerButtons.Length != 4)
        {
            Debug.LogError("answerButtons 4 ta bo‘lishi kerak!");
            return;
        }

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i;
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => OnAnswerSelected(index));
        }

        backToMenuButton.onClick.RemoveAllListeners();
        backToMenuButton.onClick.AddListener(BackToMainMenu);

        restartButton.onClick.RemoveAllListeners();
        restartButton.onClick.AddListener(RestartQuiz);
    }

    void DisplayQuestion()
    {
        if (questions == null || questions.Count == 0 || currentQuestionIndex >= questions.Count)
        {
            questionText.text = "Savollar tugadi.";
            return;
        }

        questionNumberText.text = $"Savol {currentQuestionIndex + 1}/10";
        Question q = questions[currentQuestionIndex];
        questionText.text = q.question;

        List<string> answers = new List<string> { q.correctAnswer };
        if (q.wrongAnswers != null) answers.AddRange(q.wrongAnswers);
        while (answers.Count < 4) answers.Add("Javob yo‘q");
        answers = answers.OrderBy(x => Random.value).ToList();

        for (int i = 0; i < 4; i++)
        {
            answerButtons[i].buttonText = answers[i];
            answerButtons[i].UpdateUI();
            answerButtons[i].gameObject.SetActive(true);
        }

        restartButton.gameObject.SetActive(false);
        backToMenuButton.gameObject.SetActive(false);
    }

    void OnAnswerSelected(int buttonIndex)
    {
        if (buttonIndex < 0 || buttonIndex >= 4) return;

        string selected = answerButtons[buttonIndex].buttonText;
        if (selected == questions[currentQuestionIndex].correctAnswer)
            correctAnswers++;

        currentQuestionIndex++;

        if (currentQuestionIndex < questions.Count)
            DisplayQuestion();
        else
            ShowResults();
    }

    void ShowResults()
    {
        float percentage = (correctAnswers / 10f) * 100f;
        GameDataController.Instance.testScore = percentage;

        // Ism va familiya GameDataController dan olinadi
        string firstName = GameDataController.Instance.Ism ?? "Anonim";
        string lastName = GameDataController.Instance.Familiya ?? "";

        string fullName = string.IsNullOrEmpty(lastName) ? firstName : $"{firstName} {lastName}";

        string resultText = percentage >= passingScore
            ? $"<color=green>Tabriklaymiz, {fullName}!</color>\nSiz {correctAnswers}/10 ({percentage:F1}%) ball oldingiz!\nKeyingi bosqichga o‘tdingiz!"
            : $"<color=red>Afsus, {fullName}!</color>\nSiz {correctAnswers}/10 ({percentage:F1}%) ball oldingiz.\n90% kerak. Qayta urining!";

        questionText.text = resultText;

        foreach (var btn in answerButtons)
            btn.gameObject.SetActive(false);

        backToMenuButton.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(percentage < passingScore);

        // NATIJANI users.json GA SAQLASH
        SaveUserResultToJson(firstName, lastName, percentage);
    }

    void SaveUserResultToJson(string firstName, string lastName, float percentage)
    {
        UserResult newResult = new UserResult
        {
            firstName = firstName,
            lastName = lastName,
            score = percentage,
            correct = correctAnswers,
            total = 10,
            date = System.DateTime.Now.ToString("dd.MM.yyyy"),
            time = System.DateTime.Now.ToString("HH:mm")
        };

        UsersWrapper wrapper = new UsersWrapper();
        string path = Application.dataPath + "/Resources/users.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            try
            {
                wrapper = JsonUtility.FromJson<UsersWrapper>(json);
                if (wrapper == null) wrapper = new UsersWrapper();
            }
            catch
            {
                wrapper = new UsersWrapper();
            }
        }

        wrapper.users.Add(newResult);

        string finalJson = JsonUtility.ToJson(wrapper, true);

        try
        {
            File.WriteAllText(path, finalJson);
            Debug.Log($"<color=yellow>Natija saqlandi: {firstName} {lastName} — {percentage:F1}%</color>");

#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
        }
        catch (System.Exception e)
        {
            Debug.LogError("users.json ga saqlashda xato: " + e.Message);
        }
    }

    public void RestartQuiz()
    {
        currentQuestionIndex = 0;
        correctAnswers = 0;
        LoadQuestions();
        DisplayQuestion();
    }

    void BackToMainMenu()
    {
        SceneManager.LoadScene("MainScene");
    }
}