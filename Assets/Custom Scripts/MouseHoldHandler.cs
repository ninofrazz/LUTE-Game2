using UnityEngine;
using UnityEngine.InputSystem;

public class MouseHoldHandler : MonoBehaviour
{
    public bool isHolding = false;
    public GameObject draggedObject = null;
    private Camera mainCamera;
    private float initialZPosition;
    public DragAndDropReceiver[] receiver;
    public Color HoldColor;
    public Color notHoldColor;

    private void Awake()
    {
        mainCamera = Camera.main;
        receiver = FindObjectsOfType<DragAndDropReceiver>();
        SetReceiverColors(notHoldColor);
    }

    private void Update()
    {
        // Handle input release to stop dragging
        if (isHolding)
        {
            bool inputReleased = false;

            // Check for touch release
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            {
                // Touch is still active
            }
            // Check for mouse release (editor testing)
            else if (Mouse.current != null && Mouse.current.leftButton.isPressed)
            {
                // Mouse is still active
            }
            else
            {
                inputReleased = true;
            }

            if (inputReleased)
            {
                EndHold();
                return;
            }

            // Handle drag movement
            if (draggedObject != null)
            {
                Vector3 inputPosition = GetInputPosition();
                if (inputPosition == Vector3.negativeInfinity)
                {
                    EndHold();
                    return;
                }

                inputPosition.z = initialZPosition;
                Vector3 worldPosition = mainCamera.ScreenToWorldPoint(inputPosition);
                draggedObject.transform.position = worldPosition;

                SetReceiverColors(HoldColor);
            }
        }
        else // Not holding
        {
            // Check for new input to start dragging
            if (IsNewInputStarted())
            {
                StartHold();
            }
            else
            {
                SetReceiverColors(notHoldColor);
            }
        }
    }

    private Vector3 GetInputPosition()
    {
        // Priority to touch input
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            return Touchscreen.current.primaryTouch.position.ReadValue();
        }
        // Fallback to mouse input (for editor testing)
        else if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            return Mouse.current.position.ReadValue();
        }

        return Vector3.negativeInfinity;
    }

    private bool IsNewInputStarted()
    {
        // Check for new touch
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            return true;
        }
        // Check for new mouse click (editor testing)
        else if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            return true;
        }

        return false;
    }

    private void StartHold()
    {
        Vector3 inputPosition = GetInputPosition();
        if (inputPosition == Vector3.negativeInfinity) return;

        Ray ray = mainCamera.ScreenPointToRay(inputPosition);
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.CompareTag("Draggable"))
        {
            draggedObject = hit.collider.gameObject;
            initialZPosition = mainCamera.WorldToScreenPoint(draggedObject.transform.position).z;
            isHolding = true;
            SetReceiverColors(HoldColor);
        }
    }

    private void EndHold()
    {
        isHolding = false;
        draggedObject = null;
        SetReceiverColors(notHoldColor);
    }

    private void SetReceiverColors(Color color)
    {
        foreach (var rec in receiver)
        {
            if (!rec.isTriggered)
            {
                Renderer renderer = rec.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = color;
                }
            }
        }
    }
}