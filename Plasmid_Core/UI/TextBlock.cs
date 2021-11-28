using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Plasmid.UI
{
    public class TextBlock : Widget
    {
        public static SpriteFont DefaultFont { get; set; }
        public string Text { get; set; }
        public SpriteFont Font { get; set; }


        private Vector2 printPosition;
        private string printString;
        private bool printStringAligned;

        public TextBlock(Vector2 dimensions, string text, SpriteFont font, Color color, Alignment align = Alignment.TopLeft) : base(dimensions, align, color)
        {
            this.Text = text;
            this.Font = font;
            
            this.WrapText();

            this.printStringAligned = false;
        }

        private void WrapText()
        {
            this.printString = Util.WrapString(this.Font, this.Text, this.Dimensions.X);
        }

        public override bool Align(Rectangle canvas, bool respectPadding=true)
        {
            bool result = base.Align(canvas);
            if (!result)
                throw new Exception("Failed to align TextBlock.");
            this.AlignPrintString();
            return true;

        }

        public override void UnAlign()
        {
            base.UnAlign();
            this.printStringAligned = false;
        }

        private void AlignPrintString()
        {
            if (!this.IsAligned)
                throw new Exception("Align TextBlock before aligning its printString.");

            if (!Util.Align(this.Font.MeasureString(this.printString), this.Area, Alignment.TopLeft, out this.printPosition))
                throw new Exception("Failed to align TextBlock printString.");

            this.printStringAligned=true;
        }
        public override void Draw()
        {
            this.DrawRegular();
        }

        private void DrawRegular()
        {
            if (!this.IsVisible)
                return;

            Widget.Game.Sprites.DrawString(this.Font, this.printString, this.printPosition, this.Color);
        }

        public void DrawDebug(Color? lineColor=null)
        {
            Color color = lineColor ?? Color.Red;
            this.DrawRegular();

            Game.Shapes.Begin();
            Game.Shapes.DrawRectangle(this.Area, 2f, color);
            Game.Shapes.End();
        }

     
    }
}
