using UnityEngine;

using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool
{
    [System.Serializable]
    public class CutoutTextureTransformation : TextureTransformation
    {
        public Rect rect;
        public bool positionRelative;
        public bool sizeRelative;

        public void Init(Rect rect, bool positionRelative, bool sizeRelative)
        {
            this.rect = rect;
            this.positionRelative = positionRelative;
            this.sizeRelative = sizeRelative;
        }

        public override string TransformationName()
        {
            return "Cutout";
        }

        public override string[] PropertyNames()
        {
            return new string[] { "rect", "positionRelative", "sizeRelative" };
        }

        public override string LabelForPropertyName(string propertyName)
        {
            if (propertyName.StartsWith("Element")) return propertyName;

            switch (propertyName)
            {
                case "size":
                    return "Size";
                case "rect":
                    return "Rect";
                case "positionRelative":
                    return "Position Relative";
                case "sizeRelative":
                    return "Size Relative";
                default:
                    throw new UnityException("Unhandled property name: " + propertyName);
            }
        }

        public Rect RectForTextureSize(int textureWidth, int textureHeight)
        {
            if (positionRelative && sizeRelative)
                return rect.ReverseNormalize(new Vector2(textureWidth, textureHeight));
            else if (positionRelative)
                return rect.ReverseNormalizePosition(new Vector2(textureWidth, textureHeight));
            else if (sizeRelative)
                return rect.ReverseNormalizeSize(new Vector2(textureWidth, textureHeight));
            return rect;
        }

        public override Texture2D ApplyTransformation(Texture2D texture, bool apply = true, bool destroyOriginal = true)
        {
            if (!active) return texture;
            return texture.Cutout(RectForTextureSize(texture.width, texture.height), apply, destroyOriginal);
        }

        public override TextureTransformation Clone()
        {
            CutoutTextureTransformation clone = ScriptableObject.CreateInstance<CutoutTextureTransformation>();
            clone.active = this.active;
            clone.rect = this.rect;
            clone.positionRelative = this.positionRelative;
            clone.sizeRelative = this.sizeRelative;
            return clone;
        }
    }
}