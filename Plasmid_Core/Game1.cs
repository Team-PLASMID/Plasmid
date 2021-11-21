
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Plasmid.Microbes;
using Plasmid.Graphics;
using Plasmid.Cards;
using Plasmid.Input;

namespace Plasmid
{
    public class Game1 : Game
    {
        private Random rand = new Random();

        private GraphicsDeviceManager graphics;

        public Camera Camera { get; set; }
        public Screen Screen { get; set; }
        public ShapeBatcher Shapes { get; set; }
        public SpriteBatcher Sprites { get; set; }
        public TouchInput Touch { get; set; }
        public CardDeck Deck { get; set; }
        public CardDeck Discard { get; set; }
        public CardHand Hand { get; set; }
        public FloatCard Float { get; set; }
        public List<Animation> Animations { get; set; }
        

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
        //private Rectangle DeckArea = new Rectangle(31, -48, Card.Width, Card.Height);
        //private Rectangle DiscardArea = new Rectangle(175, -48, Card.Width, Card.Height);
        //private Rectangle MessageArea = new Rectangle(15, 480 - 16, 240, 32);

        private Vector2 HpPanelLeftPos = new Vector2(0, 195);
        private Vector2 HpPanelRightPos = new Vector2(125, 386);
        private Vector2 HpBarLeftPos = new Vector2(4, 200);
        private Vector2 HpBarRightPos = new Vector2(131, 391);

        private Vector2 PlayerSpritePos = new Vector2(142, 157);
        private Vector2 EnemySpritePos = new Vector2(5, 70);

        private Microbe microbeA;
        private Microbe microbeB;

        private Vector2[] polyVertices;
        private int[] polyTriangles;
        private Vector2 clickPosition;
        private double clickTime = 0;

        Battle BattleDemo;
        private Texture2D Logo;
        public Game1()
        {
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

            this.Screen = new Screen(this, 270, 480);
            this.Camera = new Camera(this.Screen);

            this.Touch = new TouchInput();
            this.Touch.Init(this);
            this.Sprites = new SpriteBatcher(this);
            this.Shapes = new ShapeBatcher(this);

            Animation.Init(this);
            this.Animations = new List<Animation>();

            BaseCard.Init(this);

            Hand = new CardHand(new Vector2(20, 70));
            Deck = new CardDeck(new Vector2(31, -48));
            Discard = new CardDeck(new Vector2(175, -48), state: CardState.FaceUp, isDrawAllowed: false, isSearchAllowed: true);
            Float = new FloatCard();

            //microbeA = new Microbe();
            polyVertices = new Vector2[] {
                new Vector2(4,-1), new Vector2(7, -2), new Vector2(3, -10), new Vector2(1, -5),
                new Vector2(-1, -5), new Vector2(-3, -10), new Vector2(-7, -2), new Vector2(-4,-1),
                new Vector2(-4,1), new Vector2(-7, 2), new Vector2(-3, 10), new Vector2(-1, 5),
                new Vector2(1, 5), new Vector2(3, 10), new Vector2(7, 2), new Vector2(4, 1)
            };
            //microbeA.Vertices = GraphUtils.RemoveColinear(microbeA.Vertices);
            GraphUtils.Triangulate(polyVertices, out polyTriangles, out string errorMessage);

            //microbeB = new Microbe();
            //
            //microbeA.Vertices = GraphUtils.RemoveColinear(microbeA.Vertices);
            //GraphUtils.Triangulate(microbeA.Vertices, out triangles, out errorMessage);
            //microbeB.Triangles = triangles;

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
            //ShapeBatch sb = new ShapeBatch(GraphicsDevice, Content);
            //microbeA.GenerateDNA();
            //microbeA.GenSprite(GraphicsDevice);
            //microbeB.GenerateDNA();
            //microbeB.GenSprite(GraphicsDevice);
            BaseCard.Load();
            Card.Load(GraphicsDevice);

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
            this.Deck.Shuffle();

        }

