using UnityEngine;


namespace TRS.CaptureTool
{
    [System.Serializable]
    public class MaterialTextureTransformation : TextureTransformation
    {
        public Material material;

        public void Init(Material material)
        {
            this.material = material;
        }

        public override string TransformationName()
        {
            return "Material";
        }

        public override string[] PropertyNames()
        {
            return new string[] { "material" };
        }

        public override string LabelForPropertyName(string propertyName)
        {
            if (propertyName.StartsWith("Element")) return propertyName;

            switch (propertyName)
            {
                case "size":
                    return "Size";
                case "material":
                    return "Material";
                default:
                    throw new UnityException("Unhandled property name: " + propertyName);
            }
        }

        public override Texture2D ApplyTransformation(Texture2D texture, bool apply = true, bool destroyOriginal = true)
        {
            if (!active) return texture;

            return texture.ApplyMaterial(material, apply, destroyOriginal);
        }

        public override TextureTransformation Clone()
        {
            MaterialTextureTransformation clone = ScriptableObject.CreateInstance<MaterialTextureTransformation>();
            clone.active = this.active;
            clone.material = this.material;
            return clone;
        }
    }
}