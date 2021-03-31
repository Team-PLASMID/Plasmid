using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Plasmid_Core
{
class Microbe
    {
        public static int GENOME_SIZE = 60;

        public string Genome { get; set; }
        public List<Card> Deck { get; set; }
        public Texture2D Sprite { get; set; }

        public Microbe()
        {
            Genome = GenerateDNA(GENOME_SIZE);
            Deck = new List<Card>();
            //Sprite = generateSprite();
        }

        // Randomly generate a sequence of a g t c
        // In the future, there should probably be some limitation on the ratio
        public static string GenerateDNA(int len)
        {
            Random rand = new Random();
            int caseSwitch = rand.Next(0, 4);
            string dna = "";

            for (int i = 0; i < len; i++)
            {
                // generate DNA sequence
                switch(caseSwitch)
                {
                    // 0: a
                    case 0:
                        dna += "a";
                        break;
                    // 1: g
                    case 1:
                        dna += "g";
                        break;
                    // 2: t
                    case 2:
                        dna += "t";
                        break;
                    // 3: c 
                    case 3:
                        dna += "c";
                        break;
                    // unexpected value
                    default:
                        break;
                }
            }

            return dna;
        }

        /* Procedurally generate a sprite texture based on the Genome string
        public static Texture2D generateSprite()
        {
            // placeholder, this will actually make a sprite later
            return new Texture2D();
        }
        */

        // Print a nicely formatted genome sequence (for debugging in console)
        public void PrintGenome()
        {
            int line = 0;

            while (true)
            {
                Console.Write((line+1).ToString().PadLeft(5, ' '));
                for (int i = 0; i < 6; i++)
                {
                    Console.Write(' ');
                    for (int j = 0; j < 10; j++)
                    {
                        int c = j + 10 * i + 60 * line;
                        if ( c < Genome.Length)
                            Console.Write(Genome[c]);
                        else
                        {
                            Console.WriteLine();
                            return;
                        }
                    }
                    Console.WriteLine();
                }
            }
        }

        public static void Test()
        {
            Microbe germbug = new Microbe();
            germbug.PrintGenome();
        }
    }
}
