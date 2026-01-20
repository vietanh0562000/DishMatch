using UnityEngine;

namespace TRS.CaptureTool.Extras
{
    public static class PathExtensions
    {
        public static string MimeTypeForFilePath(string filePath)
        {
            string extension = System.IO.Path.GetExtension(filePath);
            switch (extension)
            {
                case ".png":
                    return "image/png";
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".tga":
                    return "image/x-targa";
                case ".gif":
                    return "image/gif";
                case ".mp4":
                    return "video/mp4";
                default:
                    throw new UnityException("Unhandled Extension");
            }
        }

        public static string GetFilePath(string path, string extension, string targetFileName)
        {
            string[] filePaths = System.IO.Directory.GetFiles(path, extension, System.IO.SearchOption.AllDirectories);
            foreach (string filePath in filePaths)
            {
                string filename = System.IO.Path.GetFileName(filePath);
                if (filename == targetFileName)
                    return filePath;
            }

            return null;
        }
    }
}