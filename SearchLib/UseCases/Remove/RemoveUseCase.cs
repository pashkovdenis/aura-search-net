using AuraSearch.Abstractions;
using AuraSearch.Models;
using AuraSearch.UseCases.Remove.Request;

namespace AuraSearch.UseCases.Remove
{
    public sealed class RemoveUseCase : IUseCase<RemoveRequest>
    {
        private readonly IThoughtRepository _repository;

        public RemoveUseCase(IThoughtRepository repository)
        {
            _repository = repository;
        }

        public async Task Execute(RemoveRequest input, Context context, CancellationToken cancellationToken)
        {
            if (context.ClientIdentifier != input.ClientIdentifier)
            {
                throw new InvalidOperationException("Context identifier and client id mistmatch");
            }

            var thought = await _repository.GetById(input.ThoughtId.ToString(), cancellationToken); 

            if (thought == null)
            {
                throw new InvalidOperationException("Thought not found");
            }

            if (input.IdeaId == Guid.Empty)
            {
                await _repository.RemoveById(input.ThoughtId.ToString(), cancellationToken);

                context.RemoveFromContext(input.ThoughtId.ToString()); 

                return; 
            }

            var idea = thought.Ideas.First(x => x.IdeaId == input.IdeaId);

            thought.Ideas.Remove(idea);

            await _repository.Update(thought, cancellationToken);

            context.RemoveFromContext(input.IdeaId.ToString());
        }
    }
}
