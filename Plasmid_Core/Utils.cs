using Microsoft.Xna.Framework;
using Plasmid;
using Plasmid.Graphics;
using Plasmid.Cards;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Plasmid
{
    class Utils
    {
        //public static Vector2[] RandomPoly()
        //{
        //    int height = 128;
        //    int width = 128;

        //    Random rand = new Random();
        //    int radius = rand.Next(Convert.ToInt32(Math.Round(Math.Min(height, width) * .5))) + Convert.ToInt32(Math.Round(Math.Min(height, width) * .10));
        //    double irreg = rand.NextDouble();
        //    double spike = rand.NextDouble() * 0.15;
        //    int verts = rand.Next(12) + 4;

        //    Vector2[] vertices = Microbes.GenCellWallVertices(rand, Vector2.Zero, radius, irreg, spike, verts);

        //    return vertices;
        //}

        public static T GetItem<T>(T[] array, int index)
        {
            if (index >= array.Length)
                return array[index % array.Length];
            else if (index < 0)
                return array[index % array.Length + array.Length];
            else
                return array[index];
        }

        public static T GetItem<T>(List<T> array, int index)
        {
            if (index >= array.Count)
                return array[index % array.Count];
            else if (index < 0)
                return array[index % array.Count + array.Count];
            else
                return array[index];

        }

        public static void Wait(int milliseconds)
        {
            throw new NotImplementedException();
        }
    }
}
