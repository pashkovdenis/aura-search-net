using AuraSearch.Abstractions;
using AuraSearch.Models;
using AuraSearch.UseCases.Reinforce.Request;

namespace InputTokenizerFilter
{
    public sealed class ReinforceTokenizerFilter : IInputFilter<ReinforceRequest>
    {


        public ValueTask FilterAsync(ReinforceRequest input, Context context, CancellationToken cancellationToken)
        {

            var tokenizer = new StringTokenizer(input.OriginalRequest);
            var tokens = new List<Token>();
            var token = tokenizer.Next();
            do
            {
                tokens.Add(token);
                token = tokenizer.Next();
            } while (token.Kind != TokenKind.EOF);

            var result = tokens.Where(t => t.Kind == TokenKind.Word || t.Kind == TokenKind.Number).Select(t => t.Value).ToArray();

            input.Keywords = result;

            return ValueTask.CompletedTask;
        }

    }
}
