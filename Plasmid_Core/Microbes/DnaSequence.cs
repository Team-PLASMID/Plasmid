using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Plasmid.Microbes
{
    public enum Dna { A, G, T, C }
    public class DnaSequence : List<Dna>, IEnumerable<Dna>
    {
        public static int LENGTH = 60;
        public static int MAX_INIT = 30;
        public static int MIN_INIT = 10;

        public int A { get => this.CountBasePairs(Dna.A); }
        public int G { get => this.CountBasePairs(Dna.G); }
        public int T { get => this.CountBasePairs(Dna.T); }
        public int C { get => this.CountBasePairs(Dna.C); }

        public void Generate(Random? random=null)
        {
            Random rand = random ?? new Random();

            // generate DNA sequence
            for (int i = 0; i < DnaSequence.LENGTH; i++)
            {
                int x = rand.Next(4);
                if (x == 0)
                    this.Add(Dna.A);
                if (x == 1)
                    this.Add(Dna.G);
                if (x == 2)
                    this.Add(Dna.T);
                if (x == 3)
                    this.Add(Dna.C);
            }

            // check bounds for initial genome
            if (this.A > DnaSequence.MAX_INIT || this.G > DnaSequence.MAX_INIT || this.T > DnaSequence.MAX_INIT || this.C > DnaSequence.MAX_INIT ||
                this.A < DnaSequence.MIN_INIT || this.G < DnaSequence.MIN_INIT || this.T < DnaSequence.MIN_INIT || this.C < DnaSequence.MIN_INIT)
            {
                this.Generate(rand);
            }
        }

        //public IEnumerator<Dna> GetEnumerator()
        //{
        //    foreach (Dna dna in this)
        //    {
        //        yield return dna;
        //    }
        //}

        public int CountBasePairs(Dna type)
        {
            int count = 0;
            foreach (Dna dna in this)
                if (dna == type)
                    count++;
            return count;
        }

        public override string ToString()
        {
            string sequence = "";
            foreach (Dna dna in this)
            {
                if (dna == Dna.A)
                    sequence += "A";
                if (dna == Dna.G)
                    sequence += "G";
                if (dna == Dna.T)
                    sequence += "T";
                if (dna == Dna.C)
                    sequence += "C";
            }

            return sequence;
        }

        public void Print()
        {
            Debug.WriteLine("GENOME: " + this.ToString());
            Debug.WriteLine("     A: " + A + " (" + Math.Round((double)A / LENGTH * 100, 2) + "%)");
            Debug.WriteLine("     G: " + G + " (" + Math.Round((double)G / LENGTH * 100, 2) + "%)");
            Debug.WriteLine("     T: " + T + " (" + Math.Round((double)T / LENGTH * 100, 2) + "%)");
            Debug.WriteLine("     C: " + C + " (" + Math.Round((double)C / LENGTH * 100, 2) + "%)");
        }

        public DnaRandom GetDnaRandom()
        {
            return new DnaRandom(this);
        }
    }
}
