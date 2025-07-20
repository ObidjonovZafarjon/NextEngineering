using UnityEngine;
using UnityEngine.UI;

public class WallMaterialChanger : MonoBehaviour
{
    public Renderer wallRenderer; // Devorning Renderer komponenti
    public Material[] wallMaterials; // Turli teksturalar uchun materiallar (Inspector'da biriktiriladi)
    public Dropdown materialDropdown; // Dropdown element

    void Start()
    {
        // Dropdown tanlashda funksiyani qo'shish
        materialDropdown.onValueChanged.AddListener(ChangeMaterial);
    }

    // Materialni o'zgartirish funksiyasi
    public void ChangeMaterial(int index)
    {
        if (index >= 0 && index < wallMaterials.Length)
        {
            wallRenderer.material = wallMaterials[index];
        }
    }
}
