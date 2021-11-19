using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Plasmid.Cards
{
    public class CardHand : CardContainer //, IDrawable
    {
        public static readonly int DefaultLimit = 7;
        public static readonly int Width = 230;
        public static readonly int Height = 100;

        new public Rectangle Area { get => new Rectangle(this.X, this.Y, CardHand.Width, CardHand.Height); }

        public CardHand(Vector2 position) : base(position, CardState.FaceUp, CardHand.DefaultLimit) { }

        public void Align()
        {
            // empty hand, do nothing
            if (this.cards.Count < 1)
                return;
            // calculate overlap
            int overlap = 0;
            if (this.cards.Count > 1)
                overlap = (this.cards.Count * BaseCard.Width - this.Area.Width) / (this.cards.Count - 1);
            if (overlap < 5)
                overlap = 5;
            // calculate width of hand
            int totalWidth = BaseCard.Width + (this.cards.Count - 1) * (BaseCard.Width - overlap);
            // center hand and set start coordinates
            int y = this.Area.Y + (this.Area.Height - BaseCard.Height) / 2;
            int x = this.Area.X + (this.Area.Width - totalWidth) / 2;

            foreach (Card card in this.cards)
            {
                card.SetPosition(new Vector2(x, y));
                x += BaseCard.Width - overlap;
            }
        }

        public override void Draw()
        {
            BaseCard.CheckInitialized();

            if (this.cards is null || this.cards.Count < 1)
                return;

            // TODO: reduce Align() calls
            // maybe only re-align when cards are added/removed
            this.Align();

            foreach (Card card in this.cards)
                card.Draw();
        }
    }
}
