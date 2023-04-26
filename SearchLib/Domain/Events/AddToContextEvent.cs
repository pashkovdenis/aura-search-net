using AuraSearch.Abstractions;
using AuraSearch.ValueObject;

namespace AuraSearch.Domain.Events
{
    public sealed class AddToContextEvent : IDomainEvent
    {
        public Guid EventId { get; set; } = Guid.NewGuid();

        public DateTimeOffset Created { get; set; } = DateTimeOffset.Now; 

        public Guid ContextId { get; set; }  

        public List<ContextEntry> Tokens { get; set; }
    }
}
