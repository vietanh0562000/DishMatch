
namespace PuzzleGames
{
    using System.Linq;
    using Sirenix.Utilities.Editor;
    using UnityEditor;
    using UnityEngine;

    public class TextureProcessor : AssetPostprocessor
    {
        private const string TEXTURE_PATTERN = @"/_Texture/";
        private const string SPRITE_PATTERN = @"/Sprite/";

        private static string[] NormalFilter = new[] { "normal", "nor" }; 
        
        void OnPreprocessTexture()
        {
            if (assetPath.Contains(TEXTURE_PATTERN))
            {
                // TODO if needed
                TextureImporter importer = assetImporter as TextureImporter;
                
                var lower = assetPath.ToLower();
                if (NormalFilter.Any(_ => lower.Contains(_)))
                {
                    importer.textureType = TextureImporterType.NormalMap;
                }
                else
                {
                    importer.textureType = TextureImporterType.Default;
                }

                importer.npotScale     = TextureImporterNPOTScale.None;           // Do not modify texture size.
                importer.mipmapEnabled = false;
                importer.anisoLevel    = 0;                                  // RTS game do need anisoLevel
            }
            else if (assetPath.Contains(SPRITE_PATTERN))                     // This allow us custom import ImageAsset as Sprite
            {
                TextureImporter importer = assetImporter as TextureImporter;

                importer.textureType = TextureImporterType.Sprite;
                TextureImporterSettings textureSettings = new TextureImporterSettings();
                importer.ReadTextureSettings(textureSettings);
                textureSettings.spriteMeshType = SpriteMeshType.FullRect;
                textureSettings.spriteGenerateFallbackPhysicsShape = false;
                importer.SetTextureSettings(textureSettings);
                importer.mipmapEnabled = false;
            }

        }
        
        void OnPostprocessSprites(Texture2D texture, Sprite[] sprites)
        {
            if (sprites == null || sprites.Length == 0) return;
            var sprite = sprites[0];
            if (sprite != null)
            {
                var  size = sprite.rect.size;

                int  w    = (int)size.x;
                int  h    = (int)size.y;
                bool wPOT = Mathf.IsPowerOfTwo(w);
                bool hPOT = Mathf.IsPowerOfTwo(h);

                if (!wPOT || !hPOT)
                {
                    if (w % 4 != 0 || h % 4 != 0)
                    {
                        Debug.LogError($"Sprite `{sprite.name}` is NOT divisible by 4");
                    }
                }
            }
        }
    }
}