namespace AuraSearch.ValueObject
{
    /// <summary>
    /// Resulting vector
    /// </summary>
    public sealed class ResultVector
    { 
        public Guid RequestId { get; set; }

        public bool Succeeded { get; set; }

        public IReadOnlyCollection<ResultValue> Results { get; set; }
    }
}
 
