using UnityEngine;

public class BezierCurve : MonoBehaviour
{
    public Transform[] points;
    public float accuracy, pointSize;
    public bool draw;

    public Vector3 GetPoint(float t)
    {
        return Mathf.Pow(1 - t, 3) * points[0].position + 3 * Mathf.Pow(1 - t, 2) * t * points[1].position
                + 3 * (1 - t) * Mathf.Pow(t, 2) * points[2].position + Mathf.Pow(t, 3) * points[3].position;
    }

    public float GetLength()
    {
        var length = 0.0f;
        if (accuracy == 0.0f) accuracy = 0.1f;

        var previous = GetPoint(0);
        for (float i = accuracy; i < 1.0f; i += accuracy)
        {
            var current = GetPoint(i);
            length += (current - previous).magnitude;
            previous = current;
        }
        return length;
    }

    private void OnDrawGizmos()
    {
        if (!draw) return;

        if (accuracy == 0.0f) accuracy = 0.1f;
        for (float i = 0; i < 1.0f; i += accuracy)
        {
            Gizmos.DrawSphere(GetPoint(i), pointSize);
        }
    }


}
