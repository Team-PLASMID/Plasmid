using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;

namespace Plasmid.Graphics
{
    public sealed class Touchscreen
    {
        private static readonly Lazy<Touchscreen> Lazy = new Lazy<Touchscreen>(() => new Touchscreen());

        public static Touchscreen Instance { get { return Lazy.Value; } }

        private TouchCollection prevTouchState;
        private TouchCollection currTouchState;

        public Touchscreen()
        {
            this.prevTouchState = TouchPanel.GetState();
            this.currTouchState = prevTouchState;
        }

        public void Update()
        {
            this.prevTouchState = this.currTouchState;
            this.currTouchState = TouchPanel.GetState();
        }

        // catch press/move
        public bool IsTouched(Screen? screen, Rectangle region)
        {
            if (currTouchState.Count < 1)
                return false;

            if (screen is null)
            {
                foreach (TouchLocation touch in currTouchState)
                    if (!touch.IsReleased() && region.Contains(touch.WindowPosition()))
                        return true;
            }
            else
                foreach (TouchLocation touch in currTouchState)
                    if (!touch.IsReleased() && region.Contains(touch.ScreenPosition(screen)))
                        return true;

            return false;
        }

        // catch press
        public bool IsPressed(Screen? screen, Rectangle region)
        {
            if (currTouchState.Count < 1)
                return false;

            if (screen is null)
            {
                foreach (TouchLocation touch in currTouchState)
                    if (touch.IsPressed() && region.Contains(touch.WindowPosition()))
                        return true;
            }
            else
                foreach (TouchLocation touch in currTouchState)
                    if (touch.IsPressed() && region.Contains(touch.ScreenPosition(screen)))
                        return true;

            return false;
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
