using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Plasmid.Cards;
using Plasmid.Graphics;
using Plasmid.Microbes;
using Plasmid.UI;

namespace Plasmid
{
    public enum BattlePhase { None, Load, Setup, Start, Pause, PlayerTurn, EnemyTurn, Victory, Defeat}
    class Battle
    {
        public static int colorcounter = 0;
        public Game1 Game { get; set; }

        public BattlePhase Phase { get; private set; } = BattlePhase.Load;
        public BattlePhase PrePause { get; private set; }
        public Microbe Player { get; private set; }
        public Microbe Enemy { get; private set; }
        public int APool { get; private set; }
        public int GPool { get; private set; }
        public int TPool { get; private set; }
        public int CPool { get; private set; }
        public string MessageText { get; set; } = "Get ready to fight!";
        public Vector2 MessagePosition { get; set; } = new Vector2(24, 437);

        private Texture2D background;
        private Texture2D console;
        private Texture2D messageScreen;
        private Texture2D microscope;
        private Texture2D cutout;
        private Texture2D hpPanelA;
        private Texture2D hpPanelB;
        private Texture2D hpOverlay;
        private Texture2D energyOverlay;
        private Texture2D eyepiece;
        private Texture2D pauseButton;
        private Texture2D dnaStrand;

        public Button PauseButton { get; private set; }

        public DnaCycler SourceDna { get; private set; }
        public List<Dna> VisibleDna { get; private set; }
        public int DnaOffset { get; private set; }
        public bool DnaRotating { get; private set; }
        public bool DnaOffPosition { get; private set; }
        public float PreviousTime { get; private set; }

        private Vector2 hpPanelAPosition = new Vector2(0, 197);
        private Vector2 hpPanelBPosition = new Vector2(125, 369);

        private Color backgroundColor;
        private Color consoleColor;
        private Color messageScreenColor;
        private Color messageTextColor;


        public MoveAnimator playerAnimator { get; private set; }
        public MoveAnimator enemyAnimator { get; private set; }
        public Transform playerTransform { get; set; }
        public Transform enemyTransform { get; set; }

        public Random Rand;

        public Battle(Game1 game, Microbe player, Microbe opponent)
        {
            Game = game;
            Player = player;
            Enemy = opponent;


            VisibleDna = new List<Dna>();
            for (int i = 0; i < 17; i++)
                VisibleDna.Add(Dna.None);
            DnaOffset = 0;
            DnaOffPosition = false;

            Rand = new Random();
            Phase = BattlePhase.Load;

        }


        public void Load()
        {
            if (Phase < BattlePhase.Load)
                return;


            this.backgroundColor = Game.ColorPalette.GetColor(14);
            this.consoleColor = Game.ColorPalette.GetColor(14);
            this.messageScreenColor = Game.ColorPalette.GetColor(15, 3);
            this.messageTextColor = Game.ColorPalette.GetColor(3, 1);

            this.background = new Texture2D(this.Game.GraphicsDevice, 1, 1);
            background.SetData(new[] { backgroundColor });

            this.console = Game.Content.Load<Texture2D>("battle_console");
            this.microscope = Game.Content.Load<Texture2D>("microscope");
            this.cutout = Game.Content.Load<Texture2D>("cutout");
            this.messageScreen = Game.Content.Load<Texture2D>("message_screen");
            this.hpPanelA = Game.Content.Load<Texture2D>("hp_panel_a");
            this.hpPanelB = Game.Content.Load<Texture2D>("hp_panel_b");
            this.hpOverlay = Game.Content.Load<Texture2D>("hp_overlay");
            this.energyOverlay = Game.Content.Load<Texture2D>("energy_overlay");
            this.eyepiece = Game.Content.Load<Texture2D>("eyepiece");
            this.pauseButton = Game.Content.Load<Texture2D>("pause_button");
            this.dnaStrand = Game.Content.Load<Texture2D>("dna_strand");

            this.Phase = BattlePhase.Setup;
        }

