using UnityEngine;

public class DrawGizmoEditor : MonoBehaviour
{
    public float radius = 1.0f;
    public Color gizmoColor = Color.red;

    // Draw Gizmo when the object is selected
    private void OnDrawGizmos()
    {
        // Set the color of the Gizmo
        Gizmos.color = gizmoColor;

        // Draw a wireframe cube at the object's position
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}