using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using Plasmid.Graphics;

namespace Plasmid.Input
{
    public sealed class TouchInput
    {
        private static readonly Lazy<TouchInput> Lazy = new Lazy<TouchInput>(() => new TouchInput());

        public static TouchInput Instance { get { return Lazy.Value; } }

        private TouchCollection prevTouches;
        private TouchCollection touches;

        private Game1 game;

        private bool isInitialized;

        public TouchInput()
        {
            this.game = null;
            this.prevTouches = TouchPanel.GetState();
            this.touches = prevTouches;
            this.isInitialized = false;
        }

        public void Init(Game1 game)
        {
            this.game = game;
            this.isInitialized = true;
        }

        public void CheckInitialized()
        {
            if (!this.isInitialized)
                throw new Exception("Touchscreen object must be initialized with Touchscreen.Init(Game1 game).");
        }

        public void Update()
        {
            this.CheckInitialized();

            this.prevTouches = this.touches;
            this.touches = TouchPanel.GetState();

        }

        public void HandleTouch(double time)
        {
            this.CheckInitialized();

            this.Update();
            if (touches.Count < 1)
                return;

            Vector2 position = touches[0].ScreenPosition(game.Screen);
            TouchLocationState state = touches[0].State;

#if DEBUG
            // Track touches in debug mode
            this.SetTouchPing(position, state, time);
#endif

            // PRESSED
            if ( state == TouchLocationState.Pressed)
            {
                // DECK
                if (game.Deck.Area.Contains(position))
                    game.Deck.TouchPress(position);
            }

            // MOVED
            if (state == TouchLocationState.Moved)
            {
                // FLOAT
                if (game.Float.IsActive)
                    game.Float.TouchMove(position);
            }

            // RELEASED
            if (state == TouchLocationState.Released)
            {
                // FLOAT
                if (game.Float.IsActive)
                    game.Float.TouchRelease(position);
            }
        }

        private void SetTouchPing(Vector2 position, TouchLocationState state, double time)
        {
            Animation anim = new Animation(AnimationType.SHAPE_SINGLE);
            anim.SetDuration(1000);

            if (state == TouchLocationState.Pressed)
            {
                anim.AddDrawable(new Circle(position, 10f, true, 0f, Color.Magenta));
                anim.AddDrawable(new Circle(position, 10f, false, 2f, Color.DarkMagenta));
                anim.AddDrawable(new Circle(position, 3f, false, 1f, Color.DarkMagenta));
            }
            if (state == TouchLocationState.Moved)
            {
                anim.AddDrawable(new Circle(position, 5f, true, 0f, Color.LimeGreen));
            }
            if (state == TouchLocationState.Released)
            {
                anim.AddDrawable(new Circle(position, 10f, true, 0f, Color.Cyan));
                anim.AddDrawable(new Circle(position, 10f, false, 2f, Color.DarkCyan));
                anim.AddDrawable(new Circle(position, 3f, false, 1f, Color.DarkCyan));
            }

            this.game.Animations.Add(anim);
            this.game.Animations[this.game.Animations.Count - 1].Start(time);
        }
    }

    public static class TouchLocationExtensions
    {
        public static Vector2 WindowPosition(this TouchLocation touch)
        {
            return touch.Position;
        }

        public static Vector2 ScreenPosition(this TouchLocation touch, Screen screen)
        {
            Rectangle screenDestinationRectangle = screen.CalcDestRectangle();
            Vector2 windowPosition = touch.WindowPosition();

            float sx = windowPosition.X - screenDestinationRectangle.X;
            float sy = windowPosition.Y - screenDestinationRectangle.Y;

            sx /= (float)screenDestinationRectangle.Width;
            sy /= (float)screenDestinationRectangle.Height;

            sx *= (float)screen.Width;
            sy *= (float)screen.Height;

            sy = (float)screen.Height - sy;

            return new Vector2(sx, sy);
        }

        public static bool IsPressed(this TouchLocation touch)
        {
            return touch.State == TouchLocationState.Pressed;
        }
        public static bool IsMoved(this TouchLocation touch)
        {
            return touch.State == TouchLocationState.Moved;
        }
        public static bool IsReleased(this TouchLocation touch)
        {
            return touch.State == TouchLocationState.Released;
        }
    }
}

// Old Game1.Update() for reference

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