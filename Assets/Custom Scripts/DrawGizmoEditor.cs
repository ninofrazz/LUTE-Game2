using UnityEngine;

public class DrawGizmoEditor : MonoBehaviour
{
    public float radius = 1.0f; // Base radius of the sphere
    public Color gizmoColor = Color.red;

    // Draw Gizmo when the object is selected
    private void OnDrawGizmos()
    {
        radius = 0.5f;

        // Set the color of the Gizmo
        Gizmos.color = gizmoColor;

        // Save the current Gizmos matrix
        Matrix4x4 originalMatrix = Gizmos.matrix;

        // Apply the object's transform to the Gizmos matrix
        Gizmos.matrix = transform.localToWorldMatrix;

        // Draw a wireframe sphere at the object's position with the base radius
        Gizmos.DrawWireSphere(Vector3.zero, radius);

        // Restore the original Gizmos matrix
        Gizmos.matrix = originalMatrix;
    }
}