using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Plasmid.Cards
{

    public enum CardState { FaceUp, FaceDown, FaceUpTapped, FaceDownTapped }

    [Flags]
    public enum CardEffectType { Damage = 0, Block = 1 }
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

        public static readonly int Height = 96;
        public static readonly int Width = 64;
        public static readonly Color CardColor = Color.OldLace;
        public static readonly Color CardBackColor = Color.CornflowerBlue;
        public static readonly Color CardFrameColor = Color.DarkGoldenrod;

        public static Texture2D CardBlankTexture { get; set; }
        public static Texture2D CardBackTexture { get; set; }
        public static Texture2D CardFrameTexture { get; set; }
        public static SpriteFont Font { get; set; }

        public bool IsVisible { get => this.isVisible; }
        protected bool isVisible;
        public CardState State { get => this.state; }
        protected CardState state;
        public Vector2 Position { get => this.position; }
        protected Vector2 position;
        public int X { get => (int)position.X; }
        public int Y { get => (int)position.Y; }
        public Rectangle Area { get => new Rectangle(this.X, this.Y, BaseCard.Width, BaseCard.Height); }


        public BaseCard(CardState state = CardState.FaceDown) : this(Vector2.Zero, state) { }
        public BaseCard(Vector2 position) : this(position, CardState.FaceDown) { }
        public BaseCard(Vector2 position, CardState state)
        {
            this.position = position;
            this.state = state;
            this.isVisible = true;
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
            CardBlankTexture = BaseCard.Game.Content.Load<Texture2D>("card");
            CardBackTexture = BaseCard.Game.Content.Load<Texture2D>("card_back");
            CardFrameTexture = BaseCard.Game.Content.Load<Texture2D>("card_frame");
            // Fonts
            Font = BaseCard.Game.Content.Load<SpriteFont>("CardFont");

            BaseCard.isLoaded = true;
        }

        public static void DrawCardBack(Vector2 position)
        {
            BaseCard.Game.Sprites.Draw(BaseCard.CardBackTexture, Vector2.Zero, position, BaseCard.CardBackColor);
        }
        public abstract void Draw();
    }
}
