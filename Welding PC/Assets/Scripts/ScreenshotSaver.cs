using System;
using System.IO;
using UnityEngine;
using System.Collections;
using Michsky.MUIP;
using TMPro; // TextMeshPro kutubxonasini qo'shish

public class ScreenshotSaver : MonoBehaviour
{
    [Tooltip("Skrinshot olish tugmasi (bosilganda yashiriladi)")]
    public ButtonManager screenshotButton;

    [Tooltip("Skrinshot olayotganda yashiriladigan qo‘shimcha tugma")]
    public ButtonManager extraButtonToHide;

    [Tooltip("Saqlash manzilini ko'rsatish uchun TextMeshPro obyekti")]
    public TextMeshProUGUI statusText; // Yangi o'zgaruvchi: TextMeshProUGUI

    private string reportsFolderName = "Hisobotlar";
    private const float DisplayDuration = 7f; // Ko'rsatish muddati (5 sekund)

    void Start()
    {
        // Boshlang'ich holatda tekstni yashirish
        if (statusText != null)
            statusText.gameObject.SetActive(false);

        if (screenshotButton != null)
            screenshotButton.onClick.AddListener(() => StartCoroutine(CaptureAndSave()));
    }

    private IEnumerator CaptureAndSave()
    {
        // 🔹 1. Tugmalarni vaqtincha yashiramiz
        bool wasScreenshotBtnActive = false;
        bool wasExtraBtnActive = false;

        if (screenshotButton != null)
        {
            wasScreenshotBtnActive = screenshotButton.gameObject.activeSelf;
            screenshotButton.gameObject.SetActive(false);
        }

        if (extraButtonToHide != null)
        {
            wasExtraBtnActive = extraButtonToHide.gameObject.activeSelf;
            extraButtonToHide.gameObject.SetActive(false);
        }

        // 🔹 2. UI yangilanishi uchun 1-2 frame kutamiz
        yield return null;
        yield return new WaitForEndOfFrame();

        // 🔹 3. Ekrandan rasm olish
        int width = Screen.width;
        int height = Screen.height;
        Texture2D screenImage = new Texture2D(width, height, TextureFormat.RGB24, false);
        screenImage.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenImage.Apply();

        byte[] imageBytes = screenImage.EncodeToPNG();
        Destroy(screenImage);

        // 🔹 4. Faylni saqlash joyini tayyorlash
        string exeFolder = Path.GetDirectoryName(Application.dataPath);
        string reportsFolder = Path.Combine(exeFolder, reportsFolderName);
        if (!Directory.Exists(reportsFolder))
            Directory.CreateDirectory(reportsFolder);

        // 🔹 5. Foydalanuvchi ma’lumotlarini olish
        string ism = "Ism";
        string familiya = "Familiya";
        try
        {
            // Eslatma: GameDataController mavjud emasligi sababli buni e'tiborsiz qoldirish mumkin.
            // Bu qism original kodning bir qismi sifatida saqlanadi.
            if (GameDataController.Instance != null)
            {
                ism = GameDataController.Instance.Ism ?? "Ism";
                familiya = GameDataController.Instance.Familiya ?? "Familiya";
            }
        }
        catch
        {
            Debug.LogWarning("GameDataController ma’lumotlari olinmadi. Default qiymatlar ishlatildi.");
        }

        ism = CleanFileName(ism);
        familiya = CleanFileName(familiya);

        // 🔹 6. Fayl nomini yaratish
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string fileName = $"{familiya}_{ism}_{timestamp}.png";
        string fullPath = Path.Combine(reportsFolder, fileName);

        // 🔹 7. Faylni yozish
        try
        {
            File.WriteAllBytes(fullPath, imageBytes);
            Debug.Log($"Skrinshot saqlandi: {fullPath}");

            // Saqlash manzilini ko'rsatish funksiyasini chaqirish
            StartCoroutine(DisplaySavePath(fullPath));

        }
        catch (Exception ex)
        {
            Debug.LogError("Skrinshotni saqlashda xatolik: " + ex.Message);
            // Agar xato bo'lsa ham, xabar ko'rsatish funksiyasini ishga tushirish mumkin
            StartCoroutine(DisplaySavePath("Skrinshotni saqlashda xatolik yuz berdi!", true));
        }

        // 🔹 8. UI qayta tiklash (kechikish bilan)
        yield return new WaitForSeconds(0.1f);

        if (screenshotButton != null && wasScreenshotBtnActive)
            screenshotButton.gameObject.SetActive(true);

        if (extraButtonToHide != null && wasExtraBtnActive)
            extraButtonToHide.gameObject.SetActive(true);

        Canvas.ForceUpdateCanvases(); // ⚡ UI ni yangilash
    }

    private string CleanFileName(string input)
    {
        foreach (char c in Path.GetInvalidFileNameChars())
            input = input.Replace(c.ToString(), "_");
        return input.Trim();
    }

    // 💡 Yangi funksiya: Saqlash manzilini ko'rsatish
    private IEnumerator DisplaySavePath(string path, bool isError = false)
    {
        if (statusText != null)
        {
            // Ko'rsatiladigan xabarni tayyorlash
            string message;
            if (isError)
            {
                message = path;
                statusText.color = Color.red; // Xato bo'lsa qizil rang
            }
            else
            {
                // To'liq yo'ldan faqat papka nomini olish (qisqa ko'rinish)
                string folderPath = Path.GetDirectoryName(path);
                message = $"Hisobot saqlandi!\nManzil: {folderPath}\nFayl: {Path.GetFileName(path)}";
                statusText.color = Color.white; // Oddiy holatda oq rang (yoki boshqa rang)
            }

            statusText.SetText(message);
            statusText.gameObject.SetActive(true);

            // Ko'rsatish muddati (5 sekund) kutish
            yield return new WaitForSeconds(DisplayDuration);

            // Tekstni yashirish
            statusText.gameObject.SetActive(false);
        }
    }
}