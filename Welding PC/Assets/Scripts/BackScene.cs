using UnityEngine;
using UnityEngine.SceneManagement;


public class BackScene : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Update is called once per frame
    public void BackToVirtual()
    {
        SceneManager.LoadScene("MainScene");
    }
}
