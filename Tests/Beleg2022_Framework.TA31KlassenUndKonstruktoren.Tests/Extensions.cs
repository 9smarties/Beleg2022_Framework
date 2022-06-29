#nullable enable
using System;
using System.Reflection;

public static class Extensions

{
    public static FieldInfo? GetFieldOrBackingField(this Type? t, string name, BindingFlags bindingAttr)
    {
        if (t is null) return null;
        return t.GetField(name, bindingAttr) ?? t.GetField($"<{name}>k__BackingField", bindingAttr);
    }

    public static FieldInfo? GetFieldOrBackingField(this Type? t, string name)
    {
        return t.GetFieldOrBackingField(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
    }

    public static object? GetValueFromField(this object? inObject, string name,
        BindingFlags bindingAttr)
    {
        if (inObject is null) throw new ArgumentNullException(nameof(inObject), "Parameter must not be null.");
        FieldInfo? field = inObject.GetType().GetField(name, bindingAttr);
        return field?.GetValue(inObject);
    }

    public static object? GetValueFromField(this object? inObject, string name)
    {
        return inObject.GetValueFromField(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
    }

    public static object? GetValueFromProperty(this object? inObject, string name,
        BindingFlags bindingAttr)
    {
        if (inObject is null) throw new ArgumentNullException(nameof(inObject), "Parameter must not be null.");
        PropertyInfo? field = inObject.GetType().GetProperty(name, bindingAttr);
        if (field?.GetIndexParameters().Length == 0)
            return field?.GetValue(inObject);
        if (field is not null) throw new NotImplementedException("This method can not handle indexed properties");
        return null;
    }

    public static object? GetValueFromProperty(this object? inObject, string name)
    {
        return inObject.GetValueFromProperty(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
    }

    public static object? GetValueFieldOrProperty(this object? inObject, string name,
        BindingFlags bindingAttr)
    {
        if (inObject is null) throw new ArgumentNullException(nameof(inObject), "Parameter must not be null.");
        return inObject.GetValueFromField(name, bindingAttr) ?? inObject.GetValueFromProperty(name, bindingAttr);
    }

    public static object? GetValueFieldOrProperty(this object? inObject, string name)
    {
        return inObject.GetValueFieldOrProperty(name,
            BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
    }


    public static bool IsFieldOrProperty(this Type? inType, string name, BindingFlags bindingAttr)
    {
        if (inType is null) throw new ArgumentNullException(nameof(inType), "Parameter must not be null.");
        FieldInfo? f_info = inType.GetField(name, bindingAttr);
        PropertyInfo? p_info = inType.GetProperty(name, bindingAttr);
        return f_info is not null || p_info is not null;
    }

    public static bool IsFieldOrProperty(this Type? inType, string name)
    {
        return inType.IsFieldOrProperty(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
    }

    public static bool IsProperty(this Type? inType, string name, BindingFlags bindingAttr)
    {
        return inType?.GetProperty(name, bindingAttr) is not null;
    }

    public static bool IsProperty(this Type? inType, string name) =>
        inType.IsProperty(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
}