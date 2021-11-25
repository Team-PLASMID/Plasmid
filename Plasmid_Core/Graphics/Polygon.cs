using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Plasmid.Graphics
{
    public struct Polygon : IShape
    {
        public Vector2[] Vertices { get; set; }
        public int[] Triangles { get; set; }
        public Transform Transform { get; set; }
        public Color Color { get; set; }
        public bool Fill { get; set; }
        public float Thickness { get; set; }

        public Polygon(Polygon polygon)
            : this(polygon.Vertices, polygon.Triangles, polygon.Transform, polygon.Fill, polygon.Thickness, polygon.Color) { }
        public Polygon(Vector2[] vertices, Transform? transform, float thickness, Color color)
            : this(vertices, null, transform, false, thickness, color) { }
        public Polygon(Vector2[] vertices, int[] triangles, Transform? transform, Color color)
            : this(vertices, triangles, transform, true, null, color) { }
        public Polygon(Vector2[] vertices, int[] triangles, Transform? transform, bool fill, float? thickness, Color color)
        {
            if (vertices.Length < 3)
                throw new ArgumentException("Polygon needs 3 or more vertices.");
            //if (fill == true)
            //{
            //    if (triangles is null)
            //        throw new ArgumentException("Polygon set to Fill needs Triangles.");
            //    if (triangles.Length < 3)
            //        throw new ArgumentException("Polygon needs 3 or more vertices.");
            //}
            //else
            //{
            //    if (thickness is null || thickness <= 0)
            //        throw new ArgumentNullException("Polygon needs Thickness parameter unless set to Fill.");
            //}

            this.Vertices = vertices;
            this.Triangles = triangles;
            this.Transform = transform ?? Transform.Identity();
            this.Fill = fill;
            this.Thickness = thickness ?? 0f;
            this.Color = color;

            if (triangles is null && this.Fill)
                if (!this.Triangulate())
                    throw new Exception("Could not automatically triangulate polygon.");
        }
        public void Draw(Game1 game)
        {
            this.Draw(game, Vector2.Zero);
        }
        public void Draw(Game1 game, Vector2 position)
        {
            Transform transform = this.Transform.Combine(Transform.Shift(position));

            if (this.Fill)
                game.Shapes.DrawPolygonFill(this.Vertices, this.Triangles, transform, this.Color);
            else
                game.Shapes.DrawPolygon(this.Vertices, transform, this.Thickness, this.Color);
        }

        public void ApplyTransform(Transform otherTransform)
        {
            this.Transform = this.Transform.Combine(otherTransform);
        }

        public override bool Equals(object other)
        {
            if (other is Polygon polygon)
                return this.Vertices == polygon.Vertices &&
                    this.Triangles == polygon.Triangles &&
                    this.Transform.Equals(polygon.Transform) &&
                    this.Fill == polygon.Fill &&
                    this.Thickness == polygon.Thickness &&
                    this.Color == polygon.Color;
            else
                return false;
        }

        public bool Contains(Vector2 point)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }

        public bool Triangulate()
        {
            bool result = Polygon.Triangulate(this.Vertices, out int[] triangles, out string errorMessage);
            this.Triangles = triangles;

            return result;
        }
        public static bool Triangulate(Vector2[] vertices, out int[] triangles, out string errorMessage)
        {
            errorMessage = string.Empty;
            triangles = null;

            //Debug.WriteLine("Vertices:");
            //foreach (var item in vertices)
            //    Debug.WriteLine("  " + item.X + ", " + item.Y);

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

            ComputePolygonArea(vertices, out float area, out WindingOrder windingOrder);

            if (windingOrder is WindingOrder.Invalid)
            {
                errorMessage = "invalid polygon";
                return false;
            }

            if (windingOrder is WindingOrder.CW)
                Array.Reverse(vertices);


            List<int> indexList = new List<int>();
            for (int i = 0; i < vertices.Length; i++)
                indexList.Add(i);

            // TODO: remove indices of vertices with colinear edges

            int totalTriangleCount = vertices.Length - 2;
            int totalTriangleIndexCount = totalTriangleCount * 3;

            triangles = new int[totalTriangleIndexCount];
            int triangleIndexCount = 0;

            int progressCheck = indexList.Count;
            while (indexList.Count > 3)
            {
                for (int i = 0; i < indexList.Count; i++)
                {
                    int a = indexList[i];
                    int b = Utils.GetItem(indexList, i - 1);
                    int c = Utils.GetItem(indexList, i + 1);

                    Vector2 va = vertices[a];
                    Vector2 vb = vertices[b];
                    Vector2 vc = vertices[c];

                    Vector2 va_to_vb = vb - va;
                    Vector2 va_to_vc = vc - va;

                    // check convex
                    if (IsPointConvex(vertices, indexList, i))
                    {
                        Debug.WriteLine("convex");
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
                            Debug.WriteLine("intersection");
                            break;
                        }
                    }

                    if (isEar)
                    {
                        triangles[triangleIndexCount++] = b;
                        triangles[triangleIndexCount++] = a;
                        triangles[triangleIndexCount++] = c;

                        indexList.RemoveAt(i);

                        break;
                    }
                }

                if (progressCheck == indexList.Count)
                    throw new Exception("Polygon.Triangulate is stuck in an infinite loop :(");
                else
                    progressCheck = indexList.Count;
            }

            triangles[triangleIndexCount++] = indexList[0];
            triangles[triangleIndexCount++] = indexList[1];
            triangles[triangleIndexCount++] = indexList[2];

            return true;
        }




        public static bool IsPointConvex(Vector2[] vertices, List<int> indexList, int ind)
        {
            int iPoint = indexList[ind];
            int iPrev = Utils.GetItem(indexList, ind - 1);
            int iNext = Utils.GetItem(indexList, ind + 1);

            Vector2 point = vertices[iPoint];
            Vector2 prev = vertices[iPrev];
            Vector2 next = vertices[iNext];


            //float angle = GetAngle(prev - point, next - point);
            //if (angle < Math.PI)
            //    return false;
            //else
            //    return true;

            float dx1 = point.X - prev.X;
            float dy1 = point.Y - prev.Y;
            float dx2 = next.X - point.X;
            float dy2 = next.Y - point.Y;

            float zcrossproduct = dx1 * dy2 - dy1 * dx2;

            if (zcrossproduct < 0)
                return false;
            else
                return true;
        }

        public static bool IsPointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
        {
            if (Cross(b - a, p - a) > 0f || Cross(c - b, p - b) > 0f || Cross(a - c, p - c) > 0f)
                return false;
            else
                return true;
        }
        public static float Cross(Vector2 a, Vector2 b)
        {
            return a.X * b.Y - a.Y * b.X;
        }

        public float ComputePolygonArea()
        {
            Polygon.ComputePolygonArea(this.Vertices, out float area, out WindingOrder windingOrder);
            return area;
        }
        public static void ComputePolygonArea(Vector2[] vertices, out float area, out WindingOrder windingOrder)
        {
            area = 0f;
            windingOrder = WindingOrder.Invalid;

            for (int i = 0; i < vertices.Length; i++)
            {
                Vector2 va = vertices[i];
                Vector2 vb = vertices[(i + 1) % vertices.Length];

                float width = vb.X - va.X;
                float height = (vb.Y + va.Y) / 2f;

                area += width * height;

                if (area < 0)
                {
                    area = -area;
                    windingOrder = WindingOrder.CCW;
                }
                else
                    windingOrder = WindingOrder.CW;
            }
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
    }
}
