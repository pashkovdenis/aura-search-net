using AuraSearch.ValueObject;

namespace AuraSearch.Abstractions
{
    /// <summary>
    /// Symbol comparer
    /// </summary>
    public interface ISymbolComparer
    {

        /// <summary>
        /// Compare words and existing symbols and return match score.
        /// </summary>
        /// <param name="words"></param>
        /// <param name="symbols"></param>
        /// <returns></returns>
        double Compare(IEnumerable<string> words, IEnumerable<Symbol> symbols); 
    }
}
