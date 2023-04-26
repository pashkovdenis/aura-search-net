using AuraSearch.Abstractions;
using AuraSearch.Domain.Events;
using AuraSearch.Models;

namespace AuraSearch.EventHandlers.Handlers
{

    public sealed class RefreshEventHandler : IEventHandler<RefreshEvent>
    {
        private readonly IContextAccessor _contextAccessor;

        public RefreshEventHandler(IContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public ValueTask HandleAsync(RefreshEvent @event, CancellationToken cancellationToken = default)
        {
            return  RefreshAllSessions(cancellationToken);
        }
        
        private async ValueTask RefreshAllSessions(CancellationToken cancellationToken)
        {
            var contexts = await _contextAccessor.GetAllAsync(cancellationToken);

            foreach (var context in contexts)
            {
                Refresh(context);
            }
        }

        private static void Refresh(Context context)
        {
            context.Refresh();

            var entries = context.GetContextEntries();

            var entriesToRemove = entries.Where(x => x.Score <= 0).ToList();

            foreach (var (label, _) in entriesToRemove)
            {
                context.RemoveFromContext(label);
            }
        }

    }
}
