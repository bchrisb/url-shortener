using System;

namespace UrlShortener
{
    public interface ICodeGenerator
    {
        /// <summary>
        /// Generates a random alphanumeric string for the given length.
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        string Generate(int length);

        /// <summary>
        /// Gets a short version of a code by taking the first two characters.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        string GetShortCode(string code);
    }

    public class CodeGenerator : ICodeGenerator
    {
        private static readonly Random Random = new Random();
        
        public string Generate(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";

            string code = null;
            
            for (var i = 0; i < length; i++)
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
