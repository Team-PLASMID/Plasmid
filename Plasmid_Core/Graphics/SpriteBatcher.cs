using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Plasmid.Graphics
{
    public sealed class SpriteBatcher : IDisposable
    {
        private bool isDisposed;
        private Game game;
        private SpriteBatch sprites;
        private BasicEffect effect;

        public SpriteBatcher(Game game)
        {
            if (game is null)
                throw new ArgumentNullException("game");

            this.game = game;
            this.sprites = new SpriteBatch(this.game.GraphicsDevice);
            this.isDisposed = false;

            this.effect = new BasicEffect(this.game.GraphicsDevice);
            this.effect.FogEnabled = false;
            this.effect.TextureEnabled = true;
            this.effect.LightingEnabled = false;
            this.effect.VertexColorEnabled = true;
            this.effect.World = Matrix.Identity;
            this.effect.Projection = Matrix.Identity;
            this.effect.View = Matrix.Identity;
        }

        public void Dispose()
        {
            if (this.isDisposed)
                return;

            this.effect?.Dispose();
            this.sprites?.Dispose();
            this.isDisposed = true;
        }

        public void Begin() { this.Begin(null); }
        public void Begin(Camera camera, bool isTextureFilteringEnabled = false)
        {
            SamplerState sampler = SamplerState.PointClamp;
            if (isTextureFilteringEnabled)
                sampler = SamplerState.LinearClamp;

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

            this.sprites.Begin(
                sortMode: SpriteSortMode.Immediate,
                //sortMode: SpriteSortMode.Deferred,
                blendState: BlendState.AlphaBlend,
                samplerState: sampler,
                rasterizerState: RasterizerState.CullNone,
                effect: this.effect);
        }

        public void End()
        {
            this.sprites.End();
        }

        public void Draw(Texture2D texture, Vector2 origin, Vector2 position, Color color)
        {
            this.sprites.Draw(texture, position, null, color, 0f, origin, 1f, SpriteEffects.FlipVertically, 0f);
        }

        public void Draw(Texture2D texture, Rectangle? sourceRectangle, Vector2 origin, Vector2 position, float rotation, Vector2 scale, Color color)
        {
            this.sprites.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, SpriteEffects.FlipVertically, 0f);
        }

        public void Draw(Texture2D texture, Rectangle? sourceRectangle, Rectangle destinationRectangle, Color color)
        {
            this.sprites.Draw(texture, destinationRectangle, sourceRectangle, color, 0f, Vector2.Zero, SpriteEffects.FlipVertically, 0f);
        }
        //(font, MessageText, position + new Vector2(1 * scale, 1 * scale), backColor, 0, origin, scale, SpriteEffects.None, 1f)
        public void DrawString(SpriteFont font, string text, Vector2 position, Color color)
        {
            this.DrawString(font, text, position, color, 1f);
        }

        public void DrawString(SpriteFont font, string text, Vector2 position, Color color, float scale)
        {
            this.sprites.DrawString(font, text, position, color, 0f, Vector2.Zero, scale, SpriteEffects.FlipVertically, 0f);
        }
    }
}
