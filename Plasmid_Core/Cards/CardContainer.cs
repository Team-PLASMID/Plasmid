using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Plasmid.Cards
{
    public abstract class CardContainer : BaseCard
    {
        public List<Card> Cards { get => this.cards; }
        protected List<Card> cards;
        public int Limit { get => this.cards.Capacity; }

        public CardContainer(Vector2 position, CardState state, int limit) : base(position, state)
        {
            this.cards = new List<Card>(limit);
        }

        public int Count()
        {
            return this.cards.Count;
        }

        public void Add(Card card)
        {
            this.cards.Add(card);
        }

        public void Insert(Card card, int index = -1)
        {
            if (index < 0)
                this.cards.Add(card);
            else
                this.cards.Insert(index, card);
        }

        public Card Peek()
        {
            return this.Get(this.cards.Count - 1);
        }

        public Card Get(int index)
        {
            return this.cards[index];
        }

        public Card Remove(int index)
        {
            Card card = this.Get(index);
            this.cards.RemoveAt(index);
            return card;
        }

        public Card Take()
        {
            return this.Remove(this.cards.Count - 1);
        }

        public void Shuffle()
        {
            Random rand = new Random();
            for (int i = 0; i < this.cards.Count; i++)
            {
                int r = rand.Next(this.cards.Count);
                Card tmp = this.cards[i];
                this.cards[i] = this.cards[r];
                this.cards[r] = tmp;
            }
        }

    }
}
