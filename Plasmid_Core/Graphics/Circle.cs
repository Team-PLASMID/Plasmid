using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Plasmid.Graphics
{
    public readonly struct Circle : IDrawable
    {
        public readonly Vector2 Center;
        public readonly float Radius;
        public readonly bool Fill;
        public readonly float Thickness;
        public readonly Color Color;
        public readonly int NumVertices;

        public Circle(float x, float y, float radius, bool fill, float thickness, Color color) : this(new Vector2(x, y), radius, fill, thickness, color) { }

        public Circle(Vector2 center, float radius, bool fill, float thickness, Color color, int numVertices=0)
        {
            this.Center = center;
            this.Radius = radius;
            this.Fill = fill;
            this.Thickness = thickness;
            this.Color = color;

            if (numVertices < 3)
                this.NumVertices = CalcOptimalVertices(this.Radius);
            else
                this.NumVertices = numVertices;
        }

        public void Draw(Game1 game)
        {
            if (this.Fill)
                game.Shapes.DrawCircleFill(this.Center.X, this.Center.Y, this.Radius, this.NumVertices, this.Color);
            else
                game.Shapes.DrawCircle(this.Center.X, this.Center.Y, this.Radius, this.NumVertices, this.Thickness, this.Color);
        }

        public static int CalcOptimalVertices(float radius, float error=0.5f)
        {
            float th = (float)Math.Acos(2 * Math.Pow(1 - error / radius, 2) - 1);
            return (int)Math.Ceiling(2 * Math.PI / Math.Acos(2 * Math.Pow(1 - error / radius, 2) - 1));
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


    }
}
