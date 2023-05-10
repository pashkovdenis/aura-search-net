using AuraSearch.Abstractions;
using AuraSearch.Domain;
using AuraSearch.Models;
using AuraSearch.UseCases.Index.Request;
using AuraSearch.Utils;
using AuraSearch.ValueObject;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AuraSearch.UseCases.Index.Filter
{
    public sealed class ProduceSymbolsFilter : IInputFilter<StoreIndexRequest>
    {
        private readonly IWordRepository _wordsRepository;
        private ILogger _logger;
        private Settings _settings;

        public ProduceSymbolsFilter(IWordRepository repository, ILoggerFactory loggerFactory, IOptions<Settings> settings)
        {
            _wordsRepository = repository;
            _logger = loggerFactory.CreateLogger<ProduceSymbolsFilter>();
            _settings = settings?.Value ?? new Settings();
        }
        
        public async ValueTask FilterAsync(
            StoreIndexRequest input,
            Context context,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Filtering request {input}");
            input.Validate();

            _logger.LogInformation("Request is valid");

            foreach (var payload in input.Payloads)
            {
                var words = payload.KeyWords.Where(x=>x.Length >= _settings.MinimumSymbolLength).Take(_settings.WordsToIndexThresshold).ToList();
                 
                var result = new List<Symbol>();

                var wordsAll = await _wordsRepository.GetAllWords( cancellationToken:cancellationToken);

                var newWords = words.Where(x => !wordsAll.Any(w => w.Token == x)).Distinct().ToList();

                var existingWords = wordsAll.Where(x => words.Contains(x.Token)).DistinctBy(x=>x.Token).ToList();

                var newSymbolDominance =  GetSymbolWeight(1);

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

                    var symbolDominanceWeight = GetSymbolWeight(1); 

                    if (existingWord.Count + 1 > _settings.WordCounterThresshold)
                    {
                        symbolDominanceWeight = GetSymbolWeight(existingWord.Count + 1);
                        existingWord.Count= 0;  
                    }

                    result.Add(new Symbol
                    {
                        Weight = symbolDominanceWeight,
                        Word = existingWord.Token
                    });

                    existingWord.Count++;
                    tasks.Add(_wordsRepository.Update(existingWord));
                }
  
                payload.Symbols = result;

                _logger.LogInformation($"Added {result.Count} symbols");

                await Task.WhenAll(tasks);
            }
        }

        private double GetSymbolWeight(double bias) 
            => MathUtils.Normilize( _settings.DefNeuronDom + bias * (_settings.NeuronDominanceWeigh * _settings.NeuronReinforce));
    }
}
