
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
using Microsoft.Xna.Framework.Input;
using System.Xml;
using System.IO;

namespace Plasmid
{
    public enum GameState { Start, Battle, Init };
    public class Game1 : Game
    {

        private GraphicsDeviceManager graphics;


        public GameState State { get; set; }
        public Screen Screen { get; set; }
        public Camera Camera { get; set; }
        public TouchInput Touch { get; set; }
        public ShapeBatcher Shapes { get; set; }
        public SpriteBatcher Sprites { get; set; }

        public CardDeck Deck { get; set; }
        public CardDeck Discard { get; set; }
        public CardHand Hand { get; set; }
        public FloatCard Float { get; set; }
        public Palette ColorPalette { get; set; }

        private Splash startSplash;

        private Texture2D BackgroundTexture;
        private Color BackgroundColor;
        private Texture2D HpPanelLeftTexture;
        private Texture2D HpPanelRightTexture;
        private Texture2D HpBarTexture;

        public SpriteFont LabelFont { get; private set; }
        public SpriteFont MessageFont { get; private set; }
        public SpriteFont CardFont { get; private set; }
        public SpriteFont CardTitleFont { get; private set; }

        private Vector2 HpPanelLeftPos = new Vector2(0, 195);
        private Vector2 HpPanelRightPos = new Vector2(125, 386);
        private Vector2 HpBarLeftPos = new Vector2(4, 200);
        private Vector2 HpBarRightPos = new Vector2(131, 391);


        private Microbe microbeA;
        private Microbe microbeB;

        private Panel startPanel;

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
            //Animation.Init(this);
            BaseCard.Init(this);

            this.Sprites = new SpriteBatcher(this);
            this.Shapes = new ShapeBatcher(this);

            ///////////////////////////////////////////////////////////////////

            //Hand = new CardHand(new Vector2(20, 70));
            Hand = new CardHand(20, 102, 230, 70);
            //Deck = new CardDeck(new Vector2(31, -48));
            Deck = new CardDeck(new Vector2(2, -16));
            //Discard = new CardDeck(new Vector2(175, -48), state: CardState.FaceUp, isDrawAllowed: false, isSearchAllowed: true);
            Discard = new CardDeck(new Vector2(204, -16), state: CardState.FaceUp, isDrawAllowed: false, isSearchAllowed: true);
            
            Float = new FloatCard();


            microbeA = new Microbe(new Vector2(207, 249));
            microbeB = new Microbe(new Vector2(52, 359));

            //List<Color> colorList = new List<Color>() {
            //    new Color(179, 0, 0),
            //    new Color(197, 140, 0),
            //    new Color(203, 201, 14),
            //    new Color(132, 187, 0),
            //    new Color(124, 171, 204),
            //    new Color(145, 58, 161) };
            //rainbow = new ColorCycler(colorList, 100);

            BattleDemo = new Battle(this, microbeA, microbeB);


            base.Initialize();

        }

        protected override void LoadContent()
        {
            ColorPalette = new Palette(Content.Load<Texture2D>("color_palette"));

            DnaExtensions.Load(this);

            LabelFont = Content.Load<SpriteFont>("LabelFont");
            MessageFont = Content.Load<SpriteFont>("MessageFont");
            CardFont = Content.Load<SpriteFont>("CardFont");
            CardTitleFont = Content.Load<SpriteFont>("CardTitleFont");

            BaseCard.Load();
            Card.Load(GraphicsDevice);

            ///////////////////////////////////////////////////////////////////


            // Demo Deck

            DnaRandom rand = new DnaRandom(microbeA.Genome);
            for (int i = 0; i < 30; i++)
            {
                int index = rand.Next(Card.All.Count - 1);
                Deck.Add(Card.Copy(Card.All[index].Name));
            }

            // Shuffle Demo Deck
            this.Deck.Shuffle();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Launch start screen
            if (this.State == GameState.Init)
            {
                if (!(this.Sprites is null) && !(this.Shapes is null))
                {
                    this.startSplash = StartScreen();
                    this.State = GameState.Start;
                    return;
                }
            }

            float gt = (float)gameTime.TotalGameTime.TotalMilliseconds;

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

            if (this.State == GameState.Battle)
            {
                BattleDemo.Update(gt);
                MoveCard.UpdateAll(gt);
            }
          

            this.Touch.HandleTouch(gt);

            //Animation.UpdateAll(gt);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            int width = graphics.GraphicsDevice.Viewport.Width;
            int height = graphics.GraphicsDevice.Viewport.Height;

            this.Screen.Set();

            // BATTLE
            if (this.State == GameState.Battle)
            {
                BattleDemo.Draw();
            }

            // Start Screen
            else if (this.State == GameState.Start)
            {

                GraphicsDevice.Clear(Color.Black);
                this.Sprites.Begin();

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
                this.State = GameState.Battle;
        }

        private Splash StartScreen()
        {
            Splash splash = new Splash(new Rectangle(0, 0, 270, 480));

            splash.AddTexture(Content, "microbe_background", Vector2.Zero, Color.White);
            //splash.AddTexture(Content, "microbe_background_shield", Vector2.Zero, new Color(157, 190, 217));
            //splash.AddTexture(Content, "microbe_background_shield", Vector2.Zero, new Color(71, 153, 198));
            splash.AddTexture(Content, "start_dna", new Vector2(0, -100), Color.White);
            splash.AddTexture(Content, "title", new Vector2(17, 300), Color.White);

            startPanel = new Panel(new Vector2(128, 64), new Color(99, 155, 255), Alignment.BottomCenter);
            startPanel.SetPadding(50, 50, 50, 50);

            Button startButton = new Button(new Vector2(96, 32), startPanel.Color.ModifyL(.9f), Alignment.Center, "Start", Color.Wheat);

            startButton.Click += StartButtonClick;


            //startPanel.AddWidget(new Label("Touch to Start!", cardFont, Color.Gold, Alignment.TopCenter));
            startPanel.AddWidget(startButton);

            splash.AddWidget(startPanel);
            splash.AlignWidgets();
            splash.IsVisible = true;

            return splash;
        }
    }
}
