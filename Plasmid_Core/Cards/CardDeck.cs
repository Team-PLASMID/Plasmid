using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Plasmid.Input;

namespace Plasmid.Cards
{
    public class CardDeck : CardContainer
    {
        // TODO:
        // fix card positions as they are added to the deck

        public bool IsDrawAllowed { get => this.isDrawAllowed; }
        private bool isDrawAllowed;
        public bool IsSearchAllowed { get => this.isSearchAllowed; }
        private bool isSearchAllowed;

        public CardDeck(Vector2 position, CardState state=CardState.FaceDown, bool isDrawAllowed=true, bool isSearchAllowed=false, int limit=30) : base(position, state)
        {
            this.isDrawAllowed = isDrawAllowed;
            this.isSearchAllowed = isSearchAllowed;
            this.Limit = limit;

            BaseCard.Game.Touch.Pressed += this.TouchPressed;
        }

        public void TouchPressed(object sender, Vector2 position, float gametime)
        {
            if (this.IsDrawAllowed && this.Cards.Count > 0 && this.Area.Contains(position))
            {
                Game.Hand.DrawFromDeck(1);
            }      
        }

        public override void Draw()
        {
            if (!this.IsVisible || this.Cards.Count < 1)
                return;

            if (this.State == CardState.FaceDown)
                BaseCard.Game.Sprites.Draw(BaseCard.CardBackTexture, Vector2.Zero, this.Position, BaseCard.CardBackColor);
            else if (this.State == CardState.FaceUp)
                this.Peek().Draw();
        }

        public override bool Add(Card card)
        {
            if (Cards.Count + 1 <= Limit)
            {
                card.Position = this.Position;
                Cards.Add(card);
                return true;
            }
            return false;
        }
    }
}
