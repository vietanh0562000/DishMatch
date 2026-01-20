using UnityEngine;

namespace TRS.CaptureTool.Extras
{
    public static class WWWExtensions
    {
        public static string CombinedUrl(string url, string parameters)
        {
            url = url.Trim();
            parameters = parameters.Trim();

            if (!url.EndsWith("?", System.StringComparison.Ordinal))
                url += "?";

            // Trim first & if arguments exist or trim extra ? at the end of url if they do not
            if (!string.IsNullOrEmpty(parameters))
            {
                parameters = parameters.Trim();
                if (parameters.StartsWith("&", System.StringComparison.Ordinal))
                    parameters = parameters.Substring(1);
                url += parameters;
            }
            else if (!string.IsNullOrEmpty(url))
                url = url.Substring(0, url.Length - 1);

            return url;
        }

        public static string PlusEscapeUrl(string url)
        {
#pragma warning disable 0618
            return WWW.EscapeURL(url).Replace("+", "%20");
#pragma warning restore 0618
        }
    }
}