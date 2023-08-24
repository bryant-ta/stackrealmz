using UnityEngine;

public class OverlapBoxVisualization : MonoBehaviour
{
    public Vector3 halfExtents = Vector3.one;
    public Quaternion orientation = Quaternion.identity;
    
    private void OnDrawGizmos()
    {
        Vector3 center = transform.position; // Use the position of the attached GameObject

        // Calculate the box corners
        Vector3[] corners = new Vector3[8];
        for (int i = 0; i < 8; i++)
        {
            Vector3 corner = new Vector3(
                (i & 1) == 0 ? -halfExtents.x : halfExtents.x,
                (i & 2) == 0 ? -halfExtents.y : halfExtents.y,
                (i & 4) == 0 ? -halfExtents.z : halfExtents.z
            );
            corners[i] = orientation * corner + center;
        }

        // Draw the box outline using Gizmos
        Gizmos.color = Color.yellow;

        for (int i = 0; i < 4; i++)
        {
            int nextIndex = (i + 1) % 4;
            Gizmos.DrawLine(corners[i], corners[nextIndex]);
            Gizmos.DrawLine(corners[i + 4], corners[nextIndex + 4]);
            Gizmos.DrawLine(corners[i], corners[i + 4]);
        }
    }
}