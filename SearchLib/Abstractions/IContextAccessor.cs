using AuraSearch.Models;

namespace AuraSearch.Abstractions
{
    public interface IContextAccessor
    { 
        ValueTask<Context> GetContextAsync(Guid clientIdentifier, CancellationToken cancellationToken);
        ValueTask<IReadOnlyCollection<Context>> GetAllAsync(CancellationToken cancellationToken);

        void SetContext(Context context);   

        Task GetAllAsync();
    }
}
