using Apos.Shapes;
using LilyPath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Plasmid.Graphics;
using Plasmid.Cards;

namespace Plasmid
{
    public class Game1 : Game
    {
        private Random rand = new Random();
        //private ExtendedGraphicsDeviceManager graphics;
        private GraphicsDeviceManager graphics;
        private Shapes shapes;
        private Sprites sprites;
        private Camera camera;
        //private Touchscreen touchscreen;
        private TouchCollection touchState;
        private Screen screen;
        //private SpriteBatch _spriteBatch;
        //private TouchCollection TouchState;

        private List<Card> Deck;
        private List<Card> Discard;
        private CardHand Hand;

        private Card floatCard;
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
        //private Rectangle PlayArea = new Rectangle(0, 64, 270, 224);
        //private Rectangle HandArea = new Rectangle(20, 304, 230, 112);
        //private Rectangle DeckArea = new Rectangle(31, 432, Card.Width, Card.Height);
        //private Rectangle DiscardArea = new Rectangle(175, 432, Card.Width, Card.Height);
        //private Rectangle MessageArea = new Rectangle(15, 16, 240, 32);
        //private Vector2 HpPanelLeftPos = new Vector2(0, 257);
        //private Vector2 HpPanelRightPos = new Vector2(125, 68);
        //private Vector2 HpBarLeftPos = new Vector2(4, 261);
        //private Vector2 HpBarRightPos = new Vector2(131, 72);

        //private Vector2 PlayerSpritePos = new Vector2(142, 157);
        //private Vector2 EnemySpritePos = new Vector2(5, 70);

        private Rectangle PlayArea = new Rectangle(0, 193, 270, 224);
        private Rectangle HandArea = new Rectangle(20, 70, 230, 100);
        private Rectangle DeckArea = new Rectangle(31, -48, Card.Width, Card.Height);
        private Rectangle DiscardArea = new Rectangle(175, -48, Card.Width, Card.Height);
        //private Rectangle MessageArea = new Rectangle(15, 480 - 16, 240, 32);

        private Vector2 HpPanelLeftPos = new Vector2(0, 195);
        private Vector2 HpPanelRightPos = new Vector2(125, 386);
        private Vector2 HpBarLeftPos = new Vector2(4, 200);
        private Vector2 HpBarRightPos = new Vector2(131, 391);

        private Vector2 PlayerSpritePos = new Vector2(142, 157);
        private Vector2 EnemySpritePos = new Vector2(5, 70);

        private Microbe microbeA;
        private Microbe microbeB;

