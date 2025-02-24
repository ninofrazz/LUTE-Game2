using System.Collections;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARSpawnOnStart : MonoBehaviour
{
    public GameObject objectToSpawn;   // Object to spawn
    public GameObject arSessionOrigin;  // Reference to AR Session Origin
    public float spawnDistance = 1f;   // Distance in front of the camera to spawn the object

    private GameObject arCamera;  // Reference to the AR camera



    void Start()
    {
        // Get the AR camera from the ARSessionOrigin's Camera component
        arCamera = arSessionOrigin;

        // Start the coroutine to spawn the object after the delay
        SpawnObjectInFrontOfCamera();
    }



    void SpawnObjectInFrontOfCamera()
    {
        if (arCamera != null && objectToSpawn != null)
        {
            // Calculate the spawn position in front of the camera
            Vector3 spawnPosition = arCamera.transform.position + arCamera.transform.forward * spawnDistance;

            // Instantiate the object at that position with the default rotation
            Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
        }
    }
}
