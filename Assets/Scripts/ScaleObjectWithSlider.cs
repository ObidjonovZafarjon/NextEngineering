using UnityEngine;
using UnityEngine.UI;

public class ScaleObjectWithSlider : MonoBehaviour
{
    public GameObject targetObject;  // Masshtab o'zgartiriladigan obyekt
    public Slider scaleSlider;      // Masshtab o'zgartirish uchun slider

    private Vector3 initialScale;   // Obyektning boshlang'ich masshtabi
    private Vector3 initialPosition; // Obyektning boshlang'ich pozitsiyasi
    private float initialY;         // Yer bilan tutashgan nuqta (pastki nuqta)

    void Start()
    {
        if (targetObject != null)
        {
            // Obyektning boshlang'ich masshtabi va pozitsiyasini saqlash
            initialScale = targetObject.transform.localScale;
            initialPosition = targetObject.transform.position;

            // Pastki nuqtaning Y koordinatasi
            initialY = initialPosition.y - (initialScale.y / 2f);
        }

        if (scaleSlider != null)
        {
            scaleSlider.onValueChanged.AddListener(UpdateScale);
        }
    }

    void UpdateScale(float scaleFactor)
    {
        if (targetObject != null)
        {
            // Yangi masshtabni hisoblash
            Vector3 newScale = initialScale * scaleFactor;
            targetObject.transform.localScale = newScale;

            // Yangi pozitsiyani hisoblash (pastki nuqtani yerda qoldirish)
            float newHeight = newScale.y / 2f;
            Vector3 newPosition = targetObject.transform.position;
            newPosition.y = initialY + newHeight;
            targetObject.transform.position = newPosition;
        }
    }
}
