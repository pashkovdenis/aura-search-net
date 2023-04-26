using AuraSearch.Enumerations;

namespace AuraSearch.ValueObject
{
    public sealed class Symbol
    { 
        public string Word { get; init; }

        public double Weight { get; set; }

        public SymbolType Type { get; init; } = SymbolType.Word;

        public override string ToString() => $"{Word} - {Weight}"; 
    }
}
