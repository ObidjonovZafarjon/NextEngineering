using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelsRotation : MonoBehaviour
{
    public float RotationSpeed;
    public float xRotation = 0;

    void Start()
    {
        
    }

    void Update()
    {
        transform.Rotate(xRotation * Time.deltaTime, RotationSpeed * Time.deltaTime, 0f);
    }
}
