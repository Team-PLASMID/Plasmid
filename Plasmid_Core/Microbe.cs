using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plasmid.Cards;
using Plasmid.Graphics;

namespace Plasmid
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
        public static Vector2 CENTER = new Vector2(X_DIM / 2, Y_DIM / 2);
        public static float MAX_RADIUS_FACTOR = 1.8f;     //TODO: hard max/min instead of factor? maybe a function to scale down oversized polygons?
        public static double MIN_RADIUS_FACTOR = .6f;

        private String genome;
        private int a;
        private int g;
        private int t;
        private int c;
        private List<Card> deck;

        public Vector2[] Vertices { get; set; }
        public int[] Triangles { get; set; }
        public Color WallColor { get; set; }
        public Color CytoColor { get; set; }

        //public RenderTarget2D Sprite { get; set; }


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
                genome = "";
                a = 0;
                g = 0;
                t = 0;
                c = 0;
                for (int i = 0; i < LENGTH; i++)
                {
                    int caseSwitch = rand.Next(0, 4);
                    // generate DNA sequence
                    switch (caseSwitch)
                    {
                        // 0: a
                        case 0:
                            genome += "a";
                            a++;
                            break;
                        // 1: g
                        case 1:
                            genome += "g";
                            g++;
                            break;
                        // 2: t
                        case 2:
                            genome += "t";
                            t++;
                            break;
                        // 3: c 
                        case 3:
                            genome += "c";
                            c++;
                            break;
                        // unexpected value
                        default:
                            break;
                    }
                    // stop generating if one nucleotide exceeds max allowed
                    if (a > max || g > max || t > max || c > max)
                        break;
                }
                // make sure sequence is correct length (meaning we didn't break early)
                // as long as all nucleotides have at least the min, accept the sequence.
                if (genome.Length == LENGTH)
                    if (a >= min && g >= min && t >= min && c >= min)
                        return;
            }
        }

        public void TestGen(GraphicsDevice graphics)
        {
            Random rand = new Random();

            // calculate cell wall parameters
            int aveRad = a + this.g;                                        // 20-40        A+G
            double angVar = .5 * Math.Tanh(c / t - 1) + .5;                 // 0.0-1.0      C:T
            double radVar = .2 * Math.Tanh(4.0 / 5.0 * (c / t) - 1) + .2;   // 0.00-0.15    A:G

            int max = Math.Max(Math.Max(a, this.g), Math.Max(t, c));
            int min = Math.Min(Math.Min(a, this.g), Math.Min(t, c));

            int verts = Convert.ToInt32(8 * Math.Tanh(max / min / 16 - 1) + 10);    // 4-16         MAX:MIN

            // determine base color
            int r = 0;
            int g = 0;
            int b = 0;
            // attack > defense -> RED
            if (a >= this.g)
            {
                r = 255;
                // buff > debuff -> ORANGE
                if (t >= c)
                    g = 150 * (t / a);
                // debuff > buff -> MAGENTA
                else
                    b = 200 * (c / a);
            }
            // defense > attack -> GREEN
            else
            {
                g = 255;
                // debuff > buff -> TEAL
                if (c >= t)
                    b = 200 * (c / this.g);
                // buff > debuff -> YELLOW
                else
                {
                    b = 100;
                    r = 100 + 155 * (t / this.g);
                }
            }

            this.CytoColor = new Color(r, g, b);
            this.WallColor = Color.Navy;

            // generate cell wall polygon
            this.Vertices = GenCellWallVertices(rand, CENTER, aveRad, angVar, radVar, verts);
            GraphUtils.Triangulate(this.Vertices, out int[] triangles, out string errorMessage);
            this.Triangles = triangles;

        }

        public void GenSprite(GraphicsDevice graphicsDevice)
        {
            Random rand = new Random();

            // calculate cell wall parameters
            int aveRad = a + this.g;                                             // 20-40        A+G
            double angVar = .5 * Math.Tanh(c / t - 1) + .5;                 // 0.0-1.0      C:T
            double radVar = .2 * Math.Tanh(4.0 / 5.0 * (c / t) - 1) + .2;   // 0.00-0.15    A:G

            int max = Math.Max(Math.Max(a, this.g), Math.Max(t, c));
            int min = Math.Min(Math.Min(a, this.g), Math.Min(t, c));

            int verts = Convert.ToInt32(8 * Math.Tanh(max / min / 16 - 1) + 10);    // 4-16         MAX:MIN

            // determine base color
            int r = 0;
            int g = 0;
            int b = 0;
            // attack > defense -> RED
            if (a >= this.g)
            {
                r = 255;
                // buff > debuff -> ORANGE
                if (t >= c)
                    g = 150 * (t / a);
                // debuff > buff -> MAGENTA
                else
                    b = 200 * (c / a);
            }
            // defense > attack -> GREEN
            else
            {
                g = 255;
                // debuff > buff -> TEAL
                if (c >= t)
                    b = 200 * (c / this.g);
                // buff > debuff -> YELLOW
                else
                {
                    b = 100;
                    r = 100 + 155 * (t / this.g);
                }
            }

            Color baseColor = new Color(r, g, b);
            Color wallColor = Color.Navy;

            // generate cell wall polygon
            this.Vertices = GenCellWallVertices(rand, CENTER, aveRad, angVar, radVar, verts);
            //wallPoints = ExpandPoints(wallPoints, CENTER);

            /*
            // here's an ugly stupid conversion from Point to Vector2 :p
            Vector2[] converted = new Vector2[wallPoints.Length+1];
            for (int i = 0; i < wallPoints.Length; i++)
                converted[i] = new Vector2(wallPoints[i].X, wallPoints[i].Y);
            converted[converted.Length - 1] = converted[0];
            */

            // draw cell wall
            //Sprite = new RenderTarget2D(graphicsDevice, X_DIM, Y_DIM);
            //graphicsDevice.SetRenderTarget(Sprite);
            //graphicsDevice.Clear(Color.Transparent);
            //Brush brush = new SolidColorBrush(baseColor);

            //BasicEffect basicEffect = new BasicEffect(graphicsDevice);
            //basicEffect.VertexColorEnabled = true;
            //basicEffect.LightingEnabled = false;
            ////basicEffect.World = Matrix.CreateOrthographicOffCenter(0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height, 0, 0, 1);
            //EffectTechnique effectTechnique = basicEffect.Techniques[0];
            //EffectPassCollection effectPassCollection = effectTechnique.Passes;

            //VertexBuffer vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionColor), wallPoints.Length, BufferUsage.WriteOnly);
            //vertexBuffer.SetData<VertexPositionColor>(wallPoints);
            //graphicsDevice.SetVertexBuffer(vertexBuffer);
            //foreach(EffectPass pass in effectPassCollection)
            //{
            //    pass.Apply();
            //    graphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip, wallPoints, 0, wallPoints.Length-1);
            //}
            //graphicsDevice.SetRenderTarget(null);
       
            /*
            drawBatch.Begin();
            //drawBatch.FillCircle(Brush.Black, new Vector2(CENTER.X, CENTER.Y), 50);
            drawBatch.FillPath(brush, converted);
            //drawBatch.DrawPath(new GraphicsPath(new Pen(Color.Black, 3), converted));
            drawBatch.End();
            graphicsDevice.SetRenderTarget(null);
            */


            /*
            // draw cell wall
            Sprite = new RenderTarget2D(graphicsDevice, X_DIM, Y_DIM);
            graphicsDevice.SetRenderTarget(Sprite);
            graphicsDevice.Clear(Color.Transparent);
            sb.Begin();
            for (int i = 0; i < wallPoints.Length-1; i++)
                sb.BorderLine(converted[i], converted[i + 1], 0f, Color.Black, 1);
            sb.BorderLine(converted[converted.Length - 1], converted[0], 0f, Color.Black, 1);
            sb.End();
            graphicsDevice.SetRenderTarget(null);
            */

            /*
            // Before refactoring GenPolygonPoints() and ExpandPoints(),
            // I want to make sure MonoGame.Extended.Shapes works.
            // So here's an ugly stupid conversion from Point to Vector2 :p
            Vector2[] converted = new Vector2[wallPoints.Length];
            for (int i = 0; i < converted.Length; i++)
                converted[i] = new Vector2(wallPoints[i].X, wallPoints[i].Y);
            
            Polygon polygon = new Polygon(converted);
            PrimitiveBatch primitiveBatch = new PrimitiveBatch(graphicsDevice);
            PrimitiveDrawing primitiveDrawing = new PrimitiveDrawing(primitiveBatch);
            Matrix localProjection = Matrix.CreateOrthographicOffCenter(0f, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height, 0f, 0f, 1f);
            Matrix localView = Matrix.Identity;

            Texture2D tx = new RenderTarget2D(graphicsDevice, X_DIM, Y_DIM);
            primitiveBatch.Begin(ref localProjection, ref localView);
            primitiveDrawing.DrawSolidPolygon(Vector2.Zero, polygon.Vertices, baseColor);
            primitiveDrawing.DrawPolygon(Vector2.Zero, polygon.Vertices, Color.Black);
            primitiveBatch.End();
            graphicsDevice.SetRenderTarget(null);

            Sprite = tx;
            */

            /*
            Bitmap bmp = new Bitmap(X_DIM, Y_DIM);
            Graphics graphics = Graphics.FromImage(bmp);
            Pen pen1 = new Pen(Color.Navy, 3);
            Brush brush1 = new SolidBrush(baseColor);
            //Pen pen2 = new Pen(COLOR_3, LINE_WEIGHT);
            //Brush brush2 = new SolidBrush(COLOR_4);
            graphics.FillPolygon(brush1, wallPoints);
            graphics.DrawPolygon(pen1, wallPoints);
            //graphics.FillPolygon(brush2, points2);
            //graphics.DrawPolygon(pen2, points2);
            //bmp.Save(name + "\\sample_" + (i + 1) + ".png");

            Texture2D tx = null;
            using (MemoryStream s = new MemoryStream())
            {
                bmp.Save(s, Imaging.ImageFormat.Png);
                s.Seek(0, SeekOrigin.Begin);
                tx = Texture2D.FromStream(graphicsDevice, s);
            }

            Sprite = tx;
            */
        }

        // helper function for GenSprite()
        public static Vector2[] GenCellWallVertices(Random rand, Vector2 center, int aveRadius, double irregularity, double spikeyness, int numVerts, double slice = 0.25)
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
            List<Vector2> points = new List<Vector2>();
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
                points.Add(new Vector2(Convert.ToInt32(Math.Round(x)), Convert.ToInt32(Math.Round(y))));
            }

            return ExpandPoints(points.ToArray(), center);
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
            Console.WriteLine("A: " + a + " (" + Math.Round((double)a / LENGTH * 100, 2) + "%)");
            Console.WriteLine("G: " + g + " (" + Math.Round((double)g / LENGTH * 100, 2) + "%)");
            Console.WriteLine("T: " + t + " (" + Math.Round((double)t / LENGTH * 100, 2) + "%)");
            Console.WriteLine("C: " + c + " (" + Math.Round((double)c / LENGTH * 100, 2) + "%)");

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
                        if ( c < genome.Length)
                            Console.Write(genome[c]);
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
