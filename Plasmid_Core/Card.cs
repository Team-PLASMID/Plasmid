using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;

namespace Plasmid_Core
{
    class Card
    {
        public static int Height = 96;
        public static int Width = 64;
        public static Color CardColor = Color.OldLace;
        public static Color CardFrameColor = Color.DarkGoldenrod;
        public static Texture2D CardTexture;
        public static Texture2D CardFrameTexture;

        public string Name { get; }
        public string Text { get; }
        public string Art { get; }
        public RenderTarget2D Texture { get; set; }
        public int CostA { get; set; }
        public int CostG { get; set; }
        public int CostT { get; set; }
        public int CostC { get; set; }
        public Card Above { get; set; }
        public Card Below { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public Card(string name, string text, string art, int a, int g, int t, int c)
        {
            Name = name;
            Text = text;
            Art = art;
            CostA = a;
            CostG = g;
            CostT = t;
            CostC = c;

            Above = null;
            Below = null;
        }

        public static void LoadTextures(ContentManager content)
        {
            CardTexture = content.Load<Texture2D>("card");
            CardFrameTexture = content.Load<Texture2D>("card_frame");
        }

        public void BuildTexture(ContentManager content, GraphicsDevice graphics, SpriteBatch sb)
        {
            Debug.WriteLine("\n\n BUILD SPRITE \n\n");
            Texture = new RenderTarget2D(graphics, CardTexture.Width, CardTexture.Height);
            graphics.SetRenderTarget(Texture);
            graphics.Clear(Color.Transparent);

            Texture2D artTexture = content.Load<Texture2D>(Art);

            sb.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null, null, Matrix.CreateScale(1));

            sb.Draw(CardTexture, Vector2.Zero, CardColor);
            sb.Draw(artTexture, new Rectangle(3,3,58,58), Color.White);
            sb.Draw(CardFrameTexture, Vector2.Zero, CardFrameColor);

            // Name
            // Art
            // Frame
            // Description
            // Energy signature
            // etc.

            sb.End();
            graphics.SetRenderTarget(null);
        }

    }


}
