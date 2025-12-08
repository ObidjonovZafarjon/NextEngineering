using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpheresConroller : MonoBehaviour
{
    public Color Gray;

    void Start()
    {
        StartCoroutine(waitToChangeColor());
    }

    void Update()
    {
        
    }

    IEnumerator waitToChangeColor()
    {
        yield return new WaitForSeconds(0.5f);
        GetComponent<MeshRenderer>().material.color = Gray;
        GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
    }
}
