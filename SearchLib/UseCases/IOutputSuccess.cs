using AuraSearch.Abstractions;


namespace AuraSearch.UseCases
{
    public interface IOutputSuccess<in TUseCaseOutput>
        where TUseCaseOutput : IUseCaseOutput
    {
        void Ok(TUseCaseOutput output);
    }
}
