using UnityEngine;

public class DragAndDropReceiver : MonoBehaviour
{
    public bool isTriggered;
    public Color objectColor;

    private ObjectInteraction script_objectInteraction;

    private void Start()
    {
        // Cache the initial color of the receiver object
        objectColor = GetComponent<Renderer>().material.color;

        script_objectInteraction = FindAnyObjectByType<ObjectInteraction>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object has the "Draggable" tag
        if (other.CompareTag("Draggable"))
        {
            isTriggered = true;

            // Get the Renderer component of the draggable object
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                // Change the color of the draggable object
                objectColor = new Color(0f, 1f, 0f, 0.3f); // Use values between 0 and 1
                renderer.material.color = objectColor;
            }

            // Get the HitChecker component of the draggable object
            HitChecker hitChecker = other.GetComponent<HitChecker>();
            if (hitChecker != null)
            {
                hitChecker.Hit = true;

                script_objectInteraction.PlayBingSound();
            }
            else
            {
                Debug.LogWarning("HitChecker component not found on the draggable object.");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Reset isTriggered when the object exits the trigger zone
        if (other.CompareTag("Draggable"))
        {
            isTriggered = false;

            // Get the HitChecker component of the draggable object
            HitChecker hitChecker = other.GetComponent<HitChecker>();
            if (hitChecker != null)
            {
                hitChecker.Hit = false;
            }
        }
    }
}