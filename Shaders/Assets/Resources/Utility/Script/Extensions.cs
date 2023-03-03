using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Text;

public static class Extensions 
{
    ///List
    public static T RandomItemGet<T>(this List<T> list)
    {
        if (list.Count == 0)
            throw new IndexOutOfRangeException("Empty List");

        var t = Random.Range(0, list.Count);
        return list[t];
    }

    public static T RandomItemRemove<T>(this List<T> list)
    {
        if (list.Count == 0)
            return default;

        var t = Random.Range(0, list.Count);
        var item = list[t];
        list.RemoveAt(t);
        return item;
    }

    public static void AddFront<T>(this List<T> list, T item)
    {
        list.Insert(0, item);
    }

    public static void AddBefore<T>(this List<T> list, T item, T newItem)
    {
        var t = list.IndexOf(item);
        list.Insert(t, newItem);
    }

    public static void AddAfter<T>(this List<T> list, T item, T newItem)
    {
        var t = list.IndexOf(item) + 1;
        list.Insert(t, newItem);
    }

    //Component
    public static T AddComponent<T>(this Component component) where T : Component => component.gameObject.AddComponent<T>();
    public static T GetOrAddComponent<T>(this Component component) where T : Component => component.GetComponent<T>() ?? component.AddComponent<T>();
    public static bool HasComponent<T>(this Component component) where T : Component => component.GetComponent<T>() != null;

    //GameObject
    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component => gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
    public static bool HasComponent<T>(this GameObject gameObject) where T : Component => gameObject.GetComponent<T>() != null;

    //Transforms
    public static void AddChildren(this Transform transform, GameObject[] children) =>
        Array.ForEach(children, child => child.transform.parent = transform);

    public static void AddChildren(this Transform transform, Component[] children) =>
        Array.ForEach(children, child => child.transform.parent = transform);

    public static void ResetChildPositions(this Transform transform, bool recursive = false)
    {
        foreach(Transform child in transform)
        {
            child.position = Vector3.zero;

            if (recursive) child.ResetChildPositions(recursive);
        }
    }

    public static void SetX(this Transform transform, float x) =>
        transform.position = new Vector3(x, transform.position.y, transform.position.z);
    public static void SetY(this Transform transform, float y) =>
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    public static void SetZ(this Transform transform, float z) =>
        transform.position = new Vector3(transform.position.x, transform.position.y, z);

    //Vector
    public static Vector3 GetClosest(this Vector3 position, IEnumerable<Vector3> others)
    {
        var closest = Vector3.zero;
        var shortest = Mathf.Infinity;

        foreach(var other in others)
        {
            var dist = (position - other).sqrMagnitude;

            if(dist < shortest)
            {
                closest = other;
                shortest = dist;
            }
        }

        return closest;
    }

    public static Vector2 XY(this Vector3 v) => new Vector2(v.x, v.y);
    public static Vector2 XZ(this Vector3 v) => new Vector2(v.x, v.z);
    public static Vector3 XZA(this Vector2 v) => new Vector3(v.x, 0, v.y);
    public static Vector3 XZY(this Vector3 v) => new Vector3(v.x, v.z, v.y);

    public static Vector3 NearestPointOnAxis(this Vector3 axisDirection, Vector3 point, bool isNormalized = false)
    {
        if (!isNormalized)
            axisDirection.Normalize();
        var d = Vector3.Dot(point, axisDirection);
        return axisDirection * d;
    }

    public static Vector3 NearestPointOnLine(this Vector3 lineDirection, Vector3 point, Vector3 pointOnLine, bool isNormalized = false)
    {
        if (!isNormalized)
            lineDirection.Normalize();
        var d = Vector3.Dot(point - pointOnLine, lineDirection);
        return pointOnLine + lineDirection * d;
    }

    ///Colour    
    public static string ColourString(string text, Color color)
    {
        var str = new StringBuilder();
        str.Append("<color=#").Append(ColorUtility.ToHtmlStringRGBA(color)).Append(">").Append(text).Append("</color>");
        return str.ToString();
    }
}
