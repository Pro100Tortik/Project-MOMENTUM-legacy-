using System.Collections.Generic;
using UnityEngine;
using TMPro;

public static class Extensions
{
    public static Vector3 OnlyXZ(this Vector3 vector) => new Vector3(vector.x, 0, vector.z);
    public static Vector3 OnlyXY(this Vector3 vector) => new Vector3(vector.x, vector.y, 0);
    public static Vector3 OnlyYZ(this Vector3 vector) => new Vector3(0, vector.y, vector.z);
    public static void SetTextDynamic(this TMP_Text text, object value) => text.SetText(value.ToString());
    public static void SetTextDynamic(this TextMeshProUGUI text, object value) => text.SetText(value.ToString());
    public static T GetRandomElement<T>(this IList<T> list) => list[Random.Range(0, list.Count)];
    public static T GetRandomElement<T>(this T[] array) => array[Random.Range(0, array.Length)];
}
