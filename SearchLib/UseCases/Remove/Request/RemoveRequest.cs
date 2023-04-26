using AuraSearch.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace AuraSearch.UseCases.Remove.Request
{
    public sealed class RemoveRequest : IUseCaseInput
    { 
        public Guid RequestId { get; } = Guid.NewGuid();

        [Required]
        public Guid ClientIdentifier { get; set; }

        [Required]
        public Guid ThoughtId { get; set; }

        public Guid IdeaId { get; set; }


        public void Validate()
        {
            if (ThoughtId == Guid.Empty)
            {
                throw new ArgumentException(nameof(ThoughtId));
            }

            if (ClientIdentifier == Guid.Empty)
            {
                throw new ArgumentException(nameof(ClientIdentifier));
            }
        }
    }
}
