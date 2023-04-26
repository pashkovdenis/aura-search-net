using AuraSearch.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace AuraSearch.UseCases.Search.Request
{
    public sealed class SearchRequest : IUseCaseInput
    {

        public Guid RequestId { get; } = Guid.NewGuid();

        [Required]
        public Guid ClientIdentifier { get; set; }

        [Required]
        public string Request { get; set; }
        public string[] KeyWords { get; set; }

        public double Thresshold { get; set; } = 0; 

        public void Validate()
        {
            if (string.IsNullOrEmpty(Request))
            {
                throw new ArgumentNullException(nameof(Request));
            }

            if (ClientIdentifier == Guid.Empty) { 

                throw new ArgumentException(nameof(ClientIdentifier)); 
            }
             
        }
    }
}
