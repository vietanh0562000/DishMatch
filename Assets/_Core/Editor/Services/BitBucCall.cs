using System.Collections.Generic;
using System.Net.Http;
using BasePuzzle.Core.Editor.Payloads;
using BasePuzzle.Core.Scripts.Utils;
using BasePuzzle.Core.Scripts.Utils.FActions.Variances.Starts;

namespace BasePuzzle.Core.Editor.Services
{
    using BasePuzzle.Core.Editor.Payloads;
    using BasePuzzle.Core.Scripts.Utils;
    using BasePuzzle.Core.Scripts.Utils.FActions.Variances.Starts;

    public static class BitBucCall
    {
        public static List<BitBucObj> OfUrl(string url)
        {
            List<BitBucObj> result = new List<BitBucObj>();
            
            var currentUrl = url;

            while (!string.IsNullOrEmpty(currentUrl))
            {
                BitBucResponse response = JsonUtil.FromJson<BitBucResponse>(new HttpRequest
                {
                    RequestType = HttpMethod.Get,
                    URL = currentUrl
                }.InvokeAndGet());
                result.AddRange(response.Values);

                currentUrl = response.Next;
            }

            return result;
        }
    }
}