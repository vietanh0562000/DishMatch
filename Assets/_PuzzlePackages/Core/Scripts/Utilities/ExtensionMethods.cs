using System.Collections.Generic;
using Random = System.Random;

namespace BasePuzzle.PuzzlePackages.Core
{
    public static class ExtensionMethods
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            Random rand = new Random();
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = rand.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }
    }
}
