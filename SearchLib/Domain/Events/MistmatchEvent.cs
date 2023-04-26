using AuraSearch.Abstractions;

namespace AuraSearch.Domain.Events
{
    public sealed class MistmatchEvent : IDomainEvent
    {
        public Guid EventId { get; set; } = Guid.NewGuid(); 

        public DateTimeOffset Created { get; set; } = DateTimeOffset.Now;

        public Guid ThoughtId { get; set; }

        public Guid IdeaId { get; set; }
    }
}
