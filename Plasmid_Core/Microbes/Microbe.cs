using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plasmid.Cards;
using Plasmid.Graphics;

namespace Plasmid.Microbes
{
public class Microbe
    {
        public static int X_DIM = 100; //128;
        public static int Y_DIM = 100; //128;
        public static float MAX_RADIUS_FACTOR = 1.8f;
        public static float MIN_RADIUS_FACTOR = .6f;

        public DnaSequence Genome { get; private set; }
        public int A { get => this.Genome.A; }
        public int G { get => this.Genome.G; }
        public int T { get => this.Genome.T; }
        public int C { get => this.Genome.C; }

        public Polygon CellBody { get; set; }
        public Polygon CellWall { get; set; }

        public Vector2 Position { get; set; }

        public List<Card> Deck { get; private set; }

        public int MaxHP { get; private set; }
        public int HP { get; set; }

        public Microbe(Vector2 position)
        {
            this.Deck = new List<Card>();
            this.MaxHP = 20;
            this.HP = MaxHP;

            this.Position = position;

            this.Genome = new DnaSequence();
            GenerateVisual();

        }

        public void Draw(Game1 game, Transform transform)
        {
            transform = transform.Combine(Transform.Shift(this.Position));
            this.CellBody.Draw(game, transform);
            this.CellWall.Draw(game, transform);
        }
        public void Draw(Game1 game)
        {
            this.CellBody.Draw(game, Position);
            this.CellWall.Draw(game, Position);
        }

        public void GenerateVisual()
        {
            // TODO
            // flesh out and refine color calculation

            // determine base color
            int r = 0;
            int g = 0;
            int b = 0;
            // attack > defense -> RED
            if (Genome.A >= Genome.G)
            {
                r = 255;
                // buff > debuff -> ORANGE
                if (Genome.T >= Genome.C)
                    g = 150 * (Genome.T / Genome.A);
                // debuff > buff -> MAGENTA
                else
                    b = 200 * (Genome.C / Genome.A);
            }
            // defense > attack -> GREEN
            else
            {
                g = 255;
                // debuff > buff -> TEAL
                if (Genome.C >= Genome.T)
                    b = 200 * (Genome.C / Genome.G);
                // buff > debuff -> YELLOW
                else
                {
                    b = 100;
                    r = 100 + 155 * (Genome.T / Genome.G);
                }
            }

            Color cytoColor = new Color(r, g, b);

            // TODO wallColor
            Color wallColor = GraphUtils.GetRandomColor();
            Vector2[] vertices = GenVertices(Genome);

            // null param for triangles tells Polygon to generate it's own
            this.CellBody = new Polygon(vertices, null, Transform.Identity(), cytoColor);
            this.CellWall = new Polygon(vertices, Transform.Identity(), 4, wallColor);

        }

        public Vector2[] GenVertices(DnaSequence dna)
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

            List<double> angleSteps = new List<double>();
            double lower = (slice / numVerts) - irregularity;
            double upper = (slice / numVerts) + irregularity;
            double sum = 0;

            // Pseudo randomizer, gets reproducible values based on dna sequence
            DnaRandom rand = dna.GetDnaRandom();
            for (int i = 0; i < numVerts + 1; i++)
            {
                double tmp = rand.NextDouble() * (upper - lower) + lower;
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
                if (r_i >  MAX_RADIUS_FACTOR * aveRadius)
                    r_i =  MAX_RADIUS_FACTOR * aveRadius;
                if (r_i <  MIN_RADIUS_FACTOR * aveRadius)
                    r_i =  MIN_RADIUS_FACTOR * aveRadius;
                double x = r_i * Math.Cos(angle);
                double y = r_i * Math.Sin(angle);

                //System.OverflowException: 'Value was either too large or too small for an Int32.'
                points.Add(new Vector2(Convert.ToInt32(Math.Round(x)), Convert.ToInt32(Math.Round(y))));
            }

            return RemoveColinear(ExpandPoints(points.ToArray(), Vector2.Zero));
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
            // TODO: Sometimes this returns NaN
            double u1 = 1.0 - rand.NextDouble(); //uniform(0,1] random doubles
            double u2 = 1.0 - rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)

            double result = mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)

            if (result is double.NaN)
                return Gauss(rand, mean, stdDev);
            else
                return result;
        }

    }

}

