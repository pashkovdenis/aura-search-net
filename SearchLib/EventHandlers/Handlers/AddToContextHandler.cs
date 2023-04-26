using AuraSearch.Abstractions;
using AuraSearch.Domain.Events;

namespace AuraSearch.EventHandlers.Handlers
{
    public sealed class AddToContextHandler : IEventHandler<AddToContextEvent>
    {
        private readonly IContextAccessor _contextAccessor;

        public AddToContextHandler(IContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
        
        public async ValueTask HandleAsync(AddToContextEvent @event,
            CancellationToken cancellationToken = default)
        {
            var context = await _contextAccessor.GetContextAsync(@event.ContextId, cancellationToken);

            if (context == null) return; 

            foreach( var item in @event.Tokens)
            {
                context.AddToContext(item.Label, item.Weight, item.Pros, item.Cons, item.AvgScore);
            }
        }
    }
}
