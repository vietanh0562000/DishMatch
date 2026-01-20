using UnityEngine;


namespace TRS.CaptureTool
{
    [System.Serializable]
    public class ShadersTextureTransformation : TextureTransformation
    {
        public Shader[] shaders;

        public void Init(Shader[] shaders)
        {
            this.shaders = shaders;
        }

        public override string TransformationName()
        {
            return "Shaders";
        }

        public override string[] PropertyNames()
        {
            return new string[] { "shaders" };
        }

        public override string LabelForPropertyName(string propertyName)
        {
            if (propertyName.StartsWith("Element")) return propertyName;

            switch (propertyName)
            {
                case "size":
                    return "Size";
                case "shaders":
                    return "Shaders";
                default:
                    throw new UnityException("Unhandled property name: " + propertyName);
            }
        }

        public override Texture2D ApplyTransformation(Texture2D texture, bool apply = true, bool destroyOriginal = true)
        {
            if (!active) return texture;

            return texture.ApplyShaders(shaders, apply, destroyOriginal);
        }

        public override TextureTransformation Clone()
        {
            ShadersTextureTransformation clone = ScriptableObject.CreateInstance<ShadersTextureTransformation>();
            clone.active = this.active;
            clone.shaders = this.shaders;
            return clone;
        }
    }
}