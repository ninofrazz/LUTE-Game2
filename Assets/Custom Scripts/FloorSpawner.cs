using LoGaCulture.LUTE;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class FloorSpawner : MonoBehaviour
{
    public ObjectInteraction objectInteractionScript;
    public ARPlaneManager planeManager_; // Reference to the ARPlaneManager
    public float minPlaneWidth = 1.0f; // Minimum required width in meters
    public float minPlaneHeight = 1.0f; // Minimum required height in meters
    public float flowerScale;
    public List<GameObject> flowerPrefabs; // List of flower prefabs to choose from
    public float flowerSpacing = 0.5f; // Spacing between flowers in meters
    public int flowerQuantity = 10; // Number of flowers to spawn


    private ARPlane targetPlane; // The plane where flowers are placed
    private List<GameObject> placedFlowers = new List<GameObject>(); // List to track placed flowers
    private bool flowersPlaced = false; // Flag to track if flowers have been placed

    public GameObject Trackables;

    private void OnEnable()
    {
        // Subscribe to the planesChanged event
        planeManager_.planesChanged += OnPlanesChanged;

        Trackables = GameObject.Find("Trackables");
    }

    private void OnDisable()
    {
        // Unsubscribe from the planesChanged event
        planeManager_.planesChanged -= OnPlanesChanged;

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
        foreach (var plane in planeManager_.trackables)
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

                    // Place flowers evenly on the plane, starting from the center
                    PlaceFlowersFromCenter(targetPlane);

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

    private void PlaceFlowersFromCenter(ARPlane plane)
    {
        // Clear existing flowers
        foreach (var flower in placedFlowers)
        {
            if (flower != null)
            {
                Destroy(flower);
            }
        }
        placedFlowers.Clear();

        // Get the plane's center and extents
        Vector3 planeCenter = plane.center;
        Vector3 planeExtents = new Vector3(plane.extents.x, 0, plane.extents.y);

        // Calculate the bounds of the plane
        Vector3 planeMin = planeCenter - planeExtents;
        Vector3 planeMax = planeCenter + planeExtents;

        // Spawn the specified number of flowers
        for (int i = 0; i < flowerQuantity; i++)
        {
            // Calculate a random position within the plane bounds
            Vector3 flowerPosition = new Vector3(
                Random.Range(planeMin.x, planeMax.x),
                planeCenter.y, // Use the plane's Y position
                Random.Range(planeMin.z, planeMax.z)
            );

            Debug.Log($"Placing flower at position: {flowerPosition}");

            // Choose a random flower prefab from the list
            if (flowerPrefabs.Count > 0)
            {
                GameObject randomFlowerPrefab = flowerPrefabs[Random.Range(0, flowerPrefabs.Count)];

                // Instantiate the flower at the calculated position
                GameObject flower = Instantiate(randomFlowerPrefab, flowerPosition, Quaternion.identity);

                // Set the flower's scale
                flower.transform.localScale = new Vector3(flowerScale, flowerScale, flowerScale);

                // Add the flower to the list of placed flowers
                placedFlowers.Add(flower);
            }
            else
            {
                Debug.LogError("No flower prefabs assigned!");
            }
        }
        //FindAnyObjectByType<ARPlaneMeshVisualizer>().enabled = false;
        Trackables.SetActive(false);
        Debug.Log($"Flowers placed randomly on the plane. Total flowers: {flowerQuantity}");
        objectInteractionScript.FlowerSpawned = true;
        FindAnyObjectByType<GlobalVariableComm>().Completed();
    }
}