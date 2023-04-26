namespace AuraSearch.ValueObject
{
    public readonly struct ContextEntry
    {
        public string Label { get; init; }

        public double Weight { get; init; }

        public int Pros { get; init; }

        public int Cons { get; init; }

        public double AvgScore { get; init; }

        public override int GetHashCode() => Label.GetHashCode();
    }
}
