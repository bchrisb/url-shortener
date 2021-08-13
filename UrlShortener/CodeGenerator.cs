using System;

namespace UrlShortener
{
    public class CodeGenerator : ICodeGenerator
    {
        private static readonly Random Random = new Random();
        
        private const int CodeLength = 8;

        public string Generate()
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            string code = null;

            for (var i = 0; i < CodeLength; i++)
            {
                var rand = Random.Next(chars.Length);
                code += chars[rand];
            }

            return code;
        }

        public string GetShortCode(string code)
        {
            return code.Substring(0, 2);
        }
    }
}
