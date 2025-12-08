using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class StartWelding : MonoBehaviour
{
    public GameObject cursor;
    private bool isPlayerInTrigger = false;
    public GameObject StartPanel;
    public MonoBehaviour firstPersonController;
    public GameObject ProtectedPhoto;
    public GameObject UserPhoto;
    public TMP_Text UserInfo;



    public GameObject WallForStart;
    private void Start()
    {
        cursor.SetActive(false);
        StartPanel.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        bool objectState = GameDataController.Instance.targetObjectState;
        float testResult = GameDataController.Instance.testScore;

        UserInfo.text = GameDataController.Instance.Ism + " " + GameDataController.Instance.Familiya;

        if (objectState == false && testResult >= 90)
        {
            WallForStart.SetActive(false);
        }
       
    }
    private void Update()
    {
        if (isPlayerInTrigger && Input.GetMouseButtonDown(0))
        {
            OpenPanel();
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            WallForStart.SetActive(false);
        }
    }

    private void OpenPanel()
    {
        StartPanel.SetActive(true);

        // Show mouse
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Freeze movement
        if (firstPersonController != null)
            firstPersonController.enabled = false;
    }
    public void ClosePanel()
    {
        StartPanel.SetActive(false);

        // Hide mouse
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Enable movement
        if (firstPersonController != null)
            firstPersonController.enabled = true;
    }

    public void OpenStarter()
    {
        SceneManager.LoadScene("1");
    }

    public void OpenStandart()
    {
        SceneManager.LoadScene("WeldingScene");
    }

    public void OpenPro()
    {
        SceneManager.LoadScene("2_CanvasWebViewDemo");
    }
    public void OpenNet()
    {
        SceneManager.LoadScene("NetlifyCanvasWebViewDemo");
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            cursor.SetActive(true);
            isPlayerInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            cursor.SetActive(false);
            isPlayerInTrigger = false;
        }
    }
}
