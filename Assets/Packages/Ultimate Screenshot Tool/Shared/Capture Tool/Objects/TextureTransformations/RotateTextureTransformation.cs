using UnityEngine;

namespace TRS.CaptureTool
{
    [System.Serializable]
    public enum RotationType
    {
        None,
        Clockwise,
        CounterClockwise,
        Flip,
    }

    [System.Serializable]
    public class RotateTextureTransformation : TextureTransformation
    {
        public RotationType rotationType;

        public void Init(RotationType rotationType)
        {
            this.rotationType = rotationType;
        }

        public override string TransformationName()
        {
            return "Rotate";
        }

        public override string[] PropertyNames()
        {
            return new string[] { "rotationType" };
        }

        public override string LabelForPropertyName(string propertyName)
        {
            if (propertyName.StartsWith("Element")) return propertyName;

            switch (propertyName)
            {
                case "size":
                    return "Size";
                case "rotationType":
                    return "Rotation";
                default:
                    throw new UnityException("Unhandled property name: " + propertyName);
            }
        }

        public override Texture2D ApplyTransformation(Texture2D texture, bool apply = true, bool destroyOriginal = true)
        {
            if (!active) return texture;

            switch(rotationType)
            {
                case RotationType.Clockwise:
                    return texture.Rotate90Degrees(true, apply, destroyOriginal);
                case RotationType.CounterClockwise:
                    return texture.Rotate90Degrees(false, apply, destroyOriginal);
                case RotationType.Flip:
                    return texture.Rotate180Degrees(apply, destroyOriginal);
            }

            return null;
        }

        public override TextureTransformation Clone()
        {
            RotateTextureTransformation clone = ScriptableObject.CreateInstance<RotateTextureTransformation>();
            clone.active = this.active;
            clone.rotationType = this.rotationType;
            return clone;
        }
    }
}