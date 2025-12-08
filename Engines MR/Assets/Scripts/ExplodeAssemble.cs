using UnityEngine;

public class ExplodeAssemble : MonoBehaviour
{
    private Vector3[] originalPositions; // Boshlang'ich pozitsiyalarni saqlash uchun
    private Transform[] parts;           // Modelning barcha qismlarini saqlash uchun
    private bool isExploded = false;     // Model ajralgan yoki ajralmaganligini tekshirish
    private float explodeDistance = 1f;  // Qismlar qanchalik uzoqlashishini belgilaydi
    private float speed = 5f;            // Harakat tezligi

    void Start()
    {
        // Modelning barcha qismlarini olish
        parts = GetComponentsInChildren<Transform>();
        originalPositions = new Vector3[parts.Length];

        // Har bir qismning boshlang'ich pozitsiyasini saqlash
        for (int i = 0; i < parts.Length; i++)
        {
            originalPositions[i] = parts[i].localPosition;
        }

        // O'zini (root ob'ektni) qismlar ro'yxatidan chiqarib tashlash
        parts = System.Array.FindAll(parts, p => p != transform);
    }



    public void ToggleExplode()
    {
        if (!isExploded)
        {
            Explode();
        }
        else
        {
            Assemble();
        }
        isExploded = !isExploded;
    }

    void Explode()
    {
        foreach (Transform part in parts)
        {
            // Har bir qismni markazdan tashqariga yo'naltirish
            Vector3 direction = (part.localPosition - Vector3.zero).normalized;
            Vector3 targetPosition = part.localPosition + direction * explodeDistance;

            // Silliq harakat uchun coroutine ishlatish mumkin
            StartCoroutine(MovePart(part, targetPosition));
        }
    }

    void Assemble()
    {
        for (int i = 0; i < parts.Length; i++)
        {
            // Har bir qismni boshlang'ich pozitsiyasiga qaytarish
            StartCoroutine(MovePart(parts[i], originalPositions[i]));
        }
    }

    System.Collections.IEnumerator MovePart(Transform part, Vector3 target)
    {
        while (Vector3.Distance(part.localPosition, target) > 0.01f)
        {
            part.localPosition = Vector3.Lerp(part.localPosition, target, Time.deltaTime * speed);
            yield return null;
        }
        part.localPosition = target;
    }

    // Mixed Reality uchun qo'shimcha konfiguratsiya
    void OnValidate()
    {
        if (explodeDistance < 0) explodeDistance = 0;
        if (speed < 0) speed = 0;
    }
}