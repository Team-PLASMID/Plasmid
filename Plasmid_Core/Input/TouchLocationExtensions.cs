using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using Plasmid.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Plasmid.Input
{
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

