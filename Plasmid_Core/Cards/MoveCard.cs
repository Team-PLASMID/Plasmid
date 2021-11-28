using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Plasmid.Input;


namespace Plasmid.Cards
{
    public class MoveCard : BaseCard //, IDisposable
    {
        //private bool IsDisposed;
        public static List<MoveCard> Cards = new List<MoveCard>();

        public Card HeldCard { get; protected set; }
        public BaseCard Origin { get; protected set; }
        public Vector2 Destination { get; protected set; }
        public Vector2 LastDestination { get; protected set; }
        public Vector2 TotalDistance { get; protected set; }
        public Vector2 Velocity { get; protected set; }
        public Vector2 MinAccel { get; protected set; }
        public Vector2 MaxAccel { get; protected set; }
        public Vector2 Acceleration { get; protected set; }
        public float Time { get; protected set; }
        public bool IsActive { get; protected set; }
        public bool IsComplete { get; protected set; }

        public Vector2 Position
        {
            get => this.HeldCard.Position;
            set => this.HeldCard.Position = value;
        }

        public MoveCard() : base()
        {
            this.IsActive = false;
            this.Origin = null;
            this.HeldCard = null;
        }

         
        private MoveCard(Card card, Vector2 destination)
        {
            this.State = CardState.FaceUp;
            this.HeldCard = card;
            this.Position = this.HeldCard.Position;
            this.Destination = destination;
            this.LastDestination = Vector2.Zero;
            if (destination == Vector2.Zero)
                this.LastDestination = new Vector2(-1, -1);
            this.TotalDistance = this.Destination - this.Position;
            this.Acceleration = Vector2.Zero;
            this.Velocity = Vector2.Zero;
            this.Time = -1;

            if (this.HeldCard.InTransit)
                this.IsActive = false;
            else
                this.IsActive = true;

            this.IsComplete = false;
            this.HeldCard.InTransit = true;
        }
       
        public static void New(Card card, Vector2 destination)
        {
            foreach(MoveCard move in MoveCard.Cards)
            {
                if (card == move.HeldCard)
                {
                    move.Destination = destination;
                    return;
                }
            }
            MoveCard.Cards.Add(new MoveCard(card, destination));
        }

        public void Update(float gameTime)
        {
            if (!this.IsActive)
            {
                if (!this.IsComplete && !this.HeldCard.InTransit)
                    this.IsActive = true;
                else
                    return;
            }

            if (Position == Destination)
            {
                //this.HeldCard.Position = this.Position;
                this.IsActive = false;
                this.IsComplete = true;
                this.HeldCard.InTransit = false;
                this.HeldCard = null;
                //this.Dispose();
                return;
            }

            // set initial values on first update
            // reset if destination changes midway
            if (LastDestination != Destination)
            {
                Velocity = Destination - Position;
                Velocity = Vector2.Normalize(Velocity);
                MinAccel = Velocity;
                MaxAccel = 2f * Velocity;
                LastDestination = Destination;
                return;
            }

            LastDestination = Destination;

            Vector2 distance = this.Destination - this.Position;
            float progress = (TotalDistance.Length() - distance.Length()) / TotalDistance.Length();
            if (distance.Length() <= MinAccel.Length())
            {
                Position = Destination;
                return;
            }
            if (progress > .8f)
                Velocity = Vector2.Lerp(MinAccel, MaxAccel, distance.Length() / TotalDistance.Length());
            else
                Velocity = Vector2.Lerp(MinAccel, MaxAccel, distance.Length() / (.2f * TotalDistance.Length()));

            Position += Velocity;


            //float progress = (TotalDistance.Length() - distance.Length()) / TotalDistance.Length();
            //float inflection = 0.5f;
            //float acc = 1f - (Math.Abs(inflection - progress) / inflection);

            //Acceleration = Vector2.Lerp(MinAccel, MaxAccel, acc);
            //Velocity += Acceleration * dTime;
            //Position += Velocity * dTime;

            //if (Velocity.Length() > TotalDistance.Length() - distance.Length())
            //    Velocity = Destination - Position;




        }

        public override void Draw()
        {
            this.Draw();
        }

        public void Draw(int shadowSetting=0)
        {
            // 0 = No Shadow
            // 1 = Minimal shadow, align to card
            // 2+ = Large shadow, align to shadow

            if (!this.IsActive)
                return;

            Vector2 cardOffset = Vector2.Zero;
            Vector2 shadowOffset = Vector2.Zero;
            if (shadowSetting >= 2)
                cardOffset = new Vector2(5, -10);
            else if (shadowSetting == 1)
                shadowOffset = new Vector2(-2, 5);


            if (shadowSetting > 0)
                BaseCard.Game.Sprites.Draw(BaseCard.CardBlankTexture, Vector2.Zero, this.Position + shadowOffset, new Color(Color.Black, 50));
            this.HeldCard.Draw(this.Position + cardOffset);

            // Draw shadow and card
            //if (this.State == CardState.FaceDown)
            //{
            //    BaseCard.Game.Sprites.Draw(BaseCard.CardBlankTexture, Vector2.Zero, this.Position + shadowOffset, new Color(Color.Black, 50));
            //    BaseCard.DrawCardBack(this.Position + cardOffset);
            //}
            //else if (this.State == CardState.FaceUp)
            //{
            //    BaseCard.Game.Sprites.Draw(BaseCard.CardBlankTexture, Vector2.Zero, this.Position + shadowOffset, new Color(Color.Black, 50));
            //    this.HeldCard.Draw(this.Position + cardOffset);
            //}
        }

        public static void UpdateAll(float gametime)
        {
            List<MoveCard> complete = new List<MoveCard>();
            foreach (var card in MoveCard.Cards)
            {
                if (card.IsComplete)
                    complete.Add(card);
                else
                    card.Update(gametime);
            }
            foreach (var card in complete)
            {
               MoveCard.Cards.Remove(card);
            }
        }

        public static bool Find(Card card, out MoveCard movecard)
        {
            movecard = null;
            foreach (MoveCard move in MoveCard.Cards)
            {
                if (move.HeldCard == card)
                {
                    movecard = move;
                    return true;
                }
            }

            return false;
        }

        public static void DrawAll()
        {
            foreach (MoveCard card in MoveCard.Cards)
                card.Draw();
        }
        //protected virtual void Dispose(bool disposing)
        //{
        //    if (!IsDisposed)
        //    {

        //        IsDisposed = true;
        //    }
        //}
        //public void Dispose()
        //{
        //    // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //    Dispose(disposing: true);
        //    GC.SuppressFinalize(this);
        //}
    }
}
