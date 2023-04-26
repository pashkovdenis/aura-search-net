namespace AuraSearch.Models
{ 

    public sealed class Settings
    {
        public double DefNeuronDom { get; set; } = 0.02d;
        public double NeuronReinforce { get; set; } = 0.0030d;
        public int MinimumSymbolLength { get; set; } = 2;
        public double NeuronDominanceWeigh { get; set; } = 0.08d;
        public double ActivationThress { get; set; } = .58d;
      
        public int WordCounterThresshold = 3;

        public int WordsToIndexThresshold = 15; 
    } 
}
