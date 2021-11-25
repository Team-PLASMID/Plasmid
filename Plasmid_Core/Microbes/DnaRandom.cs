using System;
using System.Collections.Generic;

namespace Plasmid.Microbes
{
    public class DnaRandom : Random
    {
        private static Dictionary<string, double> conversions = new Dictionary<string, double>()
        {
            { "AA", 0.0 },
            { "AG", 1.0/15 },
            { "AT", 2.0/15 },
            { "AC", 3.0/15 },
            { "GA", 4.0/15 },
            { "GG", 5.0/15 },
            { "GT", 6.0/15 },
            { "GC", 7.0/15 },
            { "TA", 8.0/15 },
            { "TG", 9.0/15 },
            { "TT", 10.0/15 },
            { "TC", 11.0/15 },
            { "CA", 12.0/15 },
            { "CG", 13.0/15 },
            { "CT", 14.0/15 },
            { "CC", 1.0 },
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
            double value = conversions[str];

            this.sequence = sequence.Substring(2) + str;

            return value;
        }

    }
}
