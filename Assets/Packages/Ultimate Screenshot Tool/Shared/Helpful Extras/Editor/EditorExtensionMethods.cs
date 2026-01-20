using System.Collections.Generic;
using UnityEditor;

namespace TRS.CaptureTool.Extras
{
    public static class EditorExtensionMethods
    {
        public static IEnumerable<SerializedProperty> GetChildren(this SerializedProperty property)
        {
            property = property.Copy();
            var nextElement = property.Copy();
            bool hasNextElement = nextElement.NextVisible(false);
            if (!hasNextElement)
                nextElement = null;

            property.NextVisible(true);
            do
            {
                if ((SerializedProperty.EqualContents(property, nextElement)))
                    yield break;

                yield return property;
            } while (property.NextVisible(false));
        }
    }
}