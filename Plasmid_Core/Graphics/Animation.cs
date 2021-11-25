using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Plasmid.Graphics
{
    public enum AnimationType
    {
        SINGLE,
        REPEAT
    }

    public class Animation : IDisposable
    {
        private static Game1 game;
        public static List<Animation> All = new List<Animation>();

        private List<IShape> shapeList;
        private Queue<Transform> transformList;
        private double duration;
        private double startTime;

        public AnimationType Type { get => this.type; }
        private AnimationType type;

        public bool IsActive { get => this.isActive; }
        private bool isActive;
        public bool IsDisposed { get => this.isDisposed; }
        private bool isDisposed;



        protected Animation(AnimationType type)
        {
            this.type = type;
            this.shapeList = new List<IShape>();
            this.transformList = new Queue<Transform>();
            this.duration = 0;

            this.isActive = false;
        }

        public static Animation New(AnimationType type = AnimationType.SINGLE)
        {
            if (Animation.game is null)
                throw new Exception("Run Animation.Init(Game1 game) before creating instance.");

            Animation.All.Add(new Animation(type));
            return Animation.All[^1];
        }

        public static void Init(Game1 game)
        {
            Animation.game = game;
        }

        public void AddShape(IShape item)
        {
            this.shapeList.Add(item);
        }

        public void AddTransformation(Transform transform)
        {
            this.transformList.Enqueue(transform);
        }

        public void SetDuration(double milliseconds)
        {
            this.duration = milliseconds;
        }

        public void Start()
        {
            if (this.isActive)
                throw new Exception("Animation already started");

            if (this.type == AnimationType.REPEAT)
            {
                Random rand = new Random();
                for (int i = 0; i < rand.Next(this.transformList.Count); i++)
                    this.NextTransform();
            }

            this.startTime = -1;
            this.isActive = true;
        }
        public void End()
        {
            if (!this.isActive)
                throw new Exception("Animation already ended");

            this.isActive = false;

            //if (this.type == AnimationType.SPRITE_SINGLE || this.type == AnimationType.SHAPE_SINGLE)
            //    this.Dispose();
        }

        public static void UpdateAll(double time)
        {
            foreach (Animation animation in Animation.All)
                animation.Update(time);
        }

        public void Update(double time)
        {
            if (!this.isActive)
                return;

            if (this.startTime < 0)
                this.startTime = time;

            // SINGLE
            if (this.type == AnimationType.SINGLE)
            {
                if (time - this.startTime > duration)
                    this.End();
            }
            // REPEAT
            if (this.type == AnimationType.REPEAT)
            {
                if (time - this.startTime > duration)
                {
                    this.NextTransform();
                    this.startTime = time;
                }
            }
        }

        private void NextTransform()
        {
            Transform transform = this.transformList.Dequeue();
            foreach (IShape shape in this.shapeList)
                shape.ApplyTransform(transform);
            this.transformList.Enqueue(transform);
        }

        public static void DrawAll()
        {
            foreach (Animation animation in Animation.All)
                animation.Draw();
        }

        public void Draw() { this.Draw(Vector2.Zero); }
        public void Draw(Vector2 position)
        {
            if (!this.isActive)
                return;
            //if (!position.Equals(Vector2.Zero))
            //    foreach (IShape shape in this.shapeList)
            //        shape.ApplyTransform(Transform.Shift(position));

            foreach (IShape shape in this.shapeList)
                shape.Draw(Animation.game, position);
                //shape.Draw(Animation.game);
        }

        public void Dispose()
        {
            if (this.isDisposed)
                return;

            this.isActive = false;

            this.isDisposed = true;
            GC.SuppressFinalize(this);
        }

    }
}
