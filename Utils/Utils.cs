using UnityEngine;

public static class Utils
{
    // Used to generate evenly spaced 2D vectors around a point on the y plane.
    // Returns the *ith* vector of *total* evenly spaced *radius* long 2D Vectors around *center*.
    public static Vector3 GenerateCircleVector(int i, int total, float radius, Vector3 center) {
        float angle = i * Mathf.PI * 2f / total;
        return center + new Vector3(Mathf.Cos(angle) * radius, center.y, Mathf.Sin(angle) * radius);
    }
}
