using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TunnelScript : MonoBehaviour
{
    public GameObject player;
    public PolygonCollider2D tunnelExits;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var exitIdx = Random.Range(0, tunnelExits.points.Length);
        var polygon = tunnelExits.GetPath(exitIdx);
        var centerPos = GetCenter(polygon);
        player.transform.position = centerPos;
    }

    private Vector2 GetCenter(Vector2[] vectors)
    {
        float xMax = float.MinValue;
        float yMax = float.MinValue;
        float xMin = float.MaxValue;
        float yMin = float.MaxValue;
        foreach (var vector in vectors)
        {
            if (xMax < vector.x)
            {
                xMax = vector.x;
            }
            if (yMax < vector.y)
            {
                yMax = vector.y;
            }
            if (xMin > vector.x)
            {
                xMin = vector.x;
            }
            if (yMin > vector.y)
            {
                yMin = vector.y;
            }
        }
        var xCenter = (xMax + xMin) / 2;
        var yCenter = (yMax + yMin) / 2;
        return new Vector2(xCenter, yCenter);
    }
}
