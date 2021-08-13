namespace UrlShortener
{
    public interface ICodeGenerator
    {
        /// <summary>
        /// Generates a random 8 character alphanumeric code.
        /// </summary>
        /// <returns></returns>
        string Generate();

        /// <summary>
        /// Gets a short version of a code by taking the first two characters.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        string GetShortCode(string code);
    }
}
