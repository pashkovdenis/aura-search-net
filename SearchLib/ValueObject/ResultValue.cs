using AuraSearch.Domain;

namespace AuraSearch.ValueObject
{   
    /// <summary>
    /// ResultValue
    /// </summary>
    public struct ResultValue
    {
        public Thought Thought { get; set; }

        public Idea TopIdea { get; set; }

        public IReadOnlyCollection<(ThoughtMetric metrics, Idea idea, double score)> OtherIdeas { get; set; }

        public double Score { get; set; }

        public override string ToString() => $"{TopIdea} - {Score}";
    }
}
