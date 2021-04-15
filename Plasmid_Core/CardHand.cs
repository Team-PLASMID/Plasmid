using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Plasmid_Core
{
    class CardHand : List<Card>, IDrawable
    {
        public Rectangle Area { get; set; }
        public ExtendedGraphicsDeviceManager Graphics { get; set; }
        public SpriteBatch SB { get; set;
        }
        public int DrawOrder { get; set; }
        public bool Visible { get; set; }
        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;

        public CardHand(Rectangle area, ExtendedGraphicsDeviceManager graphics, SpriteBatch sb)
        {
            Area = area;
            Graphics = graphics;
            SB = sb;
            DrawOrder = 10;
            Visible = true;
        }

        public void Align()
        {
            if (Count < 1)
                return;

            int y = Area.Y + (Area.Height - Card.Height) / 2;
            int PadSides = 5;
            int x = Area.X + PadSides;
            int spacing = (Area.Width - Card.Width - PadSides) / Count;

            foreach (Card c in this)
            {
                c.Pos = new Vector2(x, y);
                x += spacing;
            }
        }
        public void Draw(GameTime gameTime)
        {

            //SB.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null, null, Graphics.Transform);

            //SB.Draw(card.Texture, card.Pos, Color.White);

        }
    }
}
