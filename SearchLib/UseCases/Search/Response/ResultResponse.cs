using AuraSearch.Abstractions;
using AuraSearch.ValueObject;

namespace AuraSearch.UseCases.Search.Response
{
    public sealed class ResultResponse : IUseCaseOutput
    {
        public Guid ClientId { get; set; }

        public Guid RequestId { get; set; }

        public ResultVector Result { get; set; }

    }
}
