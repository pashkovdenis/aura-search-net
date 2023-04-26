using AuraSearch.Models;

namespace AuraSearch.Abstractions
{
    public interface IUseCase<in TUseCaseInput> where TUseCaseInput : IUseCaseInput

    {
        /// <summary>
        /// Executes the Use Case.
        /// </summary>
        /// <param name="input">Input Message.</param>
        /// 
        /// <returns>Task.</returns>
        Task Execute(TUseCaseInput input, Context context, CancellationToken cancellationToken);
    }
}
