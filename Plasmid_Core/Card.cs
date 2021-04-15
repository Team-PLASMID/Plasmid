using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;

namespace Plasmid_Core
{
    [Flags]
    public enum EffectType { Damage=0, Block=1 }
    public struct Effect {
        public EffectType Type { get; set; }
        public int Value { get; set; }
        public Effect(EffectType type, int value) { Type = type; Value = value; }
    }
    class Card
    {
        public static List<Card> All = new List<Card>();
        public static int Height = 96;
        public static int Width = 64;
        public static Color CardColor = Color.OldLace;
        public static Color CardBackColor = Color.CornflowerBlue;
        public static Color CardFrameColor = Color.DarkGoldenrod;
        public static Texture2D CardTexture;
        public static Texture2D CardBackTexture;
        public static Texture2D CardFrameTexture;
        public static SpriteFont Font;

        public string Name { get; }
        public string Text { get; }
        public string Art { get; }
        public RenderTarget2D Texture { get; set; }
        public int CostA { get; set; }
        public int CostG { get; set; }
        public int CostT { get; set; }
        public int CostC { get; set; }
        public List<Effect> Effects { get; set; }
        private Vector2 _pos;
        public Vector2 Pos
        {
            get { return _pos; }
            set { _pos = value; }
        }
        public int X
        {
            get { return (int)_pos.X; }
            set { _pos.X = value; }
        }
        public int Y
        {
            get { return (int)_pos.Y; }
            set { _pos.Y = value; }
        }

        private Card(Vector2 pos, string name, string text, string art, int a, int g, int t, int c, List<Effect> effects) : this(name, text, art, a, g, t, c, effects)
        {
            Pos = pos;
        }

        private Card(string name, string text, string art, RenderTarget2D texture, int a, int g, int t, int c, List<Effect> effects) : this(name, text, art, a, g, t, c, effects)
        {
            Texture = texture;
        }
        private Card(string name, string text, string art, int a, int g, int t, int c, List<Effect> effects)
        {
            Name = name;
            Text = text;
            Art = art;
            CostA = a;
            CostG = g;
            CostT = t;
            CostC = c;
            Effects = effects;
        }

        public static Card New(Vector2 pos, string name, string text, string art, int a, int g, int t, int c, List<Effect> effects)
        {
            All.Add(new Card(pos, name, text, art, a, g, t, c, effects));
            return All[All.Count - 1];
        }

        public static Card Copy(string searchName)
        {
            Card card = All.Find(c => c.Name == searchName);

            return new Card(card.Name, card.Text, card.Art, card.Texture, card.CostA, card.CostG, card.CostT, card.CostC, card.Effects);
        }
        public static void Load(ContentManager content, GraphicsDevice graphics, SpriteBatch sb)
        {
            // Textures
            CardTexture = content.Load<Texture2D>("card");
            CardBackTexture = content.Load<Texture2D>("card_back");
            CardFrameTexture = content.Load<Texture2D>("card_frame");
            // Fonts
            Font = content.Load<SpriteFont>("CardFont");

            // Load card data from file
            // JANKY PLACEHOLDER CARDSET:
            All.Add(new Card("Enzyme", "Attack 1", "art02", 0, 0, 0, 0, new List<Effect>{new Effect(EffectType.Damage, 1)}));
            All.Add(new Card("Cytotoxin", "Attack 2", "art08", 0, 0, 0, 0, new List<Effect> { new Effect(EffectType.Damage, 2) }));
            All.Add(new Card("Phagocytosis", "Attack 3", "art01", 0, 0, 0, 0, new List<Effect> { new Effect(EffectType.Damage, 3) }));

            // Build textures
            foreach (Card c in All)
                c.BuildTexture(content, graphics, sb);


        }

        public void BuildTexture(ContentManager content, GraphicsDevice graphics, SpriteBatch sb)
        {
            Debug.WriteLine("\n\n BUILD SPRITE \n\n");
            Texture = new RenderTarget2D(graphics, CardTexture.Width, CardTexture.Height);
            graphics.SetRenderTarget(Texture);
            graphics.Clear(Color.Transparent);

            Texture2D artTexture = content.Load<Texture2D>(Art);

            sb.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null, null, Matrix.CreateScale(1));

            sb.Draw(CardTexture, Vector2.Zero, CardColor);
            sb.Draw(artTexture, new Rectangle(3,9,58,58), Color.White);
            sb.Draw(CardFrameTexture, Vector2.Zero, CardFrameColor);

            // Name
            sb.DrawString(Font, Name, new Vector2(7, 3), Color.WhiteSmoke);
            // Art
            // Frame
            // Description
            sb.DrawString(Font, Text, new Vector2(8, 70), Color.Black);

            // Energy signature
            // etc.

            sb.End();
            graphics.SetRenderTarget(null);
        }

        public bool Touched(Vector2 loc)
        {
            return (new Rectangle(X, Y, Card.Width, Card.Height).Contains(loc));
        }

    }


}
