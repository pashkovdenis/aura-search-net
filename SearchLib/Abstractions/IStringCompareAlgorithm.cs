using AuraSearch.ValueObject;

namespace AuraSearch.Abstractions
{
    public interface IStringCompareAlgorithm
    {

        /// <summary>
        /// List of keywords to search for
        /// </summary>
        string[] Keywords { get; set; }


        /// <summary>
        /// Searches passed text and returns all occurrences of any keyword
        /// </summary>
        /// <param name="text">Text to search</param>
        /// <returns>Array of occurrences</returns>
        StringSearchResult[] FindAll(string text);
         
    }
}
