using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Plasmid.Microbes
{
    public enum Dna { None, A, G, T, C }
    public static class DnaExtensions
    {
        private static Color colorA;
        private static Color colorG;
        private static Color colorT;
        private static Color colorC;
        public static Texture2D Texture { get; private set; }
        public static void Load(Game1 game)
        {
            Texture = game.Content.Load<Texture2D>("nucleotides");
            colorA = game.ColorPalette.GetColor(0);
            colorG = game.ColorPalette.GetColor(3);
            colorT = game.ColorPalette.GetColor(4);
            colorC = game.ColorPalette.GetColor(2);
        }
        public static bool IsValid(this Dna dna)
        {
            if (dna > Dna.None)
                return true;
            return false;
        }
        public static Dna Compliment(this Dna dna)
        {
            if (dna == Dna.A)
                return Dna.T;
            if (dna == Dna.T)
                return Dna.A;
            if (dna == Dna.G)
                return Dna.C;
            if (dna == Dna.C)
                return Dna.G;

            return Dna.None;
        }
        public static Rectangle GetRect(this Dna dna)
        {
            if (dna == Dna.A)
                return new Rectangle(0, 0, 16, 16);
            if (dna == Dna.G)
                return new Rectangle(16, 0, 16, 16);
            if (dna == Dna.T)
                return new Rectangle(32, 0, 16, 16);
            if (dna == Dna.C)
                return new Rectangle(48, 0, 16, 16);

            return new Rectangle();
        }
        public static Color GetColor(this Dna dna)
        {
            if (dna == Dna.A)
                return colorA;
            if (dna == Dna.G)
                return colorG;
            if (dna == Dna.T)
                return colorT;
            if (dna == Dna.C)
                return colorC;

            return new Color();
        }
        public static string ToString(this Dna dna)
        {
            if (dna == Dna.A)
                return "A";
            if (dna == Dna.G)
                return "G";
            if (dna == Dna.T)
                return "T";
            if (dna == Dna.C)
                return "C";

            return "Error";
        }
    }

    public class DnaSequence : List<Dna>
    {
        public static int LENGTH = 60;
        public static int MAX_INIT = 30;
        public static int MIN_INIT = 10;

        public int A { get => this.CountBasePairs(Dna.A); }
        public int G { get => this.CountBasePairs(Dna.G); }
        public int T { get => this.CountBasePairs(Dna.T); }
        public int C { get => this.CountBasePairs(Dna.C); }

        public DnaSequence(Random? random=null)
        {
            this.Generate(random);
        }
        public void Generate(Random? random=null)
        {
            Random rand = random ?? new Random();
            this.Clear();

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
