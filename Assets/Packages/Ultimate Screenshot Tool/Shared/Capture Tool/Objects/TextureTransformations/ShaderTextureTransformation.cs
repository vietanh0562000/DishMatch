using UnityEngine;


namespace TRS.CaptureTool
{
    [System.Serializable]
    public class ShaderTextureTransformation : TextureTransformation
    {
        public Shader shader;

        public void Init(Shader shader)
        {
            this.shader = shader;
        }

        public override string TransformationName()
        {
            return "Shader";
        }

        public override string[] PropertyNames()
        {
            return new string[] { "shader" };
        }

        public override string LabelForPropertyName(string propertyName)
        {
            if (propertyName.StartsWith("Element")) return propertyName;

            switch (propertyName)
            {
                case "size":
                    return "Size";
                case "shader":
                    return "Shader";
                default:
                    throw new UnityException("Unhandled property name: " + propertyName);
            }
        }

        public override Texture2D ApplyTransformation(Texture2D texture, bool apply = true, bool destroyOriginal = true)
        {
            if (!active) return texture;

            return texture.ApplyShader(shader, apply, destroyOriginal);
        }

        public override TextureTransformation Clone()
        {
            ShaderTextureTransformation clone = ScriptableObject.CreateInstance<ShaderTextureTransformation>();
            clone.active = this.active;
            clone.shader = this.shader;
            return clone;
        }
    }
}