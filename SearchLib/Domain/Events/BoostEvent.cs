using AuraSearch.Abstractions;

namespace AuraSearch.Domain.Events
{
    public sealed class BoostEvent : IDomainEvent
    {
        public Guid EventId { get; } = Guid.NewGuid();

        public DateTimeOffset Created { get; } = DateTimeOffset.UtcNow; 

        public Guid ThoughtId { get; set; }

        public Guid IdeaId { get; set; } 
    }
}
