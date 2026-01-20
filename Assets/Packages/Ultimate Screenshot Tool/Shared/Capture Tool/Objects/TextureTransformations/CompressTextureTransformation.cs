using UnityEngine;

namespace TRS.CaptureTool
{
    [System.Serializable]
    public class CompressTextureTransformation : TextureTransformation
    {
        public bool highQuality;

        public void Init(bool highQuality) {
            this.highQuality = highQuality;
        }

        public override string TransformationName()
        {
            return "Compress";
        }

        public override string[] PropertyNames()
        {
            return new string[] { "highQuality" };
        }

        public override string LabelForPropertyName(string propertyName)
        {
            if (propertyName.StartsWith("Element")) return propertyName;

            switch (propertyName)
            {
                case "size":
                    return "Size";
                case "highQuality":
                    return "High Quality";
                default:
                    throw new UnityException("Unhandled property name: " + propertyName);
            }
        }

        public override Texture2D ApplyTransformation(Texture2D texture, bool apply = true, bool destroyOriginal = true)
        {
            if (!active) return texture;

            texture.Compress(highQuality);
            return texture;
        }

        public override TextureTransformation Clone()
        {
            CompressTextureTransformation clone = ScriptableObject.CreateInstance<CompressTextureTransformation>();
            clone.active = this.active;
            clone.highQuality = this.highQuality;
            return clone;
        }
    }
}