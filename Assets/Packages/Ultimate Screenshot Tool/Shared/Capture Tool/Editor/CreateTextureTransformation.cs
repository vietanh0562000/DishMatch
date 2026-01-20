using UnityEngine;
using UnityEditor;

using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool
{
    public class CreateTextureTransformation : EditorWindow
    {
        const string folderName = "TextureTransformations";

        [MenuItem("Assets/Create/Texture Transformation/Cutout")]
        public static TextureTransformation CreateCutoutTextureTransformation()
        {
            CutoutTextureTransformation asset = ScriptableObject.CreateInstance<CutoutTextureTransformation>();
            return CreateWithFileName(asset, "CutoutTextureTransformation.asset");
        }

        [MenuItem("Assets/Create/Texture Transformation/Layer Behind")]
        public static TextureTransformation CreateLayerBehindTextureTransformation()
        {
            LayerBehindTextureTransformation asset = ScriptableObject.CreateInstance<LayerBehindTextureTransformation>();
            return CreateWithFileName(asset, "LayerBehindTextureTransformation.asset");
        }

        [MenuItem("Assets/Create/Texture Transformation/Layer In Front")]
        public static TextureTransformation CreateLayerInFrontTextureTransformation()
        {
            LayerInFrontTextureTransformation asset = ScriptableObject.CreateInstance<LayerInFrontTextureTransformation>();
            return CreateWithFileName(asset, "LayerInFrontTextureTransformation.asset");
        }

        [MenuItem("Assets/Create/Texture Transformation/Language Specific Layer Behind")]
        public static TextureTransformation CreateLanguageSpecificLayerBehindTextureTransformation()
        {
            LanguageSpecificLayerBehindTextureTransformation asset = ScriptableObject.CreateInstance<LanguageSpecificLayerBehindTextureTransformation>();
            return CreateWithFileName(asset, "LanguageSpecificLayerBehindTextureTransformation.asset");
        }

        [MenuItem("Assets/Create/Texture Transformation/Language Specific Layer In Front")]
        public static TextureTransformation CreateLanguageSpecificLayerInFrontTextureTransformation()
        {
            LanguageSpecificLayerInFrontTextureTransformation asset = ScriptableObject.CreateInstance<LanguageSpecificLayerInFrontTextureTransformation>();
            return CreateWithFileName(asset, "LanguageSpecificLayerInFrontTextureTransformation.asset");
        }

        [MenuItem("Assets/Create/Texture Transformation/Material")]
        public static TextureTransformation CreateMaterialTextureTransformation()
        {
            MaterialTextureTransformation asset = ScriptableObject.CreateInstance<MaterialTextureTransformation>();
            return CreateWithFileName(asset, "MaterialTextureTransformation.asset");
        }

        [MenuItem("Assets/Create//Texture Transformation/Resize")]
        public static TextureTransformation CreateResizeTextureTransformation()
        {
            ResizeTextureTransformation asset = ScriptableObject.CreateInstance<ResizeTextureTransformation>();
            return CreateWithFileName(asset, "ResizeTextureTransformation.asset");
        }

        [MenuItem("Assets/Create//Texture Transformation/Rotate")]
        public static TextureTransformation CreateRotateTextureTransformation()
        {
            RotateTextureTransformation asset = ScriptableObject.CreateInstance<RotateTextureTransformation>();
            return CreateWithFileName(asset, "RotateTextureTransformation.asset");
        }

        [MenuItem("Assets/Create/Texture Transformation/Shader")]
        public static TextureTransformation CreateShaderTextureTransformation()
        {
            ShaderTextureTransformation asset = ScriptableObject.CreateInstance<ShaderTextureTransformation>();
            return CreateWithFileName(asset, "ShaderTextureTransformation.asset");
        }

        [MenuItem("Assets/Create/Texture Transformation/Solidify")]
        public static TextureTransformation CreateSolidiftTextureTransformation()
        {
            SolidifyTextureTransformation asset = ScriptableObject.CreateInstance<SolidifyTextureTransformation>();
            return CreateWithFileName(asset, "SolidifyTextureTransformation.asset");
        }

        public static TextureTransformation CreateWithFileName(TextureTransformation asset, string fileName)
        {
            if (!fileName.EndsWith(".asset", System.StringComparison.InvariantCulture))
                fileName += ".asset";

            string finalFilePath = AssetDatabase.GenerateUniqueAssetPath(NewScriptableObjectPath.AssetDatabaseDirectory(folderName) + "/" + fileName);
            AssetDatabase.CreateAsset(asset, finalFilePath);
            AssetDatabase.SaveAssets();
            Debug.Log("Saved TextureTransformation to: " + finalFilePath);
            return asset;
        }
    }
}