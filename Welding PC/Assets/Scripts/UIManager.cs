using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI speedText; // Tezlikni ko'rsatish uchun
    public TextMeshProUGUI arcLengthText; // Yoy uzunligini ko'rsatish uchun
    public TextMeshProUGUI horizontalAngleText; // Gorizontal burchakni ko'rsatish uchun
    public TextMeshProUGUI verticalAngleText; // Vertikal burchakni ko'rsatish uchun
    public GameObject evaluationPanel; // Baholash natijalarini ko'rsatish paneli
    public TextMeshProUGUI finalScoreText; // Yakuniy ball
    public GameObject messagePanel; // Xabarlar paneli (tabrik/qaytadan urinish)
    public TextMeshProUGUI messageText;

    // Grafiklar uchun RectTransform'lar (bu yerda grafik komponentlari yo'q, shunchaki joy egalari)
    public RectTransform speedGraphContainer;
    public RectTransform straightnessGraphContainer;
    public RectTransform offPathGraphContainer;
    public RectTransform horizontalAngleGraphContainer;
    public RectTransform verticalAngleGraphContainer;
    public RectTransform arcLengthGraphContainer;


    void Start()
    {
        HideEvaluationResult();
        HideMessage();
        HideWeldingUI();
    }

    public void ShowWeldingUI()
    {
        // Payvandlash paytida ko'rsatiladigan UI elementlarini faollashtirish
        speedText.gameObject.SetActive(true);
        arcLengthText.gameObject.SetActive(true);
        horizontalAngleText.gameObject.SetActive(true);
        verticalAngleText.gameObject.SetActive(true);
    }

    public void HideWeldingUI()
    {
        // Payvandlash tugaganida UI elementlarini o'chirish
        speedText.gameObject.SetActive(false);
        arcLengthText.gameObject.SetActive(false);
        horizontalAngleText.gameObject.SetActive(false);
        verticalAngleText.gameObject.SetActive(false);
    }

    public void UpdateSpeedText(string text)
    {
        speedText.text = "Tezlik: " + text;
    }

    public void UpdateArcLengthText(string text)
    {
        arcLengthText.text = "Yoy uzunligi: " + text;
    }

    public void UpdateAngleDisplay(float horizAngle, float vertAngle)
    {
        horizontalAngleText.text = $"Gorizontal burchak: {horizAngle:F1}°";
        verticalAngleText.text = $"Vertikal burchak: {vertAngle:F1}°";
    }

    public void ShowEvaluationResult(float finalScore)
    {
        evaluationPanel.SetActive(true);
        finalScoreText.text = $"Yakuniy ball: {finalScore:F1}%";
        // Grafik funksiyalarini chaqirish (bu yerda faqat joy egalari)
        // GenerateSpeedGraph(dataLogger.weldingSpeeds);
        // ... boshqa grafik funksiyalari
    }

    public void HideEvaluationResult()
    {
        evaluationPanel.SetActive(false);
    }

    public void DisplayMessage(string message, bool isSuccess)
    {
        messagePanel.SetActive(true);
        messageText.text = message;
        messageText.color = isSuccess ? Color.green : Color.red;
    }

    public void HideMessage()
    {
        messagePanel.SetActive(false);
    }

    // Grafiklarni chizish funksiyalari (bu yerda siz grafik kutubxonalarini (masalan, Unity UI Lines, LineRenderer) ishlatishingiz kerak bo'ladi)
    public void GenerateGraph(RectTransform container, List<float> data, float minValue, float maxValue, Color lineColor, string title)
    {
        // Bu erda grafik chizish logikasi bo'ladi.
        // Misol uchun, LineRenderer komponentini ishlatib yoki UI uchun CanvasRenderer bilan.
        // Haqiqiy grafik komponentini Unity UI'da yaratishingiz kerak.
        Debug.Log($"Grafik chizilmoqda: {title} - {data.Count} nuqta");
        // Container ichida yangi GameObject yarating va unga LineRenderer yoki Image/RawImage qo'shing
        // Va ma'lumotlarni chizing.
    }
}
