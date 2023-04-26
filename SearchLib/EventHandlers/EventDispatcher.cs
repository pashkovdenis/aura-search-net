using AuraSearch.Abstractions;

namespace AuraSearch.EventHandlers
{
    public abstract class EventDispatcher
    {
        private static AsyncLocal<EventDispatcher> _dispatcher = new();
        public static EventDispatcher Instance => _dispatcher.Value;

        public static void SetInstance (EventDispatcher instance) 
        { 
            _dispatcher.Value = instance;
        }

        public abstract void Dispatch(IDomainEvent @event);
    }
}
