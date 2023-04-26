using AuraSearch.Abstractions;
using AuraSearch.ValueObject;

namespace AuraSearch.Comparers
{
    public sealed class DefaultSymbolComparer : ISymbolComparer
    {
        private readonly IStringCompareAlgorithm _stringComparer;

        public DefaultSymbolComparer(IStringCompareAlgorithm stringComparer)
        {
            _stringComparer = stringComparer;
        }

        public double Compare(IEnumerable<string> words, IEnumerable<Symbol> symbols)
        {
            _stringComparer.Keywords = symbols.Select(x => x.Word).ToArray();

            var searchStr = string.Join(" ", words);

            var searchResults = _stringComparer.FindAll(searchStr);

            var matchedSymbols = symbols.Where(x => searchResults.Any(s => s.Keyword.Equals(x.Word, StringComparison.InvariantCultureIgnoreCase))).ToList();

            if (matchedSymbols.Count == 0)
            {
                return 0;
            }
                 
            return matchedSymbols.Sum(x => x.Weight);
        }
    }
}
