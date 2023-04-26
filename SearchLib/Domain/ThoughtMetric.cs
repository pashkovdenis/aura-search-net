namespace AuraSearch.Domain
{
    
    public sealed class ThoughtMetric
    { 
        public int MatchCount { get; set; } 

        public int DismatchCount { get; set; }

        public DateTimeOffset LastTimeHit { get; set; }

        public double AvgScore { get; set; }

        public double TotalScore { get; set; }

        public override string ToString() 
            => $"Match: {MatchCount}, DisMatch: {DismatchCount}, TotalScore {TotalScore}";
    }
}
