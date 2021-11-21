using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Plasmid.Graphics
{
    public class ShapeBatcher : IDisposable
    {

        public static readonly int MaxVertexCount = 1024;
        public static readonly int MaxIndexCount = MaxVertexCount * 3;
        public static readonly float MinLineThickness = 1f;
        public static readonly float MaxLineThickness = 64f;

        private Game game;
        private Camera camera;

        private BasicEffect effect;

        private VertexPositionColor[] vertices;
        private int[] indices;

        private int shapeCount;
        private int vertexCount;
        private int indexCount;

        private bool isStarted;
        private bool isDisposed;


        public ShapeBatcher(Game game)
        {
            this.game = game ?? throw new ArgumentNullException("game");
            isDisposed = false;

            effect = new BasicEffect(game.GraphicsDevice);
            effect.TextureEnabled = false;
            effect.FogEnabled = false;
            effect.LightingEnabled = false;
            effect.VertexColorEnabled = true;
            effect.World = Matrix.Identity;
            effect.View = Matrix.Identity;
            effect.Projection = Matrix.Identity;

            vertices = new VertexPositionColor[MaxVertexCount];
            indices = new int[MaxIndexCount];

            this.shapeCount = 0;
            this.vertexCount = 0;
            this.indexCount = 0;

            this.isStarted = false;

            this.camera = null;
        }

        public void Dispose()
        {
            if (isDisposed)
                return;

            effect?.Dispose();
            isDisposed = true;
        }

        public void Begin(Camera camera=null)
        {
            if (isStarted)
                throw new Exception("batch already started");

            if (camera is null)
            {
                Viewport vp = this.game.GraphicsDevice.Viewport;
                this.effect.Projection = Matrix.CreateOrthographicOffCenter(0, vp.Width, 0, vp.Height, 0f, 1f);
                this.effect.View = Matrix.Identity;
            }
            else
            {
                camera.UpdateMatrices();

                this.effect.View = camera.View;
                this.effect.Projection = camera.Projection;
            }

            this.camera = camera;

            isStarted = true;
        }

        public void End()
        {
            Flush();
            isStarted = false;
        }

        public void Flush()
        {
            if (shapeCount == 0)
                return;

            CheckStarted();
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                game.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
                    PrimitiveType.TriangleList, vertices, 0, vertexCount, indices, 0, indexCount / 3);
            }

            shapeCount = 0;
            vertexCount = 0;
            indexCount = 0;

        }

        private void CheckStarted()
        {
            if (!isStarted)
                throw new Exception("batch never started");
        }

        private void CheckSpace(int shapeVertexCount, int shapeIndexCount)
        {
            // Max size limits for single shape
            if (shapeVertexCount > vertices.Length)
                throw new Exception("Max vertex count is: " + vertices.Length);
            if (shapeIndexCount > indices.Length)
                throw new Exception("Max index count is: " + indices.Length);

            // Space remaining for new shape
            if (vertexCount + shapeVertexCount >= vertices.Length || indexCount + shapeIndexCount >= indices.Length)
                Flush();
        }

        public void DrawRectangleFill(Rectangle rectangle, Color color)
        {
            DrawRectangleFill(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, color);
        }

        public void DrawRectangleFill(float x, float y, float width, float height, Color color)
        {
            CheckStarted();

            const int shapeVertexCount = 4;
            const int shapeIndexCount = 6;

            this.CheckSpace(shapeVertexCount, shapeIndexCount);

            float left = x;
            float right = x + width;
            float bottom = y;
            float top = y + height;

            indices[indexCount++] = 0 + vertexCount;
            indices[indexCount++] = 1 + vertexCount;
            indices[indexCount++] = 2 + vertexCount;
            indices[indexCount++] = 0 + vertexCount;
            indices[indexCount++] = 2 + vertexCount;
            indices[indexCount++] = 3 + vertexCount;

            vertices[vertexCount++] = new VertexPositionColor(new Vector3(left, top, 0f), color);
            vertices[vertexCount++] = new VertexPositionColor(new Vector3(right, top, 0f), color);
            vertices[vertexCount++] = new VertexPositionColor(new Vector3(right, bottom, 0f), color);
            vertices[vertexCount++] = new VertexPositionColor(new Vector3(left, bottom, 0f), color);

            shapeCount++;

        }

        public void DrawLine(Vector2 a, Vector2 b, float thickness, Color color)
        {
            this.DrawLine(a.X, a.Y, b.X, b.Y, thickness, color);
        }

        public void DrawLine(float ax, float ay, float bx, float by, float thickness, Color color)
        {
            CheckStarted();

            const int shapeVertexCount = 4;
            const int shapeIndexCount = 6;

            CheckSpace(shapeVertexCount, shapeIndexCount);

            thickness = GraphUtils.Clamp(thickness, ShapeBatcher.MinLineThickness, ShapeBatcher.MaxLineThickness);
            thickness++;

            if (this.camera != null)
                thickness *= this.camera.Z / this.camera.BaseZ;
            float halfThickness = thickness / 2;

            //Vector2 e1 = b - a;
            float e1x = bx - ax;
            float e1y = by - ay;

            //e1.Normalize();
            //e1 *= halfThickness;
            GraphUtils.Normalize(ref e1x, ref e1y);
            e1x *= halfThickness;
            e1y *= halfThickness;

            //Vector2 e2 = -e1;
            float e2x = -e1x;
            float e2y = -e1y;

            //Vector2 n1 = new Vector2(-e1.Y, e1.X);
            float n1x = -e1y;
            float n1y = e1x;

            //Vector2 n2 = -n1;
            float n2x = -n1x;
            float n2y = -n1y;

            //Vector2 q1 = a + n1 + e2;
            float q1x = ax + n1x + e2x;
            float q1y = ay + n1y + e2y;

            //Vector2 q2 = b + n1 + e1;
            float q2x = bx + n1x + e1x;
            float q2y = by + n1y + e1y;

            //Vector2 q3 = b + n2 + e1;
            float q3x = bx + n2x + e1x;
            float q3y = by + n2y + e1y;

            //Vector2 q4 = a + n2 + e2;
            float q4x = ax + n2x + e2x;
            float q4y = ay + n2y + e2y;

            indices[indexCount++] = 0 + vertexCount;
            indices[indexCount++] = 1 + vertexCount;
            indices[indexCount++] = 2 + vertexCount;
            indices[indexCount++] = 0 + vertexCount;
            indices[indexCount++] = 2 + vertexCount;
            indices[indexCount++] = 3 + vertexCount;

            vertices[vertexCount++] = new VertexPositionColor(new Vector3(q1x, q1y, 0f), color);
            vertices[vertexCount++] = new VertexPositionColor(new Vector3(q2x, q2y, 0f), color);
            vertices[vertexCount++] = new VertexPositionColor(new Vector3(q3x, q3y, 0f), color);
            vertices[vertexCount++] = new VertexPositionColor(new Vector3(q4x, q4y, 0f), color);


            shapeCount++;
        }

        public void DrawRectangle(float x, float y, float width, float height, float thickness, Color color)
        {
            float left = x;
            float right = x + width;
            float bottom = y;
            float top = y + height;

            this.DrawLine(left, top, right, top, thickness, color);
            this.DrawLine(right, top, right, bottom, thickness, color);
            this.DrawLine(right, bottom, left, bottom, thickness, color);
            this.DrawLine(left, bottom, left, top, thickness, color);
        }

        public void DrawCircle(float x, float y, float radius, int points, float thickness, Color color)
        {
            const int minPoints = 3;
            const int maxPoints = 256;

            points = GraphUtils.Clamp(points, minPoints, maxPoints);

            float rotation = MathHelper.TwoPi / (float)points;

            float sin = MathF.Sin(rotation);
            float cos = MathF.Cos(rotation);

            float ax = radius;
            float ay = 0f;
            float bx;
            float by;

            for (int i = 0; i < points; i++)
            {
                bx = cos * ax - sin * ay;
                by = sin * ax + cos * ay;

                this.DrawLine(ax + x, ay + y, bx + x, by + y, thickness, color);

                ax = bx;
                ay = by;
            }

        }

        public void DrawCircleFill(float x, float y, float radius, int points, Color color)
        {
            this.CheckStarted();

            const int minPoints = 3;
            const int maxPoints = 256;

            int shapeVertexCount = GraphUtils.Clamp(points, minPoints, maxPoints);
            int shapeTriangleCount = shapeVertexCount - 2;
            int shapeIndexCount = shapeTriangleCount * 3;

            this.CheckSpace(shapeVertexCount, shapeIndexCount);

            for (int i = 0; i < shapeTriangleCount; i++)
            {
                this.indices[this.indexCount++] = this.vertexCount;
                this.indices[this.indexCount++] = i + 1 + this.vertexCount;
                this.indices[this.indexCount++] = i + 2 + this.vertexCount;
            }

            points = GraphUtils.Clamp(points, minPoints, maxPoints);

            float rotation = MathHelper.TwoPi / (float)shapeVertexCount;

            float sin = MathF.Sin(rotation);
            float cos = MathF.Cos(rotation);

            float ax = radius;
            float ay = 0f;

            for (int i = 0; i < shapeVertexCount; i++)
            {
                this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(ax + x, ay + y, 0f), color);

                float bx = cos * ax - sin * ay;
                float by = sin * ax + cos * ay;

                ax = bx;
                ay = by;
            }

            this.shapeCount++;

        }

        public void DrawPolygon(Vector2[] polyVertices, Transform transform, float thickness, Color color)
        {
            if (polyVertices is null)
                return;

            for (int i = 0; i < polyVertices.Length; i++)
            {
                Vector2 a = polyVertices[i];
                Vector2 b = polyVertices[(i + 1) % polyVertices.Length];

                a = GraphUtils.ApplyTransform(a, transform);
                b = GraphUtils.ApplyTransform(b, transform);

                this.DrawLine(a, b, thickness, color);
            }
        }

        public void DrawPolygonFill(Vector2[] polyVertices, int[] triangleIndices, Transform transform, Color color)
        {
            if (polyVertices is null || indices is null)
                return;

            //if (vertices is null)
            //    throw new ArgumentNullException("vertices");
            //if (indices is null)
            //    throw new ArgumentNullException("indices");

            if (polyVertices.Length < 3)
                throw new ArgumentOutOfRangeException("vertices");
            if (indices.Length < 3)
                throw new ArgumentOutOfRangeException("indices");

            this.CheckStarted();
            this.CheckSpace(polyVertices.Length, triangleIndices.Length);

            for (int i = 0; i < triangleIndices.Length; i++)
                this.indices[this.indexCount++] = triangleIndices[i] + this.vertexCount;

            for (int i = 0; i < polyVertices.Length; i++)
            {
                Vector2 vertex = polyVertices[i];
                vertex = GraphUtils.ApplyTransform(vertex, transform);
                this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(vertex.X, vertex.Y, 0f), color);
            }

            this.shapeCount++;
        }

        public void DrawPolyTriangles(Vector2[] vertices, int[] triangles, Transform transform, Color color)
        {
            for (int i = 0; i < triangles.Length; i++)
            {
                int a = triangles[i];
                int b = Utils.GetItem(triangles, i+1);
                int c = Utils.GetItem(triangles, i+2);

                Vector2 va = vertices[a];
                Vector2 vb = vertices[b];
                Vector2 vc = vertices[c];

                va = GraphUtils.ApplyTransform(va, transform);
                vb = GraphUtils.ApplyTransform(vb, transform);
                vc = GraphUtils.ApplyTransform(vc, transform);

                this.DrawLine(va, vb, 1f, color);
                this.DrawLine(vb, vc, 1f, color);
                this.DrawLine(vc, va, 1f, color);

            }
        }

    }

}