        public void Setup()
        {
            if (Phase < BattlePhase.Setup)
                return;

            PauseButton = new Button(new Vector2(32, 32), consoleColor, new Vector2(230, 433), pauseButton);
            PauseButton.Click += PauseClick;

            SourceDna = new DnaCycler(Player.Genome);
            VisibleDna.Add(SourceDna.Next());

            playerAnimator = new MoveAnimator(new Vector2(207, 249), new Rectangle(179, 480 - 254, 64, 64), Rand);
            enemyAnimator = new MoveAnimator(new Vector2(62, 360), new Rectangle(27, 480 - 160, 64, 64), Rand);

            APool = 3;
            GPool = 6;
            TPool = 4;
            CPool = 2;

            Phase = BattlePhase.Start;
        }

        public void Update(float gametime)
        {
            if (this.Phase == BattlePhase.Load)
                this.Load();
            if (this.Phase == BattlePhase.Setup)
                this.Setup();


            if (this.Phase < BattlePhase.Start || this.Phase == BattlePhase.Pause)
                return;


            if (DnaRotating == true)
            {
                if (DnaOffset == 16)
                {
                    VisibleDna.RemoveAt(0);
                    DnaOffset = 0;
                    DnaRotating = false;
                    DnaOffPosition = true;
                }
                else
                    DnaOffset++;
            }
            else if (gametime - PreviousTime > 1000)
            {
                VisibleDna.Add(SourceDna.Next());
                DnaRotating = true;
                PreviousTime = gametime;
            }

            playerAnimator.Update();
            Player.Position = playerAnimator.Position;
            //playerTransform = Transform.Scale(Vector2.Lerp(new Vector2(.9f, .9f), new Vector2(1.1f, 1.1f),
            //    playerAnimator.Velocity.Length()/playerAnimator.MaxVelocity.Length()));

            enemyAnimator.Update();
            Enemy.Position = enemyAnimator.Position;
            //enemyTransform = Transform.Scale(Vector2.Lerp(new Vector2(.9f, .9f), new Vector2(1.1f, 1.1f),
            //    enemyAnimator.Velocity.Length() / enemyAnimator.MaxVelocity.Length()));


        }

        #region Gameplay/Rules
        public void PlayCard(Card card)
        {
            foreach (CardEffect effect in card.CardEffects)
                switch(effect.Type)
                {
                    //case EffectType.Block:
                    case CardEffectType.Damage:
                        if (this.Phase == BattlePhase.PlayerTurn)
                            Damage(Enemy, effect.Value);
                        else
                            Damage(Player, effect.Value);
                        break;
                }

        }
        public void Damage(Microbe microbe, int amount)
        {
            //Debug.Write(amount + " damage. " + microbe.HP + " -> ");
            //microbe.hp -= amount;
            //if (microbe.hp < 0)
            //    microbe.hp = 0;
            //Debug.WriteLine(microbe.HP);
        }
        #endregion

        private void PauseClick(object sender)
        {
            if (Phase > BattlePhase.Setup)
            {
                if (Phase != BattlePhase.Pause)
                {
                    PrePause = Phase;
                    Phase = BattlePhase.Pause;
                }
                else
                    Phase = PrePause;
            }
        }

        private void RotateDna(Dna dna)
        {
            VisibleDna.RemoveAt(0);
            VisibleDna.Add(dna);
        }

