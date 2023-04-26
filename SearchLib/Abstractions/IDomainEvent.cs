namespace AuraSearch.Abstractions
{
    public interface IDomainEvent
    { 
        Guid EventId { get; }
        DateTimeOffset Created { get; } 
    }
}
