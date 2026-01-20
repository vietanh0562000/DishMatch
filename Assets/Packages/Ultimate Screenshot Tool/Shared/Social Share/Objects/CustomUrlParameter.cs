namespace TRS.CaptureTool.Share
{
    [System.Serializable]
    public class CustomUrlParameter
    {
        public string displayName;
        public string name;
        public string value;

        public CustomUrlParameter(string displayName, string name, string value)
        {
            this.displayName = displayName;
            this.name = name;
            this.value = value;
        }
    }
}