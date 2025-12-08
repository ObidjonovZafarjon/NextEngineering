using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildingController : MonoBehaviour
{
    public Camera playerCamera;
    public GameObject nextCube;
    public GameObject prevCube;
    public GameObject modelCube1;
    public GameObject modelCube2;
    public GameObject modelObject1;
    public GameObject modelObject2;
    public GameObject[] buildingParts;
    public GameObject mainBuilding;
    public float interactionDistance = 10f;
    public float animationDuration = 2f;
    public float partsDisappearanceDuration = 1f;

    private int currentStage = 0;
    private bool isAnimating = false;
    private Dictionary<GameObject, Vector3> initialPositions = new Dictionary<GameObject, Vector3>();
    private bool isModelVisible = false;

    void Start()
    {
        modelObject1.SetActive(false);
        modelObject2.SetActive(false);
        mainBuilding.SetActive(false);

        foreach (var part in buildingParts)
        {
            part.SetActive(false);
            initialPositions[part] = part.transform.position;
        }
    }

    void Update()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            if (hit.collider.gameObject == nextCube)
            {
                HighlightCube(nextCube);
                if (Input.GetMouseButtonDown(0) && !isAnimating)
                    NextStage();
            }
            else if (hit.collider.gameObject == prevCube)
            {
                HighlightCube(prevCube);
                if (Input.GetMouseButtonDown(0) && !isAnimating)
                    PrevStage();
            }
            else if (hit.collider.gameObject == modelCube1)
            {
                HighlightCube(modelCube1);
                if (Input.GetMouseButtonDown(0))
                    ShowModel(modelObject1);
            }
            else if (hit.collider.gameObject == modelCube2)
            {
                HighlightCube(modelCube2);
                if (Input.GetMouseButtonDown(0))
                    ShowModel(modelObject2);
            }
            else
            {
                ResetCubeColors();
            }
        }
        else
        {
            ResetCubeColors();
        }
    }

    void HighlightCube(GameObject cube)
    {
        ResetCubeColors();
        Renderer renderer = cube.GetComponent<Renderer>();
        if (renderer != null && renderer.material.HasProperty("_BaseColor"))
        {
            renderer.material.SetColor("_BaseColor", Color.red);
        }
    }

    void ResetCubeColors()
    {
        GameObject[] cubes = { nextCube, prevCube, modelCube1, modelCube2 };
        foreach (var cube in cubes)
        {
            if (cube != null)
            {
                Renderer renderer = cube.GetComponent<Renderer>();
                if (renderer != null && renderer.material.HasProperty("_BaseColor"))
                {
                    renderer.material.SetColor("_BaseColor", Color.white);
                }
            }
        }
    }

    void ShowModel(GameObject model)
    {
        isModelVisible = true;
        modelObject1.SetActive(false);
        modelObject2.SetActive(false);
        model.SetActive(true);

        foreach (var part in buildingParts)
        {
            part.SetActive(false);
        }
        mainBuilding.SetActive(false);
        currentStage = 0;
    }

    public void NextStage()
    {
        if (isModelVisible)
        {
            modelObject1.SetActive(false);
            modelObject2.SetActive(false);
            isModelVisible = false;
        }

        if (currentStage < buildingParts.Length && !isAnimating)
        {
            StartCoroutine(RaisePart(buildingParts[currentStage]));
            currentStage++;
        }
        else if (currentStage == buildingParts.Length && !isAnimating)
        {
            StartCoroutine(ActivateMainBuilding());
            currentStage++;
        }
    }

    public void PrevStage()
    {
        if (currentStage > 0 && currentStage <= buildingParts.Length && !isAnimating)
        {
            currentStage--;
            StartCoroutine(LowerPartQuickly(buildingParts[currentStage]));
        }
    }

    IEnumerator RaisePart(GameObject part)
    {
        isAnimating = true;
        part.SetActive(true);

        Vector3 startPosition = initialPositions[part] - new Vector3(0, 5, 0);
        Vector3 endPosition = initialPositions[part];
        part.transform.position = startPosition;

        float elapsed = 0;
        while (elapsed < animationDuration)
        {
            part.transform.position = Vector3.Lerp(startPosition, endPosition, elapsed / animationDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        part.transform.position = endPosition;
        isAnimating = false;
    }

    IEnumerator LowerPartQuickly(GameObject part)
    {
        isAnimating = true;
        Vector3 startPosition = part.transform.position;
        Vector3 endPosition = initialPositions[part] - new Vector3(0, 5, 0);

        float elapsed = 0;
        while (elapsed < partsDisappearanceDuration)
        {
            part.transform.position = Vector3.Lerp(startPosition, endPosition, elapsed / partsDisappearanceDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        part.SetActive(false);
        isAnimating = false;
    }

    IEnumerator ActivateMainBuilding()
    {
        isAnimating = true;

        foreach (GameObject part in buildingParts)
        {
            part.SetActive(false);
        }
        yield return new WaitForSeconds(partsDisappearanceDuration);

        Vector3 startPosition = mainBuilding.transform.position - new Vector3(0, 5, 0);
        Vector3 endPosition = mainBuilding.transform.position;
        mainBuilding.transform.position = startPosition;
        mainBuilding.SetActive(true);

        float elapsed = 0;
        while (elapsed < animationDuration)
        {
            mainBuilding.transform.position = Vector3.Lerp(startPosition, endPosition, elapsed / animationDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        mainBuilding.transform.position = endPosition;
        isAnimating = false;
    }
}
