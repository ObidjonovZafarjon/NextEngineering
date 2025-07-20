using UnityEngine;

public class WallControl : MonoBehaviour
{
    public GameObject FirstWall;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void FirstWallC()
    {
        FirstWall.SetActive(true);
        Debug.Log("Clicked");
    }

}
