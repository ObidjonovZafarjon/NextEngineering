using UnityEngine;
using TMPro;

public class TextureChangerTMP : MonoBehaviour
{
    public GameObject[] targetObjects; // Tekstura qo'llanadigan obyektlar
    public TMP_Dropdown textureDropdown; // TMP_Dropdown menyusi
    public Texture[] textures;          // 4 ta tekstura

    void Start()
    {
        // TMP_Dropdown tanlanganida funksiyani chaqirish
        if (textureDropdown != null)
        {
            textureDropdown.onValueChanged.AddListener(ChangeTexture);
        }
    }

    void ChangeTexture(int index)
    {
        // Tanlangan tekstura mavjudligini tekshirish
        if (index < textures.Length)
        {
            foreach (GameObject obj in targetObjects)
            {
                if (obj != null)
                {
                    // Obyektning Renderer komponentini olish
                    Renderer renderer = obj.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        Material mat = renderer.material;

                        // Tanlangan teksturani qo'llash
                        mat.mainTexture = textures[index];
                    }
                }
            }
        }
    }
}
