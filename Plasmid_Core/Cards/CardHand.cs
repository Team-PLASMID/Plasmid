using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Plasmid.Cards
{
    public class CardHand : CardContainer
    {
        public override float Width { get; protected set; }
        public override float Height { get; protected set; }

        public bool HasHover { get; protected set; }
        public int HoverIndex { get; protected set; }

        private int LastHoverIndex;
        private Vector2 LastHoverPosition;

        public CardHand(int x, int y, int width, int height, int limit=7) : base(new Vector2(x, y), CardState.FaceUp)
        {
            this.Width = width;
            this.Height = height;
            this.Limit = limit;
            this.LastHoverIndex = -1;

            Game.Touch.Pressed += TouchPressed;
        }

        private void TouchPressed(object sender, Vector2 position, float gametime)
        {
            for (int i = this.Cards.Count - 1; i >= 0; i--)
            {
                if (this.Cards[i].Area.Contains(position))
                {
                    Game.Float.GetFromHand(this.Remove(i), position);
                    break;
                }
            }
        }

        public bool DrawFromDeck(int n)
        {
            // if hand limit can't take n cards, try to reduce n.
            if (this.Cards.Count + n > this.Limit)
            {
                int originalN = n;
                n = -1;
                for (int i = 0; i < originalN; i++)
                {
                    if (this.Cards.Count + i <= this.Limit)
                        n = i;
                    else
                        break;
                }
            }
            if (n <= 0)
                return false;

            for (int i = 0; i < n; i++)
                this.Cards.Add(Game.Deck.Take());

            // Calculuate alignment
            this.Align();

            return true;


        }
        public void Align()
        {
            Vector2[] destinations = this.CalcAlignment();
            if (destinations.Length != this.Cards.Count)
                throw new Exception("This really shouldn't happen.");

            for (int i = 0; i < this.Cards.Count; i++)
                MoveCard.New(this.Cards[i], destinations[i]);
                //this.Cards[i].SetPosition(destinations[i]);
        }

        private Vector2[] CalcAlignment(int n=0)
        {
            if (n == 0)
                n = this.Cards.Count;

            Vector2[] destinations = new Vector2[n];

            // empty hand, do nothing
            if (destinations.Length < 1)
                return destinations;

            // calculate overlap
            int overlap = 0;
            if (destinations.Length > 1)
                overlap = (destinations.Length * BaseCard.CardWidth - this.Area.Width) / (destinations.Length - 1);
            if (overlap < 5)
                overlap = 5;
            // calculate width of hand
            int totalWidth = BaseCard.CardWidth + (destinations.Length - 1) * (BaseCard.CardWidth - overlap);
            // center hand and set start coordinates
            int y = this.Area.Y + (this.Area.Height - BaseCard.CardHeight) / 2;
            int x = this.Area.X + (this.Area.Width - totalWidth) / 2;

            for (int i = 0; i < destinations.Length; i++)
            {
                destinations[i] = new Vector2(x, y);
                x += BaseCard.CardWidth - overlap;
            }

            return destinations;

        }

        public void Hover(Vector2 position)
        {
            if (Cards.Count < 1)
                return;

            if (this.Area.Contains(position)
                && position.Y >= Cards[0].Y
                && position.Y <= Cards[0].Y + Cards[0].Height)
            {
                if (!this.HasHover)
                    this.HasHover = true;
                else if (Vector2.Distance(position, this.LastHoverPosition) < 3f)
                {
                    this.LastHoverPosition = position;
                    return;
                }
                this.LastHoverPosition = position;
            }
            else
            {
                if (this.HasHover)
                {
                    this.HasHover = false;
                    this.LastHoverIndex = -1;
                    this.Align();
                }
                return;
            }

            Vector2[] destinations = CalcAlignment(Cards.Count + 1);
            this.HoverIndex = 0;
            for (int i = 0 ; i < Cards.Count; i++)
            {
                //if (Cards[i].InTransit)
                    //continue;
                //if (position.X < Cards[Cards.Count-i-1].Center.X)
                if (position.X < destinations[destinations.Length - 1 - i].X)
                    continue;
                if (position.X >= destinations[destinations.Length - 1 - i].X)
                {
                    HoverIndex = Cards.Count - i;
                    break;
                }
            }

            if (HoverIndex == LastHoverIndex)
            {
                LastHoverIndex = HoverIndex;
                return;
            }
            LastHoverIndex = HoverIndex;
            
            for (int i = 0; i < Cards.Count; i++)
            {
                if (i >= HoverIndex)
                    MoveCard.New(Cards[i], destinations[i + 1]);
                else
                    MoveCard.New(Cards[i], destinations[i]);

            }

        }
        public override void Draw()
        {
            BaseCard.CheckInitialized();

            if (this.Cards is null || this.Cards.Count < 1)
                return;

            foreach (Card card in this.Cards)
                card.Draw();
        }
        public override Card Take()
        {
            Card card = base.Take();
            this.Align();
            return card;
        }
        public override Card Remove(int index)
        {
            Card card = base.Remove(index);
            this.Align();
            return card;
        }
        public override void Shuffle()
        {
            base.Shuffle();
            this.Align();
        }

        public override bool Add(Card card)
        {
            card.State = CardState.FaceUp;
            if (this.HasHover)
            {
                this.Cards.Insert(this.HoverIndex, card);
                return true;
            }
            else
                return base.Add(card);
        }

    }
}
