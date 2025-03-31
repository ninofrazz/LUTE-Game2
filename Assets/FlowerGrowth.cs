using UnityEngine;

public class FlowerGrowth : MonoBehaviour
{
    [Header("Growth Settings")]
    [Range(0, 1)] public float growthProgress = 0.0f; // Slider for growth progress
    public float maxFlowerScale = 2.0f; // Maximum scale for flowers

    [Header("Material References")]
    public Material flowerMaterial; // Assign the material using the shader


    private void Start()
    {
        growthProgress = 0.0f;
    }
    void Update()
    {
        // Clamp the growth progress between 0 and 1
        growthProgress = Mathf.Clamp01(growthProgress);

        // Update the material's growth mask property
        flowerMaterial.SetFloat("_GrowthMask", growthProgress);

        // Calculate flower scale based on growth progress
        float flowerScale = Mathf.Lerp(1.0f, maxFlowerScale, growthProgress); // Scale flowers from 1.0 to maxFlowerScale

        // Update the material's flower scale property
        flowerMaterial.SetFloat("_GrowthScale", flowerScale);
    }
}