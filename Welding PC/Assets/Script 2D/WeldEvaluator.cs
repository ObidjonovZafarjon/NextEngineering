// Scripts/WeldEvaluator.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class WeldEvaluator : MonoBehaviour
{
    [Header("Evaluation UI")]
    public TextMeshProUGUI overallResultText;
    public TextMeshProUGUI feedbackText;
    public GameObject graphsContainer; // Grafiklar joylashadigan parent

    [Header("Graph Prefabs")]
    public GameObject graphPrefab; // Umumiy grafik uchun prefab
    public GameObject graphPointPrefab; // Grafikdagi nuqta uchun prefab

    // WeldingProcess2D skriptidan ma'lumotlarni olish uchun
    private WeldingProcess2D weldingProcess;

    private void Awake()
    {
        if (GameManager2D.Instance != null)
        {
            GameManager2D.Instance.weldEvaluator = this;
            // GameManager'ning Awake'ida weldingProcess o'rnatilgan bo'lishi shart
            weldingProcess = GameManager2D.Instance.weldingProcess;
            if (weldingProcess == null)
            {
                Debug.LogError("WeldEvaluator: WeldingProcess2D reference is NULL! Check GameManager's Awake and Script Execution Order.");
            }
        }
    }

    // Natijalarni baholash funksiyasi
    public void EvaluateResults()
    {
        if (weldingProcess == null)
        {
            Debug.LogError("WeldingProcess2D reference not set in WeldEvaluator! Cannot evaluate results.");
            overallResultText.text = "Xatolik: Payvandlash ma'lumotlari topilmadi.";
            feedbackText.text = "Iltimos, dasturni qayta ishga tushiring.";
            GameManager2D.Instance.retryButton.gameObject.SetActive(true);
            GameManager2D.Instance.nextSampleButton.gameObject.SetActive(false);
            return;
        }

        ClearGraphs();

        float totalScore = 0f;
        int criteriaCount = 0;

        // 1. Payvandlash tezligi baholash
        float speedScore = CalculateSpeedScore(weldingProcess.recordedSpeeds, weldingProcess.minSpeed, weldingProcess.maxSpeed);
        totalScore += speedScore;
        criteriaCount++;
        Debug.Log($"Speed Score: {speedScore}");
        GenerateGraph(weldingProcess.recordedSpeeds, "Payvandlash Tezligi (px/s)", 0f, weldingProcess.maxSpeed * 1.5f, speedScore); // Maksimal diapazonni kengaytirdik

        // 2. Payvand chokining tekis va uzluksizligi (qizil chiziqdan chetga chiqish)
        float deviationScore = CalculateDeviationScore(weldingProcess.recordedDeviationFromLine, weldingProcess.maxDeviationForPerfectWeld);
        totalScore += deviationScore;
        criteriaCount++;
        Debug.Log($"Deviation Score: {deviationScore}");
        GenerateGraph(weldingProcess.recordedDeviationFromLine, "Qizil Chiziqdan Chetga Chiqish (px)", 0f, weldingProcess.maxDeviationForPerfectWeld * 2, deviationScore); // Maksimal chetga chiqishni ko'rsatish

        // 3. Payvandlash dastagini burchagi
        float angleScore = CalculateAngleScore(weldingProcess.recordedAngles, weldingProcess.idealAngleDegrees, weldingProcess.angleTolerance);
        totalScore += angleScore;
        criteriaCount++;
        Debug.Log($"Angle Score: {angleScore}");
        GenerateGraph(weldingProcess.recordedAngles, "Dastak Burchagi (°)", 0f, 180f, angleScore);

        // 4. Payvandlash yoyining uzunligini baholash (Arc Length)
        float arcLengthScore = CalculateArcLengthScore(weldingProcess.recordedArcLengths, weldingProcess.idealArcLength, weldingProcess.arcLengthTolerance);
        totalScore += arcLengthScore;
        criteriaCount++;
        Debug.Log($"Arc Length Score: {arcLengthScore}");
        GenerateGraph(weldingProcess.recordedArcLengths, "Yoy Uzunligi (px)", 0f, weldingProcess.idealArcLength * 2, arcLengthScore);

        // **Yangi: 5. Harakatsizlik/Qotib qolish bahosi**
        float stillnessScore = CalculateStillnessScore(weldingProcess.recordedStillnessScores);
        totalScore += stillnessScore;
        criteriaCount++;
        Debug.Log($"Stillness Score: {stillnessScore}");
        GenerateGraph(weldingProcess.recordedStillnessScores, "Harakatsizlik Bahosi (1: Ideal, 0: Yomon)", 0f, 1.1f, stillnessScore); // Baho 0-1 oralig'ida

        // Umumiy natijani hisoblash
        float overallPercentage = (totalScore / criteriaCount) * 100f;
        overallResultText.text = $"Umumiy natija: {overallPercentage:F1}%";

        // Qayta urinish yoki davom etish
        if (overallPercentage >= 80f)
        {
            feedbackText.text = "Tabriklar! Siz a'lo darajada payvandladingiz!";
            GameManager2D.Instance.retryButton.gameObject.SetActive(false);
            GameManager2D.Instance.nextSampleButton.gameObject.SetActive(true);
        }
        else
        {
            feedbackText.text = "Qayta urinib ko'ring. Natija 80%dan kamroq.";
            GameManager2D.Instance.retryButton.gameObject.SetActive(true);
            GameManager2D.Instance.nextSampleButton.gameObject.SetActive(false);
        }

        GameManager2D.Instance.SaveResults(overallPercentage); // Natijani saqlash
    }

    // Har bir kriteriya uchun ball hisoblash funksiyalari (0 dan 1 gacha)
    private float CalculateSpeedScore(List<float> speeds, float minIdeal, float maxIdeal)
    {
        if (speeds.Count == 0) return 0f;
        float score = 0f;
        foreach (float s in speeds)
        {
            if (s >= minIdeal && s <= maxIdeal)
            {
                score += 1f;
            }
            else
            {
                // Agar juda sekin bo'lsa (0ga yaqin), ball juda kam bo'lishi kerak.
                // Agar juda tez bo'lsa, ball kamroq bo'lishi kerak.
                float deviationFromMin = Mathf.Max(0, minIdeal - s);
                float deviationFromMax = Mathf.Max(0, s - maxIdeal);

                if (deviationFromMin > 0) score += Mathf.Max(0f, 1f - (deviationFromMin / minIdeal)); // Sekinlikka qarab ball kamayadi
                else if (deviationFromMax > 0) score += Mathf.Max(0f, 1f - (deviationFromMax / maxIdeal)); // Tezlikka qarab ball kamayadi
            }
        }
        return score / speeds.Count;
    }

    private float CalculateDeviationScore(List<float> deviations, float maxAllowed)
    {
        if (deviations.Count == 0) return 0f;
        float score = 0f;
        foreach (float d in deviations)
        {
            if (d <= maxAllowed)
            {
                score += 1f;
            }
            else
            {
                score += Mathf.Max(0f, 1f - (d - maxAllowed) / (maxAllowed * 2)); // Chetga chiqish oshgani sari ball kamayadi
            }
        }
        return score / deviations.Count;
    }

    private float CalculateAngleScore(List<float> angles, float idealAngle, float tolerance)
    {
        if (angles.Count == 0) return 0f;
        float score = 0f;
        foreach (float a in angles)
        {
            float diff = Mathf.Abs(a - idealAngle);
            if (diff <= tolerance)
            {
                score += 1f;
            }
            else
            {
                score += Mathf.Max(0f, 1f - (diff - tolerance) / (tolerance * 3));
            }
        }
        return score / angles.Count;
    }

    private float CalculateArcLengthScore(List<float> arcLengths, float idealLength, float tolerance)
    {
        if (arcLengths.Count == 0) return 0f;
        float score = 0f;
        foreach (float al in arcLengths)
        {
            float diff = Mathf.Abs(al - idealLength);
            if (diff <= tolerance)
            {
                score += 1f;
            }
            else
            {
                score += Mathf.Max(0f, 1f - (diff - tolerance) / (tolerance * 3));
            }
        }
        return score / arcLengths.Count;
    }

    // Yangi: Harakatsizlik/Qotib qolish bahosi
    private float CalculateStillnessScore(List<float> stillnessScores)
    {
        if (stillnessScores.Count == 0) return 0f;
        return stillnessScores.Average(); // Ballar allaqachon 0-1 oralig'ida bo'lgani uchun o'rtachasini olamiz
    }


    // Grafik yaratish funksiyasi
    private void GenerateGraph(List<float> data, string title, float minValue, float maxValue, float performanceScore)
    {
        if (data.Count == 0 || graphPrefab == null || graphPointPrefab == null || graphsContainer == null) return;

        // Grafik ob'ektini yaratish
        GameObject graphInstance = Instantiate(graphPrefab, graphsContainer.transform);
        TextMeshProUGUI graphTitle = graphInstance.transform.Find("GraphTitle").GetComponent<TextMeshProUGUI>();
        Image graphBackground = graphInstance.transform.Find("GraphBackground").GetComponent<Image>();
        Transform graphDataContainer = graphBackground.transform.Find("GraphDataContainer");
        TextMeshProUGUI minMaxLabels = graphInstance.transform.Find("MinMaxLabels").GetComponent<TextMeshProUGUI>();

        graphTitle.text = title;
        minMaxLabels.text = $"Min: {data.Min():F1}\nMax: {data.Max():F1}";

        // Grafikning samaradorlik rangini belgilash
        if (performanceScore >= 0.8f) graphBackground.color = new Color(0.7f, 1f, 0.7f, 0.5f); // Yashilroq
        else if (performanceScore >= 0.5f) graphBackground.color = new Color(1f, 1f, 0.7f, 0.5f); // Sariqroq
        else graphBackground.color = new Color(1f, 0.7f, 0.7f, 0.5f); // Qizilroq


        float graphWidth = graphBackground.rectTransform.rect.width;
        float graphHeight = graphBackground.rectTransform.rect.height;

        // Ma'lumotlarni grafikda ko'rsatish
        for (int i = 0; i < data.Count; i++)
        {
            GameObject point = Instantiate(graphPointPrefab, graphDataContainer);
            RectTransform pointRect = point.GetComponent<RectTransform>();

            // Ma'lumotni 0-1 oralig'iga normallashtirish
            // Bu yerda maxValue ga 0.001 qo'shish, agar data.Max() va maxValue bir xil bo'lsa, bo'linishda muammo bo'lmasligi uchun
            float normalizedValue = Mathf.InverseLerp(minValue, maxValue + 0.001f, data[i]);

            // Nuqtaning Y pozitsiyasi
            pointRect.anchorMin = new Vector2(0, normalizedValue);
            pointRect.anchorMax = new Vector2(0, normalizedValue);
            pointRect.pivot = new Vector2(0, normalizedValue);
            // Grafikning gorizontal bo'ylab teng taqsimlanishi
            pointRect.anchoredPosition = new Vector2(i * (graphWidth / data.Count), 0);

            // Rangini ham baholashga qarab o'zgartirish
            Color pointColor = Color.Lerp(Color.red, Color.green, normalizedValue);
            point.GetComponent<Image>().color = pointColor;
        }
    }

    // Grafiklar va natijalarni tozalash funksiyasi
    public void ClearGraphs()
    {
        overallResultText.text = "";
        feedbackText.text = "";
        if (graphsContainer != null)
        {
            foreach (Transform child in graphsContainer.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }

    // Baholovchini qayta tiklash (Qaytadan urinish uchun)
    public void ResetEvaluator()
    {
        ClearGraphs();
        overallResultText.text = "";
        feedbackText.text = "";
    }
}