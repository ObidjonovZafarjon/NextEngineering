// Scripts/WeldingProcess2D.cs

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class WeldingProcess2D : MonoBehaviour
{
    // --- UI elementlari uchun referencelar ---
    [Header("UI Elements")]
    public RawImage weldingTorchImage;
    public RawImage leftPlateImage;
    public RawImage rightPlateImage;
    public RawImage idealWeldLineImage;

    // --- Payvandlash parametrlari matnlari uchun referencelar ---
    [Header("Welding Parameters Texts")]
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI angleText;
    public TextMeshProUGUI arcLengthText;

    // --- Payvandlash sozlamalari ---
    [Header("Welding Settings")]
    public float maxSpeed = 100f;
    public float minSpeed = 30f;
    public float idealAngleDegrees = 90f;
    public float angleTolerance = 10f;
    public float idealArcLength = 20f;
    public float arcLengthTolerance = 5f;
    public float maxDeviationForPerfectWeld = 10f;
    public float maxStillnessDuration = 0.5f;
    public float updateTextInterval = 0.2f;

    // --- Payvand chog'ini yaratish sozlamalari ---
    [Header("Weld Line Generation")]
    public GameObject weldDotPrefab;
    public Transform weldLineParent;
    public Color weldColor = Color.cyan;
    public float weldDotSpacing = 5f;       // Nuqtalar orasidagi masofa (pixel)

    [Header("Visual Effects")]
    public Image screenOverlayImage;        // Ekran qorayib titrashi uchun panel
    public RawImage lightFlashImage;           // Yorug'lik effekti uchun panel
    public ParticleSystem sparkParticles;    // Uchqunlar uchun Particle System

    [Header("Effect Settings")]
    public float maxOverlayAlpha = 0.5f;     // Qorayishning maksimal shaffofligi (0 dan 1 gacha)
    public float flashDuration = 0.1f;       // Yorug'lik chaqmoqining davomiyligi
    public float flashInterval = 0.3f;       // Chaqmoqlar orasidagi interval
    public float flickerSpeed = 10f;         // Ekran titrash tezligi

    // Ichki o'zgaruvchilar
    private Coroutine flashCoroutine; // Chaqmoqlar coroutine'sini boshqarish uchun
    private bool isFlashing = false; // Chaqmoqlar aktivmi?


    // --- Debug sozlamalari ---
    public bool showWeldDebugLogs = false;

    // --- Ichki holat o'zgaruvchilari ---
    private bool isWelding = false;
    private bool isWeldingButtonHeld = false;
    private Vector2 lastMousePosition;
    private RectTransform torchRectTransform;
    private RectTransform leftPlateRectTransform;
    private RectTransform rightPlateRectTransform;
    private RectTransform idealWeldLineRectTransform;

    // --- Baholash uchun yig'iladigan ma'lumotlar ---
    [HideInInspector] public List<float> recordedSpeeds;
    [HideInInspector] public List<float> recordedAngles;
    [HideInInspector] public List<float> recordedArcLengths;
    [HideInInspector] public List<float> recordedDeviationFromLine;
    [HideInInspector] public List<float> recordedStillnessScores;

    // Yangilangan: lastWeldDotPosition - nuqtaning pozitsiyasini aniqroq kuzatish uchun
    private Vector2 lastWeldDotPosition; // Oxirgi chizilgan payvand nuqtasining pozitsiyasi
    private float currentStillnessTimer = 0f;
    private float currentTextUpdateTimer = 0f;

    private void Awake()
    {
        if (GameManager2D.Instance != null)
        {
            GameManager2D.Instance.weldingProcess = this;
        }

        torchRectTransform = weldingTorchImage.GetComponent<RectTransform>();
        leftPlateRectTransform = leftPlateImage.GetComponent<RectTransform>();
        rightPlateRectTransform = rightPlateImage.GetComponent<RectTransform>();
        idealWeldLineRectTransform = idealWeldLineImage.GetComponent<RectTransform>();

        recordedSpeeds = new List<float>();
        recordedAngles = new List<float>();
        recordedArcLengths = new List<float>();
        recordedDeviationFromLine = new List<float>();
        recordedStillnessScores = new List<float>();
    }

    public void StartWeldingProcess()
    {
        isWelding = true;
        weldingTorchImage.gameObject.SetActive(true);
        lastMousePosition = Input.mousePosition;
        torchRectTransform.position = lastMousePosition;

        // Yangi: Payvandlash boshlanganda effektlarni ishga tushirish
        if (sparkParticles != null) sparkParticles.Play();
        if (flashCoroutine != null) StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(FlickerFlash());

        ResetWeldingState();
    }

    public void StopWeldingProcess()
    {
        isWelding = false;
        isWeldingButtonHeld = false;
        weldingTorchImage.gameObject.SetActive(false);

        // Yangi: Payvandlash to'xtatilganda effektlarni o'chirish
        if (sparkParticles != null) sparkParticles.Stop();
        if (flashCoroutine != null) StopCoroutine(flashCoroutine);

        // Ekran va yorug'lik effektlarini o'chirish
        if (screenOverlayImage != null) screenOverlayImage.color = new Color(0, 0, 0, 0);
        if (lightFlashImage != null) lightFlashImage.color = new Color(1, 1, 1, 0);
    }

    public void ResetWeldingState()
    {
        if (weldLineParent != null)
        {
            foreach (Transform child in weldLineParent)
            {
                Destroy(child.gameObject);
            }
        }
        recordedSpeeds.Clear();
        recordedAngles.Clear();
        recordedArcLengths.Clear();
        recordedDeviationFromLine.Clear();
        recordedStillnessScores.Clear();

        // Reset qilganda lastWeldDotPositionni ham reset qiling
        lastWeldDotPosition = Vector2.zero; // Yoki torchRectTransform.position ga o'rnating
        currentStillnessTimer = 0f;
        currentTextUpdateTimer = 0f;

        speedText.text = "Tezlik: ---";
        angleText.text = "Burchak: ---";
        arcLengthText.text = "Yoy Uzunligi: ---";
    }

    private void Update()
    {
        if (isWelding)
        {
            if (Input.GetMouseButtonDown(0))
            {
                isWeldingButtonHeld = true;
                lastMousePosition = Input.mousePosition;
                currentStillnessTimer = 0f;
                lastWeldDotPosition = torchRectTransform.position;

                // Yangi: Payvandlash tugmasi bosilganda uchqun va titrashni boshlash
                if (sparkParticles != null) sparkParticles.Play();
                if (flashCoroutine != null) StopCoroutine(flashCoroutine);
                flashCoroutine = StartCoroutine(FlickerFlash());

            }
            else if (Input.GetMouseButtonUp(0))
            {
                isWeldingButtonHeld = false;

                // Yangi: Payvandlash tugmasi qo'yib yuborilganda effektlarni to'xtatish
                if (sparkParticles != null) sparkParticles.Stop();
                if (flashCoroutine != null) StopCoroutine(flashCoroutine);

                // Effketlarni sekin o'chirish
                StartCoroutine(FadeOutEffects());
            }


            Vector2 currentMousePosition = Input.mousePosition;
            torchRectTransform.position = currentMousePosition;

            currentTextUpdateTimer += Time.deltaTime;


            if (isWeldingButtonHeld)
            {
                float distance = Vector2.Distance(currentMousePosition, lastMousePosition);
                float speed = distance / Time.deltaTime;
                recordedSpeeds.Add(speed);

                float angle = torchRectTransform.localEulerAngles.z;
                if (angle > 180) angle -= 360;
                recordedAngles.Add(angle);

                float idealWeldLineY = idealWeldLineRectTransform.position.y;
                float arcLength = Mathf.Abs(torchRectTransform.position.y - idealWeldLineY);
                recordedArcLengths.Add(arcLength);

                float idealLineX = idealWeldLineRectTransform.position.x;
                float deviation = Mathf.Abs(torchRectTransform.position.x - idealLineX);
                recordedDeviationFromLine.Add(deviation);

                if (speed < minSpeed / 2f)
                {
                    currentStillnessTimer += Time.deltaTime;
                }
                else
                {
                    currentStillnessTimer = 0f;
                }
                float stillnessScore = 1f - Mathf.Clamp01(currentStillnessTimer / maxStillnessDuration);
                recordedStillnessScores.Add(stillnessScore);

                if (currentTextUpdateTimer >= updateTextInterval)
                {
                    UpdateSpeedText(speed);
                    UpdateAngleText(angle);
                    UpdateArcLengthText(arcLength);
                    currentTextUpdateTimer = 0f;
                }

                // Chizish funksiyasi yangilandi
                DrawWeldLine(currentMousePosition);
            }
            else
            {
                speedText.text = "Tezlik: ---";
                angleText.text = "Burchak: ---";
                arcLengthText.text = "Yoy Uzunligi: ---";

                // Tugma bosilmaganda lastWeldDotPositionni doimiy yangilab turish
                // Keyingi safar tugma bosilganda uzluksizlikni ta'minlaydi
                lastWeldDotPosition = currentMousePosition;
            }

            lastMousePosition = currentMousePosition;
        }
    }
    private IEnumerator FlickerFlash()
    {
        isFlashing = true;

        while (isFlashing)
        {
            // Yorug'lik chaqmog'i
            if (lightFlashImage != null) lightFlashImage.color = new Color(1, 1, 1, 1);
            yield return new WaitForSeconds(flashDuration);
            if (lightFlashImage != null) lightFlashImage.color = new Color(1, 1, 1, 0);

            // Ekran titrashi
            if (screenOverlayImage != null)
            {
                // Alpha qiymatini qisqa muddatga o'zgartirish
                float randomAlpha = Random.Range(0.1f, maxOverlayAlpha);
                screenOverlayImage.color = new Color(0, 0, 0, randomAlpha);
            }

            // Keyingi chaqmoqgacha kutish
            yield return new WaitForSeconds(flashInterval);
        }
    }

    // Effektlarni sekin o'chirish funksiyasi
    private IEnumerator FadeOutEffects()
    {
        isFlashing = false;

        // Ekran titrashini silliq o'chirish
        if (screenOverlayImage != null)
        {
            Color initialColor = screenOverlayImage.color;
            float timer = 0f;
            while (timer < 0.5f) // 0.5 sekundda o'chirish
            {
                timer += Time.deltaTime;
                float newAlpha = Mathf.Lerp(initialColor.a, 0, timer / 0.5f);
                screenOverlayImage.color = new Color(0, 0, 0, newAlpha);
                yield return null;
            }
            screenOverlayImage.color = new Color(0, 0, 0, 0); // To'liq shaffof qilish
        }

        // Yorug'lik chaqmoqini o'chirish
        if (lightFlashImage != null) lightFlashImage.color = new Color(1, 1, 1, 0);
    }
    private void UpdateSpeedText(float currentSpeed)
    {
        string speedStatus;
        if (currentSpeed > maxSpeed) speedStatus = "Tez";
        else if (currentSpeed < minSpeed) speedStatus = "Sekin";
        else speedStatus = "Me'yorida";
        speedText.text = $"Tezlik: {speedStatus} ({currentSpeed:F1} px/s)";
    }

    private void UpdateAngleText(float currentAngle)
    {
        string angleStatus;
        float normalizedAngle = currentAngle;
        if (normalizedAngle > 180) normalizedAngle -= 360;

        float diff1 = Mathf.Abs(normalizedAngle - idealAngleDegrees);
        float diff2 = Mathf.Abs(normalizedAngle - (idealAngleDegrees - 180));
        float diff3 = Mathf.Abs(normalizedAngle - (idealAngleDegrees + 180));

        float minDiff = Mathf.Min(diff1, diff2, diff3);

        if (minDiff <= angleTolerance)
        {
            angleStatus = "Me'yorida";
        }
        else
        {
            angleStatus = "Noto'g'ri";
        }
        angleText.text = $"Burchak: {angleStatus} ({currentAngle:F1}°)";
    }

    private void UpdateArcLengthText(float currentArcLength)
    {
        string arcLengthStatus;
        if (Mathf.Abs(currentArcLength - idealArcLength) <= arcLengthTolerance) arcLengthStatus = "Me'yorida";
        else if (currentArcLength > idealArcLength) arcLengthStatus = "Uzun";
        else arcLengthStatus = "Kalta";
        arcLengthText.text = $"Yoy Uzunligi: {arcLengthStatus} ({currentArcLength:F1} px)";
    }

    // --- Yaxshilangan DrawWeldLine funksiyasi ---
    private void DrawWeldLine(Vector2 currentTorchPosition)
    {
        float idealLineX = idealWeldLineRectTransform.position.x;
        float deviation = Mathf.Abs(torchRectTransform.position.x - idealLineX);

        if (isWeldingButtonHeld && deviation <= maxDeviationForPerfectWeld)
        {
            // Oldingi nuqta bilan joriy nuqta orasidagi masofani hisoblash
            float distanceToTravel = Vector2.Distance(lastWeldDotPosition, currentTorchPosition);

            // Agar masofa `weldDotSpacing` dan katta bo'lsa, o'rtadagi nuqtalarni ham chizish
            if (distanceToTravel >= weldDotSpacing)
            {
                int numberOfDots = Mathf.FloorToInt(distanceToTravel / weldDotSpacing);

                for (int i = 0; i <= numberOfDots; i++)
                {
                    // Nuqta joylashuvini interpolatsiya qilish (Liner interpolatsiya)
                    Vector2 dotPosition = Vector2.Lerp(lastWeldDotPosition, currentTorchPosition, (float)i / numberOfDots);

                    GameObject dot = Instantiate(weldDotPrefab, weldLineParent);
                    RectTransform dotRect = dot.GetComponent<RectTransform>();

                    dotRect.position = dotPosition;
                    dot.GetComponent<RawImage>().color = weldColor;
                    dotRect.sizeDelta = new Vector2(5, 5);

                    if (showWeldDebugLogs)
                    {
                        Debug.Log($"Weld Dot drawn at X: {dotPosition.x}, Y: {dotPosition.y}");
                    }
                }
                // Oxirgi chizilgan nuqtaning pozitsiyasini yangilash
                lastWeldDotPosition = currentTorchPosition;
            }
        }
        else
        {
            // Agar tugma bosilmagan bo'lsa yoki chegaradan chiqqan bo'lsa,
            // keyingi safar chizishni boshlash uchun oxirgi nuqta pozitsiyasini yangilab turamiz.
            lastWeldDotPosition = currentTorchPosition;
        }
    }
}