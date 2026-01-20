using UnityEngine;


namespace TRS.CaptureTool
{
    [System.Serializable]
    public class LayerInFrontTextureTransformation : TextureTransformation
    {
        public Texture2D otherLayer;
        public Vector2 otherLayerPosition;
        public TextureTransformation.LayerPositionPoint otherLayerPositionPoint;
        public bool otherLayerPositionIsRelative;
        public bool overlapOnly = true;
        public Color emptySpaceFillColor;
        public bool useAlphaBlend = true;

        public void Init(Texture2D otherLayer, Vector2 otherLayerPosition, TextureTransformation.LayerPositionPoint otherLayerPositionPoint, bool otherLayerPositionIsRelative, bool overlapOnly, Color emptySpaceFillColor, bool useAlphaBlend = true)
        {
            this.otherLayer = otherLayer;
            this.otherLayerPosition = otherLayerPosition;
            this.otherLayerPositionPoint = otherLayerPositionPoint;
            this.otherLayerPositionIsRelative = otherLayerPositionIsRelative;
            this.overlapOnly = overlapOnly;
            this.emptySpaceFillColor = emptySpaceFillColor;
            this.useAlphaBlend = useAlphaBlend;
        }

        public override string TransformationName()
        {
            return "Layer In Front";
        }

        public override string[] PropertyNames()
        {
            return new string[] { "otherLayer", "otherLayerPosition", "otherLayerPositionPoint", "otherLayerPositionIsRelative", "overlapOnly", "emptySpaceFillColor", "useAlphaBlend" };
        }

        public override string LabelForPropertyName(string propertyName)
        {
            if (propertyName.StartsWith("Element")) return propertyName;

            switch (propertyName)
            {
                case "size":
                    return "Size";
                case "otherLayer":
                    return "Other Layer";
                case "otherLayerPosition":
                    return "Other Layer Position";
                case "otherLayerPositionPoint":
                    return "Other Layer Position Point";
                case "otherLayerPositionIsRelative":
                    return "Relative Position";
                case "overlapOnly":
                    return "Overlap Only";
                case "emptySpaceFillColor":
                    return "Empty Space Fill Color";
                case "useAlphaBlend":
                    return "Use Alpha Blend";
                default:
                    throw new UnityException("Unhandled property name: " + propertyName);
            }
        }

        public Vector2Int Position(Vector2Int textureSize)
        {
            Vector2Int position;
            if (otherLayerPositionIsRelative)
                position = new Vector2Int((int)(float)(textureSize.x * otherLayerPosition.x),
                                          (int)(float)(textureSize.y * otherLayerPosition.y));
            else
                position = new Vector2Int((int)otherLayerPosition.x, (int)otherLayerPosition.y);

            if (otherLayerPositionPoint == LayerPositionPoint.Center)
                position = new Vector2Int(position.x + (textureSize.x - otherLayer.width) / 2,
                                          position.y + (textureSize.y - otherLayer.height) / 2);

            return position;
        }

        public override Texture2D ApplyTransformation(Texture2D texture, bool apply = true, bool destroyOriginal = true)
        {
            if (!active) return texture;

            Texture2D background = texture;
            Texture2D foreground = otherLayer;

            Texture2D result = background.Blend(foreground, Position(new Vector2Int(texture.width, texture.height)), overlapOnly, emptySpaceFillColor, useAlphaBlend, apply, false);
            if (destroyOriginal) texture.DestroyIfPossible();
            return result;
        }

        public override TextureTransformation Clone()
        {
            LayerInFrontTextureTransformation clone = ScriptableObject.CreateInstance<LayerInFrontTextureTransformation>();
            clone.active = this.active;
            clone.otherLayer = this.otherLayer;
            clone.otherLayerPosition = this.otherLayerPosition;
            clone.otherLayerPositionPoint = this.otherLayerPositionPoint;
            clone.otherLayerPositionIsRelative = this.otherLayerPositionIsRelative;
            clone.overlapOnly = this.overlapOnly;
            clone.emptySpaceFillColor = this.emptySpaceFillColor;
            clone.useAlphaBlend = this.useAlphaBlend;
            return clone;
        }
    }
}