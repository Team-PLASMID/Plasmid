using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plasmid.Cards;
using Plasmid.Graphics;

namespace Plasmid.Microbes
{
class Microbe
    {
        // Sprite gen params
        

        public DnaSequence Genome { get => this.genome; }
        private DnaSequence genome;
        private int a;
        private int g;
        private int t;
        private int c;
        private List<Card> deck;

        public int MaxHP { get; set; }
        public int HP { get; set; }

        public Microbe()
        {
            deck = new List<Card>();
            MaxHP = 20;
            HP = MaxHP;
            //Sprite = generateSprite();
        }

        public void GenerateDNA()
        {
            this.genome.Generate();
        }

        //public void TestGen(GraphicsDevice graphics)
        //{
        //    Random rand = new Random();

        //    // calculate cell wall parameters
        //    int aveRad = a + this.g;                                        // 20-40        A+G
        //    double angVar = .5 * Math.Tanh(c / t - 1) + .5;                 // 0.0-1.0      C:T
        //    double radVar = .2 * Math.Tanh(4.0 / 5.0 * (c / t) - 1) + .2;   // 0.00-0.15    A:G

        //    int max = Math.Max(Math.Max(a, this.g), Math.Max(t, c));
        //    int min = Math.Min(Math.Min(a, this.g), Math.Min(t, c));

        //    int verts = Convert.ToInt32(8 * Math.Tanh(max / min / 16 - 1) + 10);    // 4-16         MAX:MIN

        //    // determine base color
        //    int r = 0;
        //    int g = 0;
        //    int b = 0;
        //    // attack > defense -> RED
        //    if (a >= this.g)
        //    {
        //        r = 255;
        //        // buff > debuff -> ORANGE
        //        if (t >= c)
        //            g = 150 * (t / a);
        //        // debuff > buff -> MAGENTA
        //        else
        //            b = 200 * (c / a);
        //    }
        //    // defense > attack -> GREEN
        //    else
        //    {
        //        g = 255;
        //        // debuff > buff -> TEAL
        //        if (c >= t)
        //            b = 200 * (c / this.g);
        //        // buff > debuff -> YELLOW
        //        else
        //        {
        //            b = 100;
        //            r = 100 + 155 * (t / this.g);
        //        }
        //    }

        //    this.CytoColor = new Color(r, g, b);
        //    this.WallColor = Color.Navy;

        //    // generate cell wall polygon
        //    this.Vertices = GenCellWallVertices(rand, CENTER, aveRad, angVar, radVar, verts);
        //    GraphUtils.Triangulate(this.Vertices, out int[] triangles, out string errorMessage);
        //    this.Triangles = triangles;

        //}


        //public void PrintGenome()
        //{
        //    Console.WriteLine("NUCLEOTIDE BREAKDOWN");
        //    Console.WriteLine("A: " + a + " (" + Math.Round((double)a / LENGTH * 100, 2) + "%)");
        //    Console.WriteLine("G: " + g + " (" + Math.Round((double)g / LENGTH * 100, 2) + "%)");
        //    Console.WriteLine("T: " + t + " (" + Math.Round((double)t / LENGTH * 100, 2) + "%)");
        //    Console.WriteLine("C: " + c + " (" + Math.Round((double)c / LENGTH * 100, 2) + "%)");

        //    int line = 0;
        //    Console.WriteLine("GENE SEQUENCE:");
        //    while (true)
        //    {
        //        Console.Write((line+1).ToString().PadLeft(5, ' '));
        //        for (int i = 0; i < 6; i++)
        //        {
        //            Console.Write(' ');
        //            for (int j = 0; j < 10; j++)
        //            {
        //                int c = j + 10 * i + 60 * line;
        //                if ( c < genome.Length)
        //                    Console.Write(genome[c]);
        //                else
        //                {
        //                    Console.WriteLine();
        //                    return;
        //                }
        //            }
        //            Console.WriteLine();
        //        }
        //    }
        //}
    }
}
