using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;

public class ObjectInteraction : MonoBehaviour
{
    public bool LevelComplete;
    public bool allActive;
    // Assign materials in the Inspector
    public Camera arCam;         // Assign the AR Camera in the Inspector
    public InputActionReference tap; // Reference to the input action for tap
    public GameObject completeText;
    public GameObject scanText;
    public ARPlaneManager FloorScanner;



    private void Start()
    {
        if (arCam == null)
        {
            Debug.LogWarning("AR Camera is not assigned in the Inspector.");
        }
    }

    private void OnEnable()
    {
        // Enable the InputAction when the script is enabled
        tap.action.Enable();
    }

    private void OnDisable()
    {
        // Disable the InputAction when the script is disabled
        tap.action.Disable();
    }

    void Update()
    {
        // Use 'performed' instead of 'triggered' for reliable input detection
        if (tap.action.triggered) // Triggered works fine here too, but 'performed' might be more reliable
        {
            // Read the touch or mouse position
            Vector2 touchPosition = tap.action.ReadValue<Vector2>();

            // Cast a ray based on the input position
            Ray ray = arCam.ScreenPointToRay(touchPosition);
            RaycastHit hit;
            Debug.DrawRay(ray.origin, ray.direction * 10, Color.red);

            if (Physics.Raycast(ray, out hit) && hit.collider.tag != "Draggable")
            {


                HitChecker hitCheckerScript = hit.collider.GetComponent<HitChecker>();
                if (hitCheckerScript != null)
                {
                    hitCheckerScript.Hit = true;
                    //chnge the color from here
                }
            }
            else
            {
            }
        }





        HitChecker[] allInstances = FindObjectsByType<HitChecker>(FindObjectsSortMode.None);

        allActive = true;
        // Check each instance individually
        foreach (var instance in allInstances)
        {
            if (!instance.Hit)
            {
                allActive = false;
                break; // No need to check further if one is false
            }
        }

        if (allActive)
        {
            FloorScanner.enabled = true;
            scanText.SetActive(true);

        }
        else
        {
            //completeText.gameObject.SetActive(false);
        }

        if (LevelComplete)
        {

            scanText.SetActive(false);
            completeText.SetActive(true);
            FindAnyObjectByType<ARPlaneMeshVisualizer>().enabled = false;
        }

    }

}

