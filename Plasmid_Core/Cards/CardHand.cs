using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Plasmid.Cards
{
    class CardHand : List<Card>, IDrawable
    {
        public Rectangle Area { get; set; }
        //public ExtendedGraphicsDeviceManager Graphics { get; set; }
        //public SpriteBatch SB { get; set; }
        public int DrawOrder { get; set; }
        public bool Visible { get; set; }
        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;

        public CardHand(Rectangle area)
        {
            Area = area;
            //Graphics = graphics;
            //SB = sb;
            DrawOrder = 10;
            Visible = true;
        }

        public void Align()
        {
            // empty hand, do nothing
            if (Count < 1)
                return;
            // calculate overlap
            int overlap = 0;
            if (Count > 1)
                overlap = (Count * Card.Width - Area.Width) / (Count - 1);
            if (overlap < 5)
                overlap = 5;
            // calculate width of hand
            int totalWidth = Card.Width + (Count - 1) * (Card.Width - overlap);
            // center hand and set start coordinates
            int y = Area.Y + (Area.Height - Card.Height) / 2;
            int x = Area.X + (Area.Width - totalWidth) / 2;

            /*
            // set base overlap (cards will slightly overlap even with excess space)
            int overlap = 5;
            // figure out uncompressed width of hand
            int totalWidth = Card.Width + (Count - 1) * (Card.Width - overlap);
            // center hand vertically
            int y = Area.Y + (Area.Height - Card.Height) / 2;
            int x;
            // if hand fits uncompressed, center them horizontally
            if (totalWidth > Area.Width)
                x = Area.X + (Area.Width - totalWidth) / 2;
            // otherwise compress hand (increase overlap) and then center horizontally
            else
                overlap = (Count * Card.Width - Area.Width) / (Count - 1);
                int x = Area.X + PadSides;
            int spacing = (Area.Width - Card.Width - PadSides) / Count;
            if (spacing > Card.Width + MaxSpaceBetween)
                spacing = Card.Width + 5;
            */

            foreach (Card c in this)
            {
                c.Pos = new Vector2(x, y);
                x += Card.Width - overlap;
                // x += spacing;
            }
        }
        public void Draw(GameTime gameTime)
        {

            //SB.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null, null, Graphics.Transform);

            //SB.Draw(card.Texture, card.Pos, Color.White);

        }
    }
}
