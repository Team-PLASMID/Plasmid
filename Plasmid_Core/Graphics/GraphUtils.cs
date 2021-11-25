using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Plasmid.Graphics
{
    public enum WindingOrder { Invalid, CW, CCW }

    public static class GraphUtils
    {
        public static void ToggleFullScreen(GraphicsDeviceManager graphics)
        {
            graphics.HardwareModeSwitch = false;
            graphics.ToggleFullScreen();

        }

        public static int Clamp(int value, int min, int max)
        {
            if (min > max)
                throw new ArgumentOutOfRangeException("min is greater than max");

            if (value < min)
                return min;
            else if (value > max)
                return max;
            else
                return value;
        }

        public static float Clamp(float value, float min, float max)
        {
            if (min > max)
                throw new ArgumentOutOfRangeException("min is greater than max");

            if (value < min)
                return min;
            else if (value > max)
                return max;
            else
                return value;
        }

        public static void Normalize(ref float x, ref float y)
        {
            float invLen = 1f / MathF.Sqrt(x * x + y * y);

            x *= invLen;
            y *= invLen;
        }

        public static Vector2 ApplyTransform(Vector2 position, Transform transform)
        {
            return new Vector2(
                position.X * transform.CosScaleX - position.Y * transform.SinScaleY + transform.TranslationX,
                position.X * transform.SinScaleX + position.Y * transform.CosScaleY + transform.TranslationY );
        }

        


        public static Color GetRandomColor(Random rand = null)
        {
            if (rand is null)
                rand = new Random();
            return new Color(rand.Next(256), rand.Next(256), rand.Next(256));
        }

        public static float Distance(Vector2 a, Vector2 b)
        {
            return (float)Math.Sqrt(Math.Pow((b.X - a.X), 2) + Math.Pow((b.Y - a.Y), 2));
        }








        //public static Vector2[] GenPolygonPoints(Random rand, Screen screen, Vector2 center, int aveRadius, double irregularity, double spikeyness, int numVerts, double slice = 0.25)
        //{
        //    slice = slice * 2 * Math.PI;
        //    irregularity = irregularity * slice / numVerts;
        //    spikeyness = spikeyness * aveRadius;

        //    // angle steps
        //    List<double> angleSteps = new List<double>();
        //    double lower = (slice / numVerts) - irregularity;
        //    double upper = (slice / numVerts) + irregularity;
        //    double sum = 0;
        //    for (int i = 0; i < numVerts + 1; i++)
        //    {
        //        double tmp = rand.NextDouble() * (upper - lower) + lower;
        //        angleSteps.Add(tmp);
        //        sum += tmp;
        //    }

        //    // normalize to fit slice
        //    double k = sum / slice;
        //    for (int i = 0; i < numVerts; i++)
        //        angleSteps[i] = angleSteps[i] / k;

        //    // generate points
        //    Vector2[] vertices = new Vector2[numVerts];
        //    double angle = 0;
        //    int limitingDim = Math.Min(screen.Height, screen.Width);
        //    int maxRadius = Convert.ToInt32(limitingDim * 0.45);
        //    int minRadius = Convert.ToInt32(limitingDim * 0.05);
        //    for (int i = 0; i < numVerts; i++)
        //    {
        //        //double MAX_RADIUS_FACTOR = 2;
        //        //double MIN_RADIUS_FACTOR = 0.5;
        //        angle += angleSteps[i];
        //        double r_i = Gauss(rand, aveRadius, spikeyness);
        //        r_i = Util.Clamp(Convert.ToInt32(r_i), minRadius, maxRadius);
        //        //if (r_i > MAX_RADIUS_FACTOR * aveRadius)
        //        //    r_i = MAX_RADIUS_FACTOR * aveRadius;
        //        //if (r_i < MIN_RADIUS_FACTOR * aveRadius)
        //        //    r_i = MIN_RADIUS_FACTOR * aveRadius;
        //        double x = center.X + r_i * Math.Cos(angle);
        //        double y = center.Y + r_i * Math.Sin(angle);
        //        vertices[i] = new Vector2(Convert.ToInt32(Math.Round(x)), Convert.ToInt32(Math.Round(y)));
        //    }

        //    return ExpandPoints(vertices, center);
        //}
      
        //private static Vector2[] ExpandPoints(Vector2[] vertices, Vector2 center, int factor = 4)
        //{
        //    Vector2[] expandedVertices = new Vector2[vertices.Length * factor];

        //    int j = 0;
        //    for (int i = 0; i < vertices.Length; i++)
        //    {
        //        expandedVertices[j] = vertices[i];
        //        j++;
        //    }
        //    for (int i = vertices.Length - 1; i >= 0; i--)
        //    {
        //        expandedVertices[j] = new Vector2(expandedVertices[i].X - 2 * (expandedVertices[i].X - center.X), expandedVertices[i].Y);
        //        j++;
        //    }
        //    for (int i = 0; i < vertices.Length; i++)
        //    {
        //        expandedVertices[j] = new Vector2(expandedVertices[i].X - 2 * (expandedVertices[i].X - center.X), expandedVertices[i].Y - 2 * (expandedVertices[i].Y - center.Y));
        //        j++;
        //    }
        //    for (int i = vertices.Length - 1; i >= 0; i--)
        //    {
        //        expandedVertices[j] = new Vector2(expandedVertices[i].X, expandedVertices[i].Y - 2 * (expandedVertices[i].Y - center.Y));
        //        j++;
        //    }

        //    return expandedVertices;
        //}

    }
}
