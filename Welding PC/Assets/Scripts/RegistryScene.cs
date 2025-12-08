using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class RegistryScene : MonoBehaviour
{
    public GameObject PanelUsers;
    public GameObject AboutPanel;
    public GameObject QuestionsPanel;
    public TMP_InputField IsmField;
    public TMP_InputField FamiliyaField;

    public GameObject PasswordPanel;
    public TMP_InputField passInput;
    public TextMeshProUGUI feedbackText;
    [Header("Parol")]
    private const string CORRECT_PASSWORD = "783103";

    [Header("Ranglar")]
    public Color errorColor = Color.red;
    public Color successColor = Color.green;
    public Color defaultColor = Color.white;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowPasswordPanel()
    {
        PasswordPanel.SetActive(true);
        passInput.text = "";
        passInput.Select();
        passInput.ActivateInputField();

        feedbackText.gameObject.SetActive(false);
        feedbackText.color = defaultColor;
    }

    public void CheckPassword()
    {
        string input = passInput.text.Trim();

        if (input == CORRECT_PASSWORD)
        {
            // TO‘G‘RI PAROL
            feedbackText.text = "Muvaffaqiyatli!";
            feedbackText.color = successColor;
            feedbackText.gameObject.SetActive(true);

            // Input va tugmani o‘chirish
            PasswordPanel.SetActive(false);

            // Keyingi panelni ochish
            if (QuestionsPanel) QuestionsPanel.SetActive(true);

            Debug.Log("<color=green>Admin panel ochildi!</color>");
        }
        else
        {
            // NOTO‘G‘RI PAROL
            feedbackText.text = "Noto‘g‘ri parol! Qayta urining.";
            feedbackText.color = errorColor;
            feedbackText.gameObject.SetActive(true);

            passInput.text = "";
            passInput.Select();

            // Tebranish effekti (ixtiyoriy)
            StartCoroutine(ShakeInputField());
        }
    }

    private System.Collections.IEnumerator ShakeInputField()
    {
        Vector3 originalPos = passInput.transform.localPosition;
        float shakeAmount = 10f;
        float shakeTime = 0.4f;
        float timer = 0f;

        while (timer < shakeTime)
        {
            float x = originalPos.x + Random.Range(-shakeAmount, shakeAmount);
            float y = originalPos.y + Random.Range(-shakeAmount, shakeAmount);
            passInput.transform.localPosition = new Vector3(x, y, originalPos.z);
            timer += Time.deltaTime;
            yield return null;
        }

        passInput.transform.localPosition = originalPos;
    }

    public void ResetPanel()
    {
        PasswordPanel.SetActive(false);
        QuestionsPanel.SetActive(false);
        feedbackText.gameObject.SetActive(false);
    }
    public void ToVirtual()
    {
        PasswordPanel.SetActive(false);
        feedbackText.gameObject.SetActive(false);
        GameDataController.Instance.Ism = IsmField.text;
        GameDataController.Instance.Familiya = FamiliyaField.text;
        SceneManager.LoadScene("MainScene");
    }
    public void ShowUsers()
    {
        PasswordPanel.SetActive(false);
        feedbackText.gameObject.SetActive(false);
        PanelUsers.SetActive(true);
    }

    public void HideUsers()
    {
        PanelUsers.SetActive(false);
    }

    public void HideQuest()
    {
        QuestionsPanel.SetActive(false);
    }


    public void ShowAbout()
    {
        PasswordPanel.SetActive(false);
        feedbackText.gameObject.SetActive(false);
        AboutPanel.SetActive(true);
    }

    public void HideAbout()
    {
        AboutPanel.SetActive(false);
    }
    public void ExitScene()
    {
        Application.Quit();
    }
}

