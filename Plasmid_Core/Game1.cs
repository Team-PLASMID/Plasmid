using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

namespace Plasmid_Core
{
    public class Game1 : Game
    {
        private ExtendedGraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private TouchCollection TouchState;

        private List<Card> Deck;
        private List<Card> Discard;
        private CardHand Hand;

        private Card FloatCard;
        private List<Card> FloatOrigin;
        private Vector2 FloatOriginPos;
        private Vector2 FloatTouchPos;

        private Texture2D BackgroundTexture;
        private Color BackgroundColor;
        private Texture2D HpPanelLeftTexture;
        private Texture2D HpPanelRightTexture;
        private Texture2D HpBarTexture;
        private Texture2D BlankRectangle;

        private SpriteFont BattleFont;

        // Battle layout positions
        private Rectangle PlayArea = new Rectangle(0, 64, 270, 224);
        private Rectangle HandArea = new Rectangle(15, 304, 240, 112);
        private Rectangle DeckArea = new Rectangle(31, 432, Card.Width, Card.Height);
        private Rectangle DiscardArea = new Rectangle(175, 432, Card.Width, Card.Height);
        private Rectangle MessageArea = new Rectangle(15, 16, 240, 32);
        private Vector2 HpPanelLeftPos = new Vector2(0, 257);
        private Vector2 HpPanelRightPos = new Vector2(125, 68);
        private Vector2 HpBarLeftPos = new Vector2(4, 261);
        private Vector2 HpBarRightPos = new Vector2(131, 72);

        Battle BattleDemo;

        public Game1()
        {
            _graphics = new ExtendedGraphicsDeviceManager(this, 480, 270);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            Hand = new CardHand(HandArea, _graphics, _spriteBatch);
            Deck = new List<Card>();
            Discard = new List<Card>();

            Microbe microbeA = new Microbe();
            Microbe microbeB = new Microbe();

            BattleDemo = new Battle(microbeA, microbeB);

            // Demo deck
            /*
            List<string> art = new List<string>(new string[]{"art00", "art01", "art02", "art03", "art04", "art05", "art06", "art07", "art08"});
            Random rand = new Random();
            int i;
            while (art.Count > 0)
            {
                i = rand.Next(0, art.Count);
                Deck.Add(Card.New(DeckPos, "Sample Card" + (10 - art.Count), "Descriptive text", art[i], 1, 1, 1, 1));
                art.RemoveAt(i);
            }
            */
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

            Card.Load(Content, GraphicsDevice, _spriteBatch);

            BackgroundTexture = Content.Load<Texture2D>("battle_console");
            BackgroundColor = Color.LightCyan;

            HpPanelLeftTexture = Content.Load<Texture2D>("hp_panel_l");
            HpPanelRightTexture = Content.Load<Texture2D>("hp_panel_r");
            HpBarTexture = Content.Load<Texture2D>("hp_bar");

            BlankRectangle = new Texture2D(GraphicsDevice, 1, 1);
            BlankRectangle.SetData(new[] { Color.White });

            BattleFont = Content.Load<SpriteFont>("BattleFont");

            // Demo Deck

            Deck.Add(Card.Copy("Enzyme"));
            Deck.Add(Card.Copy("Enzyme"));
            Deck.Add(Card.Copy("Enzyme"));
            Deck.Add(Card.Copy("Enzyme"));
            Deck.Add(Card.Copy("Cytotoxin"));
            Deck.Add(Card.Copy("Cytotoxin"));
            Deck.Add(Card.Copy("Cytotoxin"));
            Deck.Add(Card.Copy("Phagocytosis"));
            Deck.Add(Card.Copy("Phagocytosis"));

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
                        if (DeckArea.Contains(pos))
                        {
                            FloatCard = Deck[Deck.Count - 1];
                            FloatCard.Pos = new Vector2(DeckArea.X, DeckArea.Y);
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
                        // Consider middle of floating card instead of touch position for drop
                        Vector2 middle = new Vector2(FloatCard.Pos.X + Card.Width / 2, FloatCard.Pos.Y + Card.Height / 2);

                        if (HandArea.Contains(middle))
                            Hand.Add(FloatCard);
                        else if (PlayArea.Contains(middle) && FloatOrigin == Hand)
                        {
                            BattleDemo.PlayCard(FloatCard);
                            Discard.Add(FloatCard);
                        }
                        else
                        {
                            FloatOrigin.Add(FloatCard);
                            FloatCard.Pos = FloatOriginPos;
                        }
                    }
                    FloatCard = null;
                }
            }

            Hand.Align();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null, null, _graphics.Transform);

            // Draw background
            _spriteBatch.Draw(BackgroundTexture, Vector2.Zero, BackgroundColor);
            _spriteBatch.DrawString(BattleFont, "PLASMID", new Vector2(MessageArea.X+5, MessageArea.Y+8), Color.Black);

            // Draw HP bars
            _spriteBatch.Draw(HpPanelLeftTexture, HpPanelLeftPos, BackgroundColor);
            _spriteBatch.Draw(HpPanelRightTexture, HpPanelRightPos, BackgroundColor);
            double playerbar = (120.0 * ((double)BattleDemo.PlayerMicrobe.HP / BattleDemo.PlayerMicrobe.MaxHP));
            double opponentbar = (120.0 * ((double)BattleDemo.OpponentMicrobe.HP / BattleDemo.OpponentMicrobe.MaxHP));
            Debug.WriteLine(playerbar + "   " + opponentbar);
            _spriteBatch.Draw(BlankRectangle, new Rectangle((int)HpBarLeftPos.X + 1, (int)HpBarLeftPos.Y + 3, (int)playerbar, 13), Color.LimeGreen);
            _spriteBatch.Draw(BlankRectangle, new Rectangle((int)HpBarRightPos.X + 1, (int)HpBarRightPos.Y + 3, (int)opponentbar, 13), Color.LimeGreen);
            _spriteBatch.Draw(HpBarTexture, HpBarLeftPos, Color.White);
            _spriteBatch.Draw(HpBarTexture, HpBarRightPos, Color.White);


            // Draw deck
            if (Deck.Count > 0)
                _spriteBatch.Draw(Card.CardBackTexture, DeckArea, Card.CardBackColor);

            // Draw discard
            if (Discard.Count > 0)
                _spriteBatch.Draw(Discard[Discard.Count-1].Texture, DiscardArea, Color.White);

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
                // shadow
                _spriteBatch.Draw(Card.CardTexture, FloatCard.Pos, new Color(Color.Black, 50));
                // card (draw card back if from the deck)
                if (FloatOrigin == Deck)
                    _spriteBatch.Draw(Card.CardBackTexture, Vector2.Add(FloatCard.Pos, new Vector2(5, -10)), Card.CardBackColor);
                else
                    _spriteBatch.Draw(FloatCard.Texture, Vector2.Add(FloatCard.Pos, new Vector2(5, -10)), Color.White);
            }

            

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
