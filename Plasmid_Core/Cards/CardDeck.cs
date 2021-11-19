using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Plasmid.Input;

namespace Plasmid.Cards
{
    public class CardDeck : CardContainer, ITouchable
    {
        public static readonly int DefaultLimit = 30;
        public bool IsDrawAllowed { get => this.isDrawAllowed; }
        private bool isDrawAllowed;
        public bool IsSearchAllowed { get => this.isSearchAllowed; }
        private bool isSearchAllowed;

        public CardDeck(Vector2 position, CardState state=CardState.FaceDown, bool isDrawAllowed=true, bool isSearchAllowed=false) : base(position, state, CardDeck.DefaultLimit)
        {
            this.isDrawAllowed = isDrawAllowed;
            this.isSearchAllowed = isSearchAllowed;
        }

        public void TouchPress(Vector2 position)
        {
            if (this.IsDrawAllowed && this.cards.Count > 0)
                BaseCard.Game.Float.GetFromDeck(position);
        }

        public void TouchMove(Vector2 position)
        {
            throw new NotImplementedException();
        }

        public void TouchRelease(Vector2 position)
        {
            throw new NotImplementedException();
        }

        public override void Draw()
        {
            if (!this.isVisible || this.cards.Count < 1)
                return;

            if (this.state == CardState.FaceDown)
                BaseCard.Game.Sprites.Draw(BaseCard.CardBackTexture, Vector2.Zero, this.position, BaseCard.CardBackColor);
            else if (this.state == CardState.FaceUp)
                this.Peek().Draw();
        }
    }
}
