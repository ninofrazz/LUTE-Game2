using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class FlowerPlacer : MonoBehaviour
{
    public ARPlaneManager planeManager; // Reference to the ARPlaneManager
    public float minPlaneWidth = 1.0f; // Minimum required width in meters
    public float minPlaneHeight = 1.0f; // Minimum required height in meters
    public List<GameObject> flowerPrefabs; // List of flower prefabs to choose from
    public float flowerSpacing = 0.5f; // Spacing between flowers in meters

    private ARPlane targetPlane; // The plane where flowers are placed
    private List<GameObject> placedFlowers = new List<GameObject>(); // List to track placed flowers
    private bool flowersPlaced = false; // Flag to track if flowers have been placed

    private void OnEnable()
    {
        // Subscribe to the planesChanged event
        planeManager.planesChanged += OnPlanesChanged;
    }

    private void OnDisable()
    {
        // Unsubscribe from the planesChanged event
        planeManager.planesChanged -= OnPlanesChanged;

        // Clean up placed flowers
        foreach (var flower in placedFlowers)
        {
            if (flower != null)
            {
                Destroy(flower);
            }
        }
        placedFlowers.Clear();
    }

    private void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        // If flowers have already been placed, do nothing
        if (flowersPlaced) return;

        // If no target plane has been selected, find a suitable one
        if (targetPlane == null)
        {
            FindSuitablePlane();
        }
    }

    private void FindSuitablePlane()
    {
        // Iterate through all detected planes
        foreach (var plane in planeManager.trackables)
        {
            // Check if the plane is horizontal (e.g., floor)
            if (plane.alignment == UnityEngine.XR.ARSubsystems.PlaneAlignment.HorizontalUp)
            {
                // Check if the plane is big enough
                if (plane.size.x >= minPlaneWidth && plane.size.y >= minPlaneHeight)
                {
                    Debug.Log("Suitable plane found! Size: " + plane.size);

                    // Set this plane as the target
                    targetPlane = plane;

                    // Place flowers evenly on the plane
                    PlaceFlowersEvenly(targetPlane);

                    // Mark flowers as placed
                    flowersPlaced = true;

                    // Exit the loop after finding a suitable plane
                    break;
                }
                else
                {
                    Debug.Log("Plane is too small. Size: " + plane.size);
                }
            }
        }
    }

    private void PlaceFlowersEvenly(ARPlane plane)
    {
        // Get the plane's center and extents
        Vector3 planeCenter = plane.center;
        Vector3 planeExtents = new Vector3(plane.extents.x, 0, plane.extents.y);

        // Calculate the number of flowers that can fit along the width and length
        int flowersPerRow = Mathf.FloorToInt((planeExtents.x * 2) / flowerSpacing);
        int flowersPerColumn = Mathf.FloorToInt((planeExtents.z * 2) / flowerSpacing);

        Debug.Log($"Plane size: {plane.size}, Flowers per row: {flowersPerRow}, Flowers per column: {flowersPerColumn}");

        // Calculate the starting position (bottom-left corner of the plane)
        Vector3 startPosition = planeCenter - new Vector3(planeExtents.x, 0, planeExtents.z);

        // Place flowers in a grid pattern
        for (int x = 0; x < flowersPerRow; x++)
        {
            for (int z = 0; z < flowersPerColumn; z++)
            {
                // Calculate the position for this flower
                Vector3 flowerPosition = startPosition + new Vector3(
                    x * flowerSpacing,
                    0,
                    z * flowerSpacing
                );

                Debug.Log($"Placing flower at position: {flowerPosition}");

                // Choose a random flower prefab from the list
                if (flowerPrefabs.Count > 0)
                {
                    GameObject randomFlowerPrefab = flowerPrefabs[Random.Range(0, flowerPrefabs.Count)];

                    // Instantiate the flower at the calculated position
                    GameObject flower = Instantiate(randomFlowerPrefab, flowerPosition, Quaternion.identity);

                    // Add the flower to the list of placed flowers
                    placedFlowers.Add(flower);
                }
                else
                {
                    Debug.LogError("No flower prefabs assigned!");
                }
            }
        }

        Debug.Log("Flowers placed evenly on the plane!");
    }
}