        #region Draw Functions
        public void Draw()
        {
            if (this.Phase < BattlePhase.Start)
                return;

            // pre-emptively draw magnified zone to rendertarget
            Game.Screen.UnSet();
            RenderTarget2D mag = new RenderTarget2D(Game.GraphicsDevice, 44, 44, false, SurfaceFormat.Color, DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents);
            Game.GraphicsDevice.SetRenderTarget(mag);
            Game.GraphicsDevice.Clear(new Color(144, 172, 228));
            Game.Sprites.Begin();


            //Game.Sprites.Draw(dnaStrand, null, new Rectangle(16 * i - DnaOffset + 6, 76, 16, dnaStrand.Height), Color.White);
            //Game.Sprites.Draw(DnaExtensions.Texture, VisibleDna[i].GetRect(),
            //        new Rectangle(16 * i - DnaOffset - 1, 69, 16, 16), VisibleDna[i].GetColor());


            if (VisibleDna[7] != Dna.None)
            {
                Game.Sprites.Draw(dnaStrand, null, new Rectangle(8-32 - 2*DnaOffset + 6, 20, 32, 2 * dnaStrand.Height), Color.White);
                Game.Sprites.Draw(DnaExtensions.Texture, VisibleDna[7].GetRect(), new Rectangle(8 - 32 - 2*DnaOffset - 1, 6, 32, 32), VisibleDna[7].GetColor());
            }

            if (VisibleDna[8] != Dna.None)
            {
                Game.Sprites.Draw(dnaStrand, null, new Rectangle(8 - 2*DnaOffset + 6, 20, 32, 2 * dnaStrand.Height), Color.White);
                Game.Sprites.Draw(DnaExtensions.Texture, VisibleDna[8].GetRect(), new Rectangle(8 - 2*DnaOffset - 1, 6, 32, 32), VisibleDna[8].GetColor());
            }

            if (VisibleDna[9] != Dna.None)
            {
                Game.Sprites.Draw(dnaStrand, null, new Rectangle(8+32 - 2*DnaOffset + 6, 20, 32, 2 * dnaStrand.Height), Color.White);
                Game.Sprites.Draw(DnaExtensions.Texture, VisibleDna[9].GetRect(), new Rectangle(8 + 32 - 2*DnaOffset - 1, 6, 32, 32), VisibleDna[9].GetColor());
            }

            Game.Sprites.End();
            Game.GraphicsDevice.SetRenderTarget((RenderTarget2D)null);
            Game.Screen.Set();


            //Game.GraphicsDevice.Clear(Color.Black);

            Game.Sprites.Begin();

            // Draw Background
            Game.Sprites.Draw(this.background, null, Game.Screen.Area, this.backgroundColor);


            // Draw Microbes
            Game.Shapes.Begin(null);
            //this.Player.Draw(Game, playerTransform);
            //this.Enemy.Draw(Game, enemyTransform);
            this.Player.Draw(Game);
            this.Enemy.Draw(Game);
            Game.Shapes.End();

            // microscope layer
            Game.Sprites.Draw(microscope, Vector2.Zero, new Vector2(0, 192), Color.White);

            // Draw Console
            Game.Sprites.Draw(this.cutout, Vector2.Zero, Vector2.Zero, Color.White);
            Game.Sprites.Draw(this.console, Vector2.Zero, Vector2.Zero, this.consoleColor);

            // Draw Message
            Game.Sprites.Draw(messageScreen, Vector2.Zero, new Vector2(15, 433), messageScreenColor);
            //Game.Sprites.DrawString(Card.Font, MessageText, new Vector2(19, 437), Game.ColorPalette.GetColor(3,2));

            
            //Vector2 position = new Vector2(15, 433);
            float scale = 1f;
            Color backColor = Color.Black;
            Color frontColor = messageTextColor;

            Game.Sprites.DrawString(Game.MessageFont, MessageText, MessagePosition + new Vector2(1 * scale, 1 * scale), backColor, scale);
            Game.Sprites.DrawString(Game.MessageFont, MessageText, MessagePosition + new Vector2(-1 * scale, 1 * scale), backColor, scale);
            Game.Sprites.DrawString(Game.MessageFont, MessageText, MessagePosition + new Vector2(-1 * scale, -1 * scale), backColor, scale);
            Game.Sprites.DrawString(Game.MessageFont, MessageText, MessagePosition + new Vector2(1 * scale, -1 * scale), backColor, scale); 
            Game.Sprites.DrawString(Game.MessageFont, MessageText, MessagePosition, frontColor, scale);
            Game.Sprites.DrawString(Game.MessageFont, MessageText, MessagePosition, frontColor, scale); 

            // Pause Button
            PauseButton.Draw();

            // Draw HP Bar panels
            Game.Sprites.Draw(this.hpPanelA, Vector2.Zero, this.hpPanelAPosition, this.consoleColor);
            Game.Sprites.Draw(this.hpPanelB, Vector2.Zero, this.hpPanelBPosition, this.consoleColor);

            Game.Sprites.End();

            // Draw HP Bars
            float playerBar = 120f * (Player.HP / Player.MaxHP);
            float enemyBar = 120f * (Enemy.HP / Enemy.MaxHP);

            Game.Shapes.Begin(null);

            Game.Shapes.DrawRectangleFill(hpPanelAPosition.X + 6, hpPanelAPosition.Y + 11, 1, 7, Color.LimeGreen);
            Game.Shapes.DrawRectangleFill(hpPanelBPosition.X + 10, hpPanelBPosition.Y + 28, 1, 7, Color.LimeGreen);
            Game.Shapes.DrawRectangleFill(hpPanelAPosition.X + 7, hpPanelAPosition.Y + 8, playerBar, 13, Color.LimeGreen);
            Game.Shapes.DrawRectangleFill(hpPanelBPosition.X + 11, hpPanelBPosition.Y + 25, enemyBar, 13, Color.LimeGreen);

          
            // Draw energy bars
            DrawEnergyBars();

            Game.Shapes.End();

            Game.Sprites.Begin(null);

            // Draw HP bar overlays
            Game.Sprites.Draw(hpOverlay, Vector2.Zero, new Vector2(hpPanelAPosition.X + 5, hpPanelAPosition.Y + 4), Color.White);
            Game.Sprites.Draw(hpOverlay, Vector2.Zero, new Vector2(hpPanelBPosition.X + 9, hpPanelBPosition.Y + 21), Color.White);

            // Draw energy bar overlays
            Game.Sprites.Draw(energyOverlay, Vector2.Zero, new Vector2(97, 4), Color.White);

            // Draw deck
            Game.Deck.Draw();

            // Draw discard
            Game.Discard.Draw();

            // Draw hand cards
            Game.Hand.Draw();

            // Draw DNA strand
            // (129,69), (129+16,69), etc
            DrawDna();


            Game.Sprites.Draw(mag, Vector2.Zero, new Vector2(113, 55), Color.White);

            // eyepiece?
            Game.Sprites.Draw(eyepiece, Vector2.Zero, new Vector2(102, 45), Color.White);

            // Draw cards in motion
            MoveCard.DrawAll();


                

            // Draw Float
            Game.Float.Draw();

            Game.Sprites.End();
                
            if (this.Phase == BattlePhase.Pause)
            {
                Game.Shapes.Begin();
                Game.Shapes.DrawRectangleFill(Game.Screen.Area, new Color(0f, 0f, 0f, .7f));
                Game.Shapes.End();
            }

            //// Debugging: Area overlays
            //Game.Shapes.Begin(null);
            //Game.Shapes.DrawRectangleFill(this.PlayArea, new Color(Color.Pink, 50));
            //Game.Shapes.DrawRectangleFill(this.HandArea, new Color(Color.Blue, 50));
            //Game.Shapes.DrawRectangleFill(this.DeckArea, new Color(Color.Green, 50));
            //Game.Shapes.DrawRectangleFill(this.DiscardArea, new Color(Color.Red, 50));
            //Game.Shapes.End();

            //Game.Shapes.Begin();
            //foreach (var item in Animation.All)
            //{
            //    item.Draw();
            //}
            //Game.Shapes.End();
             

            

        }
        private void DrawEnergyBars()
        {
            Vector2 positionA = new Vector2(98, 5);
            Vector2 positionG = new Vector2(118, 5);
            Vector2 positionT = new Vector2(138, 5);
            Vector2 positionC = new Vector2(158, 5);

            if (APool > 0)
                Game.Shapes.DrawRectangleFill(new Rectangle((int)positionA.X, (int)positionA.Y, 14, 4 * (APool - 1)), Dna.A.GetColor());
            if (GPool > 0)
                Game.Shapes.DrawRectangleFill(new Rectangle((int)positionG.X, (int)positionG.Y, 14, 4 * (GPool - 1)), Dna.G.GetColor());
            if (TPool > 0)
                Game.Shapes.DrawRectangleFill(new Rectangle((int)positionT.X, (int)positionT.Y, 14, 4 * (TPool - 1)), Dna.T.GetColor());
            if (CPool > 0)
                Game.Shapes.DrawRectangleFill(new Rectangle((int)positionC.X, (int)positionC.Y, 14, 4 * (CPool - 1)), Dna.C.GetColor());

        }
        private void DrawDna()
        {
            //int offset = DnaOffset;
            //if (DnaOffPosition)
            //    offset -= 16;
            for (int i = 0; i < VisibleDna.Count; i++)
            {
                if (VisibleDna[i] == Dna.None)
                    continue;

                Game.Sprites.Draw(dnaStrand, null, new Rectangle(16 * i - DnaOffset + 6, 76, 16, dnaStrand.Height), Color.White);
                Game.Sprites.Draw(DnaExtensions.Texture, VisibleDna[i].GetRect(),
                    new Rectangle(16 * i - DnaOffset - 1, 69, 16, 16), VisibleDna[i].GetColor());
            }

        }


        #endregion
    }
}
