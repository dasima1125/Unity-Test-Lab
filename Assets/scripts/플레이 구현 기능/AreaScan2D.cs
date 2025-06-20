using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AreaScan
{
    public static Collider2D[] Arc2D(Vector2 point, float radius, float startAngle, float endAngle)
    {
        List<Collider2D> output = new();

        Collider2D[] hits = Physics2D.OverlapCircleAll(point, radius);
        foreach (Collider2D hit in hits)
        {
            float angle = GetAngleBound(point, hit);
            if (IsAngleBetween(angle, startAngle, endAngle))
            {
                output.Add(hit);
            }
        }
        return output.ToArray();
    }

    private static float GetAnglePoint(Vector2 point, Collider2D hit)
    {
        Vector2 direction = (Vector2)hit.transform.position - point;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360f;

        return angle;
    }
    private static float GetAngleBound(Vector2 point, Collider2D hit)
    {
        Vector2 closestPoint = hit.ClosestPoint(point);
        Vector2 direction = closestPoint - point;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360f;
        return angle;
    }

    // Returns true if 'angle' lies between 'start' and 'end' angles.
    // Note:
    // - Assumes all angles are in the range [0, 360).
    // - Does NOT handle negative angles or angles >= 360 properly.
    // - If 'start' == 'end', returns false (no range).
    private static bool IsAngleBetween_Low(float angle, float start, float end)
    {
        if (start == end) return false;

        if (start < end)
            return angle >= start && angle <= end;
        else
            return angle >= start || angle <= end;
    }
    private static bool IsAngleBetween(float angle, float start, float end)
    {
        angle = NormalizeAngle(angle);
        start = NormalizeAngle(start);
        end = NormalizeAngle(end);

        if (start == end)
        {
            Debug.LogWarning("Ignore Range");
            return false;
        }    

        if (start < end)
            return angle >= start && angle <= end;
        else
            return angle >= start || angle <= end;
    }
    private static float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle < 0) angle += 360f;
        return angle;
    }

    
}
