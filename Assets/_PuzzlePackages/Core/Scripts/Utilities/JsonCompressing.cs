using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using UnityEngine;

namespace BasePuzzle.PuzzlePackages.Core
{
    public static class JsonCompressing
    {
        public static string Compressing(string inputStr)
        {
            if (inputStr == null || inputStr == "") return "";
            try
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(inputStr);
                using (var outputStream = new MemoryStream())
                {
                    using (var gZipStream = new GZipStream(outputStream, CompressionMode.Compress))
                        gZipStream.Write(inputBytes, 0, inputBytes.Length);
                    var outputBytes = outputStream.ToArray();

                    var outputbase64 = Convert.ToBase64String(outputBytes);
                    return outputbase64;
                }
            }
            catch
            {
                return "";
            }
        }

        public static string Decompressing(string inputStr)
        {
            if (inputStr == null || inputStr == "") return "";
            try
            {
                byte[] inputBytes = Convert.FromBase64String(inputStr);

                using (var inputStream = new MemoryStream(inputBytes))
                using (var gZipStream = new GZipStream(inputStream, CompressionMode.Decompress))
                using (var streamReader = new StreamReader(gZipStream))
                {
                    var decompressed = streamReader.ReadToEnd();
                    return decompressed;
                }
            }
            catch
            {
                return "";
            }
        }
    }
}