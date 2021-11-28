using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Plasmid.Cards
{

    public enum CardState { FaceUp, FaceDown, FaceUpTapped, FaceDownTapped }

    [Flags]
    public enum CardEffectType { Damage = 0, Block = 1 }
    public static class CardEffectTypeExtensions
    {
        public static string ToString(this CardEffectType cef)
        {
            if (cef == CardEffectType.Damage)
                return "Damage";
            else if (cef == CardEffectType.Block)
                return "Block";
            else
                return "Error";
        }
    }
    public struct CardEffect
    {
        public CardEffectType Type { get; set; }
        public int Value { get; set; }
        public CardEffect(CardEffectType type, int value) { Type = type; Value = value; }
      
    }

    public abstract class BaseCard
    {
        private static bool isInitialized = false;
        private static bool isLoaded = false;

        protected static Game1 Game;

        public static readonly int CardHeight = 64;
        public static readonly int CardWidth = 64;
        public static readonly Color CardColor = Color.OldLace;
        //public static readonly Color CardBackColor = Color.CornflowerBlue;
        public static readonly Color CardBackColor = Color.White;
        //public static readonly Color CardBackColor = Game.ColorPalette.GetColor(4, 1);
        public static readonly Color CardFrameColor = Color.DarkGoldenrod;

        public static Texture2D CardBlankTexture { get; protected set; }
        public static Texture2D CardBackTexture { get; protected set; }
        public static Texture2D CardFrameTexture { get; protected set; }
        public static SpriteFont TitleFont { get; protected set; }
        public static SpriteFont BodyFont { get; protected set; }

        public bool IsVisible { get; set; }
        public CardState State { get; set; }
        public Vector2 Position { get; set; }
        public float X { get => this.Position.X; }
        public float Y { get => this.Position.Y; }
        public virtual float Width { get => BaseCard.CardWidth; protected set { throw new Exception("Cards have fixed Width."); } }
        public virtual float Height { get => BaseCard.CardHeight; protected set { throw new Exception("Cards have fixed Height."); } }
        public Rectangle Area { get => new Rectangle((int)this.X, (int)this.Y, (int)this.Width, (int)this.Height); }
        public Vector2 Center {  get => new Vector2((int)this.X + (this.Width/2), ( int)this.Y + (this.Height/2)); }


        public BaseCard(CardState state = CardState.FaceDown) : this(Vector2.Zero, state) { }
        public BaseCard(Vector2 position) : this(position, CardState.FaceDown) { }
        public BaseCard(Vector2 position, CardState state)
        {
            this.Position = position;
            this.State = state;
            this.IsVisible = true;
        }

        public static void Init(Game1 game)
        {
            if (BaseCard.isInitialized)
                return;

            BaseCard.Game = game;
            BaseCard.isInitialized = true;
        }
        
        public static void CheckInitialized()
        {
            if (!BaseCard.isInitialized)
                throw new Exception("Run BaseCard.Init(Game1 game) before using Cards module.");
        }

        public static void CheckLoaded()
        {
            if (!BaseCard.isLoaded)
                throw new Exception("Run BaseCard.Load() before using Cards module.");
        }

        public static void Load()
        {
            BaseCard.CheckInitialized();

            // Textures
            CardBlankTexture = BaseCard.Game.Content.Load<Texture2D>("card_blank");
            CardBackTexture = BaseCard.Game.Content.Load<Texture2D>("card_back");
            CardFrameTexture = BaseCard.Game.Content.Load<Texture2D>("card_frame");
            // Fonts
            TitleFont = Game.CardTitleFont;
            BodyFont = Game.CardFont;

            BaseCard.isLoaded = true;
        }

        public static void DrawCardBack(Vector2 position)
        {
            BaseCard.Game.Sprites.Draw(BaseCard.CardBackTexture, Vector2.Zero, position, BaseCard.CardBackColor);
        }
        public abstract void Draw();
    }
}