        protected override void Update(GameTime gameTime)
        {
            this.Touch.HandleTouch(gameTime.TotalGameTime.TotalMilliseconds);

            for (int i = 0; i < this.Animations.Count; i++)
            {
                this.Animations[i].Update(gameTime.TotalGameTime.TotalMilliseconds);
                if (this.Animations[i].IsDisposed)
                    this.Animations.RemoveAt(i);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            this.Screen.Set();
            GraphicsDevice.Clear(Color.Black);

            // TODO: implement Transforms for shapes other than polys
            Transform defaultTrans = new Transform(Vector2.Zero, 0f, 1f);
            //Transform playerMicrobeTrans = new Transform(new Vector2(142,193), 0f, 5f);
            Transform playerMicrobeTrans = new Transform(new Vector2(200,300), 0f, 8f);
            Transform enemyMicrobeTrans = new Transform(new Vector2(0,288), 0f, 1f);


            Sprites.Begin(null);

            // Draw Background
            Sprites.Draw(BackgroundTexture, Vector2.Zero, Vector2.Zero, BackgroundColor);

            // Draw Title 
            // _spriteBatch.DrawString(BattleFont, "PLASMID", new Vector2(MessageArea.X + 5, MessageArea.Y + 8), Color.Black);

            // Draw Microbes

            Shapes.Begin(null);

            //shapes.DrawPolyTriangles(microbeA.Vertices, trianglesA, defaultTrans, Color.Red);
            Shapes.DrawPolygonFill(polyVertices, polyTriangles, playerMicrobeTrans, Color.LightGreen);
            //shapes.DrawPolyTriangles(polyVertices, polyTriangles, playerMicrobeTrans, Color.DarkGreen);
            Shapes.DrawPolygon(polyVertices, playerMicrobeTrans, 3, Color.DarkGreen);

            //shapes.DrawPolygonFill(microbeB.Vertices, microbeB.Triangles, enemyMicrobeTrans, Color.Pink);
            //shapes.DrawPolyTriangles(microbeB.Vertices, microbeB.Triangles, enemyMicrobeTrans, Color.Red);
            //shapes.DrawPolygon(microbeB.Vertices, enemyMicrobeTrans, 3, Color.Red);

            Shapes.End();
            //_spriteBatch.Draw(microbeA.Sprite, PlayerSpritePos, Color.White);
            //_spriteBatch.Draw(microbeB.Sprite, EnemySpritePos, Color.White);

            // Draw HP Bars
            Sprites.Draw(HpPanelLeftTexture, Vector2.Zero, HpPanelLeftPos, BackgroundColor);
            Sprites.Draw(HpPanelRightTexture, Vector2.Zero, HpPanelRightPos, BackgroundColor);
            Sprites.End();

            //float playerbar = 120f * (BattleDemo.PlayerMicrobe.HP / BattleDemo.PlayerMicrobe.MaxHP);
            //float opponentbar = 120f * (BattleDemo.OpponentMicrobe.HP / BattleDemo.OpponentMicrobe.MaxHP);

            Shapes.Begin(null);
            //shapes.DrawRectangleFill(HpBarLeftPos.X + 1, HpBarLeftPos.Y + 3, playerbar, 13, Color.LimeGreen);
            //shapes.DrawRectangleFill(HpBarRightPos.X + 1, HpBarRightPos.Y + 3, opponentbar, 13, Color.LimeGreen);
            Shapes.End();

            Sprites.Begin(null);

            Sprites.Draw(HpBarTexture, Vector2.Zero, HpBarLeftPos, Color.White);
            Sprites.Draw(HpBarTexture, Vector2.Zero, HpBarRightPos, Color.White);

            // Draw logo
            //_spriteBatch.Draw(Logo, new Vector2(PlayArea.X + (PlayArea.Width - Logo.Width)/2, PlayArea.Y + (PlayArea.Height - Logo.Height)/2), Color.White);

            // Draw deck
            this.Deck.Draw();
            //if (Deck.Count > 0)
            //    Sprites.Draw(Card.CardBackTexture, null, this.Deck.Area, Card.CardBackColor);

            // Draw discard
            this.Discard.Draw();
            //if (Discard.Count > 0)
            //    Sprites.Draw(Discard[Discard.Count-1].Texture, null, this.Discard.Area, Color.White);

            // Draw hand cards
            this.Hand.Draw();
            //if (Hand.Count > 0)
            //    foreach (Card card in Hand)
            //    {
            //        if (card.Texture == null)
            //            continue;
            //        Sprites.Draw(card.Texture, Vector2.Zero, card.Position, Color.White);
            //    }

            // Draw floating card w/ shadow
            this.Float.Draw();
            
            Sprites.End();

            //// Debugging: Area overlays
            //shapes.Begin(null);
            //shapes.DrawRectangleFill(this.PlayArea, new Color(Color.Pink, 50));
            //shapes.DrawRectangleFill(this.HandArea, new Color(Color.Blue, 50));
            //shapes.DrawRectangleFill(this.DeckArea, new Color(Color.Green, 50));
            //shapes.DrawRectangleFill(this.DiscardArea, new Color(Color.Red, 50));
            //shapes.End();

            Shapes.Begin();
            foreach (var item in this.Animations)
            {
                item.Draw();
            }
            Shapes.End();

            this.Screen.UnSet();
            this.Screen.Present(this.Sprites);

            base.Draw(gameTime);
        }
    }
}
