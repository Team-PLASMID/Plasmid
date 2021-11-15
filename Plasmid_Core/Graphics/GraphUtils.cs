using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Plasmid.Graphics
{
    public enum WindingOrder { CW, CCW }

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
                position.X * transform.CosScaleX - position.Y * transform.SinScaleY + transform.PosX,
                position.X * transform.SinScaleX + position.Y * transform.CosScaleY + transform.PosY );
        }

        public static float Cross(Vector2 a, Vector2 b)
        {
            return a.X * b.Y - a.Y * b.X;
        }

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

        public static bool Triangulate(Vector2[] vertices, out int[] triangles, out string errorMessage)
        {
            Debug.WriteLine("Triangulate()");
            // check vert is ear
            // 1 acute angle
            // 2 triangle formed by adjacent verts doesn't contain any other verts

            errorMessage = string.Empty;
            triangles = null;

            if (vertices is null)
            {
                errorMessage = "vertex list is null";
                return false;
            }
            if (vertices.Length < 3)
            {
                errorMessage = "vertex list must have length of at least 3";
                return false;
            }
            if (vertices.Length > 1024)
            {
                errorMessage = "vertex list must have length less than 1024";
                return false;
            }

            Debug.WriteLine("passed checks");

            //if (!IsSimplePolygon(vertices))
            //{
            //    errorMessage = "not simple polygon";
            //    return false;
            //}

            // Shouldn't I just remove the vertex on the colinear edge?
            //if (ContainsColinearEdges(vertices))
            //{
            //    errorMessage = "polygon has colinear edges";
            //    return false;
            //}

            //ComputePolygonArea(vertices, out float area, out WindingOrder windingOrder);

            //if (windingOrder is WindingOrder.Invalid)
            //{
            //    errorMessage = "invalid polygon";
            //    return false;
            //}

            //if (windingOrder is WindingOrder.CCW)
            //    Array.Reverse(vertices);
            Array.Reverse(vertices);

            List<int> indexList = new List<int>();
            for (int i = 0; i < vertices.Length; i++)
                indexList.Add(i);

            int totalTriangleCount = vertices.Length - 2;
            int totalTriangleIndexCount = totalTriangleCount * 3;

            triangles = new int[totalTriangleIndexCount];
            int triangleIndexCount = 0;


            Debug.WriteLine("reached while");
            while (indexList.Count > 3)
            {
                for (int i = 0; i < indexList.Count; i++)
                {
                    int a = indexList[i];
                    int b = GraphUtils.GetItem(indexList, i - 1);
                    int c = GraphUtils.GetItem(indexList, i + 1);

                    Vector2 va = vertices[a];
                    Vector2 vb = vertices[b];
                    Vector2 vc = vertices[c];

                    Vector2 va_to_vb = vb - va;
                    Vector2 va_to_vc = vc - va;

                    // check convex
                    if (GraphUtils.Cross(va_to_vb, va_to_vc) < 0f)
                    {
                        continue;
                    }

                    bool isEar = true;

                    // check triangle contains other vertex
                    for (int j = 0; j < vertices.Length; j++)
                    {
                        if (j == a || j == b || j == c)
                            continue;

                        Vector2 p = vertices[j];
                        if (IsPointInTriangle(p, vb, va, vc))
                        {
                            isEar = false;
                            break;
                        }
                        
                    }

                    if(isEar)
                    {
                        triangles[triangleIndexCount++] = b;
                        triangles[triangleIndexCount++] = a;
                        triangles[triangleIndexCount++] = c;

                        indexList.RemoveAt(i);

                        break;
                    }
                }
            }

            triangles[triangleIndexCount++] = indexList[0];
            triangles[triangleIndexCount++] = indexList[1];
            triangles[triangleIndexCount++] = indexList[2];

            return true;
        }

        public static bool IsPointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
        {
            Vector2 ab = b - a;
            Vector2 bc = c - b;
            Vector2 ca = a - c;

            Vector2 ap = p - a; 
            Vector2 bp = p - b; 
            Vector2 cp = p - c;

            float cross1 = GraphUtils.Cross(ab, ap);
            float cross2 = GraphUtils.Cross(bc, bp);
            float cross3 = GraphUtils.Cross(ca, cp);

            if (cross1 > 0f || cross2 > 0f || cross3 > 0f)
                return false;

            return true;
        }

        // TODO
        public static bool IsSimplePolygon(Vector2[] vertices)
        {
            throw new NotImplementedException();
        }

        // TODO
        public static bool ContainsColinearEdges(Vector2[] vertices)
        {
            throw new NotImplementedException();
        }

        //TODO
        public static void ComputePolygonArea(Vector2[] vertices, out float area, out WindingOrder windingOrder)
        {
            throw new NotImplementedException();
        }


        public static Color GetRandomColor(Random rand)
        {
            return new Color(rand.Next(256), rand.Next(256), rand.Next(256));
        }

        public static double Gauss(Random rand, double mean, double stdDev)
        {
            double u1 = 1.0 - rand.NextDouble(); //uniform(0,1] random doubles
            double u2 = 1.0 - rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            return mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
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
