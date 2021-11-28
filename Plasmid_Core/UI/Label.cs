using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Plasmid.UI
{
    public class Label : Widget
    {
        public static SpriteFont DefaultFont { get; set; }
        public string Text { get; set; }
        public SpriteFont Font { get; set; }

        public Label(string text, SpriteFont font, Color color, Alignment align = Alignment.TopLeft) : base(font.MeasureString(text), align, color)
        {
            this.Text = text;
            this.Font = font;
        }

        public override void Draw()
        {
            if (!this.IsVisible)
                return;

            Widget.Game.Sprites.DrawString(this.Font, this.Text, this.Position, this.Color);
        }
    }
}
