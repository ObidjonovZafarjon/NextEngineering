using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class ConstructionManagerWithAnimation : MonoBehaviour
{
    public GameObject[] constructionStages; // Bosqich obyektlari
    public float animationDuration = 2.0f; // Obyekt ko'tarilish vaqti
    public float liftHeight = 5.0f; // Qancha masofaga ko'tariladi
    
    public GameObject HidroSystem;
    public GameObject ElectroSystem;
    public GameObject BuildFloor;

    private int currentStage = -1;
    private bool isAnimating = false;

    public RectTransform panel1, panel2;
    public float duration = 0.5f;
    private Vector2 hiddenPosition1, hiddenPosition2;
    private Vector2 visiblePosition1, visiblePosition2;
    private bool isVisible1 = false, isVisible2 = false;

    // Next tugmasi bosilganda chaqiriladi
    private void Start()
    {
        visiblePosition1 = panel1.anchoredPosition;
        hiddenPosition1 = new Vector2(Screen.width, panel1.anchoredPosition.y);
        panel1.anchoredPosition = hiddenPosition1;

        visiblePosition2 = panel2.anchoredPosition;
        hiddenPosition2 = new Vector2(Screen.width, panel2.anchoredPosition.y);
        panel2.anchoredPosition = hiddenPosition2;
    }
    public void TogglePanel1()
    {
        StopAllCoroutines();
        StartCoroutine(SlidePanel1(isVisible1 ? hiddenPosition1 : visiblePosition1));
        isVisible1 = !isVisible1;
    }

    public void TogglePanel2()
    {
        StopAllCoroutines();
        StartCoroutine(SlidePanel2(isVisible2 ? hiddenPosition2 : visiblePosition2));
        isVisible2 = !isVisible2;
    }

    IEnumerator SlidePanel1(Vector2 targetPosition)
    {
        float elapsedTime = 0;
        Vector2 startingPos = panel1.anchoredPosition;

        while (elapsedTime < duration)
        {
            panel1.anchoredPosition = Vector2.Lerp(startingPos, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        panel1.anchoredPosition = targetPosition;
    }

    IEnumerator SlidePanel2(Vector2 targetPosition)
    {
        float elapsedTime = 0;
        Vector2 startingPos = panel2.anchoredPosition;

        while (elapsedTime < duration)
        {
            panel2.anchoredPosition = Vector2.Lerp(startingPos, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        panel2.anchoredPosition = targetPosition;
    }


    public void NextStage()
    {
        if (!isAnimating && currentStage < constructionStages.Length - 1)
        {
            currentStage++;
            StartCoroutine(AnimateLift(constructionStages[currentStage]));
        }
        HidroSystem.SetActive(false);
        ElectroSystem.SetActive(false);
    }

    // Prev tugmasi bosilganda chaqiriladi
    public void PrevStage()
    {
        if (!isAnimating && currentStage > 0)
        {
            StartCoroutine(AnimateLift(constructionStages[currentStage], reverse: true));
            currentStage--;
        }
        HidroSystem.SetActive(false);
        ElectroSystem.SetActive(false);
    }

    public void HidroControl()
    {
        HidroSystem.SetActive(true);
        ElectroSystem.SetActive(false);
        foreach (GameObject stage in constructionStages)
        {
            if (stage != null) // Ob'ekt null emasligini tekshirish
            {
                stage.SetActive(false);
            }
        }
    }

    public void ElectroControl()
    {
        ElectroSystem.SetActive(true);
        HidroSystem.SetActive(false);
        foreach (GameObject stage in constructionStages)
        {
            if (stage != null) // Ob'ekt null emasligini tekshirish
            {
                stage.SetActive(false);
            }
        }
    }

    public void BuildStart()
    {
        BuildFloor.SetActive(true);
    }



    // Ko'tarilishni animatsiya qilish
    private System.Collections.IEnumerator AnimateLift(GameObject stage, bool reverse = false)
    {
        isAnimating = true;
        stage.SetActive(true);

        Vector3 startPosition = stage.transform.position;
        Vector3 targetPosition = startPosition + (reverse ? Vector3.down : Vector3.up) * liftHeight;

        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / animationDuration;
            stage.transform.position = Vector3.Lerp(startPosition, targetPosition, progress);
            yield return null;
        }

        stage.transform.position = targetPosition;

        if (reverse)
        {
            stage.SetActive(false);
        }

        isAnimating = false;
    }
}