        Battle BattleDemo;
        private Texture2D Logo;
        public Game1()
        {
            //graphics = new ExtendedGraphicsDeviceManager(this, 480, 270);
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {

            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();
            //graphics.CalculateTransformation();

            this.screen = new Screen(this, 270, 480);
            this.sprites = new Sprites(this);
            this.shapes = new Shapes(this);
            this.camera = new Camera(this.screen);
            //this.touchscreen = new Touchscreen();

            //Hand = new CardHand(HandArea, graphics, _spriteBatch);
            Hand = new CardHand(HandArea);
            Deck = new List<Card>();
            Discard = new List<Card>();

            microbeA = new Microbe();
            microbeB = new Microbe();

            microbeA.Vertices = Utils.RandomPoly(new Screen(this, 128, 128));
            microbeB.Vertices = Utils.RandomPoly(new Screen(this, 128, 128));
            //microbeA.TestGen(GraphicsDevice);

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

            base.Initialize();

        }

        protected override void LoadContent()
        {
            DrawBatch drawBatch = new DrawBatch(GraphicsDevice);
            //ShapeBatch sb = new ShapeBatch(GraphicsDevice, Content);
            microbeA.GenerateDNA();
            microbeA.GenSprite(GraphicsDevice);
            microbeB.GenerateDNA();
            microbeB.GenSprite(GraphicsDevice);

            Card.Load(Content, GraphicsDevice);

            BackgroundTexture = Content.Load<Texture2D>("battle_console");
            BackgroundColor = Color.LightCyan;

            HpPanelLeftTexture = Content.Load<Texture2D>("hp_panel_l");
            HpPanelRightTexture = Content.Load<Texture2D>("hp_panel_r");
            HpBarTexture = Content.Load<Texture2D>("hp_bar");

            BlankRectangle = new Texture2D(GraphicsDevice, 1, 1);
            BlankRectangle.SetData(new[] { Color.White });

            BattleFont = Content.Load<SpriteFont>("BattleFont");

            Logo = Content.Load<Texture2D>("logo");

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

            // Shuffle Demo Deck
            // ( This should be made into a proper Shuffle function )
            Random rand = new Random();
            int n = Deck.Count;
            for (int i = Deck.Count - 1; i > 1; i--)
            {
                int rnd = rand.Next(i + 1);
                Card card = Deck[rnd];
                Deck[rnd] = Deck[i];
                Deck[i] = card;
            }

        }

        //protected override void Update(GameTime gameTime)
        //{
        //    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
        //        Exit();

        //    touchscreen.Update();

        //    if (this.Deck.Count > 0)
        //    {
        //        if (touchscreen.IsRegionPressed(this.screen, this.DeckArea))
        //        {
        //            FloatCard = Deck[Deck.Count - 1];
        //            FloatCard.Pos = new Vector2(DeckArea.X, DeckArea.Y);
        //            FloatOrigin = Deck;
        //            FloatOriginPos = FloatCard.Pos;
        //            FloatTouchPos = pos;
        //            Deck.RemoveAt(Deck.Count - 1);
        //        }
        //    }




        //        // NEW TOUCH
        //        if (touch.State == TouchLocationState.Pressed)
        //        {
        //            // Grab card from deck
        //            if (Deck.Count > 0)
        //            {
        //                if (DeckArea.Contains(pos))
        //                {
        //                    FloatCard = Deck[Deck.Count - 1];
        //                    FloatCard.Pos = new Vector2(DeckArea.X, DeckArea.Y);
        //                    FloatOrigin = Deck;
        //                    FloatOriginPos = FloatCard.Pos;
        //                    FloatTouchPos = pos;
        //                    Deck.RemoveAt(Deck.Count - 1);
        //                }
        //            }
        //            // Grab card from hand
        //            for (int i = Hand.Count - 1; i >= 0; i--)
        //                if (Hand[i].Touched(pos))
        //                {
        //                    FloatCard = Hand[i];
        //                    FloatOrigin = Hand;
        //                    FloatOriginPos = FloatCard.Pos;
        //                    FloatTouchPos = pos;
        //                    Hand.Remove(FloatCard);
        //                    break;
        //                }
        //        }

        //        // MOVED TOUCH
        //        else if (touch.State == TouchLocationState.Moved)
        //        {
        //            Debug.WriteLine("DRAG");
        //            if (FloatCard != null)
        //                FloatCard.Pos = Vector2.Subtract(Vector2.Add(FloatOriginPos, pos), FloatTouchPos);
        //        }

        //        // RELEASED TOUCH
        //        else if (touch.State == TouchLocationState.Released)
        //        {
        //            Debug.WriteLine("RELEASING CARD, null = ", FloatCard == null);
        //            if (FloatCard != null)
        //            {
        //                // Consider middle of floating card instead of touch position for drop
        //                Vector2 middle = new Vector2(FloatCard.Pos.X + Card.Width / 2, FloatCard.Pos.Y + Card.Height / 2);

        //                if (HandArea.Contains(middle))
        //                    Hand.Add(FloatCard);
        //                else if (PlayArea.Contains(middle) && FloatOrigin == Hand)
        //                {
        //                    BattleDemo.PlayCard(FloatCard);
        //                    Discard.Add(FloatCard);
        //                }
        //                else
        //                {
        //                    FloatOrigin.Add(FloatCard);
        //                    FloatCard.Pos = FloatOriginPos;
        //                }
        //            }
        //            FloatCard = null;
        //        }
        //    }

        //    Hand.Align();

        //    base.Update(gameTime);
        //}

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            touchState = TouchPanel.GetState();
            foreach (var touch in touchState)
            {
                // Scale touch position
                //Vector2 pos = Vector2.Transform(touch.Position, Matrix.Invert(graphics.Transform));
                Vector2 pos = touch.Position;

                // NEW TOUCH
                if (touch.State == TouchLocationState.Pressed)
                {
                    // Grab card from deck
                    if (Deck.Count > 0)
                    {
                        if (DeckArea.Contains(pos))
                        {
                            floatCard = Deck[Deck.Count - 1];
                            floatCard.Pos = new Vector2(DeckArea.X, DeckArea.Y);
                            FloatOrigin = Deck;
                            FloatOriginPos = floatCard.Pos;
                            FloatTouchPos = pos;
                            Deck.RemoveAt(Deck.Count - 1);
                        }
                    }
                    // Grab card from hand
                    for (int i = Hand.Count - 1; i >= 0; i--)
                        if (Hand[i].Touched(pos))
                        {
                            floatCard = Hand[i];
                            FloatOrigin = Hand;
                            FloatOriginPos = floatCard.Pos;
                            FloatTouchPos = pos;
                            Hand.Remove(floatCard);
                            break;
                        }
                }

                // MOVED TOUCH
                else if (touch.State == TouchLocationState.Moved)
                {
                    Debug.WriteLine("DRAG");
                    if (floatCard != null)
                        floatCard.Pos = Vector2.Subtract(Vector2.Add(FloatOriginPos, pos), FloatTouchPos);
                }

                // RELEASED TOUCH
                else if (touch.State == TouchLocationState.Released)
                {
                    Debug.WriteLine("RELEASING CARD, null = ", floatCard == null);
                    if (floatCard != null)
                    {
                        // Consider middle of floating card instead of touch position for drop
                        Vector2 middle = new Vector2(floatCard.Pos.X + Card.Width / 2, floatCard.Pos.Y + Card.Height / 2);

                        if (HandArea.Contains(middle))
                            Hand.Add(floatCard);
                        else if (PlayArea.Contains(middle) && FloatOrigin == Hand)
                        {
                            BattleDemo.PlayCard(this.floatCard);
                            Discard.Add(floatCard);
                        }
                        else
                        {
                            FloatOrigin.Add(floatCard);
                            floatCard.Pos = FloatOriginPos;
                        }
                    }
                    floatCard = null;
                }
            }

            Hand.Align();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            this.screen.Set();
            GraphicsDevice.Clear(Color.Black);

            // TODO: implement Transforms for shapes other than polys
            Transform defaultTrans = new Transform(Vector2.Zero, 0f, 1f);
            Transform playerMicrobeTrans = new Transform(new Vector2(142,193), 0f, 1f);
            Transform enemyMicrobeTrans = new Transform(new Vector2(0,288), 0f, 1f);

            sprites.Begin(null);

            // Draw Background
            sprites.Draw(BackgroundTexture, Vector2.Zero, Vector2.Zero, BackgroundColor);

            // Draw Title 
            // _spriteBatch.DrawString(BattleFont, "PLASMID", new Vector2(MessageArea.X + 5, MessageArea.Y + 8), Color.Black);

            // Draw Microbes
            GraphUtils.Triangulate(microbeA.Vertices, out int[] trianglesA, out string errorMessageA);
            GraphUtils.Triangulate(microbeB.Vertices, out int[] trianglesB, out string errorMessageB);

            shapes.Begin(null);
            shapes.DrawPolygonFill(microbeA.Vertices, trianglesA, playerMicrobeTrans, GraphUtils.GetRandomColor(this.rand));
            shapes.DrawPolygon(microbeA.Vertices, playerMicrobeTrans, 3, GraphUtils.GetRandomColor(this.rand));
            shapes.DrawPolygonFill(microbeB.Vertices, trianglesB, enemyMicrobeTrans, GraphUtils.GetRandomColor(this.rand));
            shapes.DrawPolygon(microbeB.Vertices, enemyMicrobeTrans, 3, GraphUtils.GetRandomColor(this.rand));
            shapes.End();
            //_spriteBatch.Draw(microbeA.Sprite, PlayerSpritePos, Color.White);
            //_spriteBatch.Draw(microbeB.Sprite, EnemySpritePos, Color.White);

            // Draw HP Bars
            sprites.Draw(HpPanelLeftTexture, Vector2.Zero, HpPanelLeftPos, BackgroundColor);
            sprites.Draw(HpPanelRightTexture, Vector2.Zero, HpPanelRightPos, BackgroundColor);
            sprites.End();

            float playerbar = 120f * (BattleDemo.PlayerMicrobe.HP / BattleDemo.PlayerMicrobe.MaxHP);
            float opponentbar = 120f * (BattleDemo.OpponentMicrobe.HP / BattleDemo.OpponentMicrobe.MaxHP);

            shapes.Begin(null);
            shapes.DrawRectangleFill(HpBarLeftPos.X + 1, HpBarLeftPos.Y + 3, playerbar, 13, Color.LimeGreen);
            shapes.DrawRectangleFill(HpBarRightPos.X + 1, HpBarRightPos.Y + 3, playerbar, 13, Color.LimeGreen);
            shapes.End();

            sprites.Begin(null);

            sprites.Draw(HpBarTexture, Vector2.Zero, HpBarLeftPos, Color.White);
            sprites.Draw(HpBarTexture, Vector2.Zero, HpBarRightPos, Color.White);

            // Draw logo
            //_spriteBatch.Draw(Logo, new Vector2(PlayArea.X + (PlayArea.Width - Logo.Width)/2, PlayArea.Y + (PlayArea.Height - Logo.Height)/2), Color.White);

            // Draw deck
            if (Deck.Count > 0)
                sprites.Draw(Card.CardBackTexture, null, DeckArea, Card.CardBackColor);

            // Draw discard
            if (Discard.Count > 0)
                sprites.Draw(Discard[Discard.Count-1].Texture, null, DiscardArea, Color.White);

            // Draw hand cards
            if (Hand.Count > 0)
                foreach (Card card in Hand)
                {
                    if (card.Texture == null)
                        continue;
                    sprites.Draw(card.Texture, Vector2.Zero, card.Pos, Color.White);
                }
                

            // Draw floating card w/ shadow
            if (floatCard != null)
            {
                // shadow
                sprites.Draw(Card.CardTexture, Vector2.Zero, floatCard.Pos, new Color(Color.Black, 50));
                // card (draw card back if from the deck)
                if (FloatOrigin == Deck)
                    sprites.Draw(Card.CardBackTexture, Vector2.Zero, Vector2.Add(floatCard.Pos, new Vector2(5, -10)), Card.CardBackColor);
                else
                    sprites.Draw(floatCard.Texture, Vector2.Zero, Vector2.Add(floatCard.Pos, new Vector2(5, -10)), Color.White);
            }
            
            sprites.End();

            //// Debugging: Area overlays
            //shapes.Begin(null);
            //shapes.DrawRectangleFill(this.PlayArea, new Color(Color.Pink, 50));
            //shapes.DrawRectangleFill(this.HandArea, new Color(Color.Blue, 50));
            //shapes.DrawRectangleFill(this.DeckArea, new Color(Color.Green, 50));
            //shapes.DrawRectangleFill(this.DiscardArea, new Color(Color.Red, 50));
            //shapes.End();


            this.screen.UnSet();
            this.screen.Present(this.sprites);

            base.Draw(gameTime);
        }
    }
}
