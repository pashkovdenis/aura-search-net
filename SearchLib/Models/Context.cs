using AuraSearch.Utils;
using AuraSearch.ValueObject;

namespace AuraSearch.Models
{
    public sealed class Context
    { 
        private readonly List<ContextEntry> contextEntry = new();

        public Guid ClientIdentifier { get; set; } = Guid.NewGuid();

        public DateTimeOffset Created { get; set; } = DateTimeOffset.Now;
         
        public IEnumerable<(string Label, double Score)> GetContextEntries() => contextEntry.Select(x => (x.Label, x.Weight));

        public void Refresh()
        {
            var result = contextEntry.Select(x => x.Weight).ToList();
            var index = 0.1d;

            for (var x = 0; x < result.Count; x++)
            {
                result[x] -= index;

                index += MathUtils.DistanceRegression(index);

                contextEntry[x] = new ContextEntry
                {
                    Label = contextEntry[x].Label,
                    Weight = result[x]
                };
            }
        }
 
        public void AddToContext(string label, double score, int pros, int cons, double avg)
        {
            if (!contextEntry.Any())
            {
                contextEntry.Add(new ContextEntry
                {
                    Label = label,
                    Weight = MathUtils.Sigmoid(score),
                    Pros = pros, Cons = cons, AvgScore= avg
                });

                return;
            }

            contextEntry.Add(new ContextEntry
            {
                Label = label,
                Weight = score,
                Pros = pros,
                Cons = cons,
                AvgScore = avg
            });

            var scoreVector = contextEntry.Select(x => x.Weight).ToArray();
            
            if (scoreVector.Count() == 1)
            {
                return;
            }
              
            var result = MathUtils.SoftMax(scoreVector).ToList();

            for (var x = 0; x < result.Count; x++)
            {
                contextEntry[x] = new ContextEntry
                {
                    Label = contextEntry[x].Label,
                    Weight = result[x],
                    Pros = contextEntry[x].Pros,
                    Cons = contextEntry[x].Cons,
                    AvgScore = contextEntry[x].AvgScore
                };
            }
        }

        public void RemoveFromContext(string label) => contextEntry.RemoveAll(x => x.Label == label);

        public double GetThresshold(double min)
        {
            var top = contextEntry.Where(x=>x.Weight > 0).OrderByDescending(x => x.AvgScore).ToList();
             
            if (!top.Any())
            {
                return min;  
            }
                  
            var summAvg = Math.Atan( top.Sum(x => x.Weight)/top.Count);  
            
            return min + summAvg;
  
        } 
    }
}
