using Microsoft.Xna.Framework;
using System;

namespace Plasmid.Graphics
{
    public struct Transform
    {
        public float TranslationX;
        public float TranslationY;
        public float Rotation;
        public float ScaleX;
        public float ScaleY;
        public float CosScaleX;
        public float SinScaleX;
        public float CosScaleY;
        public float SinScaleY;


        public Transform(Vector2 translation, float rotation, Vector2 scale)
        {
            this.TranslationX = translation.X;
            this.TranslationY = translation.Y;
            this.Rotation = rotation;
            this.ScaleX = scale.X;
            this.ScaleY = scale.Y;

            float sin = MathF.Sin(rotation);
            float cos = MathF.Cos(rotation);

            this.CosScaleX = cos * ScaleX;
            this.SinScaleX = sin * ScaleX;
            this.CosScaleY = cos * ScaleY;
            this.SinScaleY = sin * ScaleY;
        }
        public Transform(Vector2 translation, float rotation, float scale)
        {
            this.TranslationX = translation.X;
            this.TranslationY = translation.Y;
            this.Rotation = rotation;
            this.ScaleX = scale;
            this.ScaleY = scale;

            float sin = MathF.Sin(rotation);
            float cos = MathF.Cos(rotation);

            this.CosScaleX = cos * ScaleX;
            this.SinScaleX = sin * ScaleX;
            this.CosScaleY = cos * ScaleY;
            this.SinScaleY = sin * ScaleY;
        }

        public static Transform Identity() { return new Transform(Vector2.Zero, 0f, 1f); }
        public static Transform Shift(float deltaX, float deltaY) { return new Transform(new Vector2(deltaX, deltaY), 0f, 1f); }
        public static Transform Shift(Vector2 translation) { return Shift(translation.X, translation.Y); }
        public static Transform HorizontalShift(float deltaX) { return new Transform(new Vector2(deltaX, 0), 0f, 1f); }
        public static Transform VerticalShift(float deltaY) { return new Transform(new Vector2(0, deltaY), 0f, 1f); }
        public static Transform Rotate(float radians) { return new Transform(Vector2.Zero, radians, 1f); }
        public static Transform Scale(float factor) { return new Transform(Vector2.Zero, 0f, factor); }
        public static Transform Scale(float horizontal, float vertical) { return new Transform(Vector2.Zero, 0f, new Vector2(horizontal, vertical) ); }
        public static Transform Scale(Vector2 scale) { return new Transform(Vector2.Zero, 0f, scale ); }

        public Transform Combine(Transform other)
        {
            return new Transform(
                new Vector2(this.TranslationX + other.TranslationX, this.TranslationY + other.TranslationY),
                this.Rotation + other.Rotation,
                new Vector2(this.ScaleX * other.ScaleX, this.ScaleY * other.ScaleY) );
        }

        public Vector2 Apply(Vector2 position)
        {
            return new Vector2(
                position.X * this.CosScaleX - position.Y * this.SinScaleY + this.TranslationX,
                position.X * this.SinScaleX + position.Y * this.CosScaleY + this.TranslationY);
        }

        public void Apply(float x, float y, out float newX, out float newY)
        {
            newX = x * this.CosScaleX - y * this.SinScaleY + this.TranslationX;
            newY = x * this.SinScaleX + y * this.CosScaleY + this.TranslationY;
        }

        public Matrix ToMatrix()
        {
            Matrix result = Matrix.Identity;

            result.M11 = this.CosScaleX;
            result.M12 = this.SinScaleY;
            result.M21 = -this.SinScaleX;
            result.M22 = this.CosScaleY;
            result.M41 = this.TranslationX;
            result.M42 = this.TranslationY;
            return result;
        }

        public override bool Equals(object other)
        {
            if (other is Transform trans)
                return this.TranslationX == trans.TranslationX &&
                    this.TranslationY == trans.TranslationY &&
                    this.CosScaleX == trans.CosScaleX &&
                    this.SinScaleX == trans.SinScaleX &&
                    this.CosScaleY == trans.CosScaleY &&
                    this.SinScaleY== trans.SinScaleY;
            else
                return false;
        }
    }
}
