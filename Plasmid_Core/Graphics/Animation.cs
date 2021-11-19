using System;
using System.Collections.Generic;
using System.Text;

namespace Plasmid.Graphics
{
    public enum AnimationType { SPRITE_SINGLE, SPRITE_MULTI, SHAPE_SINGLE, SHAPE_MULTI }
    public class Animation : IDisposable
    {
        private static Game1 game;

        private AnimationType type;
        private List<IDrawable> drawables;
        private int index;
        private double duration;
        private double startTime;

        public bool IsActive { get { return this.isActive; } }
        private bool isActive;
        public bool IsDisposed { get { return this.isDisposed; } }
        private bool isDisposed;



        public Animation(AnimationType type)
        {
            if (Animation.game is null)
                throw new Exception("Run Animation.Init(Game1 game) before creating instance.");

            this.type = type;
            this.drawables = new List<IDrawable>();
            this.index = 0;
            this.duration = 0;

            this.isActive = false;
        }

        public static void Init(Game1 game)
        {
            Animation.game = game;
        }

        public void AddDrawable(IDrawable item)
        {
            this.drawables.Add(item);
        }

        public void SetDuration(double milliseconds)
        {
            this.duration = milliseconds;
        }

        public void Start(double time)
        {
            if (this.isActive)
                throw new Exception("Animation already started");

            this.startTime = time;
            this.index = 0;
            this.isActive = true;
        }
        public void End()
        {
            if (!this.isActive)
                throw new Exception("Animation already ended");

            this.isActive = false;

            if (this.type == AnimationType.SPRITE_SINGLE || this.type == AnimationType.SHAPE_SINGLE)
                this.Dispose();
        }

        public void Update(double time)
        {
            if (!this.isActive)
                return;

            // SHAPE_SINGLE
            if (this.type == AnimationType.SHAPE_SINGLE)
            {
                if (time - this.startTime > duration)
                    this.End();
            }
            // SHAPE_MULTI
            if (this.type == AnimationType.SHAPE_MULTI)
            {
                if (time - this.startTime > duration)
                {
                    this.index++;
                    if (index >= this.drawables.Count)
                        this.End();
                }
            }
        }

        public void Draw()
        {
            if (!this.isActive)
                return;

            this.drawables[this.index].Draw(Animation.game);
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
