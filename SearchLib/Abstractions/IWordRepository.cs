using AuraSearch.Domain;

namespace AuraSearch.Abstractions
{
    public interface IWordRepository : IRepository<Word>
    {

        Task<IQueryable<Word>> GetAllWords(bool includeNoise = false, CancellationToken cancellationToken = default);

    }
}
