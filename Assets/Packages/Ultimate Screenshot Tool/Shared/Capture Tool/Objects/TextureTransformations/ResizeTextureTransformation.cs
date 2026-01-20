using UnityEngine;

namespace TRS.CaptureTool
{
    [System.Serializable]
    public class ResizeTextureTransformation : TextureTransformation
    {
        public int width;
        public int height;

        public void Init(Resolution resolution)
        {
            this.width = resolution.width;
            this.height = resolution.height;
        }

        public void Init(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public override string TransformationName()
        {
            return "Resize";
        }

        public override string[] PropertyNames()
        {
            return new string[] { "width", "height" };
        }

        public override string LabelForPropertyName(string propertyName)
        {
            if (propertyName.StartsWith("Element")) return propertyName;

            switch (propertyName)
            {
                case "size":
                    return "Size";
                case "width":
                    return "Width";
                case "height":
                    return "Height";
                default:
                    throw new UnityException("Unhandled property name: " + propertyName);
            }
        }

        public override Texture2D ApplyTransformation(Texture2D texture, bool apply = true, bool destroyOriginal = true)
        {
            if (!active) return texture;

            return texture.Resize(new Resolution { width = width, height=height }, apply);
        }

        public override TextureTransformation Clone()
        {
            ResizeTextureTransformation clone = ScriptableObject.CreateInstance<ResizeTextureTransformation>();
            clone.active = this.active;
            clone.width = this.width;
            clone.height = this.height;
            return clone;
        }
    }
}