using UnityEngine;
using UnityEngine.UI; // RawImage uchun kerak

public class ChangeColorOnClick : MonoBehaviour
{
    private RawImage rawImage; // RawImage komponentiga kirish

    void Start()
    {
        // RawImage komponentini olish
        rawImage = GetComponent<RawImage>();
        if (rawImage == null)
        {
            Debug.LogError("RawImage komponenti topilmadi! Iltimos, skriptni RawImage joylashgan ob'ektga o'rnating.");
        }
    }

    public void OnClick()
    {
        if (rawImage != null)
        {
            // Rangi yashilga o'zgartiriladi
            rawImage.color = Color.green;
        }
    }
}
