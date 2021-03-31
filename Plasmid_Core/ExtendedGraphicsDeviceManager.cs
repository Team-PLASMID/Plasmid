using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace Plasmid_Core
{
    class ExtendedGraphicsDeviceManager : GraphicsDeviceManager
    {
        public int BaseHeight { get; set; }
        public int BaseWidth { get; set; }
        public Matrix Transform { get; set; }


        public bool IsHorizontal() { return (BaseWidth > BaseHeight); }
        public bool IsVertical() { return !IsHorizontal(); }

        public ExtendedGraphicsDeviceManager(Game game, int h, int w ) : base(game)
        {
            BaseHeight = h;
            BaseWidth = w;
        }

        public void CalculateTransformation()
        {
            // Calculate Scale
            float scale = Math.Min((float)PreferredBackBufferHeight / BaseHeight, (float)PreferredBackBufferWidth / BaseWidth);
            int transDown = (int)((PreferredBackBufferHeight - (scale * BaseHeight)) / 2);
            int transRight = (int)((PreferredBackBufferWidth - (scale * BaseWidth)) / 2);

            /*
            Debug.WriteLine("SCALING");
            Debug.WriteLine(PreferredBackBufferHeight);
            Debug.WriteLine(PreferredBackBufferWidth);
            Debug.WriteLine(scale);
            Debug.WriteLine(transDown);
            Debug.WriteLine(transRight);
            */

            Transform = Matrix.CreateScale(scale) * Matrix.CreateTranslation(transRight, transDown, 0);

        }


    }
}
