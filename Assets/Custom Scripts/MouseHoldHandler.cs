using UnityEngine;
using UnityEngine.InputSystem;

public class MouseHoldHandler : MonoBehaviour
{
    public bool isHolding = false; // Tracks whether the mouse or touch is being held down
    public GameObject draggedObject = null; // Object being dragged
    private Camera mainCamera;
    private float initialZPosition; // To store the initial Z position of the object

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // Handle drag movement
        if (isHolding && draggedObject != null)
        {
            // Get mouse or touch position (check for touchscreen input)
            Vector3 inputPosition;
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            {
                // If there is a touch, use the primary touch position
                inputPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            }
            else if (Mouse.current != null && Mouse.current.leftButton.isPressed)
            {
                // If no touch, check for mouse input
                inputPosition = Mouse.current.position.ReadValue();
            }
            else
            {
                EndHold();
                return; // No valid input, so return
            }

            // Maintain the original Z position of the object during dragging
            inputPosition.z = initialZPosition;
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(inputPosition);
            draggedObject.transform.position = worldPosition;
        }

        // Check for input press to start dragging
        if (!isHolding)
        {
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            {
                StartHold();
            }
            else if (Mouse.current.leftButton.isPressed)
            {
                StartHold();
            }
        }

        // Check for input release to stop dragging

    }

    private void StartHold()
    {
        // Check if an object is under the cursor or touch point
        Vector3 inputPosition;
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            inputPosition = Touchscreen.current.primaryTouch.position.ReadValue();
        }
        else
        {
            inputPosition = Mouse.current.position.ReadValue();
        }

        Ray ray = mainCamera.ScreenPointToRay(inputPosition);
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.tag == "Draggable")
        {
            draggedObject = hit.collider.gameObject;

            // Store the initial Z position of the object when drag starts
            initialZPosition = mainCamera.WorldToScreenPoint(draggedObject.transform.position).z;
            isHolding = true;
        }
    }

    private void EndHold()
    {
        isHolding = false;
        if (draggedObject != null)
        {
        }
        draggedObject = null; // Stop dragging the object
    }
}
