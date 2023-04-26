using AuraSearch.Abstractions;
using AuraSearch.Models;
using AuraSearch.UseCases.Search.Request;

namespace InputTokenizerFilter
{
    public sealed class TokenizerSearchRequestFilter : IInputFilter<SearchRequest>
    {
        public ValueTask FilterAsync(SearchRequest input, Context context, CancellationToken cancellationToken)
        {
            input.KeyWords = GetKeys(input.Request);           

            return ValueTask.CompletedTask;
        }
        
        private static string[] GetKeys(string input)
        {
            var tokenizer = new StringTokenizer(input);
            var tokens = new List<Token>();
            var token = tokenizer.Next();
            do
            {
                tokens.Add(token);
                token = tokenizer.Next();
            } while (token.Kind != TokenKind.EOF);

            return tokens.Where(t => t.Kind == TokenKind.Word || t.Kind == TokenKind.Number).Select(t => t.Value).ToArray();
        }

    }
}
