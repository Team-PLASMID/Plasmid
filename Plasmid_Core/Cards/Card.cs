using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;

namespace Plasmid.Cards
{
    public class Card : BaseCard
    {
        private static bool isLoaded = false;
        public static List<Card> All = new List<Card>();

        public string Name { get; }
        public string Text { get; }
        public string Art { get; }
        public RenderTarget2D Texture { get; set; }

        public int CostA { get; set; }
        public int CostG { get; set; }
        public int CostT { get; set; }
        public int CostC { get; set; }
        public List<CardEffect> CardEffects { get; set; }

        private Card(string name, string text, string art, RenderTarget2D texture, int a, int g, int t, int c, List<CardEffect> cardEffects) : this(Vector2.Zero, name, text, art, texture, a, g, t, c, cardEffects) { }
        private Card(Vector2 position, string name, string text, string art, RenderTarget2D texture, int a, int g, int t, int c, List<CardEffect> cardEffects) : this(position, name, text, art, a, g, t, c, cardEffects)
        {
            this.Texture = texture;
        }
        private Card(string name, string text, string art, int a, int g, int t, int c, List<CardEffect> cardEffects) : this(Vector2.Zero, name, text, art, a, g, t, c, cardEffects) { }
        private Card(Vector2 position, string name, string text, string art, int a, int g, int t, int c, List<CardEffect> cardEffects) : base(position)
        {
            this.Name = name;
            this.Text = text;
            this.Art = art;
            this.CostA = a;
            this.CostG = g;
            this.CostT = t;
            this.CostC = c;
            this.CardEffects = cardEffects;

        }

        public static void CheckLoaded()
        {
            BaseCard.CheckInitialized();
            BaseCard.CheckLoaded();

            if (!Card.isLoaded)
                throw new Exception("Run Card.Load(Game1 game) before using Cards module.");
        }

        public static Card New(Vector2 position, string name, string text, string art, int a, int g, int t, int c, List<CardEffect> effects)
        {
            BaseCard.CheckInitialized();

            All.Add(new Card(position, name, text, art, a, g, t, c, effects));
            return All[All.Count - 1];
        }

        public static Card Copy(string searchName)
        {
            Card card = All.Find(c => c.Name == searchName);

            return new Card(card.Position, card.Name, card.Text, card.Art, card.Texture, card.CostA, card.CostG, card.CostT, card.CostC, card.CardEffects);
        }

        public static void Load(GraphicsDevice graphics)
        {
            BaseCard.CheckInitialized();
            BaseCard.CheckLoaded();

            // Load card data from file
            // JANKY PLACEHOLDER CARDSET:
            All.Add(new Card("Enzyme", "Attack 1", "art02", 0, 0, 0, 0, new List<CardEffect>{new CardEffect(CardEffectType.Damage, 1)}));
            All.Add(new Card("Cytotoxin", "Attack 2", "art08", 0, 0, 0, 0, new List<CardEffect> { new CardEffect(CardEffectType.Damage, 2) }));
            All.Add(new Card("Phagocytosis", "Attack 3", "art01", 0, 0, 0, 0, new List<CardEffect> { new CardEffect(CardEffectType.Damage, 3) }));

            // Build textures
            foreach (Card card in All)
                card.BuildTexture(graphics);

            Card.isLoaded = true;
        }

        public void BuildTexture(GraphicsDevice graphics)
        {
            this.Texture = new RenderTarget2D(graphics, BaseCard.CardBlankTexture.Width, BaseCard.CardBlankTexture.Height);
            graphics.SetRenderTarget(this.Texture);
            graphics.Clear(Color.Transparent);

            Texture2D artTexture = BaseCard.Game.Content.Load<Texture2D>(this.Art);

            BaseCard.Game.Sprites.Begin();

            BaseCard.Game.Sprites.Draw(BaseCard.CardBlankTexture, Vector2.Zero, Vector2.Zero, BaseCard.CardColor);
            BaseCard.Game.Sprites.Draw(artTexture, new Rectangle(3,9,58,58), Vector2.Zero, Vector2.Zero, 0f, Vector2.One, Color.White);
            BaseCard.Game.Sprites.Draw(BaseCard.CardFrameTexture, Vector2.Zero, Vector2.Zero, BaseCard.CardFrameColor);

            // Name
            BaseCard.Game.Sprites.DrawString(BaseCard.Font, this.Name, new Vector2(7, 3), Color.WhiteSmoke);
            // Art
            // Frame
            // Description
            BaseCard.Game.Sprites.DrawString(BaseCard.Font, this.Text, new Vector2(8, 70), Color.Black);

            // Energy signature
            // etc.

            BaseCard.Game.Sprites.End();
            graphics.SetRenderTarget(null);

        }

        public void SetPosition(Vector2 position)
        {
            this.position = position;
        }

        public override void Draw()
        {
            this.Draw(this.position);
        }

        public void Draw(Vector2 position)
        {
            BaseCard.Game.Sprites.Draw(this.Texture, Vector2.Zero, position, Color.White);
        }
        
    }


}
