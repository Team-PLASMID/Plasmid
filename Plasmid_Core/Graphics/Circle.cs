using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Plasmid.Graphics
{
    public struct Circle : IShape
    {
        public Vector2 Center { get; set; }
        public float Radius { get; set; }
        public Transform Transform { get; set; }
        public Color Color { get; set; }
        public bool Fill { get; set; }
        public float Thickness { get; set; }
        public int NumVertices { get; set; }

        public Circle(float x, float y, float radius, Color color, bool fill, float thickness)
            : this(new Vector2(x, y), radius, color, fill, thickness) { }
        public Circle(Vector2 center, float radius, Color color, bool fill, float thickness, int numVertices=0)
        {
            this.Center = center;
            this.Radius = radius;
            this.Transform = Transform.Identity();
            this.Color = color;
            this.Fill = fill;
            this.Thickness = thickness;

            if (numVertices < 3)
                this.NumVertices = CalcOptimalVertices(this.Radius);
            else
                this.NumVertices = numVertices;
        }
        public void Draw(Game1 game)
        {
            this.Draw(game, Vector2.Zero);
        }
        public void Draw(Game1 game, Vector2 position)
        {
            Transform transform = this.Transform.Combine(Transform.Shift(position));

            if (this.Fill)
                game.Shapes.DrawCircleFill(this.Center.X, this.Center.Y, this.Radius, transform, this.NumVertices, this.Color);
            else
                game.Shapes.DrawCircle(this.Center.X, this.Center.Y, this.Radius, transform, this.NumVertices, this.Thickness, this.Color);
        }

        public static int CalcOptimalVertices(float radius, float error=0.5f)
        {
            float th = (float)Math.Acos(2 * Math.Pow(1 - error / radius, 2) - 1);
            return (int)Math.Ceiling(2 * Math.PI / Math.Acos(2 * Math.Pow(1 - error / radius, 2) - 1));
        }

        public void ApplyTransform(Transform otherTransform)
        {
            this.Transform = this.Transform.Combine(otherTransform);
        }

        public override bool Equals(object other)
        {
            if (other is Circle circle)
                return this.Center == circle.Center && this.Radius == circle.Radius && this.Thickness == circle.Thickness && this.Color == circle.Color;
            else
                return false;
        }

        public bool Contains(Vector2 point)
        {
            return GraphUtils.Distance(this.Center, point) < this.Radius;
        }

        public override string ToString()
        {
            string result = string.Format("Center: {0}, Radius: {1}", this.Center, this.Radius);
            return result;
        }

        public void Draw(Game1 game, Color color, Transform transform, bool fill, int thickness)
        {
            throw new NotImplementedException();
        }
    }
}
