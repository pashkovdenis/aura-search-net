using AuraSearch.Models;
using AuraSearch.ValueObject;

namespace AuraSearch.Abstractions
{
    /// <summary>
    /// Aply filter to result vectors
    /// </summary>
    public interface IResultFilter
    {
        /// <summary>
        /// Filter result vector.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask FilterAsync(ResultVector result, Context context, CancellationToken cancellationToken); 
    }
}
