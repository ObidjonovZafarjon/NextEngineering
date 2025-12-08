using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;

public class LeaderboardManager : MonoBehaviour
{
    [Header("UI Sozlamalar")]
    public Transform contentPanel;
    public GameObject resultItemPrefab;

    [Header("Ranglar")]
    public Color passColor = new Color(0, 1, 0, 1);
    public Color failColor = new Color(1, 0, 0, 1);

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
        LoadAndDisplayResults();
    }

    void LoadAndDisplayResults()
    {
        foreach (Transform child in contentPanel)
            Destroy(child.gameObject);

        string path = Application.dataPath + "/Resources/users.json";
        if (!File.Exists(path))
        {
            Debug.LogWarning("users.json topilmadi! Natijalar yo‘q.");
            return;
        }

        string json = File.ReadAllText(path);
        UsersWrapper wrapper = new UsersWrapper();

        try
        {
            wrapper = JsonUtility.FromJson<UsersWrapper>(json);
            if (wrapper == null || wrapper.users == null || wrapper.users.Count == 0)
            {
                Debug.Log("Natijalar bo‘sh.");
                return;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("users.json o‘qishda xato: " + e.Message);
            return;
        }

        wrapper.users.Sort((a, b) => b.score.CompareTo(a.score));

        foreach (var user in wrapper.users)
        {
            CreateResultItem(user);
        }

        Debug.Log($"<color=cyan>{wrapper.users.Count} ta natija yuklandi!</color>");
    }

    void CreateResultItem(UserResult user)
    {
        GameObject item = Instantiate(resultItemPrefab, contentPanel);
        TMP_Text[] texts = item.GetComponentsInChildren<TMP_Text>();

        if (texts.Length < 3)
        {
            Debug.LogError("Prefabda 3 ta TMP_Text bo‘lishi kerak!");
            return;
        }

        // ISHGA TUSHGAN QATOR: Agar ism yoki familiya bo‘sh bo‘lsa → "Foydalanuvchi"
        string fullName = "Foydalanuvchi";

        if (!string.IsNullOrEmpty(user.firstName))
        {
            fullName = user.firstName.Trim();
            if (!string.IsNullOrEmpty(user.lastName))
            {
                fullName += " " + user.lastName.Trim();
            }
        }
        else if (!string.IsNullOrEmpty(user.lastName))
        {
            fullName = user.lastName.Trim();
        }

        texts[0].text = fullName;

        // Ball + rang
        texts[1].text = $"{user.score:F1}%";
        texts[1].color = user.score >= 90f ? passColor : failColor;

        // Sana + Vaqt
        texts[2].text = $"{user.date} | {user.time}";
    }

#if UNITY_EDITOR
    private float lastCheckTime = 0f;
    void Update()
    {
        if (Time.time - lastCheckTime > 2f)
        {
            lastCheckTime = Time.time;
            string path = Application.dataPath + "/Resources/users.json";
            if (File.Exists(path))
            {
                var info = new FileInfo(path);
                if (info.LastWriteTime != System.DateTime.MinValue)
                {
                    LoadAndDisplayResults();
                }
            }
        }
    }
#endif
}