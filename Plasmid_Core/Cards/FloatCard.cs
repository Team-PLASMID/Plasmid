using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Plasmid.Input;

namespace Plasmid.Cards
{
    public class FloatCard : BaseCard
    { 
        public Card HeldCard { get; protected set; }
        public CardContainer Origin { get; protected set; }
        public Vector2 TouchOffset { get; protected set; }
        public bool IsActive { get; protected set; }

        public Vector2 Position
        {
            get => this.HeldCard.Position;
            set => this.HeldCard.Position = value;
        }


        public FloatCard() : base()
        {
            this.IsActive = false;
            this.Origin = null;
            this.HeldCard = null;

            Game.Touch.Moved += TouchMoved;
            Game.Touch.Released += TouchReleased;
        }

        public void GetFromDeck(Vector2 touchPosition)
        {
            if (this.IsActive)
                return;

            this.Origin = BaseCard.Game.Deck;
            this.HeldCard = BaseCard.Game.Deck.Take();
            this.State = CardState.FaceDown;
            this.Position = BaseCard.Game.Deck.Position;
            this.TouchOffset = this.Position - touchPosition;


            this.IsActive = true;

        }

        public void GetFromHand(Card card, Vector2 touchPosition)
        {
            if (this.IsActive)
                return;

            this.Origin = BaseCard.Game.Hand;
            card.State = CardState.FaceUp;
            this.HeldCard = card;
            this.TouchOffset = this.Position - touchPosition;

            this.IsActive = true;
        }

        public void TouchMoved(object sender, Vector2 touchPosition, float gameTime)
        {
            if (!this.IsActive)
                return;

            this.Position = touchPosition + this.TouchOffset;

            Game.Hand.Hover(this.HeldCard.Center);

        }

        public void TouchReleased(object sender, Vector2 touchPosition, float gameTime)
        {
            if (!this.IsActive)
                return;

            Vector2 center = this.HeldCard.Center;

            if (Game.State == GameState.Battle && Game.Battle.Phase == BattlePhase.PlayerTurn_Main)
            {
                if (BaseCard.Game.Hand.Area.Contains(center))
                {
                    BaseCard.Game.Hand.Add(this.HeldCard);
                    BaseCard.Game.Hand.Align();
                }
                if (Game.PlayArea.Contains(center))
                {

                }
                else
                    BaseCard.Game.Deck.Add(this.HeldCard);

            }
            this.HeldCard = null;
            this.IsActive = false;
        }


        public override void Draw()
        {
            if (!this.IsActive)
                return;


            BaseCard.Game.Sprites.Draw(BaseCard.CardBlankTexture, Vector2.Zero, this.Position, new Color(Color.Black, 50));
            this.HeldCard.Draw(new Vector2(this.Position.X + 5, this.Position.Y - 10));

            // Draw shadow and card
            //if (this.State == CardState.FaceDown)
            //{
            //    BaseCard.Game.Sprites.Draw(BaseCard.CardBlankTexture, Vector2.Zero, this.Position, new Color(Color.Black, 50));
            //    BaseCard.DrawCardBack(new Vector2(this.Position.X + 5, this.Position.Y - 10));
            //}
            //else if (this.State == CardState.FaceUp)
            //{
            //    BaseCard.Game.Sprites.Draw(BaseCard.CardBlankTexture, Vector2.Zero, this.Position, new Color(Color.Black, 50));
            //    this.HeldCard.Draw(new Vector2(this.Position.X + 5, this.Position.Y - 10));
            //}
        }
    }
}
