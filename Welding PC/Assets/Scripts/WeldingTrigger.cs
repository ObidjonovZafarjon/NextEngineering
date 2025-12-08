using UnityEngine;

public class WeldingTrigger : MonoBehaviour
{
    public Transform weldingCamera;
    public GameObject weldingTool;
    public GameObject playerController; // FirstPersonController ob'ektini bog'lang
    public LineRenderer weldLine;
    public AudioSource audioSource;
    public AudioClip[] speedClips; // Tez, sekin, me'yorida
    public AudioClip[] arcClips;   // Uzun, kalta, me'yorida

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Kamera almashuvi
            Camera.main.transform.position = weldingCamera.position;
            Camera.main.transform.rotation = weldingCamera.rotation;
            weldingTool.SetActive(true);

            // Player harakatini to'xtatish (CharacterController ni o'chirish)
            CharacterController charController = playerController.GetComponent<CharacterController>();
            if (charController != null)
            {
                charController.enabled = false;
            }

            // Agar maxsus harakat skripti bo'lsa, uni ham o'chirish
            MonoBehaviour movementScript = playerController.GetComponent<MonoBehaviour>(); // O'z skriptingizni ko'rsating
            if (movementScript != null)
            {
                movementScript.enabled = false;
            }

            // Kursorni ko'rinadigan qilish va qulaylik
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void Update()
    {
        if (weldingTool.activeSelf)
        {
            if (Input.GetMouseButton(0))
            {
                // Tezlikni tekshirish
                float speed = Input.GetAxis("Mouse X") * 10f;
                PlaySpeedFeedback(speed);

                // Yo'nalish chizig'ini yangilash
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
                weldLine.SetPosition(1, mousePos);

                // Burchakni tekshirish
                float horizontalAngle = weldingTool.transform.eulerAngles.y;
                float verticalAngle = weldingTool.transform.eulerAngles.x;
                Debug.Log($"Horizontal: {horizontalAngle}, Vertical: {verticalAngle}");

                // Yoy uzunligini tekshirish
                float arcLength = Vector3.Distance(weldingTool.transform.position, mousePos);
                PlayArcFeedback(arcLength);
            }
        }
    }

    void PlaySpeedFeedback(float speed)
    {
        if (speed > 5f) audioSource.PlayOneShot(speedClips[0]); // Tez
        else if (speed < 2f) audioSource.PlayOneShot(speedClips[1]); // Sekin
        else audioSource.PlayOneShot(speedClips[2]); // Me'yorida
    }

    void PlayArcFeedback(float length)
    {
        if (length > 0.5f) audioSource.PlayOneShot(arcClips[0]); // Uzun
        else if (length < 0.2f) audioSource.PlayOneShot(arcClips[1]); // Kalta
        else audioSource.PlayOneShot(arcClips[2]); // Me'yorida
    }
}