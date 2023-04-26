using AuraSearch.ValueObject;

namespace AuraSearch.Domain
{

    public sealed class Idea
    { 
        public Guid IdeaId { get; set; } = Guid.NewGuid();  

        public string Overview { get; set; }

        public Guid Reference { get; set; } = Guid.Empty;

        public double Delta { get; set; } = 0;

        public double Bias { get; set; } = 0; 
        
        public List<Symbol> Symbols { get; set; } = new();

        public override string ToString() => $"{Overview} D{Delta}";
    }
}
