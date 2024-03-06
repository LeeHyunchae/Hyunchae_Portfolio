using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class RectExtention
{
#if UNITY_EDITOR
    public static void DrawDebugLine(this Rect _rect)
    {
        Debug.DrawLine(new Vector3(_rect.x, _rect.y), new Vector3(_rect.x + _rect.width, _rect.y), Color.red);
        Debug.DrawLine(new Vector3(_rect.x, _rect.y), new Vector3(_rect.x, _rect.y - _rect.height), Color.red);
        Debug.DrawLine(new Vector3(_rect.x + _rect.width, _rect.y - _rect.height), new Vector3(_rect.x + _rect.width, _rect.y), Color.red);
        Debug.DrawLine(new Vector3(_rect.x + _rect.width, _rect.y - _rect.height), new Vector3(_rect.x, _rect.y - _rect.height), Color.red);
    }
#endif
}
