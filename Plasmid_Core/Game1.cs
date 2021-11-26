
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Plasmid.Microbes;
using Plasmid.Graphics;
using Plasmid.Cards;
using Plasmid.Input;
using Plasmid.UI;
using static Plasmid.UI.Button;

namespace Plasmid
{
    public enum GameState { Start, Battle, Init };
    public class Game1 : Game
    {

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

        public GameState State { get; set; }

        private Splash startSplash;

        private Texture2D BackgroundTexture;
        private Color BackgroundColor;
        private Texture2D HpPanelLeftTexture;
        private Texture2D HpPanelRightTexture;
        private Texture2D HpBarTexture;

        private SpriteFont BattleFont;
        private SpriteFont bitCellFont;

        private Texture2D title;
        private Texture2D dnaStrand;
        private int dnaStrandShift;
        private double dnaStrandStartTime = -1;
        private Texture2D titleBG;
        private Texture2D titleBGshield;

        private Vector2 HpPanelLeftPos = new Vector2(0, 195);
        private Vector2 HpPanelRightPos = new Vector2(125, 386);
        private Vector2 HpBarLeftPos = new Vector2(4, 200);
        private Vector2 HpBarRightPos = new Vector2(131, 391);

        private Vector2 PlayerSpritePos = new Vector2(142, 157);
        private Vector2 EnemySpritePos = new Vector2(5, 70);

        private Microbe microbeA;
        private Microbe microbeB;

        private Panel startPanel;

        private ColorCycler rainbow;

        Battle BattleDemo;
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

            this.State = GameState.Init;

            this.Screen = new Screen(this, 270, 480);
            this.Camera = new Camera(this.Screen);

            this.Touch = new TouchInput();
            this.Touch.Init(this);
            Widget.Init(this);
            Animation.Init(this);
            BaseCard.Init(this);

            this.Sprites = new SpriteBatcher(this);
            this.Shapes = new ShapeBatcher(this);

            ///////////////////////////////////////////////////////////////////

            Hand = new CardHand(new Vector2(20, 70));
            Deck = new CardDeck(new Vector2(31, -48));
            Discard = new CardDeck(new Vector2(175, -48), state: CardState.FaceUp, isDrawAllowed: false, isSearchAllowed: true);
            Float = new FloatCard();

            microbeA = new Microbe(new Vector2(207, 480 - 223));
            microbeB = new Microbe(new Vector2(63, 480 - 127));

            List<Color> colorList = new List<Color>() {
                new Color(179, 0, 0),
                new Color(197, 140, 0),
                new Color(203, 201, 14),
                new Color(132, 187, 0),
                new Color(124, 171, 204),
                new Color(145, 58, 161) };
            rainbow = new ColorCycler(colorList, 100);

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
            BaseCard.Load();
            Card.Load(GraphicsDevice);

            ///////////////////////////////////////////////////////////////////


            BackgroundTexture = Content.Load<Texture2D>("battle_console");
            BackgroundColor = Color.LightCyan;

            HpPanelLeftTexture = Content.Load<Texture2D>("hp_panel_l");
            HpPanelRightTexture = Content.Load<Texture2D>("hp_panel_r");
            HpBarTexture = Content.Load<Texture2D>("hp_bar");

            BattleFont = Content.Load<SpriteFont>("BattleFont");
            bitCellFont = Content.Load<SpriteFont>("CardFont");
            Label.DefaultFont = bitCellFont;

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
            this.Deck.Shuffle();

        }

