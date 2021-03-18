using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Plasmid_Core
{
    class GraphicWrapper
    {
        private GraphicsDeviceManager Graphics;
        private int BaseHeight;
        private int BaseWidth;

        private Matrix ScaleMatrix;

        public GraphicWrapper(GraphicsDeviceManager graphics, int h, int w )
        {
            Graphics = graphics;
            BaseHeight = h;
            BaseWidth = w;

            // *TODO*
            // Make this calculate scale

            Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            Graphics.IsFullScreen = true;
            Graphics.ApplyChanges();
            ScaleMatrix = Matrix.CreateScale(2);
        }

    }
}
