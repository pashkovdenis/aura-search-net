using AuraSearch.Abstractions;
using AuraSearch.Enumerations;
using AuraSearch.ValueObject;
using System.ComponentModel.DataAnnotations;

namespace AuraSearch.UseCases.Index.Request
{ 
    public sealed class StoreIndexRequest : IUseCaseInput
    {
        [Required]
        public Guid ClientIdentifier { get; set; }

        [Required]
        public IReadOnlyCollection<Payload> Payloads { get; set; }  

        public List<string> Tags { get; set; }

        public void Validate()
        {
            if (Payloads == null || !Payloads.Any())
            {
                throw new ArgumentNullException(nameof(Payload));
            }
             
            if (ClientIdentifier== Guid.Empty)
            {
                throw new ArgumentNullException(nameof(ClientIdentifier));
            }
        }         
    } 
    
    public sealed class Payload
    {
        [Required]
        public string Title { get; set; }

        public string[] KeyWords { get; set; }

        public Guid Reference { get; set; }
               
        public string Document { get; set; } 

        public IReadOnlyCollection<RequestTag> Tags { get; set; }

        public IList<Symbol> Symbols { get; set; }
    } 
}
