using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Xna.Framework;
using Plasmid.Graphics;

namespace Plasmid.Microbes
{
    public class MicrobeVisual
    {
        public static int X_DIM = 128;
        public static int Y_DIM = 128;
        public static Vector2 CENTER = new Vector2(X_DIM / 2, Y_DIM / 2);
        public static float MAX_RADIUS_FACTOR = 1.8f;     //TODO: hard max/min instead of factor? maybe a function to scale down oversized polygons?
        public static float MIN_RADIUS_FACTOR = .6f;

        public Polygon CellBody { get; set; }
        public Polygon CellWall { get; set; }
        //public Circle NuclearBody { get; set; }
        //public Circle NuclearWall { get; set; }
        //public Circle Nucleolus { get; set; }

        private float nucleusScale;
        private float nubleolusScale;
        private Color nucleusColor;

        public void Generate(DnaSequence dna)
        {
            // Pseudo randomizer, gets reproducible values based on dna sequence
            DnaRandom rand = dna.GetDnaRandom();

            // determine base color
            int r = 0;
            int g = 0;
            int b = 0;
            // attack > defense -> RED
            if (dna.A >= dna.G)
            {
                r = 255;
                // buff > debuff -> ORANGE
                if (dna.T >= dna.C)
                    g = 150 * (dna.T / dna.A);
                // debuff > buff -> MAGENTA
                else
                    b = 200 * (dna.C / dna.A);
            }
            // defense > attack -> GREEN
            else
            {
                g = 255;
                // debuff > buff -> TEAL
                if (dna.C >= dna.T)
                    b = 200 * (dna.C / dna.G);
                // buff > debuff -> YELLOW
                else
                {
                    b = 100;
                    r = 100 + 155 * (dna.T / dna.G);
                }
            }

            Color cytoColor = new Color(r, g, b);
            Color wallColor = GraphUtils.GetRandomColor(rand);
            Vector2[] vertices = GenVertices(dna, rand);

            // null param for triangles tells Polygon to generate it's own
            this.CellBody = new Polygon(vertices, null, Transform.Identity, cytoColor);
            this.CellWall = new Polygon(vertices, Transform.Identity, 4, wallColor);

            // TODO
        }

        public Vector2[] GenVertices(DnaSequence dna, Random rand)
        {
            // calculate cell wall parameters
            int aveRadius = dna.A + dna.G;                                              // 20-40        A+G
            double irregularity = .5 * Math.Tanh(dna.C / dna.T - 1) + .5;               // 0.0-1.0      C:T
            double spikeyness = .2 * Math.Tanh(4.0 / 5.0 * (dna.C / dna.T) - 1) + .2;   // 0.00-0.15    A:G

            int max = Math.Max(Math.Max(dna.A, dna.G), Math.Max(dna.T, dna.C));
            int min = Math.Min(Math.Min(dna.A, dna.G), Math.Min(dna.T, dna.C));

            int numVerts = Convert.ToInt32(8 * Math.Tanh(max / min / 16 - 1) + 10);    // 4-16         MAX:MIN

            float slice = (float)Math.PI / 2;
            irregularity = irregularity * slice / numVerts;
            spikeyness = spikeyness * aveRadius;

            // angle steps

            // DEBUG
            dna.Print();

            List<double> angleSteps = new List<double>();
            double lower = (slice / numVerts) - irregularity;
            double upper = (slice / numVerts) + irregularity;
            double sum = 0;
            // Use dna sequence as reproducible pseudo-randomizer
            var nextDna = dna.GetEnumerator();
            for (int i = 0; i < numVerts + 1; i++)
            {
                double value;
                Dna bp = nextDna.Current;

                // DEBUG
                Debug.WriteLine("Current bp: " + bp.ToString());

                if (bp == Dna.C)
                    value = 0.9;
                else if (bp == Dna.A)
                    value = 0.6;
                else if (bp == Dna.T)
                    value = 0.3;
                else // Dna.G
                    value = 0.1;

                double tmp = value * (upper - lower) + lower;
                angleSteps.Add(tmp);
                sum += tmp;
            }

            // normalize to fit slice
            double k = sum / slice;
            for (int i = 0; i < numVerts; i++)
                angleSteps[i] = angleSteps[i] / k;

            // generate points
            List<Vector2> points = new List<Vector2>();
            double angle = 0;
            for (int i = 0; i < numVerts; i++)
            {
                angle += angleSteps[i];
                double r_i = Gauss(rand, aveRadius, spikeyness);
                if (r_i > MicrobeVisual.MAX_RADIUS_FACTOR * aveRadius)
                    r_i = MicrobeVisual.MAX_RADIUS_FACTOR * aveRadius;
                if (r_i < MicrobeVisual.MIN_RADIUS_FACTOR * aveRadius)
                    r_i = MicrobeVisual.MIN_RADIUS_FACTOR * aveRadius;
                double x = MicrobeVisual.CENTER.X + r_i * Math.Cos(angle);
                double y = MicrobeVisual.CENTER.Y + r_i * Math.Sin(angle);
                points.Add(new Vector2(Convert.ToInt32(Math.Round(x)), Convert.ToInt32(Math.Round(y))));
            }

            return RemoveColinear(ExpandPoints(points.ToArray(), MicrobeVisual.CENTER));
        }

