using AuraSearch.Abstractions;

namespace AuraSearch.Domain.Events
{
    public sealed class RefreshEvent : IDomainEvent
    {
        public Guid EventId { get; set; } = Guid.NewGuid(); 

        public DateTimeOffset Created => DateTimeOffset.UtcNow;
    }
}
