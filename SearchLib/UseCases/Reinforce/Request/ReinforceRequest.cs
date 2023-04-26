using AuraSearch.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace AuraSearch.UseCases.Reinforce.Request
{
    public sealed class ReinforceRequest : IUseCaseInput
    {
        [Required]
        public Guid ClientIdentifier { get; set; }
         
        public Guid ThoughtId { get; set; }

        public Guid IdeaId { get; set; }

        [Required]
        public string OriginalRequest { get; set; }

        public string[] Keywords { get; set; }


        public string Addition { get; set; }

        public List<string> Tags { get; set; }

        public ReinforceType Type { get; set; } 

        public void Validate()
        {
            if (ClientIdentifier == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(ClientIdentifier));
            }

            if (string.IsNullOrEmpty(OriginalRequest))
            {
                throw new ArgumentNullException(nameof(OriginalRequest)); 
            }

            if (Type == ReinforceType.Boost)
            {
                if (ThoughtId == Guid.Empty)
                {
                    throw new ArgumentNullException(nameof(ThoughtId));
                }
            }

            if (Type == ReinforceType.SelectedIdea)
            {
                if (IdeaId == Guid.Empty)
                {
                    throw new ArgumentNullException(nameof(IdeaId));
                }
            } 


            if (Type == ReinforceType.Append && string.IsNullOrEmpty(Addition))
            {
                throw new ArgumentNullException(nameof(Addition));
            } 
        }
    }
}
