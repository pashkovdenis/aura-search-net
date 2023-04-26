using AuraSearch.Abstractions;

namespace AuraSearch.Domain.Events
{
    public record ActivationEvent : IDomainEvent
    {
        public Guid EventId { get; } = Guid.NewGuid();

        public DateTimeOffset Created { get; } = DateTimeOffset.UtcNow;

        public double Score { get; set; } 

        public Guid ThoughtId { get; set; }

        public Guid IdeaId { get; set; }
    }
}
