using AuraSearch.ValueObject;

namespace AuraSearch.Utils
{
    /// <summary>
    /// MathUtils static class
    /// </summary>
    public static class MathUtils
    {
        public static double CalculateWebersForce(double multiplier, double force, double low) => multiplier * Math.Log10(force / low);
        public static double Derivative(double x) => x * (1 - x);


        public static double ActivationFunc(double value)
        {
            var k = Exp(value);

            var score = k / (1.0f + k); 

            return 0.5 * score * (1 + Math.Tanh(Math.Sqrt(2 / Math.PI) * (score + 0.044715 * Math.Pow(score, 3))));
        }

        public static double Sigmoid(double value)
        {
            var k = Exp(value);
            return k / (1.0f + k);
        }
  
        public static double Exp(double val)
        {
            long tmp = (long)(1512775 * val + 1072632447);
            return BitConverter.Int64BitsToDouble(tmp << 32);
        }

        public static double Normilize(double input) => input / (1.0f + Math.Abs(input)) * .5f + .5f;
        
        /// <summary>
        /// Softmax vector normalization aka probability distribution
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static double[] SoftMax(double[] input)
        {
            var z_exp = input.Select(Math.Exp);
            var sum_z_exp = z_exp.Sum();
            return z_exp.Select(i => i / sum_z_exp).ToArray();
        }

        /// <summary>
        /// Calculate force dependent on distance value
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static double DistanceRegression(double distance)
        {
            return CalculateWebersForce(0.01d, distance, .222d);
        }

        /// <summary>
        /// Calculate random normal distribution
        /// </summary>
        /// <param name="numberOfInputs"></param>
        /// <returns></returns>
        public static double CalculateRandomDistribution(int numberOfInputs)
        {
            var std = Math.Sqrt(2.0 / numberOfInputs);
            var dist = new List<double>();

            for (int i = 0; i < numberOfInputs * 2; i++)
            {
                var rand = new Random();
                double normRand = Phi(rand.NextDouble());
                dist.Add(normRand * std);
            }

            return dist.Average();
        }

        public static double CalcDeviation(IReadOnlyCollection<int> values)
        {
            double standardDeviation = 0;

            if (!values.Any()) return standardDeviation;

            // Compute the average.     
            var avg = values.Average();

            // Perform the Sum of (value-avg)_2_2.      
            var sum = values.Sum(d => Math.Pow(d - avg, 2));

            // Put it all together.      
            standardDeviation = Math.Sqrt(sum / (values.Count() - 1));
            return standardDeviation;
        }

        public static double Phi(double x)
        {
            // constants
            double a1 = 0.254829592;
            double a2 = -0.284496736;
            double a3 = 1.421413741;
            double a4 = -1.453152027;
            double a5 = 1.061405429;
            double p = 0.3275911;

            // Save the sign of x
            int sign = 1;
            if (x < 0)
                sign = -1;
            x = Math.Abs(x) / Math.Sqrt(2.0);

            // A&S formula 7.1.26
            double t = 1.0 / (1.0 + p * x);
            double y = 1.0 - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1) * t * Math.Exp(-x * x);

            return 0.5 * (1.0 + sign * y);
        }


        public static IReadOnlyCollection<ResultValue> ClaculateScores(IList<ResultValue> set)
        {
            var alpha = 0.01d;
            var result = new List<ResultValue>(set.Count);

            for (var x = 1; x < set.Count; x++)
            {
                var record = set[x];
                var previousRecord = set[x - 1];

                var score = alpha * record.Score + (1 - alpha) * previousRecord.Score;

                result.Add(new ResultValue
                {
                    Score = score,
                    Thought = record.Thought,
                    TopIdea = record.TopIdea
                });
            }

            return result.OrderByDescending(x => x.Score).ToList();
        }
    }
}
