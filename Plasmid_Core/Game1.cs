using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Plasmid_Core
{
    public class Game1 : Game
    {
        private ExtendedGraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private TouchCollection TouchState;

        private List<Card> Deck;
        private List<Card> Discard;
        private List<Card> Hand;

        private Card FloatCard;
        private List<Card> FloatOrigin;
        private Vector2 FloatOriginPos;
        private Vector2 FloatTouchPos;

        private Texture2D BackgroundTexture;
        private Color BackgroundColor;
        private Texture2D BlankRectangle;

        //Demo regions
        private Rectangle PlayArea = new Rectangle(34, 34, 202, 142);
        private Rectangle HandArea = new Rectangle(31, 211, 208, 118);

        public Game1()
        {
            _graphics = new ExtendedGraphicsDeviceManager(this, 480, 270);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            Hand = new List<Card>();
            Deck = new List<Card>();
            Discard = new List<Card>();

            // Demo deck
            List<string> art = new List<string>(new string[]{"art00", "art01", "art02", "art03", "art04", "art05", "art06", "art07", "art08"});
            Random rand = new Random();
            int i;
            while (art.Count > 0)
            {
                i = rand.Next(0, art.Count);
                Deck.Add(Card.New(30, 354, "", "", art[i], 1, 1, 1, 1));
                art.RemoveAt(i);
            }
        }

        protected override void Initialize()
        {

            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();
            _graphics.CalculateTransformation();
            base.Initialize();

        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Card.LoadTextures(Content);
            foreach (Card card in Card.All)
                card.BuildTexture(Content, GraphicsDevice, _spriteBatch);

            BackgroundTexture = Content.Load<Texture2D>("demo_bg");
            BackgroundColor = Color.LightCyan;
            BlankRectangle = new Texture2D(GraphicsDevice, 1, 1);
            BlankRectangle.SetData(new[] { Color.White });
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            TouchState = TouchPanel.GetState();
            foreach (var touch in TouchState)
            {
                // Scale touch position
                Vector2 pos = Vector2.Transform(touch.Position, Matrix.Invert(_graphics.Transform));

                // NEW TOUCH
                if (touch.State == TouchLocationState.Pressed)
                {
                    // Grab card from deck
                    if (Deck.Count > 0)
                    {
                        if (Deck[0].Touched(pos))
                        {
                            FloatCard = Deck[Deck.Count - 1];
                            FloatOrigin = Deck;
                            FloatOriginPos = FloatCard.Pos;
                            FloatTouchPos = pos;
                            Deck.RemoveAt(Deck.Count - 1);
                        }
                    }
                    // Grab card from hand
                    for (int i = Hand.Count - 1; i >= 0; i--)
                        if (Hand[i].Touched(pos))
                        {
                            FloatCard = Hand[i];
                            FloatOrigin = Hand;
                            FloatOriginPos = FloatCard.Pos;
                            FloatTouchPos = pos;
                            Hand.Remove(FloatCard);
                            break;
                        }
                }

                // MOVED TOUCH
                else if (touch.State == TouchLocationState.Moved)
                {
                    Debug.WriteLine("DRAG");
                    if (FloatCard != null)
                        FloatCard.Pos = Vector2.Subtract(Vector2.Add(FloatOriginPos, pos), FloatTouchPos);
                }

                // RELEASED TOUCH
                else if (touch.State == TouchLocationState.Released)
                {

                    Debug.WriteLine("RELEASING CARD, null = ", FloatCard == null);
                    if (FloatCard != null)
                    {
                        if (HandArea.Contains(pos))
                            Hand.Add(FloatCard);
                        else if (PlayArea.Contains(pos))
                            Discard.Add(FloatCard);
                        else
                        {
                            FloatOrigin.Add(FloatCard);
                            FloatCard.Pos = FloatOriginPos;
                        }
                    }
                    FloatCard = null;
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null, null, _graphics.Transform);

            // Draw background
            _spriteBatch.Draw(BackgroundTexture, Vector2.Zero, BackgroundColor);

            // Draw deck
            if (Deck.Count > 0)
                _spriteBatch.Draw(Card.CardBackTexture, new Vector2(30, 354), Card.CardBackColor);

            // Draw discard
            if (Discard.Count > 0)
                _spriteBatch.Draw(Discard[Discard.Count-1].Texture, new Vector2(176, 354), Color.White);

            // Draw hand cards
            if (Hand.Count > 0)
                foreach (Card card in Hand)
                {
                    if (card.Texture == null)
                        continue;
                    _spriteBatch.Draw(card.Texture, card.Pos, Color.White);
                }
                

            // Draw floating card w/ shadow
            if (FloatCard != null)
            {
                _spriteBatch.Draw(Card.CardTexture, FloatCard.Pos, new Color(Color.Black, 50));
                _spriteBatch.Draw(FloatCard.Texture, Vector2.Add(FloatCard.Pos, new Vector2(5, -10)), Color.White);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
