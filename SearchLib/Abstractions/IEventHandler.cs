namespace AuraSearch.Abstractions
{
    public interface IEventHandler <T>  where T: IDomainEvent
    {
        ValueTask HandleAsync(T @event, CancellationToken cancellationToken = default);

    }

   
}
