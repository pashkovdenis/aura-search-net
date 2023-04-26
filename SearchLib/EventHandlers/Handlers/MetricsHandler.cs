using AuraSearch.Abstractions;
using AuraSearch.Domain;
using AuraSearch.Domain.Events; 

namespace AuraSearch.EventHandlers.Handlers
{
    public sealed class MismatchMetricHandler : IEventHandler<MistmatchEvent>
    {
        private readonly IThoughtRepository _thoughtRepository;

        public MismatchMetricHandler(IThoughtRepository thoughtRepository)
        {
            _thoughtRepository = thoughtRepository;
        }

        public async ValueTask HandleAsync(MistmatchEvent @event, CancellationToken cancellationToken = default)
        {
            var thought = await _thoughtRepository.GetById(@event.ThoughtId.ToString(), cancellationToken);

            thought.Metrics.LastTimeHit = DateTimeOffset.Now;
            thought.Metrics.DismatchCount++;

            RecalculateMetrics(thought.Metrics);

            await _thoughtRepository.Update(thought, cancellationToken);
        }

        private static void RecalculateMetrics(ThoughtMetric metric) 
            => metric.AvgScore = metric.TotalScore / (metric.MatchCount + metric.DismatchCount);
        
    }
     
    public sealed class MatchMetricHandler : IEventHandler<ActivationEvent>
    {
        private readonly IThoughtRepository _thoughtRepository;

        public MatchMetricHandler(IThoughtRepository thoughtRepository)
        {
            _thoughtRepository = thoughtRepository;
        }
         
        public async ValueTask HandleAsync(ActivationEvent @event, CancellationToken cancellationToken = default)
        {
            var thought = await _thoughtRepository.GetById(@event.ThoughtId.ToString(), cancellationToken);

            thought.Metrics.LastTimeHit = DateTimeOffset.Now;
            thought.Metrics.TotalScore += @event.Score; 
            thought.Metrics.MatchCount++;

            RecalculateMetrics(thought.Metrics);

            await _thoughtRepository.Update(thought, cancellationToken);
        } 

        /// <summary>
        /// Recalculate metrics 
        /// </summary>
        /// <param name="metric"></param>
        private static void RecalculateMetrics(ThoughtMetric metric)
        {
            metric.AvgScore = metric.TotalScore / (metric.MatchCount + metric.DismatchCount);
        } 
    }
}
