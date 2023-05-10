using AuraSearch.Abstractions;
using AuraSearch.Domain;
using AuraSearch.Domain.Events;
using AuraSearch.EventHandlers;
using AuraSearch.Models;
using AuraSearch.UseCases.Search.Request;
using AuraSearch.Utils;
using AuraSearch.ValueObject;
using Microsoft.Extensions.Options;
using System.Runtime.InteropServices;

namespace AuraSearch.UseCases.Search
{
    public sealed class SearchUseCase : IUseCase<SearchRequest>
    {
        private readonly ISearchOutput _searchOutput;
        private readonly IThoughtRepository _thoughtRepository;
        private readonly ISymbolComparer _symbolComparer;
        private readonly IEnumerable<IInputFilter<SearchRequest>> _filters;
        private readonly IEnumerable<IResultFilter> _resultFilters;
        private readonly Settings _settings;
         
        public SearchUseCase(
            ISearchOutput searchOutput,
            IThoughtRepository thoughtRepository,
            ISymbolComparer symbolComparer,
            IEnumerable<IInputFilter<SearchRequest>> filters,
            IEnumerable<IResultFilter> resultFilters,
            IOptions<Settings> settings)
        {
            _searchOutput = searchOutput;
            _thoughtRepository = thoughtRepository;
            _symbolComparer = symbolComparer;
            _filters = filters;
            _resultFilters = resultFilters;
            _settings = settings?.Value ?? new Settings();
        }

        public async Task Execute(SearchRequest input, Context context, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) return;

            input.Validate();

            if (_filters != null && _filters.Any())
            {
                foreach (var filter in _filters)
                {
                    await filter.FilterAsync(input, context, cancellationToken);
                }
            }

            if (context.ClientIdentifier != input.ClientIdentifier)
            {
                throw new InvalidOperationException("Context identifier and client id mistmatch"); 
            }

            var thoughts = await _thoughtRepository.GetPreMatchBySymbols(input.KeyWords, context.ClientIdentifier, cancellationToken); 
            
            if (!thoughts.Any())
            {
                _searchOutput.Error("No results found");
                return; 
            }
             
            var results = ScoreMatches(thoughts, input,context, cancellationToken);

            var vector = new ResultVector
            {
                RequestId = input.RequestId,
                Succeeded = results.Count > 0, 
                Results = results.OrderByDescending(x=>x.Score).ToList()
            };

            if (_resultFilters != null && _resultFilters.Any())
            {
                foreach(var filter in _resultFilters) 
                { 
                  await filter.FilterAsync(vector, context, cancellationToken);
                }
            }

            _searchOutput.Ok(new Response.ResultResponse
            {
                 ClientId = input.ClientIdentifier,
                 RequestId= input.RequestId,
                 Result = vector
            });
        }

        private List<ResultValue> ScoreMatches(
            List<Thought> thoughts, 
            SearchRequest request,
            Context context, 
            CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                throw new OperationCanceledException();
            }
             
            var contextEntries = context.GetContextEntries();
            var keywords = request.KeyWords;
            var results = new List<ResultValue>();  
            
            foreach(var thought in CollectionsMarshal.AsSpan<Thought>(thoughts))
            {

                var resultPoint = new ResultValue { Thought = thought };
                var set = new List<(ThoughtMetric metrics, Idea idea, double score)>();

                foreach(var idea in thought.Ideas)
                {
                    var compared = _symbolComparer.Compare(keywords, idea.Symbols);

                    var scoreThoughtBoost = contextEntries.Where(x => x.Label == thought.Id.ToString()).Sum(x => x.Score);

                    var matchedContextSum = idea.Symbols.Where(x => contextEntries.Any(c => c.Label == x.Word)).Sum(x=>x.Weight) + scoreThoughtBoost;

                    var score = MathUtils.ActivationFunc(compared + matchedContextSum + thought.Boost);
                    var thresshold = context.GetThresshold(request.Thresshold > 0 ? request.Thresshold : _settings.ActivationThress) + Math.Atan(request.KeyWords.Length / 100); 
                    // Math Sqrt: 
                    if (score > thresshold)
                    { 
                        set.Add((thought.Metrics, idea, score));

                        var contextAddedSymbols  = idea.Symbols.OrderByDescending(x => x.Weight).Take(_settings.WordCounterThresshold);   
                        
                        EventDispatcher.Instance?.Dispatch(new ActivationEvent
                        {
                             ThoughtId = thought.Id,
                             IdeaId = idea.IdeaId,
                             Score = score
                        }); 
                    }
                    else
                    {
                        EventDispatcher.Instance?.Dispatch(new MistmatchEvent
                        {
                             IdeaId = idea.IdeaId,
                             ThoughtId = thought.Id                            
                        });
                    }                    
                }

                if (set.Any())
                {
                    var topIdea = set.OrderByDescending(x => x.score).First();
                    var otherIdeas = set.Where(x => x.idea.IdeaId != topIdea.idea.IdeaId).ToList();
                    resultPoint.OtherIdeas = otherIdeas;
                    resultPoint.TopIdea = topIdea.idea;
                    resultPoint.Score = topIdea.score;
                    results.Add(resultPoint);
                }
            }


            AddToContext(results.Take(_settings.WordCounterThresshold).ToList(), context); 

            return results;
        }


        private static void AddToContext(List<ResultValue> results, Context context)
        {
            foreach (var result in CollectionsMarshal.AsSpan(results))
            {
                context.AddToContext(result.TopIdea.IdeaId.ToString(),
                    result.Score,
                    result.Thought.Metrics.MatchCount,
                    result.Thought.Metrics.DismatchCount,
                    result.Thought.Metrics.AvgScore); 
            }
        }
    }
}
