using Microsoft.Xna.Framework;
using Plasmid;
using Plasmid.Graphics;
using Plasmid.Cards;
using System;

namespace Plasmid
{
    class Utils
    {
        public static Vector2[] RandomPoly()
        {
            int height = 128;
            int width = 128;

            Random rand = new Random();
            int radius = rand.Next(Convert.ToInt32(Math.Round(Math.Min(height, width) * .5))) + Convert.ToInt32(Math.Round(Math.Min(height, width) * .10));
            double irreg = rand.NextDouble();
            double spike = rand.NextDouble() * 0.15;
            int verts = rand.Next(12) + 4;

            Vector2[] vertices = Microbe.GenCellWallVertices(rand, Vector2.Zero, radius, irreg, spike, verts);

            return vertices;
        }
    }
}
