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
        const static GENOME_SIZE = 60;

        public string Genome { get; set; }
        public List<Plasmid> Deck { get; set; }
        public Texture2D Sprite { get; set; }

        public Microbe()
        {
            Genome = generateDNA(GENOME_SIZE);
            Deck = new List<Plasmid>();
            Sprite = generateSprite();
        }

        // Randomly generate a sequence of a g t c
        // In the future, there should probably be some limitation on the ratio
        public static string generateDNA(int len)
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

        // Procedurally generate a sprite texture based on the Genome string
        public static Texture2D generateSprite()
        {
            // placeholder, this will actually make a sprite later
            return new Texture2D();
        }

        // Print a nicely formatted genome sequence (for debugging in console)
        public void printGenome()
        {
            // *TODO*
            // print the Genome formatted like a dna sequence
            // ie:
            //     1 gatcctccat atacaacggt atctccacct caggtttaga tctcaacaac ggaaccattg
            //    61 ccgacatgag acagttaggt atcgtcgaga gttacaagct aaaacgagca gtagtcagct
        }
    }
}
