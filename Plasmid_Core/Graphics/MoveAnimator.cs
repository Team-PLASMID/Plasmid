using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Plasmid.Graphics
{
    public class MoveAnimator
    {
        public Random Rand { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public Vector2 MinVelocity { get; set; }
        public Vector2 MaxVelocity { get; set; }
        public Vector2 Destination { get; set; }
        public float TotalDistance { get; set; }
        public Rectangle Area { get; set; }
        public float MinX { get => Area.X; }
        public float MaxX { get => Area.X + Area.Width; }
        public float MinY { get => Area.Y; }
        public float MaxY { get => Area.Y + Area.Height; }
        public float SpeedFactor { get; set; }

        public MoveAnimator(Vector2 startPosition, Rectangle area, Random rand, float speedFactor=1f)
        {
            Rand = rand;
            Position = Destination = startPosition;
            SpeedFactor = speedFactor;
            Area = area;
            Velocity = Vector2.Zero;
        }

        public void Update()
        {
            if (Position == Destination)
            {
                Destination = NextDestination();
                TotalDistance = Vector2.Distance(Position, Destination);
                //RushFactor = (float)Rand.NextDouble() * .5f + .5f;
            }

            Velocity = Destination - Position;
            Velocity = Vector2.Normalize(Velocity);
            MinVelocity = .1f * Velocity;
            //MaxVelocity = .5f * RushFactor * Velocity;
            MaxVelocity = .5f * Velocity * SpeedFactor;
            float distance = Vector2.Distance(this.Destination, this.Position);
            if (distance <= 1f)
            {
                Position = Destination;
                return;
            }

            //if (distance < 6f)
            //    Velocity = Vector2.Lerp(MinVelocity, MaxVelocity, distance / TotalDistance.Length());
            //else
            //    Velocity = Vector2.Lerp(MinVelocity, MaxVelocity, (TotalDistance.Length() - distance)/TotalDistance.Length());
            
            Velocity = Vector2.Lerp(MinVelocity, MaxVelocity, (float)Math.Sqrt(TotalDistance - distance));
            Velocity = Vector2.Clamp(Velocity, MinVelocity, MaxVelocity);

            Position += Velocity;
        }


        public Vector2 NextDestination()
        {
            Vector2 destination = Position;
            while (Vector2.Distance(destination, Position) < 5f)
            {
                destination.X  = (float)Rand.NextDouble() * (MaxX - MinX) + MinX;
                destination.Y  = (float)Rand.NextDouble() * (MaxY - MinY) + MinY;
            }

            return destination;
        }
    }
}
