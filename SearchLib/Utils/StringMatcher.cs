using AuraSearch.Abstractions;
using AuraSearch.ValueObject;

namespace AuraSearch.Utils
{
    public sealed class StringMatcher : IStringCompareAlgorithm
    {
        public string[] Keywords { get; set; }

        private static List<char> _chars = new List<char> { ' ', '.', ',', '!', '?', ':' }; 
         
        public StringSearchResult[] FindAll(string text)
        { 
            Keywords = text.Split(_chars.ToArray(), StringSplitOptions.RemoveEmptyEntries).ToArray();
            return Keywords.Select(x => new StringSearchResult (0,x)).ToArray();
        }
    }
}
