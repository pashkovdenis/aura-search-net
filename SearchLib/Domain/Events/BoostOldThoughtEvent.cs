using AuraSearch.Abstractions;

namespace AuraSearch.Domain.Events
{
    public sealed class BoostOldThoughtEvent : IDomainEvent
    {
        public Guid EventId { get; set; } = Guid.NewGuid(); 

        public DateTimeOffset Created { get; set; } = DateTimeOffset.Now;    



    }
}
