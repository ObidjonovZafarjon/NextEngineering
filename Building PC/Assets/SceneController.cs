using UnityEngine;

public class SceneController : MonoBehaviour
{
    public GameObject tipsPanel; // 🎛️ Inspector orqali biriktiring
    private bool tipsVisible = true; // ✅ Tips holatini kuzatish

    void Start()
    {
        ShowTips(); // 💡 Dastur boshida tips ko‘rsatiladi
    }

    void Update()
    {
        // ⌨️ Space yoki F1 bosilganda tips yashiriladi
        if (tipsVisible && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.F1)))
        {
            HideTips();
        }
        // ❓ F1 bosilganda tips qayta ko‘rsatiladi
        else if (!tipsVisible && Input.GetKeyDown(KeyCode.F1))
        {
            ShowTips();
        }
    }

    void ShowTips()
    {
        tipsPanel.SetActive(true);
        tipsVisible = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked; // 🎯 Sichqonchani bloklash
    }

    void HideTips()
    {
        tipsPanel.SetActive(false);
        tipsVisible = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked; // 🎮 3D boshqaruv uchun tayyor
    }
}
