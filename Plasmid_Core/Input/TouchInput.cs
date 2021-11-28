using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using Plasmid.Graphics;

namespace Plasmid.Input
{
    public delegate void TouchEventHandler(object sender, Vector2 position, float gametime);
    public sealed class TouchInput
    {
        private static readonly Lazy<TouchInput> Lazy = new Lazy<TouchInput>(() => new TouchInput());

        public static TouchInput Instance { get { return Lazy.Value; } }

        private TouchCollection prevTouches;
        private TouchCollection touches;

        private Game1 game;

        private bool isInitialized;

        public event TouchEventHandler Pressed;
        public event TouchEventHandler Moved;
        public event TouchEventHandler Released;

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

        public void HandleTouch(float time)
        {
            this.CheckInitialized();

            this.Update();
            if (touches.Count < 1)
                return;

            Vector2 position = touches[0].ScreenPosition(game.Screen);
            TouchLocationState state = touches[0].State;

            // PRESSED
            if (state == TouchLocationState.Pressed)
            {
                if (Pressed != null)
                    Pressed(this, position, time);
            }

            // MOVED
            if (state == TouchLocationState.Moved)
            {
                if (Moved != null)
                    Moved(this, position, time);
            }

            // RELEASED
            if (state == TouchLocationState.Released)
            {
                if (Released != null)
                    Released(this, position, time);
            }
        }

    }
}