using UnityEngine;

public class DrawGizmoPlayer : MonoBehaviour
{
    public Vector3 size = new Vector3(1, 1, 1);
    public Color gizmoColor = Color.green;

    private void OnDrawGizmos()
    {
        // Uncomment this if you want the sphere to always be visible
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireCube(transform.position, size);
    }
}