using UnityEngine;


namespace TRS.CaptureTool
{
    [System.Serializable]
    public class SolidifyTextureTransformation : TextureTransformation
    {
        public void Init() {}

        public override string TransformationName()
        {
            return "Solidify";
        }

        public override string[] PropertyNames()
        {
            return new string[0];
        }

        public override string LabelForPropertyName(string propertyName)
        {
            throw new UnityException("Unhandled property name: " + propertyName);
        }

        public override Texture2D ApplyTransformation(Texture2D texture, bool apply = true, bool destroyOriginal = true)
        {
            if (!active) return texture;

            return texture.Solidify(apply);
        }

        public override TextureTransformation Clone()
        {
            SolidifyTextureTransformation clone = ScriptableObject.CreateInstance<SolidifyTextureTransformation>();
            clone.active = this.active;
            return clone;
        }
    }
}