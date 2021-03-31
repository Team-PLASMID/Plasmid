﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
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
        private List<Card> Hand;
        private Card FloatCard;
        private Vector2 FloatOriginPos;
        private Vector2 FloatTouchPos;

        private Texture2D BlankRectangle;

        public Game1()
        {
            _graphics = new ExtendedGraphicsDeviceManager(this, 480, 270);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            Hand = new List<Card>();
            Deck = new List<Card>();

            Hand.Add(Card.New(20, 20, "Bulbasaur", "Seed Pokemon", "bulb", 1, 1, 1, 1));
            Hand.Add(Card.New(100, 20, "Charmander", "Lizard Pokemon", "char", 1, 1, 1, 1));
            Hand.Add(Card.New(180, 20, "Squirtle", "Tiny Turtle Pokemon", "squirt", 1, 1, 1, 1));
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
                    Debug.WriteLine("TOUCH");
                    for (int i = Hand.Count - 1; i >= 0; i--)
                        if (Hand[i].Touched(pos))
                        {
                            FloatCard = Hand[i];
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
                    if (FloatCard != null)
                        Hand.Add(FloatCard);
                    FloatCard = null;
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, _graphics.Transform);

            // Draw background
            _spriteBatch.Draw(BlankRectangle, new Rectangle(0, 0, 270, 480), Color.CornflowerBlue);

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
