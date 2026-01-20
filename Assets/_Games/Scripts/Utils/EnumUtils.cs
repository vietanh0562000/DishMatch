using System;
using System.ComponentModel;
using System.Reflection;

public static class EnumExtensions
{
    public static string GetDescription(this Enum value)
    {
        // Lấy kiểu của enum
        Type type = value.GetType();
        // Lấy FieldInfo của phần tử enum tương ứng
        string name = Enum.GetName(type, value);
        if (name == null)
            return value.ToString();

        FieldInfo field = type.GetField(name);
        if (field == null)
            return name;

        // Lấy DescriptionAttribute (nếu có)
        var attr = field.GetCustomAttribute<DescriptionAttribute>();
        return attr != null
            ? attr.Description
            : name;
    }
}