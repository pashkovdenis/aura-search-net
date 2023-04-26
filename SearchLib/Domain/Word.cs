using AuraSearch.Abstractions;

namespace AuraSearch.Domain
{
    public sealed class Word : IEntity
    { 
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Token { get; set; }

        public int Count { get; set; }

        public bool IsNoiseSymbol { get; set; }

        public override string ToString() => $"{Token}-{Count}";

    }
}
