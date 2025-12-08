using UnityEngine;
using UnityEngine.SceneManagement;

public class CubeClickLoader : MonoBehaviour
{
    public string sceneToLoad;
    public GameObject cursor;

    private bool isPlayerInTrigger = false;

    private void Start()
    {
        cursor.SetActive(false);
    }
    private void Update()
    {
        if (isPlayerInTrigger && Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene(sceneToLoad);
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