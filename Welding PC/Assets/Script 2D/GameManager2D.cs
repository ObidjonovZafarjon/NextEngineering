// Scripts/GameManager.cs
using UnityEngine;
using UnityEngine.UI; // UI elementlari bilan ishlash uchun
using TMPro; // Agar TextMeshPro ishlatayotgan bo'lsangiz
using System.Collections; // Coroutine uchun

public class GameManager2D : MonoBehaviour
{
    public static GameManager2D Instance { get; private set; } // Singleton pattern

    // UI elementlari uchun referencelar
    [Header("UI Panels")]
    public GameObject startPanel;
    public GameObject weldingPanel;
    public GameObject evaluationPanel;
    public GameObject mainInfoPanel; // Yuqori panel

    [Header("Buttons")]
    public Button turnOnApparatusButton;
    public Button openVentilButton;
    public Button startWeldingButton;
    public Button finishWeldingButton;
    public Button retryButton;
    public Button nextSampleButton;

    [Header("UI Texts")]
    public TextMeshProUGUI statusText; // Holat xabarlari uchun

    // O'yin holatlari
    public bool isApparatusOn { get; private set; }
    public bool isVentilOpen { get; private set; }
    public bool isWeldingActive { get; private set; }

    // Boshqa skriptlar uchun referencelar
    [Header("Other Script References")] public WeldingProcess2D weldingProcess;
    [HideInInspector] public WeldEvaluator weldEvaluator;

    private void Awake()
    {
        // Singleton Pattern: Ob'ektning faqat bitta nusxasi bo'lishini ta'minlaydi
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }


        // Boshida barcha panellarni yopish
        startPanel.SetActive(true);
        weldingPanel.SetActive(false);
        evaluationPanel.SetActive(false);
        mainInfoPanel.SetActive(true); // Asosiy info panel doim ochiq
    }

    private void Start()
    {
        // Buttonlarga listenerlar qo'shish
        turnOnApparatusButton.onClick.AddListener(TurnOnApparatus);
        openVentilButton.onClick.AddListener(OpenVentil);
        startWeldingButton.onClick.AddListener(StartWelding);
        finishWeldingButton.onClick.AddListener(FinishWelding);
        retryButton.onClick.AddListener(RetryWelding);
        nextSampleButton.onClick.AddListener(NextSample);

        // Boshlang'ich holatni o'rnatish
        ResetSimulationState();
        UpdateStatusText("Payvandlash apparatini yoqing.");
    }

    // Apparatni yoqish funksiyasi
    public void TurnOnApparatus()
    {
        isApparatusOn = true;
        UpdateStatusText("Apparat yoqildi. Argon ventelini oching.");
        turnOnApparatusButton.interactable = false; // Tugmani o'chirish
        openVentilButton.interactable = true; // Keyingi tugmani yoqish
    }

    // Ventilni ochish funksiyasi
    public void OpenVentil()
    {
        isVentilOpen = true;
        UpdateStatusText("Ventil ochildi. Payvandlashni boshlashga tayyor.");
        openVentilButton.interactable = false; // Tugmani o'chirish
        startWeldingButton.interactable = true; // Keyingi tugmani yoqish
    }

    // Payvandlashni boshlash funksiyasi
    public void StartWelding()
    {
        if (isApparatusOn && isVentilOpen)
        {
            isWeldingActive = true;
            UpdateStatusText("Payvandlash boshlandi. Dastakni boshqaring!");
            startPanel.SetActive(false);
            weldingPanel.SetActive(true);
            finishWeldingButton.interactable = true; // Tugatish tugmasini yoqish
            weldingProcess.StartWeldingProcess(); // WeldingProcess2D skriptini ishga tushirish
        }
        else
        {
            UpdateStatusText("Iltimos, avval apparatni yoqing va ventilni oching!");
        }
    }

    // Payvandlashni tugatish funksiyasi
    public void FinishWelding()
    {
        if (isWeldingActive)
        {
            isWeldingActive = false;
            weldingProcess.StopWeldingProcess(); // WeldingProcess2D skriptini to'xtatish
            UpdateStatusText("Payvandlash tugatildi. Natijalar hisoblanmoqda...");
            StartCoroutine(EvaluateWeldingAfterDelay(1.0f)); // Kechikish bilan baholash
        }
    }

    private IEnumerator EvaluateWeldingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        weldingPanel.SetActive(false);
        evaluationPanel.SetActive(true);
        weldEvaluator.EvaluateResults(); // Natijalarni baholash
    }

    // Qaytadan takrorlash funksiyasi
    public void RetryWelding()
    {
        ResetSimulationState();
        weldingProcess.ResetWeldingState(); // Payvandlash jarayonini qayta tiklash
        weldEvaluator.ResetEvaluator(); // Baholovchini qayta tiklash
        startPanel.SetActive(true);
        evaluationPanel.SetActive(false);
        UpdateStatusText("Qaytadan urinish. Payvandlash apparatini yoqing.");
    }

    // Keyingi namunani payvandlash funksiyasi
    public void NextSample()
    {
        // Bu joyda keyingi namunani yuklash yoki sozlash mantig'i yoziladi
        // Hozircha oddiygina holatni qayta tiklaymiz
        ResetSimulationState();
        weldingProcess.ResetWeldingState();
        weldEvaluator.ResetEvaluator();
        startPanel.SetActive(true);
        evaluationPanel.SetActive(false);
        UpdateStatusText("Keyingi namunaga o'tildi. Payvandlash apparatini yoqing.");
    }

    // Simulyatsiya holatini boshlang'ich holatiga qaytarish
    private void ResetSimulationState()
    {
        isApparatusOn = false;
        isVentilOpen = false;
        isWeldingActive = false;

        turnOnApparatusButton.interactable = true;
        openVentilButton.interactable = false;
        startWeldingButton.interactable = false;
        finishWeldingButton.interactable = false;
        retryButton.interactable = true; // Har doim urinish imkoniyati
        nextSampleButton.interactable = true; // Har doim keyingi imkoniyat
    }

    // Holat matnini yangilash
    public void UpdateStatusText(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
    }
    // GameManager.cs skriptiga qo'shimchalar
    public void SaveResults(float overallPercentage)
    {
        // PlayerPrefs - bu Unityda kichik ma'lumotlarni saqlash uchun qulay usul.
        // Odatda so'nggi natijani saqlash uchun ishlatiladi.
        // Agar bir nechta natijani saqlamoqchi bo'lsangiz, murakkabroq mexanizmlar (JSON/fayl) kerak.
        PlayerPrefs.SetFloat("LastWeldingScore", overallPercentage);
        PlayerPrefs.Save(); // O'zgarishlarni saqlash
        Debug.Log($"Natija saqlandi: {overallPercentage:F1}%");
    }

    public float LoadLastResult()
    {
        return PlayerPrefs.GetFloat("LastWeldingScore", 0f); // Agar topilmasa 0 qaytaradi
    }
}