using AuraSearch.Domain;

namespace AuraSearch.Abstractions
{
    /// <summary>
    /// Thought Accessor 
    /// </summary>
    public interface IThoughtRepository : IRepository<Thought>
    { 
        Task<List<Thought>> GetPreMatchBySymbols(
            IEnumerable<string> words, Guid clientIdentifier, CancellationToken cancellationToken );

    }
}
