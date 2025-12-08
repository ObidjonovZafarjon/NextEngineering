using UnityEngine;

public class WearForm : MonoBehaviour
{
    public GameObject cursor;
    public GameObject UserPhoto;
    public GameObject ProtectedPhoto;
    public GameObject Uniform;

    private bool isPlayerInTrigger = false;
    bool objectState = GameDataController.Instance.targetObjectState;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cursor.SetActive(false);
        ProtectedPhoto.SetActive(false);
        UserPhoto.SetActive(true);
        if(objectState==false)
        {
            ProtectedPhoto.SetActive(true);
            UserPhoto.SetActive(false);
            Uniform.SetActive(false);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (isPlayerInTrigger && Input.GetMouseButtonDown(0))
        {
            ProtectedPhoto.SetActive(true);
            UserPhoto.SetActive(false);
            Uniform.SetActive(false);
            GameDataController.Instance.targetObjectState = false;
        }
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
