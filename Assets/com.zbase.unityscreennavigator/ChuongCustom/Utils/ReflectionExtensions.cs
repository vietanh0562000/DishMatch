namespace ChuongCustom.Utils
{
    using System;
    using UnityEngine.UI;

    public static class ReflectionExtensions
    {
        public static Type GetGenericType<T>(this T type) where T : Type
        {
            var baseType = type.BaseType;
            
            return baseType is { IsGenericType: true } ? baseType.GetGenericArguments()[0] : null;
        }
    }
}