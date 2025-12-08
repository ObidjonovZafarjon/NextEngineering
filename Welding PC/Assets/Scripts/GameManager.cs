using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("GameObjects")]
    public List<GameObject> Meshs;
    public List<GameObject> WeldTarget;
    public GameObject Sphere;
    public GameObject model;
    public GameObject Gun;
    public GameObject Spatula;
    public GameObject Spray;
    public GameObject SpheresPar;
    public GameObject SprayingEffec;

    [Header("Game Settings")]
    public Transform FirsteGunPos;
    public GameObject SprayEffect;
    public GameObject GunLight;

    [Header("Ui")]
    public GameObject FinishButton;
    public GameObject NextStepButton;
    public GameObject Colors;
    public Text RewaredText;
    public Text DiamoundText;
    public GameObject getDimonds;

    ////////////////////////////////////////SELECT ADS NETWORK HERE///////////////////////////////////////////////
    
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////

    int sphersAmount = 0;
    private bool Spawn = true;
    private bool Spawning = false;
    private GameObject spheresFinal;
    private bool toNextStep = false;
    private bool toColoring = false;
    private bool finish = false;
    private Color PaintColor = Color.red;
    private int RewaredCount;
    private int DiamounIndex = 0;

    void Awake()
    {
        Texture2D texture = new Texture2D(1, 1);
        //model.transform.GetChild(0).GetComponent<Renderer>().material.mainTexture = texture;
        for (int i = 0; i < Meshs.Count; i++)
        {
            Meshs[i].GetComponent<Renderer>().material.mainTexture = texture;
        }

        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                Color color = Color.gray;
                texture.SetPixel(x, y, color);
            }
        }
        texture.Apply();


        RewaredCount = Random.Range(50, 100);
        RewaredText.text = "REWARED : " + RewaredCount;
        Spray.gameObject.transform.GetChild(3).GetComponent<MeshRenderer>().material.color = PaintColor;
        ParticleSystem.MainModule settings = SprayingEffec.GetComponent<ParticleSystem>().main;
        PaintColor.a = .2f;
        settings.startColor = PaintColor;

    }

    void Update()
    {
        DiamounIndex = PlayerPrefs.GetInt("dimound");
        DiamoundText.text = DiamounIndex.ToString();
        Vector3 pos = new Vector3();
        int Mask = 1 << 8;
        int spheresMask = 1 << 9;

        if (Input.GetMouseButton(0))
        {
            Vector3 offset = new Vector3(Input.mousePosition.x, Input.mousePosition.y + 200f, transform.position.z);
            Ray ray = Camera.main.ScreenPointToRay(offset);
            Vector3 direction = Vector3.forward;
            RaycastHit hitinfo;
            if (Physics.Raycast(ray, out hitinfo))
            {
                Gun.transform.position = new Vector3(hitinfo.point.x, hitinfo.point.y, hitinfo.point.z);
                Spatula.transform.position = new Vector3(hitinfo.point.x, hitinfo.point.y, hitinfo.point.z - 0.5f);
                Spray.transform.position = new Vector3(hitinfo.point.x, hitinfo.point.y, hitinfo.point.z - 2f);
            }

            StartCoroutine(Waiting(1.25f));

            if (Physics.Raycast(ray, out hitinfo, Mathf.Infinity, Mask) && Spawn && Spawning && !toNextStep)
            {
                if (hitinfo.transform.tag != "Bodey")
                {
                    pos = new Vector3(hitinfo.point.x, hitinfo.point.y, hitinfo.point.z);
                    spheresFinal = Instantiate(Sphere, pos, Quaternion.identity);
                    spheresFinal.transform.SetParent(SpheresPar.transform);
                    sphersAmount += 1;
                    Spawn = false;
                    GunLight.SetActive(true);
                    if (sphersAmount >= 50)
                    {
                        NextStepButton.SetActive(true);
                    }
                    StartCoroutine(Delay(1.25f * Time.deltaTime));
                }
            }
            else if (Physics.Raycast(ray, out hitinfo, Mathf.Infinity, spheresMask) && toNextStep)
            {
                hitinfo.transform.SetParent(gameObject.transform);
                hitinfo.transform.gameObject.GetComponent<Rigidbody>().isKinematic = false;
                Destroy(hitinfo.transform.gameObject, 1f);
            }
            else if (Physics.SphereCast(Spray.transform.position, 2f, Vector3.forward, out hitinfo, spheresMask) && toColoring)
            {
                Renderer rend = hitinfo.transform.GetComponent<Renderer>();
                MeshCollider meshCollider = hitinfo.collider as MeshCollider;

                if (rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null || meshCollider == null)
                    return;

                Texture2D tex = rend.material.mainTexture as Texture2D;
                Vector2 pixelUV = hitinfo.textureCoord;
                pixelUV.x *= tex.width;
                pixelUV.y *= tex.height;

                tex.SetPixel((int)pixelUV.x, (int)pixelUV.y, PaintColor);
                tex.Apply();
            }


            SprayEffect.SetActive(true);
        }
        else
        {
            Spawning = false;
            Spawn = true;
            SprayEffect.SetActive(false);
            StopAllCoroutines();
        }

        if (Input.GetMouseButtonUp(0))
        {
            Gun.transform.position = new Vector3(FirsteGunPos.position.x, Gun.transform.position.y, FirsteGunPos.position.z);
            Spawning = false;
            Spawn = true;
            StopAllCoroutines();

        }

        if (toNextStep)
        {
            int SphersCount = GameObject.FindGameObjectsWithTag("Spheres").Length;
            if (SphersCount == 0 && !finish)
            {
                toColoring = true;
                Spatula.SetActive(false);
                Spray.SetActive(true);
                Colors.SetActive(true);
                StartCoroutine(DisplayFinishButton());
            }
        }

        bool wr = false;
    

    }


    public void PaintingColor(Button btn)
    {
        PaintColor = btn.gameObject.transform.GetChild(0).gameObject.GetComponent<Image>().color;
        ParticleSystem.MainModule settings = SprayingEffec.GetComponent<ParticleSystem>().main;
        PaintColor.a = .2f;
        settings.startColor = PaintColor;
        Spray.transform.GetChild(3).gameObject.GetComponent<MeshRenderer>().material.color = PaintColor;
        PaintColor.a = 1;

    }

    IEnumerator Waiting(float wait)
    {
        yield return new WaitForSeconds(wait);
        Spawning = true;
    }

    IEnumerator Delay(float wait)
    {
        yield return new WaitForSeconds(wait);
        Spawn = true;
        GunLight.SetActive(false);
    }

    IEnumerator DisplayFinishButton()
    {
        yield return new WaitForSeconds(2f);
        FinishButton.SetActive(true);
        finish = true;
    }

    public void NextStep()
    {
        toNextStep = true;

        for (int i = 0; i < WeldTarget.Count; i++)
        {
            WeldTarget[i].GetComponent<MeshRenderer>().material.color = Color.gray;
            WeldTarget[i].GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
        }
        for (int i = 0; i < SpheresPar.transform.childCount; i++)
        {
            SpheresPar.transform.GetChild(i).gameObject.GetComponent<SphereCollider>().enabled = true;
        }
    }

    
    

    public void nextLevel()
    {
        DiamounIndex += RewaredCount;
        PlayerPrefs.SetInt("dimound", DiamounIndex);
    }
}
