using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Twity.Helpers
{
    public class Helper
    {
        public static SortedDictionary<string, string> ConvertToSortedDictionary(Dictionary<string, string> APIParams)
        {
            SortedDictionary<string, string> parameters = new SortedDictionary<string, string>();
            foreach (KeyValuePair<string, string> APIParam in APIParams)
            {
                parameters.Add(APIParam.Key, APIParam.Value);
            }
            return parameters;
        }

        public static string GenerateRequestparams(SortedDictionary<string, string> parameters)
        {
            StringBuilder requestParams = new StringBuilder();
            foreach (KeyValuePair<string, string> param in parameters)
            {
                requestParams.Append(Helper.UrlEncode(param.Key) + "=" + Helper.UrlEncode(param.Value) + "&");
            }
            requestParams.Length -= 1; // Remove "&" at the last of string
            return requestParams.ToString();
        }

        public static string UrlEncode(string original)
        {
            if (string.IsNullOrEmpty(original)) return "";

            string encoded = Uri.EscapeDataString(original);
            encoded = Regex.Replace(encoded, "(%[0-9a-f][0-9a-f])", c => c.Value.ToUpper());
            encoded = encoded
                .Replace("(", "%28")
                .Replace(")", "%29")
                .Replace("$", "%24") // May be unnecessary
                .Replace("!", "%21") // May be unnecessary
                .Replace("*", "%2A")
                .Replace("'", "%27");
            encoded = encoded.Replace("%7E", "~"); // May be unnecessary
            return encoded;

        }
    }

}
