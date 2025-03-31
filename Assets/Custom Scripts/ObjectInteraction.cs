using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;

public class ObjectInteraction : MonoBehaviour
{
    public bool FlowerSpawned;
    public bool allActive;
    public Camera arCam; // Assign the AR Camera in the Inspector
    public InputActionReference tap; // Reference to the input action for tap
    public GameObject completeText;
    public GameObject scanText;
    public GameObject goBackButton;
    public GameObject FlowerButton;
    public GameObject SolvePuzzle;
    public ARPlaneManager FloorScanner;
    public FlowerGrowth flowerGrowth;
    public DragAndDropReceiver[] dragAndDropReceivers;

    private bool _uiToggled; // Track if the UI has already been toggled
    private float _currentCompletionProgress = 0f; // Current interpolated progress
    private float _targetCompletionProgress = 0f; // Target progress value
    private float _smoothingSpeed = 5f; // Speed of interpolation (adjust as needed)
    private bool FlowerSpawnToggled;
    private bool solvePuzzleActivated = false; // Track if SolvePuzzle has been activated

    public HitChecker[] testAllInstances;
    private void Start()
    {
        if (arCam == null)
        {
            Debug.LogWarning("AR Camera is not assigned in the Inspector.");
        }

        flowerGrowth = FindAnyObjectByType<FlowerGrowth>();

        // Initialize UI elements
        completeText.SetActive(false);
        goBackButton.SetActive(false);
        FlowerButton.SetActive(false);
        _uiToggled = false; // Reset the toggle flag



        FloorScanner.enabled = true;
        scanText.SetActive(true);

        StartCoroutine(DelayedStart(0.01f));
    }

    IEnumerator DelayedStart(float sec)
    {
        yield return new WaitForSeconds(sec);

        testAllInstances = FindObjectsByType<HitChecker>(FindObjectsSortMode.None);


        foreach (var instance in testAllInstances)
        {

            instance.GetComponent<Collider>().enabled = false;
            instance.particlesystems[0].SetActive(false);
        }

        dragAndDropReceivers = FindObjectsByType<DragAndDropReceiver>(FindObjectsSortMode.None);

        if (dragAndDropReceivers != null)
        {
            foreach (var receiver in dragAndDropReceivers)
            {
                receiver.gameObject.SetActive(false);

            }
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
        // Handle tap input
        if (tap.action.triggered)
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
                    // Change the color from here
                }
            }
        }

        // Get all HitChecker instances
        HitChecker[] allInstances = FindObjectsByType<HitChecker>(FindObjectsSortMode.None);

        int totalInstances = allInstances.Length; // Total number of instances
        int activeInstances = 0; // Counter for active instances

        allActive = true;

        // Check each instance individually
        foreach (var instance in allInstances)
        {
            if (instance.Hit)
            {
                activeInstances++; // Increment the active instances counter
            }
            else
            {
                allActive = false; // Set allActive to false if any instance is not hit

                if (FlowerSpawned)
                {
                    SolvePuzzle.SetActive(true);
                }
            }
        }

        // Calculate the target completion progress (activeInstances / totalInstances)
        _targetCompletionProgress = (float)activeInstances / totalInstances;

        // Smoothly interpolate the current progress towards the target progress
        _currentCompletionProgress = Mathf.Lerp(_currentCompletionProgress, _targetCompletionProgress, Time.deltaTime * _smoothingSpeed);

        // Update the FlowerGrowth script with the interpolated progress
        if (flowerGrowth != null)
        {
            flowerGrowth.growthProgress = _currentCompletionProgress;
        }


        // Toggle UI elements based on allActive
        if (allActive && FlowerSpawned)
        {
            if (!_uiToggled) // Check if the UI has not been toggled yet
            {
                ToggleUIElements(true); // Toggle the UI elements
                _uiToggled = true; // Set the flag to true to prevent future toggles
            }
        }
        else
        {
            if (_uiToggled) // Reset the flag if allActive or LevelComplete becomes false
            {
                ToggleUIElements(false); // Disable the UI elements
                _uiToggled = false; // Reset the toggle flag
            }
        }


        // Additional logic for LevelComplete
        if (FlowerSpawned)
        {
            FindAnyObjectByType<ARPlaneMeshVisualizer>().enabled = false;
            scanText.SetActive(false);

            FlowerSpawnToggled = true;
        }

        // Activate SolvePuzzle and other logic only once
        if (FlowerSpawnToggled && !solvePuzzleActivated)
        {

            solvePuzzleActivated = true; // Set the flag to true to prevent future activations
        }

        if (FlowerSpawnToggled == true)
            foreach (var instance in testAllInstances)
            {
                if (dragAndDropReceivers != null)
                {
                    foreach (var receiver in dragAndDropReceivers)
                    {
                        receiver.gameObject.SetActive(true);

                    }
                }
                instance.GetComponent<Collider>().enabled = true;
                instance.particlesystems[0].SetActive(true);

                FlowerSpawnToggled = false;
            }


    }

    void ToggleUIElements(bool state)
    {
        completeText.SetActive(state);
        goBackButton.SetActive(state);
        FlowerButton.SetActive(state);

        SolvePuzzle.SetActive(false);
    }
}