using UnityEngine;


namespace TRS.CaptureTool
{
    [System.Serializable]
    public class MaterialsTextureTransformation : TextureTransformation
    {
        public Material[] materials;

        public void Init(Material[] materials)
        {
            this.materials = materials;
        }

        public override string TransformationName()
        {
            return "Materials";
        }

        public override string[] PropertyNames()
        {
            return new string[] { "materials" };
        }

        public override string LabelForPropertyName(string propertyName)
        {
            if (propertyName.StartsWith("Element")) return propertyName;

            switch (propertyName)
            {
                case "size":
                    return "Size";
                case "materials":
                    return "Materials";
                default:
                    throw new UnityException("Unhandled property name: " + propertyName);
            }
        }

        public override Texture2D ApplyTransformation(Texture2D texture, bool apply = true, bool destroyOriginal = true)
        {
            if (!active) return texture;

            return texture.ApplyMaterials(materials, apply, destroyOriginal);
        }

        public override TextureTransformation Clone()
        {
            MaterialsTextureTransformation clone = ScriptableObject.CreateInstance<MaterialsTextureTransformation>();
            clone.active = this.active;
            clone.materials = this.materials;
            return clone;
        }
    }
}