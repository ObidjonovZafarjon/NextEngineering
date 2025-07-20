public class DraggablePart : MonoBehaviour
{
    private Vector3 offset;
    private bool isDragging = false;
    public Transform correctPlace;
    public float snapDistance = 0.5f;
    public bool isPlaced = false;
    public int attempts = 0; 

    private ControlSystem controlSystem;

    void Start()
    {
        controlSystem = FindObjectOfType<ControlSystem>();
    }

    void OnMouseDown()
    {
        if (isPlaced) return;
        offset = transform.position - GetMouseWorldPos();
        isDragging = true;
    }

    void OnMouseDrag()
    {
        if (isDragging && !isPlaced)
        {
            transform.position = GetMouseWorldPos() + offset;
        }
    }

    void OnMouseUp()
    {
        if (isPlaced) return;

        float distance = Vector3.Distance(transform.position, correctPlace.position);

        if (distance <= snapDistance)
        {
            transform.position = correctPlace.position;
            isPlaced = true;
            controlSystem.PlayCorrectSound();
        }
        else
        {
            attempts++;
            controlSystem.PlayWrongSound();
        }

        isDragging = false;
    }

    Vector3 GetMouseWorldPos()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }
}
