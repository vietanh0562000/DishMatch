using UnityEngine;

namespace TRS.CaptureTool
{
    [System.Serializable]
    public enum FrameResizeMethod
    {
        ResizeFrameToFitTexture,
        ResizeTextureToFitFrame,
        ResizeBothToFitOriginalResolution,
    }

    [System.Serializable]
    public class AddFrameTextureTransformation : TextureTransformation
    {
        public Texture2D frame;
        public FrameResizeMethod frameResizeMethod = FrameResizeMethod.ResizeBothToFitOriginalResolution;
        public Color backgroundFillColor = Color.clear;

        public void Init(Texture2D frame, FrameResizeMethod frameResizeMethod, Color backgroundFillColor)
        {
            this.frame = frame;
            this.frameResizeMethod = frameResizeMethod;
            this.backgroundFillColor = backgroundFillColor;
        }

        public override string TransformationName()
        {
            return "Add Frame";
        }

        public override string[] PropertyNames()
        {
            return new string[] { "frame", "frameResizeMethod", "backgroundFillColor" };
        }

        public override string LabelForPropertyName(string propertyName)
        {
            if (propertyName.StartsWith("Element")) return propertyName;

            switch (propertyName)
            {
                case "size":
                    return "Size";
                case "frame":
                    return "Frame";
                case "frameResizeMethod":
                    return "Frame Resize Method";
                case "backgroundFillColor":
                    return "Background Fill Color";
                default:
                    throw new UnityException("Unhandled property name: " + propertyName);
            }
        }

        public override Texture2D ApplyTransformation(Texture2D texture, bool apply = true, bool destroyOriginal = true)
        {
            if (!active) return texture;

            return texture.AddFrame(frame, frameResizeMethod, backgroundFillColor, apply, destroyOriginal);
        }

        public override TextureTransformation Clone()
        {
            AddFrameTextureTransformation clone = ScriptableObject.CreateInstance<AddFrameTextureTransformation>();
            clone.active = this.active;
            clone.frameResizeMethod = this.frameResizeMethod;
            return clone;
        }
    }
}