        protected override void Update(GameTime gameTime)
        {
            if (this.State == GameState.Init)
            {
                this.startSplash = StartScreen();
                this.State = GameState.Start;
                return;
            }

            double gt = gameTime.TotalGameTime.TotalMilliseconds;

            //if (gt > 5000)
            //    this.State = GameState.Battle;

            if (this.State == GameState.Start)
            {
                //if (dnaStrandStartTime == -1)
                //    dnaStrandStartTime = gt;

                //if (gt - dnaStrandStartTime > 50)
                //{
                //    dnaStrandShift -= 1;
                //    if (dnaStrandShift < -(8*41))
                //        dnaStrandShift = 0;
                //    dnaStrandStartTime = gt;
                //}
            }

            this.Touch.HandleTouch(gt);

            Animation.UpdateAll(gt);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            int width = graphics.GraphicsDevice.Viewport.Width;
            int height = graphics.GraphicsDevice.Viewport.Height;

            this.Screen.Set();
            GraphicsDevice.Clear(Color.Black);

            // BATTLE
            if (this.State == GameState.Battle)
            {
                // TODO: implement Transforms for shapes other than polys
                Transform defaultTrans = new Transform(Vector2.Zero, 0f, 1f);
                //Transform playerMicrobeTrans = new Transform(new Vector2(142,193), 0f, 5f);
                Transform playerMicrobeTrans = new Transform(new Vector2(200, 300), 0f, 8f);
                Transform enemyMicrobeTrans = new Transform(new Vector2(0, 288), 0f, 1f);

                Sprites.Begin(null);

                // Draw Background
                Sprites.Draw(BackgroundTexture, Vector2.Zero, Vector2.Zero, BackgroundColor);

                // Draw Microbes
                Shapes.Begin(null);
                microbeA.Draw();
                microbeB.Draw();
                Shapes.End();

                // Draw HP Bars
                Sprites.Draw(HpPanelLeftTexture, Vector2.Zero, HpPanelLeftPos, BackgroundColor);
                Sprites.Draw(HpPanelRightTexture, Vector2.Zero, HpPanelRightPos, BackgroundColor);
                Sprites.End();

                float playerbar = 120f * (BattleDemo.PlayerMicrobe.HP / BattleDemo.PlayerMicrobe.MaxHP);
                float opponentbar = 120f * (BattleDemo.OpponentMicrobe.HP / BattleDemo.OpponentMicrobe.MaxHP);

                Shapes.Begin(null);
                Shapes.DrawRectangleFill(HpBarLeftPos.X + 1, HpBarLeftPos.Y + 3, playerbar, 13, Color.LimeGreen);
                Shapes.DrawRectangleFill(HpBarRightPos.X + 1, HpBarRightPos.Y + 3, opponentbar, 13, Color.LimeGreen);
                Shapes.End();

                Sprites.Begin(null);

                Sprites.Draw(HpBarTexture, Vector2.Zero, HpBarLeftPos, Color.White);
                Sprites.Draw(HpBarTexture, Vector2.Zero, HpBarRightPos, Color.White);

                // Draw deck
                this.Deck.Draw();

                // Draw discard
                this.Discard.Draw();

                // Draw hand cards
                this.Hand.Draw();

                // Draw float card
                this.Float.Draw();

                Sprites.End();

                //// Debugging: Area overlays
                //shapes.Begin(null);
                //shapes.DrawRectangleFill(this.PlayArea, new Color(Color.Pink, 50));
                //shapes.DrawRectangleFill(this.HandArea, new Color(Color.Blue, 50));
                //shapes.DrawRectangleFill(this.DeckArea, new Color(Color.Green, 50));
                //shapes.DrawRectangleFill(this.DiscardArea, new Color(Color.Red, 50));
                //shapes.End();

                //Shapes.Begin();
                //foreach (var item in Animation.All)
                //{
                //    item.Draw();
                //}
                //Shapes.End();

            }

            // START MENU
            else if (this.State == GameState.Start)
            {

                this.Sprites.Begin();

                //Color color = rainbow.Next();

                //this.Sprites.Draw(titleBG, Vector2.Zero, Vector2.Zero, Color.LightGray);
                //this.Sprites.Draw(titleBGshield, Vector2.Zero, Vector2.Zero, color);
                //this.Sprites.Draw(dnaStrand, Vector2.Zero, new Vector2((width - dnaStrand.Width) / 2, dnaStrandShift), Color.White);
                //this.Sprites.Draw(title, Vector2.Zero, new Vector2((width - title.Width)/2, (float)(height * 0.6)), Color.White);
                this.startSplash.DrawBG(Sprites);
                this.startSplash.DrawWidgets();


                this.Sprites.End();

            }
            
            this.Screen.UnSet();
            this.Screen.Present(this.Sprites);

            base.Draw(gameTime);
        }

        private void StartButtonClick(object sender)
        {
            if(((Button)sender).Name == "start")
            {
                this.State = GameState.Battle;
            }
        }

        private Splash StartScreen()
        {
            Splash splash = new Splash(new Rectangle(0, 0, 270, 480));

            splash.AddTexture(Content, "microbe_background", Vector2.Zero, Color.White);
            //splash.AddTexture(Content, "microbe_background_shield", Vector2.Zero, new Color(157, 190, 217));
            //splash.AddTexture(Content, "microbe_background_shield", Vector2.Zero, new Color(71, 153, 198));
            splash.AddTexture(Content, "dna_strand", new Vector2(0, -100), Color.White);
            splash.AddTexture(Content, "title", new Vector2(17, 300), Color.White);

            startPanel = new Panel(new Vector2(128, 64), new Color(99, 155, 255), Alignment.BottomCenter);
            startPanel.SetPadding(50, 50, 50, 50);

            Button startButton = new Button(new Vector2(96, 32), startPanel.Color.ModifyL(.9f), Alignment.Center, "Start", Color.Wheat);
            startButton.Name = "start";

            startButton.Click += new ButtonEventHandler(StartButtonClick);


            startPanel.AddWidget(new Label("Touch to Start!", bitCellFont, Color.Gold, Alignment.TopCenter));
            startPanel.AddWidget(startButton);

            splash.AddWidget(startPanel);
            splash.AlignWidgets();
            splash.IsVisible = true;

            return splash;
        }
    }
}
