using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;

namespace Plasmid_Core
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Matrix ScaleMatrix;

        private List<Card> CardList;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            CardList = new List<Card>();

            CardList.Add(new Card("Bulbasaur", "Seed Pokemon", "bulb", 1, 1, 1, 1));
            CardList.Add(new Card("Charmander", "Lizard Pokemon", "char", 1, 1, 1, 1));
            CardList.Add(new Card("Squirtle", "Tiny Turtle Pokemon", "squirt", 1, 1, 1, 1));
            Debug.WriteLine("MADE " + CardList + " CARDS.");
        }

        protected override void Initialize()
        {
            // TODO: Real dynamic scaling
            ScaleMatrix = Matrix.CreateScale(4);

            base.Initialize();

        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Card.LoadTextures(Content);
            foreach (Card card in CardList)
                card.BuildTexture(Content, GraphicsDevice, _spriteBatch);

            Debug.WriteLine("LOADED " + CardList + " CARDS.");
            foreach (Card card in CardList)
                Debug.WriteLine(card.Name + "   texture null:" + (card.Texture == null));

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, ScaleMatrix);

            int x = 20;
            int y = 20;
            foreach (Card card in CardList)
            {
                if (card.Texture == null)
                    continue;
                _spriteBatch.Draw(card.Texture, new Vector2(x, y), Color.White);
                x += 80;
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
