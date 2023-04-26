using AuraSearch.Abstractions;
using AuraSearch.Domain;
using AuraSearch.Utils;

namespace AuraSearch.Jobs
{
    public sealed class NoiseDetectionJob : IJob
    {
        private readonly IWordRepository _wordsRepository;
        private const double NoiseWordThreshold = 2;
        private const double MinimumSymbolVariance = 10.0d;

        public NoiseDetectionJob(IWordRepository wordsRepository)
        {
            _wordsRepository = wordsRepository;
        }

        /// <summary>
        /// Detect noise words using standart deviation approach
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask RunAsync(CancellationToken cancellationToken = default)
        {
            var sDeviation = await CalculateCountDeviationAsync();

            if (Math.Pow(sDeviation, 2) < MinimumSymbolVariance)
            {
                return;
            }

            var tCount = NoiseWordThreshold * sDeviation;

            var symbols = await _wordsRepository.GetAll();

            var updateList = new List<Task>();

            foreach (var symbol in symbols)
            {
                var isNoise = symbol.Count > tCount;

                if (symbol.IsNoiseSymbol != isNoise)
                {
                    symbol.IsNoiseSymbol = isNoise;

                    updateList.Add(_wordsRepository.Update(symbol));
                }
            }

            await Task.WhenAll(updateList);
        }
         
        private async Task<double> CalculateCountDeviationAsync()
        {
            var allSymbols = (await _wordsRepository.GetAll()).ToList();

            var vector = allSymbols.Select(w => w.Count);

            return MathUtils.CalcDeviation(vector.ToList().AsReadOnly());
        }
    }
}
