using AuraSearch.Abstractions;
using AuraSearch.Domain;
using AuraSearch.Domain.Events;
using AuraSearch.EventHandlers;
using AuraSearch.Models;
using AuraSearch.UseCases.Reinforce.Request;
using AuraSearch.Utils;
using AuraSearch.ValueObject;
using Microsoft.Extensions.Options;

namespace AuraSearch.UseCases.Reinforce
{
    public sealed class ReinforceUseCase : IUseCase<ReinforceRequest>
    {
        private readonly IThoughtRepository _thoughtRepository;
        private readonly IWordRepository _wordsRepository;
        private readonly IEnumerable<IInputFilter<ReinforceRequest>> _filters; 

        private readonly Settings _settings;

        public ReinforceUseCase(
            IThoughtRepository thoughtRepository,
            IEnumerable<IInputFilter<ReinforceRequest>> filters,
            IOptions<Settings> settings,
            IWordRepository wordsRepository)
        {
            _thoughtRepository = thoughtRepository;
            _filters = filters;
            _settings = settings?.Value ?? new Settings();
            _wordsRepository = wordsRepository;
        }
       
        public async Task Execute(ReinforceRequest input, Context context,  CancellationToken cancellationToken)
        {
            input.Validate();

            if (_filters.Any())
            {
                foreach (var filter in _filters)
                {
                    await filter.FilterAsync(input, context, cancellationToken);
                }
            }

            switch (input.Type)
            {
                case ReinforceType.Append:
                    {

                        await Append(input, context, cancellationToken);
                        break;

                    }
                case ReinforceType.Boost:
                    {

                        await BoostThought(input, context, cancellationToken);
                        break;
                    }

                case ReinforceType.SelectedIdea:
                    {
                        await ReinforceIdea(input, context, cancellationToken);
                        break;
                    }
            }

            EventDispatcher.Instance?.Dispatch(new BoostEvent
            {
                IdeaId = input.IdeaId,
                ThoughtId = input.ThoughtId
            });
        }

    
        private async Task BoostThought(ReinforceRequest input, Context context, CancellationToken cancellationToken)
        {
            var thought = await _thoughtRepository.GetById(input.ThoughtId.ToString(), cancellationToken);

            thought.Boost += _settings.NeuronReinforce;

            await _thoughtRepository.Update(thought, cancellationToken);
        }

       
        private async Task ReinforceIdea(ReinforceRequest input, Context context, CancellationToken cancellationToken)
        {
            var thought = await _thoughtRepository.GetById(input.ThoughtId.ToString(), cancellationToken);
            var idea = thought.Ideas.First(x => x.IdeaId == input.IdeaId);

            idea.Symbols = await GetSymbols(input.Keywords, context, cancellationToken, true);

            await _thoughtRepository.Update(thought, cancellationToken);
        }
 
        private async Task Append(ReinforceRequest input, Context context, CancellationToken cancellationToken)
        {
            var thought = await _thoughtRepository.GetById(input.ThoughtId.ToString(), cancellationToken);
            var symbols = await GetSymbols(input.Keywords, context, cancellationToken); 

            var idea = new Idea
            {
                Overview = input.Addition,
                Symbols = symbols
            }; 

            thought.Ideas.Add(idea);

            if (input.Tags != null && input.Tags.Count > 0) { 
            
                thought.Tags = input.Tags;
            }


            await _thoughtRepository.Update(thought, cancellationToken);
        }
 
        private async Task<List<Symbol>> GetSymbols(string[] words, Context context, CancellationToken cancellationToken, bool addReinforce = false)
        {
            var wordsAll = await _wordsRepository.GetAllWords(cancellationToken: cancellationToken);
            var newWords = words.Where(x => !wordsAll.Any(w => w.Token == x)).Distinct().ToList();
            var existingWords = wordsAll.Where(x => words.Contains(x.Token)).DistinctBy(x=>x.Token).ToList();

            var newSymbolDominance = GetSymbolWeight(1);

            var result = new List<Symbol>();
            var tasks = new List<Task>();

            foreach (var newWord in newWords)
            {
                result.Add(new Symbol
                {
                    Weight = newSymbolDominance,
                    Word = newWord
                });

                tasks.Add(_wordsRepository.Insert(new Word
                {
                    Token = newWord,
                    Count = 1
                }));
            }

            foreach (var existingWord in existingWords)
            {
                result.Add(new Symbol
                {
                    Weight = addReinforce ? GetSymbolWeight(existingWord.Count) + _settings.NeuronReinforce : GetSymbolWeight(existingWord.Count + 1),
                    Word = existingWord.Token
                });

                if (!addReinforce)
                {

                    existingWord.Count++;
                    tasks.Add(_wordsRepository.Update(existingWord)); 
                }

            }

            var contextEntries = context.GetContextEntries();

            if (contextEntries.Any())
            {
                foreach (var (Label, Score) in contextEntries)
                {
                    result.Add(new Symbol
                    {
                        Type = Enumerations.SymbolType.Context,
                        Word = Label,
                        Weight = Score
                    });
                }
            }

            await Task.WhenAll(tasks);

            return result;
        }

        private double GetSymbolWeight(double bias) 
            => MathUtils.Normilize( _settings.DefNeuronDom + bias * (_settings.NeuronDominanceWeigh * _settings.NeuronReinforce));
    }
}
