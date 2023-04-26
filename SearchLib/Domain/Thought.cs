using AuraSearch.Abstractions;

namespace AuraSearch.Domain
{
  

    public sealed class Thought : IEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
         
        public Guid ClientIdentifier { get; set; }

        public List<Idea> Ideas { get; set; } = new();

        public List<string> Tags { get; set; }= new();  

        public double Boost { get; set; }

        public string Label { get; set; }
         
        public ThoughtMetric Metrics { get; set; } = new();

        public override string ToString() => $"{Id} -> {Label}";
    }
}
