using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Plasmid.Graphics
{
    public sealed class Screen : IDisposable
    {
        private readonly static int MinDim = 64;
        private readonly static int MaxDim = 4096;

        private Game game;
        private RenderTarget2D target;
        private bool isSet;
        private bool isDisposed;

        public int Width { get; }
        public int Height { get; }

        public Rectangle Area { get => new Rectangle(0, 0, Width, Height); }

        public Screen(Game game, int width, int height)
        {
            this.game = game ?? throw new ArgumentNullException("game");

            Width = GraphUtils.Clamp(width, Screen.MinDim, Screen.MaxDim);
            Height = GraphUtils.Clamp(height, Screen.MinDim, Screen.MaxDim);

            this.target = new RenderTarget2D(this.game.GraphicsDevice, Width, Height);

            isSet = false;
        }

        public void Dispose()
        {
            if (this.isDisposed)
                return;

            this.target?.Dispose();
            this.isDisposed = true;
        }

        public void Set()
        {
            if (this.isSet)
                throw new Exception("Render target already Set. UnSet before attempting to Set.");

            this.game.GraphicsDevice.SetRenderTarget(this.target);
            this.isSet = true;
        }

        public void UnSet()
        {
            if (!this.isSet)
                throw new Exception("Render target wasn't Set before attempting to UnSet.");
            this.game.GraphicsDevice.SetRenderTarget(null);
            this.isSet = false;
        }

        public void Present(SpriteBatcher sprites, bool textureFiltering = true)
        {
            if (sprites is null)
                throw new ArgumentNullException("sprites");

            this.game.GraphicsDevice.Clear(Color.Black);

            Rectangle destinationRectangle = this.CalcDestRectangle();

            sprites.Begin(null, textureFiltering);
            sprites.Draw(this.target, null, destinationRectangle, Color.White);
            sprites.End();
        }

        internal Rectangle CalcDestRectangle()
        {
            Rectangle backbufferBounds = this.game.GraphicsDevice.PresentationParameters.Bounds;
            float backbufferAspectRatio = (float)backbufferBounds.Width / backbufferBounds.Height;
            float screenAspectRatio = (float)this.Width / this.Height;

            float rx = 0f;
            float ry = 0f;
            float rw = backbufferBounds.Width;
            float rh = backbufferBounds.Height;

            if (backbufferAspectRatio > screenAspectRatio)
            {
                rw = rh * screenAspectRatio;
                rx = ((float)backbufferBounds.Width - rw) / 2f;
            }
            else if (backbufferAspectRatio < screenAspectRatio)
            {
                rh = rw / screenAspectRatio;
                ry = ((float)backbufferBounds.Height - rh) / 2f;
            }

            Rectangle result = new Rectangle((int)rx, (int)ry, (int)rw, (int)rh);
            return result;
        }

    }
}
