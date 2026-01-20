using UnityEngine;

namespace TRS.CaptureTool
{
    // Used by LanguageSpecificLayerBehindTextureTransformation and LanguageSpecificLayerInFrontTextureTransformation
    [System.Serializable]
    public struct LanguageTexturePair
    {
        public SystemLanguage language;
        public Texture2D texture;
    }

    [System.Serializable]
    public class TextureTransformation : ScriptableObject
    {
        public bool active = true;

        [System.Serializable]
        public enum LayerPositionPoint
        {
            BottomLeft,
            Center
        }

        public virtual string TransformationName()
        {
            throw new UnityException("Unhandled virtual method");
        }

        public virtual string[] PropertyNames()
        {
            throw new UnityException("Unhandled virtual method");
        }

        public virtual string LabelForPropertyName(string propertyName)
        {
            throw new UnityException("Unhandled virtual method");
        }

        public static AddFrameTextureTransformation AddFrameTextureTransformation(Texture2D frame, FrameResizeMethod frameResizeMethod, Color backgroundFillColor)
        {
            AddFrameTextureTransformation textureTransformation = ScriptableObject.CreateInstance<AddFrameTextureTransformation>();
            textureTransformation.Init(frame, frameResizeMethod, backgroundFillColor);
            return textureTransformation;
        }

        public static CompressTextureTransformation CompressTextureTransformation(bool highQuality)
        {
            CompressTextureTransformation textureTransformation = ScriptableObject.CreateInstance<CompressTextureTransformation>();
            textureTransformation.Init(highQuality);
            return textureTransformation;
        }

        public static CutoutTextureTransformation CutoutTextureTransformation(Rect rect, bool positionRelative, bool sizeRelative)
        {
            CutoutTextureTransformation textureTransformation = ScriptableObject.CreateInstance<CutoutTextureTransformation>();
            textureTransformation.Init(rect, positionRelative, sizeRelative);
            return textureTransformation;
        }

        public static LayerBehindTextureTransformation LayerBehindTextureTransformation(Texture2D otherLayer, Vector2 otherLayerPosition, LayerPositionPoint otherLayerPositionPoint, bool otherLayerPositionIsRelative, bool overlapOnly, Color emptySpaceFillColor, bool useAlphaBlend = true)
        {
            LayerBehindTextureTransformation textureTransformation = ScriptableObject.CreateInstance<LayerBehindTextureTransformation>();
            textureTransformation.Init(otherLayer, otherLayerPosition, otherLayerPositionPoint, otherLayerPositionIsRelative, overlapOnly, emptySpaceFillColor, useAlphaBlend);
            return textureTransformation;
        }

        public static LayerInFrontTextureTransformation LayerInFrontTextureTransformation(Texture2D otherLayer, Vector2 otherLayerPosition, LayerPositionPoint otherLayerPositionPoint, bool otherLayerPositionIsRelative, bool overlapOnly, Color emptySpaceFillColor, bool useAlphaBlend = true)
        {
            LayerInFrontTextureTransformation textureTransformation = ScriptableObject.CreateInstance<LayerInFrontTextureTransformation>();
            textureTransformation.Init(otherLayer, otherLayerPosition, otherLayerPositionPoint, otherLayerPositionIsRelative, overlapOnly, emptySpaceFillColor, useAlphaBlend);
            return textureTransformation;
        }

        public static LanguageSpecificLayerBehindTextureTransformation LanguageSpecificLayerBehindTextureTransformation(LanguageTexturePair[] otherLayerForLanguage, Vector2 otherLayerPosition, LayerPositionPoint otherLayerPositionPoint, bool otherLayerPositionIsRelative, bool overlapOnly, Color emptySpaceFillColor, bool useAlphaBlend = true)
        {
            LanguageSpecificLayerBehindTextureTransformation textureTransformation = ScriptableObject.CreateInstance<LanguageSpecificLayerBehindTextureTransformation>();
            textureTransformation.Init(otherLayerForLanguage, otherLayerPosition, otherLayerPositionPoint, otherLayerPositionIsRelative, overlapOnly, emptySpaceFillColor, useAlphaBlend);
            return textureTransformation;
        }

        public static LanguageSpecificLayerInFrontTextureTransformation LanguageSpecificLayerInFrontTextureTransformation(LanguageTexturePair[] otherLayerForLanguage, Vector2 otherLayerPosition, LayerPositionPoint otherLayerPositionPoint, bool otherLayerPositionIsRelative, bool overlapOnly, Color emptySpaceFillColor, bool useAlphaBlend = true)
        {
            LanguageSpecificLayerInFrontTextureTransformation textureTransformation = ScriptableObject.CreateInstance<LanguageSpecificLayerInFrontTextureTransformation>();
            textureTransformation.Init(otherLayerForLanguage, otherLayerPosition, otherLayerPositionPoint, otherLayerPositionIsRelative, overlapOnly, emptySpaceFillColor, useAlphaBlend);
            return textureTransformation;
        }

        public static MaterialTextureTransformation MaterialTextureTransformation(Material material)
        {
            MaterialTextureTransformation textureTransformation = ScriptableObject.CreateInstance<MaterialTextureTransformation>();
            textureTransformation.Init(material);
            return textureTransformation;
        }

        public static MaterialsTextureTransformation MaterialsTextureTransformation(Material[] materials)
        {
            MaterialsTextureTransformation textureTransformation = ScriptableObject.CreateInstance<MaterialsTextureTransformation>();
            textureTransformation.Init(materials);
            return textureTransformation;
        }

        public static ResizeTextureTransformation ResizeTextureTransformation(Resolution resolution)
        {
            ResizeTextureTransformation textureTransformation = ScriptableObject.CreateInstance<ResizeTextureTransformation>();
            textureTransformation.Init(resolution);
            return textureTransformation;
        }

        public static RotateTextureTransformation RotateTextureTransformation(RotationType rotationType)
        {
            RotateTextureTransformation textureTransformation = ScriptableObject.CreateInstance<RotateTextureTransformation>();
            textureTransformation.Init(rotationType);
            return textureTransformation;
        }

        public static ShaderTextureTransformation ShaderTextureTransformation(Shader shader)
        {
            ShaderTextureTransformation textureTransformation = ScriptableObject.CreateInstance<ShaderTextureTransformation>();
            textureTransformation.Init(shader);
            return textureTransformation;
        }

        public static ShadersTextureTransformation ShadersTextureTransformation(Shader[] shaders)
        {
            ShadersTextureTransformation textureTransformation = ScriptableObject.CreateInstance<ShadersTextureTransformation>();
            textureTransformation.Init(shaders);
            return textureTransformation;
        }

        public static SolidifyTextureTransformation SolidifyTextureTransformation()
        {
            SolidifyTextureTransformation textureTransformation = ScriptableObject.CreateInstance<SolidifyTextureTransformation>();
            textureTransformation.Init();
            return textureTransformation;
        }

        public virtual Texture2D ApplyTransformation(Texture2D texture, bool apply = true, bool destroyOriginal = true)
        {
            throw new UnityException("Unhandled virtual method");
        }

        public virtual TextureTransformation Clone()
        {
            throw new UnityException("Unhandled virtual method");
        }
    }
}