using AuraSearch.Models;

namespace AuraSearch.Abstractions
{
    /// <summary>
    /// Filter user input based on request type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IInputFilter <T> where T: IUseCaseInput
    {
        ValueTask FilterAsync(T input, Context context, CancellationToken cancellationToken);
    }
}
