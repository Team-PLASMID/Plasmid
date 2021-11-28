using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Plasmid.Cards
{
    public abstract class CardContainer : BaseCard
    {
        public List<Card> Cards { get; protected set; }
        public int Limit { get; protected set; }

        public CardContainer(Vector2 position, CardState state, int limit=0) : base(position, state)
        {
            this.Cards = new List<Card>();
        }

        public virtual bool Add(Card card)
        {
            if (Cards.Count + 1 <= Limit)
            {
                Cards.Add(card);
                return true;
            }
            return false;
        }

        public virtual int Count()
        {
            return this.Cards.Count;
        }

        public virtual void Insert(Card card, int index = -1)
        {

            if (index < 0 || index > Cards.Count)
            {
                this.Cards.Add(card);
                return;
            }

            if (Cards.Count + 1 <= Limit)
                this.Cards.Insert(index, card);
        }

        public virtual Card Peek()
        {
            return this.Peek(this.Cards.Count - 1);
        }

        public virtual Card Peek(int index)
        {
            return this.Cards[index];
        }

        public virtual Card Remove(int index)
        {
            Card card = this.Peek(index);
            this.Cards.RemoveAt(index);
            return card;
        }

        public virtual Card Take()
        {
            return this.Remove(this.Cards.Count - 1);
        }

        public virtual void Shuffle()
        {
            Random rand = new Random();
            for (int i = 0; i < this.Cards.Count; i++)
            {
                int r = rand.Next(this.Cards.Count);
                Card tmp = this.Cards[i];
                this.Cards[i] = this.Cards[r];
                this.Cards[r] = tmp;
            }
        }

    }
}
