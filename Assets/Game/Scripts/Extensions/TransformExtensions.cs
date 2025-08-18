using UnityEngine;

public static class TransformExtensions
{
    public static void Clear(this Transform transform)
    {
        while (transform.childCount > 0)
        {
            Transform child = transform.GetChild(0);
            child.SetParent(null);
            Object.Destroy(child.gameObject);
        }
    }
}
