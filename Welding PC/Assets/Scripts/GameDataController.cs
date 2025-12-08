using UnityEngine;

public class GameDataController : MonoBehaviour
{
    // Singleton pattern'ni amalga oshirish
    public static GameDataController Instance;

    // GameObject holatini saqlash uchun o'zgaruvchi
    public bool targetObjectState = true;
    public string Ism="Foydalanuvchi";
    public string Familiya;


    // Test natijasini saqlash uchun o'zgaruvchi
    public float testScore = 0;

    void Awake()
    {
        // Agar Instance mavjud bo'lmasa, uni o'zgartiramiz
        if (Instance == null)
        {
            Instance = this;
            // Sahnadan sahnaga o'tishda bu GameObject o'chirilmasligini ta'minlaymiz
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Agar boshqa Instance mavjud bo'lsa, bu yangi GameObjectni yo'q qilamiz
            Destroy(gameObject);
        }
    }

}