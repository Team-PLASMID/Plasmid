using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Plasmid.UI
{
    public class SpriteWidget : Widget
    {
        public Texture2D Texture { get; set; }

        public SpriteWidget(Texture2D texture, Vector2 position) : this(texture, position, Color.White) { }
        public SpriteWidget(Texture2D texture, Vector2 position, Color color)
            : base(new Vector2(texture.Width, texture.Height), Alignment.None, color, position) { }

        public override void Draw()
        {
            Widget.Game.Sprites.Draw(Texture, Vector2.Zero, Position, Color);
        }

    }
}
