
using AuraSearch.UseCases.Search.Response;

namespace AuraSearch.UseCases.Search
{
    public interface ISearchOutput: IOutputFail, IOutputSuccess<ResultResponse>
    {

        ResultResponse GetLatestResult(Guid clientIdentifier);


    }
}