        private static Vector2[] ExpandPoints(Vector2[] baseVertices, Vector2 center, int factor = 4)
        {
            Vector2[] expandedVertices = new Vector2[baseVertices.Length * 4];

            int j = 0;
            for (int i = 0; i < baseVertices.Length; i++)
            {
                expandedVertices[j] = baseVertices[i];
                j++;
            }
            for (int i = baseVertices.Length - 1; i >= 0; i--)
            {
                expandedVertices[j] = new Vector2(expandedVertices[i].X - 2 * (expandedVertices[i].X - center.X), expandedVertices[i].Y);
                j++;
            }
            for (int i = 0; i < baseVertices.Length; i++)
            {
                expandedVertices[j] = new Vector2(expandedVertices[i].X - 2 * (expandedVertices[i].X - center.X), expandedVertices[i].Y - 2 * (expandedVertices[i].Y - center.Y));
                j++;
            }
            for (int i = baseVertices.Length - 1; i >= 0; i--)
            {
                expandedVertices[j] = new Vector2(expandedVertices[i].X, expandedVertices[i].Y - 2 * (expandedVertices[i].Y - center.Y));
                j++;
            }

            return expandedVertices;
        }

        public static Vector2[] RemoveColinear(Vector2[] vertices)
        {
            if (vertices is null || vertices.Length <= 3)
                return vertices;

            List<Vector2> newVertices = new List<Vector2>();
            Vector2 a;
            Vector2 b;
            for (int i = 0; i < vertices.Length; i++)
            {
                a = Utils.GetItem(vertices, i - 1) - vertices[i];
                b = Utils.GetItem(vertices, i + 1) - vertices[i];
                if (Math.Round(GetAngle(a, b), 2) != Math.Round(Math.PI, 2))
                    newVertices.Add(vertices[i]);
                //else
                //    throw new Exception("NOT A DRILL!! FOUND A COLINEAR VERTEX!\n" +
                //        "\npoint: " + vertices[i] +
                //        "\nprev: " + GetItem(vertices, i - 1) +
                //        "\nnext: " + GetItem(vertices, i + 1) +
                //        "\nvertex a: " + a +
                //        "\nvertex b: " + b +
                //        "\nangle: " + GetAngle(a, b) );
            }

            return newVertices.ToArray();
        }

        public static float GetAngle(Vector2 a, Vector2 b)
        {
            //float result = (float)Math.Atan2(b.Y - a.Y, b.X - a.X);
            float result = (float)Math.Acos((a.X * b.X + a.Y * b.Y) / (Math.Sqrt(a.X * a.X + a.Y * a.Y) * Math.Sqrt(b.X * b.X + b.Y * b.Y)));
            return result;
        }

        public static double Gauss(Random rand, double mean, double stdDev)
        {
            double u1 = 1.0 - rand.NextDouble(); //uniform(0,1] random doubles
            double u2 = 1.0 - rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            return mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
        }

    }
}
