
using UnityEngine;

namespace BasePuzzle.PuzzlePackages.Core
{
    public static class LocalizationHelper
    {
        private const string _REPLACE_1 = "{place_1}";
        public static string GetTranslation(string term)
        {
            return term;
        }

        public static string Replace1(this string str, string value)
        {
            return str.Replace(_REPLACE_1, value);
        }
    }
}
