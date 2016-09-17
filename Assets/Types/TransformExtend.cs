using System;
using System.Collections.Generic;
using UnityEngine; 
public static class TransformExtend
{
    public static Transform FindChild(this Transform target, Predicate<GameObject> predicate)
    {
        for (var i = 0; i < target.childCount; i++)
        {
            var child = target.GetChild(i);
            if (predicate.Invoke(child.gameObject))
                return child; 
        }
        return null;
    }

    public static Transform[] FindChildren(this Transform target, Predicate<GameObject> predicate)
    {
        List<Transform> result = new List<Transform>();
        for (int i = 0; i < target.childCount; i++)
        {
            var child = target.GetChild(i);
            if (predicate.Invoke(child.gameObject))
                result.Add(child);
        }
        return result.ToArray(); 
    }
}
