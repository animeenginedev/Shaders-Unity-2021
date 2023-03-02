using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugHelp
{
    public static bool IsMouseOnScreen()
    {
        var mousePos = Input.mousePosition;
        return mousePos.x > 0 &&
            mousePos.y > 0 &&
            mousePos.x <= Screen.width &&
            mousePos.y <= Screen.height;
    }
    public static void DrawBounds(Bounds b, Color c, float delay = 0)
    {
        // bottom
        var p1 = new Vector3(b.min.x, b.min.y, b.min.z);
        var p2 = new Vector3(b.max.x, b.min.y, b.min.z);
        var p3 = new Vector3(b.max.x, b.min.y, b.max.z);
        var p4 = new Vector3(b.min.x, b.min.y, b.max.z);

        Debug.DrawLine(p1, p2, c, delay);
        Debug.DrawLine(p2, p3, c, delay);
        Debug.DrawLine(p3, p4, c, delay);
        Debug.DrawLine(p4, p1, c, delay);

        // top
        var p5 = new Vector3(b.min.x, b.max.y, b.min.z);
        var p6 = new Vector3(b.max.x, b.max.y, b.min.z);
        var p7 = new Vector3(b.max.x, b.max.y, b.max.z);
        var p8 = new Vector3(b.min.x, b.max.y, b.max.z);

        

        Debug.DrawLine(p5, p6, c, delay);
        Debug.DrawLine(p6, p7, c, delay);
        Debug.DrawLine(p7, p8, c, delay);
        Debug.DrawLine(p8, p5, c, delay);

        // sides
        Debug.DrawLine(p1, p5, c, delay);
        Debug.DrawLine(p2, p6, c, delay);
        Debug.DrawLine(p3, p7, c, delay);
        Debug.DrawLine(p4, p8, c, delay);
    }
}
