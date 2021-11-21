using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Plasmid.Input;

#if DEBUG
using System.Diagnostics;
#endif

namespace Plasmid.Cards
{
    public class FloatCard : BaseCard, ITouchable
    {
        private BaseCard origin;
        private Vector2 touchOffset;

        public bool IsActive { get => this.isActive; }
        private bool isActive;

        public Card Card { get => this.card; }
        private Card card;

        public FloatCard() : base()
        {
            this.isActive = false;
            this.origin = null;
            this.card = null;
        }

        public void GetFromDeck(Vector2 touchPosition)
        {
            if (this.isActive)
                return;

            this.origin = BaseCard.Game.Deck;
            this.card = BaseCard.Game.Deck.Take();
            this.state = CardState.FaceDown;
            this.position = BaseCard.Game.Deck.Position;
            this.touchOffset = this.Position - touchPosition;


            this.isActive = true;

#if DEBUG   ///////////////////////////////////////////////////////////////////
            // DEBUG
            Debug.WriteLine("PRESS:");
            Debug.WriteLine("Card Position:" + this.position);
            Debug.WriteLine("Touch Position:" + touchPosition);
            Debug.WriteLine("Touch Offset:" + this.touchOffset + "\n");
#endif      ///////////////////////////////////////////////////////////////////
        }

        public void GetFromHand(Card card, Vector2 touchPosition)
        {
            if (this.isActive)
                return;

            this.origin = BaseCard.Game.Hand;

            // remove card from hand? 

            this.state = CardState.FaceUp;
            this.position = BaseCard.Game.Deck.Position;
            this.touchOffset = this.Position - touchPosition;


            this.isActive = true;

#if DEBUG   ///////////////////////////////////////////////////////////////////
            // DEBUG
            Debug.WriteLine("PRESS:");
            Debug.WriteLine("Card Position:" + this.position);
            Debug.WriteLine("Touch Position:" + touchPosition);
            Debug.WriteLine("Touch Offset:" + this.touchOffset + "\n");
#endif      ///////////////////////////////////////////////////////////////////
        }

        public void TouchMove(Vector2 touchPosition)
        {
            if (!this.isActive)
                return;

#if DEBUG   ///////////////////////////////////////////////////////////////////
            // DEBUG
            Vector2 tmp = this.position;
#endif      ///////////////////////////////////////////////////////////////////

            this.position = touchPosition + this.touchOffset;

#if DEBUG   ///////////////////////////////////////////////////////////////////
            // DEBUG
            if (tmp.X != this.position.X || tmp.Y != this.position.Y)
            {
                Debug.WriteLine("MOVE:");
                Debug.WriteLine("Card Position:" + tmp);
                Debug.WriteLine("Touch Position:" + touchPosition);
                Debug.WriteLine("Touch Offset:" + this.touchOffset);
                Debug.WriteLine("Modified Position:" + this.position + "\n");
            }
#endif      ///////////////////////////////////////////////////////////////////


        }

        public void TouchRelease(Vector2 touchPosition)
        {
            if (!this.isActive)
                return;

            if (BaseCard.Game.Hand.Area.Contains(touchPosition))
            {
                BaseCard.Game.Hand.Add(this.card);
                BaseCard.Game.Hand.Align();
            }
            else
                BaseCard.Game.Deck.Add(this.card);

            this.card = null;
            this.isActive = false;
        }

        public void TouchPress(Vector2 position)
        {
            throw new NotImplementedException("FloatCard.TouchPress(Vector2 position) not implemented.");
        }

        public override void Draw()
        {
            if (!this.isActive)
                return;

            // Draw shadow and card
            if (this.state == CardState.FaceDown)
            {
                BaseCard.Game.Sprites.Draw(BaseCard.CardBlankTexture, Vector2.Zero, this.position, new Color(Color.Black, 50));
                BaseCard.DrawCardBack(new Vector2(this.position.X + 5, this.position.Y - 10));
            }
            else if (this.state == CardState.FaceUp)
            {
                BaseCard.Game.Sprites.Draw(BaseCard.CardBlankTexture, Vector2.Zero, this.position, new Color(Color.Black, 50));
                this.card.Draw(new Vector2(this.position.X + 5, this.position.Y - 10));
            }
        }
    }
}
