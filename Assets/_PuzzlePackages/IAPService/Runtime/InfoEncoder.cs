using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace BasePuzzle.PuzzlePackages.IAPService
{
    public class UserInfo
    {
        public readonly string accountId;
        public readonly string profileId;
        public readonly string userName;

        public UserInfo(string profileId, string accountId, string userName)
        {
            this.accountId = accountId;
            this.profileId = profileId;
            this.userName = userName;
        }
    }
    
    public static class InfoEncoder
    {
        public static string HashString(string input)
        {
            var stringBuilder = new StringBuilder();
            foreach (var b in GetHash(input))
                stringBuilder.Append(b.ToString("X2"));

            return stringBuilder.ToString();
        }
    
        private static IEnumerable<byte> GetHash(string input)
        {
            using HashAlgorithm algorithm = SHA256.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
        }
    }
}
