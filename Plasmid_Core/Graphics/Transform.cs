using Microsoft.Xna.Framework;
using System;

namespace Plasmid.Graphics
{
    public struct Transform
    {
        public float PosX;
        public float PosY;
        public float CosScaleX;
        public float SinScaleX;
        public float CosScaleY;
        public float SinScaleY;

        public static Transform Identity { get => new Transform(Vector2.Zero, 0f, 1f); }

        public Transform(Vector2 translation, float rotation, Vector2 scale)
        {
            this.PosX = translation.X;
            this.PosY = translation.Y;

            float sin = MathF.Sin(rotation);
            float cos = MathF.Cos(rotation);

            this.CosScaleX = cos * scale.X;
            this.SinScaleX = sin * scale.X;
            this.CosScaleY = cos * scale.Y;
            this.SinScaleY = sin * scale.Y;
        }
        public Transform(Vector2 translation, float rotation, float scale)
        {
            this.PosX = translation.X;
            this.PosY = translation.Y;

            float sin = MathF.Sin(rotation);
            float cos = MathF.Cos(rotation);

            this.CosScaleX = cos * scale;
            this.SinScaleX = sin * scale;
            this.CosScaleY = cos * scale;
            this.SinScaleY = sin * scale;
        }

        public Matrix ToMatrix()
        {
            Matrix result = Matrix.Identity;

            result.M11 = this.CosScaleX;
            result.M12 = this.SinScaleY;
            result.M21 = -this.SinScaleX;
            result.M22 = this.CosScaleY;
            result.M41 = this.PosX;
            result.M42 = this.PosY;
            return result;
        }

        public override bool Equals(object other)
        {
            if (other is Transform trans)
                return this.PosX == trans.PosX &&
                    this.PosY == trans.PosY &&
                    this.CosScaleX == trans.CosScaleX &&
                    this.SinScaleX == trans.SinScaleX &&
                    this.CosScaleY == trans.CosScaleY &&
                    this.SinScaleY== trans.SinScaleY;
            else
                return false;
        }
    }
}
