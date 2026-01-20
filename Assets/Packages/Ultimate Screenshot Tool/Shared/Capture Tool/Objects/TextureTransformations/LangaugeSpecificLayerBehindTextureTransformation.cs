using UnityEngine;
using System.Collections.Generic;


namespace TRS.CaptureTool
{
    [System.Serializable]
    public class LanguageSpecificLayerBehindTextureTransformation : LayerBehindTextureTransformation
    {
        // Dictionaries don't work in the editor. So make an array and convert on init.
        public LanguageTexturePair[] otherLayerForLanguage;

        public void Init(LanguageTexturePair[] otherLayerForLanguage, Vector2 otherLayerPosition, TextureTransformation.LayerPositionPoint otherLayerPositionPoint, bool otherLayerPositionIsRelative, bool overlapOnly, Color emptySpaceFillColor, bool useAlphaBlend = true)
        {
            this.otherLayerForLanguage = otherLayerForLanguage;
            this.otherLayerPosition = otherLayerPosition;
            this.otherLayerPositionPoint = otherLayerPositionPoint;
            this.otherLayerPositionIsRelative = otherLayerPositionIsRelative;
            this.overlapOnly = overlapOnly;
            this.emptySpaceFillColor = emptySpaceFillColor;
            this.useAlphaBlend = useAlphaBlend;
        }

        public override string TransformationName()
        {
            return "Language Specific Layer Behind";
        }

        public override string[] PropertyNames()
        {
            return new string[] { "otherLayerForLanguage", "otherLayerPosition", "otherLayerPositionPoint", "otherLayerPositionIsRelative", "overlapOnly", "emptySpaceFillColor", "useAlphaBlend" };
        }

        public override string LabelForPropertyName(string propertyName)
        {
            if (propertyName.StartsWith("Element")) return propertyName;

            switch (propertyName)
            {
                case "size":
                    return "Size";
                case "otherLayerForLanguage":
                    return "Other Layer for Language";
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

        public override Texture2D ApplyTransformation(Texture2D texture, bool apply = true, bool destroyOriginal = true)
        {
            if (!active) return texture;

            Texture2D background = null;
            Texture2D foreground = texture;

            for (int i = 0; i < otherLayerForLanguage.Length; ++i)
            {
                if (Application.systemLanguage == otherLayerForLanguage[i].language)
                    background = otherLayerForLanguage[i].texture;
            }

            if (background == null)
                Debug.LogError("Texture not found for language: " + Application.systemLanguage);

            Vector2Int position;
            if (otherLayerPositionIsRelative)
                position = new Vector2Int((int)(float)(background.width * otherLayerPosition.x),
                                          (int)(float)(background.height * otherLayerPosition.y));
            else
                position = new Vector2Int((int)otherLayerPosition.x, (int)otherLayerPosition.y);

            if (otherLayerPositionPoint == LayerPositionPoint.Center)
                position = new Vector2Int(position.x + (background.width - foreground.width) / 2,
                                          position.y + (background.height - foreground.height) / 2);

            Texture2D result = background.Blend(foreground, position, overlapOnly, emptySpaceFillColor, useAlphaBlend, apply, false);
            if (destroyOriginal) texture.DestroyIfPossible();
            return result;
        }

        public override TextureTransformation Clone()
        {
            LanguageSpecificLayerBehindTextureTransformation clone = ScriptableObject.CreateInstance<LanguageSpecificLayerBehindTextureTransformation>();
            clone.active = this.active;
            clone.otherLayerForLanguage = this.otherLayerForLanguage;
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