using System;
using System.Collections.Generic;

namespace Plasmid.Microbes
{
    public class DnaRandom : Random
    {
        private static Dictionary<string, double> conversions = new Dictionary<string, double>()
        {
            { "AA", 0 },
            { "AG", 1/15 },
            { "AT", 2/15 },
            { "AC", 3/15 },
            { "GA", 4/15 },
            { "GG", 5/15 },
            { "GT", 6/15 },
            { "GC", 7/15 },
            { "TA", 8/15 },
            { "TG", 9/15 },
            { "TT", 10/15 },
            { "TC", 11/15 },
            { "CA", 12/15 },
            { "CG", 13/15 },
            { "CT", 14/15 },
            { "CC", 1 },
        };
        private string sequence;

        public DnaRandom(DnaSequence genome)
        {
            this.sequence = genome.ToString();
        }

        public override int Next()
        {
            return this.Next(int.MinValue, int.MaxValue);
        }
        public override int Next(int maxValue)
        {
            if (maxValue <= 0)
                throw new ArgumentException("maxValue must be greater than zero.");

            return this.Next(0, maxValue);
        }
        public override int Next(int minValue, int maxValue)
        {
            if (maxValue <= minValue)
                throw new ArgumentException("maxValue must be greater than minValue.");

            return (int)(this.NextDouble() * (maxValue - minValue) + minValue);
        }

        public override double NextDouble()
        {
            string str = sequence.Substring(0, 2);
            if (conversions.TryGetValue(str, out double value))
            {
                sequence.Remove(0, 2);
                sequence.Insert(sequence.Length - 1, str);
                return value;
            }
            else
                throw new Exception("Couldn't generate double from dna");
        }

    }
}
