using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;
using System.Xml;
using System.IO;
using Plasmid.Microbes;
using Plasmid.Graphics;

namespace Plasmid.Cards
{
    public class Card : BaseCard
    {
        private static bool isLoaded = false;
        public static List<Card> All = new List<Card>();

        public string Name { get; }
        public string Text { get; }
        public RenderTarget2D Texture { get; set; }

        public int CostA { get; set; }
        public int CostG { get; set; }
        public int CostT { get; set; }
        public int CostC { get; set; }

        public Dna Archetype
        { 
            get
            {
                Dna max = Dna.A;
                if (CostG > CostA)
                    max = Dna.G;
                if (CostT > CostG && CostT > CostA)
                    max = Dna.T;
                if (CostC > CostT && CostC > CostG && CostC > CostA)
                    max = Dna.C;
                return max;
            }
        }
        public List<CardEffect> CardEffects { get; set; }

        public bool InTransit { get; set; } = false;

        private Card(string name, string text, RenderTarget2D texture, int a, int g, int t, int c, List<CardEffect> cardEffects) : this(Vector2.Zero, name, text, texture, a, g, t, c, cardEffects) { }
        private Card(Vector2 position, string name, string text, RenderTarget2D texture, int a, int g, int t, int c, List<CardEffect> cardEffects) : this(position, name, text, a, g, t, c, cardEffects)
        {
            this.Texture = texture;
        }
        private Card(string name, string text, int a, int g, int t, int c, List<CardEffect> cardEffects) : this(Vector2.Zero, name, text, a, g, t, c, cardEffects) { }
        private Card(Vector2 position, string name, string text, int a, int g, int t, int c, List<CardEffect> cardEffects) : base(position)
        {
            this.Name = name;
            this.Text = text;
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
            Card.CheckLoaded();

            Card.All.Add(new Card(position, name, text, a, g, t, c, effects));
            return Card.All[^1];
        }

        public static Card Copy(string searchName)
        {
            Card card = Card.All.Find(c => c.Name == searchName);

            return new Card(card.Position, card.Name, card.Text, card.Texture, card.CostA, card.CostG, card.CostT, card.CostC, card.CardEffects);
        }
        public static void Load(GraphicsDevice graphics)
        {
            BaseCard.CheckInitialized();
            BaseCard.CheckLoaded();

            //string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            string path = Game.Content.RootDirectory;
            string filename = Path.Combine(path, @"cards.xml");

            using var reader = XmlReader.Create(TitleContainer.OpenStream(filename));

            string name;
            string text;
            int a;
            int g;
            int t;
            int c;
            List<CardEffect> effects = new List<CardEffect>();

            reader.ReadToFollowing("Card");
            do
            {
                reader.ReadToFollowing("Name");
                name = reader.ReadElementContentAsString();

                reader.ReadToFollowing("Text");
                text = reader.ReadElementContentAsString();

                //reader.ReadToFollowing("Cost");

                reader.ReadToFollowing("A");
                a = reader.ReadElementContentAsInt();


                reader.ReadToFollowing("G");
                g = reader.ReadElementContentAsInt();

                reader.ReadToFollowing("T");
                t = reader.ReadElementContentAsInt();

                reader.ReadToFollowing("C");
                c = reader.ReadElementContentAsInt();

                reader.ReadToFollowing("Effect");
                using var effectReader = reader.ReadSubtree();
                while (effectReader.Read())
                {
                    if (reader.Name == "Damage")
                        effects.Add(new CardEffect(CardEffectType.Damage, reader.ReadElementContentAsInt()));
                    else if (effectReader.Name == "Block")
                        effects.Add(new CardEffect(CardEffectType.Block, reader.ReadElementContentAsInt()));
                }

                All.Add(new Card(name, text, a, g, t, c, effects));
                Debug.WriteLine("\n\n - CARD - \n" + All[^1].ToString());

            } while (reader.ReadToFollowing("Card"));

            // Load card data from file
            // JANKY PLACEHOLDER CARDSET:
            //All.Add(new Card("Enzyme", "Attack 1", 0, 0, 0, 0, new List<CardEffect>{new CardEffect(CardEffectType.Damage, 1)}));
            //All.Add(new Card("Cytotoxin", "Attack 2", 0, 0, 0, 0, new List<CardEffect> { new CardEffect(CardEffectType.Damage, 2) }));
            //All.Add(new Card("Phagocytosis", "Attack 3", 0, 0, 0, 0, new List<CardEffect> { new CardEffect(CardEffectType.Damage, 3) }));

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

            //Texture2D artTexture = BaseCard.Game.Content.Load<Texture2D>(this.Art);

            BaseCard.Game.Sprites.Begin();

            BaseCard.Game.Sprites.Draw(BaseCard.CardBlankTexture, Vector2.Zero, Vector2.Zero, this.Archetype.GetColor().ModifyL(1.3f));
            BaseCard.Game.Sprites.Draw(BaseCard.CardFrameTexture, Vector2.Zero, Vector2.Zero, BaseCard.CardFrameColor);

            // Name
            float nameLength = TitleFont.MeasureString(Name).X;
            BaseCard.Game.Sprites.DrawString(Game.CardTitleFont, this.Name, new Vector2(32-(nameLength/2), 47), Color.Navy, 1f);
            // Description
            //BaseCard.Game.Sprites.DrawString(BaseCard.Font, this.Text, new Vector2(8, 32), Color.Black);

            BaseCard.Game.Sprites.End();
            graphics.SetRenderTarget(null);

        }

        public override void Draw()
        {
            this.Draw(this.Position);
        }

        public void Draw(Vector2 position)
        {
            BaseCard.Game.Sprites.Draw(this.Texture, Vector2.Zero, position, Color.White);
        }
        
        public string ToString()
        {
            string str = $"Name: {this.Name}\n" +
                        $"Text: {this.Text}\n" +
                        $"Cost: \n" +
                        $"  A: {this.CostA}\n" +
                        $"  G: {this.CostG}\n" +
                        $"  T: {this.CostT}\n" +
                        $"  C: {this.CostC}\n" +
                        $"Effects: \n";
            foreach (CardEffect effect in this.CardEffects)
                str += $"  {effect.Type.ToString()} : {effect.Value}\n";

            return str;

        }
    }


}
