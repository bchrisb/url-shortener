using System;

namespace UrlShortener
{
    public interface ICodeGenerator
    {
        string Generate();

        string GetShortCode(string code);
    }

    public class CodeGenerator : ICodeGenerator
    {
        private static readonly Random Random = new Random();
        
        public string Generate()
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";

            string code = null;
            
            for (var i = 0; i < 7; i++)
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
