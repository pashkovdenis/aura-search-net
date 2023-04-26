namespace AuraSearch.Abstractions
{
    public interface IJob
    { 
        ValueTask RunAsync(CancellationToken cancellationToken = default); 
    }
}
