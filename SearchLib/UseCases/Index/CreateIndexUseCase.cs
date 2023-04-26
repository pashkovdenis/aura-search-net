using AuraSearch.Abstractions;
using AuraSearch.Domain;
using AuraSearch.Models;
using AuraSearch.UseCases.Index.Request;
using AuraSearch.ValueObject;

namespace AuraSearch.UseCases.Index
{
    public sealed class CreateIndexUseCase : IUseCase<StoreIndexRequest>
    {
        private readonly IThoughtRepository _thoughtRepository;
        private readonly IEnumerable<IInputFilter<StoreIndexRequest>> _filters;

        public CreateIndexUseCase(
            IThoughtRepository thoughtRepository,
            IEnumerable<IInputFilter<StoreIndexRequest>> filters)
        {
            _thoughtRepository = thoughtRepository;
            _filters = filters;
        }

        public async Task Execute(StoreIndexRequest input, Context context, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            input.Validate();

            if (_filters != null && _filters.Any())
            {
                foreach (var filter in _filters)
                {
                    await filter.FilterAsync(input, context, cancellationToken);
                }
            }
       
            await CreateSingleIndex(input, context, cancellationToken);
        }

        private async ValueTask CreateSingleIndex(StoreIndexRequest input, Context context, CancellationToken cancellationToken)
        {
            var payload = input.Payloads.First();
            var symbols = payload.Symbols;
            var contextEntries = context.GetContextEntries();

            if (contextEntries.Any())
            {
                foreach (var (Label, Score) in contextEntries)
                {
                    symbols.Add(new Symbol
                    {
                        Type = Enumerations.SymbolType.Context,
                        Word = Label,
                        Weight = Score
                    });
                }
            }
 

            var thought = new Thought
            {
                Label = payload.Title,
                ClientIdentifier = context.ClientIdentifier,
                Tags = input.Tags,
                Ideas = new List<Idea>
                 {
                     new Idea
                     {
                          Overview = payload.Document,
                          Reference = payload.Reference,
                          Symbols = symbols.ToList()
                     }
                 }
            };

            await _thoughtRepository.Insert(thought);
        } 
    }
}
