namespace TRS.CaptureTool.Extras
{
    public static class StringExtensions
    {
        public static string Capitalize(this string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                return string.Empty;
            }
            char[] wordCharArray = word.ToCharArray();
            wordCharArray[0] = char.ToUpper(wordCharArray[0]);
            return new string(wordCharArray);
        }
    }
}