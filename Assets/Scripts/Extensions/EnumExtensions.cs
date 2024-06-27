using System;
using System.Collections.Generic;
using UnityEngine;

public static class EnumExtensions
{
    public static HashSet<int> GetEnumValues<T>() where T : struct
    {
        if (!typeof(T).IsEnum)
        {
            Debug.LogWarning("Cannot get enum values from a non-enum type.");
            return null;
        }

        Array enumValues = Enum.GetValues(typeof(T));
        HashSet<int> valuesSet = new();

        foreach (object value in enumValues) valuesSet.Add(Convert.ToInt32(value));

        return valuesSet;
    }
}
