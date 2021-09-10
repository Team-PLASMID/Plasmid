using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Plasmid_Core
{
class Microbe
    {
        // Genome gen params
        public static int LENGTH = 60;
        public static int MIN_INIT = 10;
        public static int MAX_INIT = 30;
        // Sprite gen params
        public static int X_DIM = 128;
        public static int Y_DIM = 128;
        public static Point CENTER = new Point(X_DIM / 2, Y_DIM / 2);
        public static double MAX_RADIUS_FACTOR = 2;
        public static double MIN_RADIUS_FACTOR = 0.5;

        public String Genome { get; set; }
        public int A { get; set; }
        public int G { get; set; }
        public int T { get; set; }
        public int C { get; set; }
        public List<Card> Deck { get; set; }
        public Texture2D Sprite { get; set; }

        public int MaxHP { get; set; }
        public int HP { get; set; }

        public Microbe()
        {
            Deck = new List<Card>();
            MaxHP = 20;
            HP = MaxHP;
            //Sprite = generateSprite();
        }

        public void GenerateDNA()
        {
            GenerateDNA(MIN_INIT, MAX_INIT);
        }
        public void GenerateDNA(bool noLimits)
        {
            if (noLimits)
                GenerateDNA(0, LENGTH);
            else
                GenerateDNA();
        }
        public void GenerateDNA(int min, int max)
        {
            Random rand = new Random();
            while (true)
            {
                Genome = "";
                A = 0;
                G = 0;
                T = 0;
                C = 0;
                for (int i = 0; i < LENGTH; i++)
                {
                    int caseSwitch = rand.Next(0, 4);
                    // generate DNA sequence
                    switch (caseSwitch)
                    {
                        // 0: a
                        case 0:
                            Genome += "a";
                            A++;
                            break;
                        // 1: g
                        case 1:
                            Genome += "g";
                            G++;
                            break;
                        // 2: t
                        case 2:
                            Genome += "t";
                            T++;
                            break;
                        // 3: c 
                        case 3:
                            Genome += "c";
                            C++;
                            break;
                        // unexpected value
                        default:
                            break;
                    }
                    // stop generating if one nucleotide exceeds max allowed
                    if (A > max || G > max || T > max || C > max)
                        break;
                }
                // make sure sequence is correct length (meaning we didn't break early)
                // as long as all nucleotides have at least the min, accept the sequence.
                if (Genome.Length == LENGTH)
                    if (A >= min && G >= min && T >= min && C >= min)
                        return;
            }
        }

        public void GenSprite(GraphicsDevice graphicsDevice)
        {
            Random rand = new Random();

            // calculate cell wall parameters
            int aveRad = A + G;                                             // 20-40        A+G
            double angVar = .5 * Math.Tanh(C / T - 1) + .5;                 // 0.0-1.0      C:T
            double radVar = .2 * Math.Tanh(4.0 / 5.0 * (C / T) - 1) + .2;   // 0.00-0.15    A:G

            int max = Math.Max(Math.Max(A, G), Math.Max(T, C));
            int min = Math.Min(Math.Min(A, G), Math.Min(T, C));

            int verts = Convert.ToInt32(8 * Math.Tanh(max / min / 16 - 1) + 10);    // 4-16         MAX:MIN

            // determine base color
            int r = 0;
            int g = 0;
            int b = 0;
            // attack > defense -> RED
            if (A >= G)
            {
                r = 255;
                // buff > debuff -> ORANGE
                if (T >= C)
                    g = 150 * (T / A);
                // debuff > buff -> MAGENTA
                else
                    b = 200 * (C / A);
            }
            // defense > attack -> GREEN
            else
            {
                g = 255;
                // debuff > buff -> TEAL
                if (C >= T)
                    b = 200 * (C / G);
                // buff > debuff -> YELLOW
                else
                {
                    b = 100;
                    r = 100 + 155 * (T / G);
                }
            }

            System.Drawing.Color baseColor = System.Drawing.Color.FromArgb(255, r, g, b);

            // generate cell wall polygon
            Point[] wallPoints = GenPolygonPoints(rand, 0.25, CENTER, aveRad, angVar, radVar, verts);
            wallPoints = ExpandPoints(wallPoints, CENTER);

            System.Drawing.PointF[] convertedPoints = new System.Drawing.PointF[wallPoints.Length];
            for (int i = 0; i < convertedPoints.Length; i++)
                convertedPoints[i] = new System.Drawing.PointF(wallPoints[i].X, wallPoints[i].Y);

            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(X_DIM, Y_DIM);
            System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bmp);
            System.Drawing.Pen pen1 = new System.Drawing.Pen(System.Drawing.Color.Navy, 3);
            System.Drawing.Brush brush1 = new System.Drawing.SolidBrush(baseColor);
            //Pen pen2 = new Pen(COLOR_3, LINE_WEIGHT);
            //Brush brush2 = new SolidBrush(COLOR_4);
            graphics.FillPolygon(brush1, convertedPoints);
            graphics.DrawPolygon(pen1, convertedPoints);
            //graphics.FillPolygon(brush2, points2);
            //graphics.DrawPolygon(pen2, points2);
            //bmp.Save(name + "\\sample_" + (i + 1) + ".png");

            Texture2D tx = null;
            using (MemoryStream s = new MemoryStream())
            {
                bmp.Save(s, System.Drawing.Imaging.ImageFormat.Png);
                s.Seek(0, SeekOrigin.Begin);
                tx = Texture2D.FromStream(graphicsDevice, s);
            }

            Sprite = tx;
        }

        // helper function for GenSprite()
        public static Point[] GenPolygonPoints(Random rand, double slice, Point center, int aveRadius, double irregularity, double spikeyness, int numVerts)
        {
            slice = slice * 2 * Math.PI;
            irregularity = irregularity * slice / numVerts;
            spikeyness = spikeyness * aveRadius;

            // angle steps
            List<double> angleSteps = new List<double>();
            double lower = (slice / numVerts) - irregularity;
            double upper = (slice / numVerts) + irregularity;
            double sum = 0;
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
            List<Point> points = new List<Point>();
            double angle = 0;
            for (int i = 0; i < numVerts; i++)
            {
                angle += angleSteps[i];
                double r_i = Gauss(rand, aveRadius, spikeyness);
                if (r_i > MAX_RADIUS_FACTOR * aveRadius)
                    r_i = MAX_RADIUS_FACTOR * aveRadius;
                if (r_i < MIN_RADIUS_FACTOR * aveRadius)
                    r_i = MIN_RADIUS_FACTOR * aveRadius;
                double x = center.X + r_i * Math.Cos(angle);
                double y = center.Y + r_i * Math.Sin(angle);
                points.Add(new Point(Convert.ToInt32(Math.Round(x)), Convert.ToInt32(Math.Round(y))));
            }

            return points.ToArray();
        }

        // helper function for GenPolygonPoints
        public static double Gauss(Random rand, double mean, double stdDev)
        {
            double u1 = 1.0 - rand.NextDouble(); //uniform(0,1] random doubles
            double u2 = 1.0 - rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            return mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
        }

        // helper function for GenSprite()
        private static Point[] ExpandPoints(Point[] quadrant, Point center)
        {
            Point[] points = new Point[quadrant.Length * 4];

            int j = 0;
            for (int i = 0; i < quadrant.Length; i++)
            {
                points[j] = quadrant[i];
                j++;
            }
            for (int i = quadrant.Length - 1; i >= 0; i--)
            {
                points[j] = new Point(points[i].X - 2 * (points[i].X - center.X), points[i].Y);
                j++;
            }
            for (int i = 0; i < quadrant.Length; i++)
            {
                points[j] = new Point(points[i].X - 2 * (points[i].X - center.X), points[i].Y - 2 * (points[i].Y - center.Y));
                j++;
            }
            for (int i = quadrant.Length - 1; i >= 0; i--)
            {
                points[j] = new Point(points[i].X, points[i].Y - 2 * (points[i].Y - center.Y));
                j++;
            }

            return points;
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
            Console.WriteLine("NUCLEOTIDE BREAKDOWN");
            Console.WriteLine("A: " + A + " (" + Math.Round((double)A / LENGTH * 100, 2) + "%)");
            Console.WriteLine("G: " + G + " (" + Math.Round((double)G / LENGTH * 100, 2) + "%)");
            Console.WriteLine("T: " + T + " (" + Math.Round((double)T / LENGTH * 100, 2) + "%)");
            Console.WriteLine("C: " + C + " (" + Math.Round((double)C / LENGTH * 100, 2) + "%)");

            int line = 0;
            Console.WriteLine("GENE SEQUENCE:");
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
    }
}
