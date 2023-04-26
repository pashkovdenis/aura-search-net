using AuraSearch.Abstractions;
using AuraSearch.Domain.Events;
using Microsoft.Extensions.Logging;

namespace AuraSearch.EventHandlers.Handlers
{
    public sealed class BoostOldThoughtHandler : IEventHandler<BoostOldThoughtEvent>
    {
        private readonly IContextAccessor _contextAccessor;
        private readonly IThoughtRepository _thoughtRepository;
        private readonly ILogger _logger; 

        public BoostOldThoughtHandler(IThoughtRepository thoughtRepository, IContextAccessor contextAccessor, ILoggerFactory loggerFactory)
        {
            _thoughtRepository = thoughtRepository;
            _contextAccessor = contextAccessor;
            _logger = loggerFactory.CreateLogger<BoostOldThoughtHandler>();
        }

        public async ValueTask HandleAsync(BoostOldThoughtEvent @event, CancellationToken cancellationToken = default)
        {
            var contexts = await _contextAccessor.GetAllAsync(cancellationToken);

            foreach (var context in contexts)
            {
                try
                {
                    var thoughts = (await _thoughtRepository.FindByPredicate(x => x.ClientIdentifier == context.ClientIdentifier && x.Metrics != null))
                        .Where(x=> (DateTimeOffset.Now - x.Metrics.LastTimeHit).TotalDays >= 5).OrderBy(x=> Guid.NewGuid()).Take(5);

                    foreach (var thought in thoughts)
                    {
                        context.AddToContext(
                            thought.Id.ToString(), 
                            thought.Metrics.AvgScore, 
                            thought.Metrics.MatchCount, 
                            thought.Metrics.DismatchCount, 
                            thought.Metrics.AvgScore);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Boost error"); 
                    throw;
                }
            }
             
        }
    }
}
