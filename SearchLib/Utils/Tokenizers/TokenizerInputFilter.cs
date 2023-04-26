using AuraSearch.Abstractions;
using AuraSearch.Models;
using AuraSearch.UseCases.Index.Request;

namespace InputTokenizerFilter
{
    public sealed class TokenizerInputFilter : IInputFilter<StoreIndexRequest>
    {
        public ValueTask FilterAsync(StoreIndexRequest input, Context context, CancellationToken cancellationToken)
        {
             
            foreach(var payload in input.Payloads)
            {
                payload.KeyWords = GetKeys(payload.Title);
            }
            
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

            return tokens.Where(t=>t.Kind == TokenKind.Word || t.Kind == TokenKind.Number).Select(t=>t.Value).ToArray();    
        }
        
    }
}
