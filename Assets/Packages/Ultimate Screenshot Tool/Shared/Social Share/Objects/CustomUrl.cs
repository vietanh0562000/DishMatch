using System.Collections.Generic;

using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool.Share
{
    [System.Serializable]
    public class CustomUrl
    {
        public string displayName;
        public string baseUrl;
        public List<CustomUrlParameter> parameters;

        public CustomUrl(string displayName, string baseUrl, List<CustomUrlParameter> parameters)
        {
            this.displayName = displayName;
            this.baseUrl = baseUrl;
            this.parameters = parameters;
        }

        public string FullUrl(Dictionary<string, string> replacements)
        {
            string parametersString = "";
            foreach (CustomUrlParameter parameter in parameters)
            {
                string parameterValue = parameter.value.Trim();
                if (replacements.ContainsKey(parameterValue))
                    parameterValue = replacements[parameterValue];
                if (!string.IsNullOrEmpty(parameterValue))
                    parametersString += "&" + parameter.name + "=" + WWWExtensions.PlusEscapeUrl(parameterValue);
            }
            if (parametersString.Length > 0)
                parametersString.Substring(1);
            return WWWExtensions.CombinedUrl(baseUrl, parametersString);
        }
    }
}
