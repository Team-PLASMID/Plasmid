using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace Plasmid_Core
{
    class Card
    {
        public static int Height = 96;
        public static int Width = 64;
        public static Color CardColor = Color.OldLace;
        public static Texture2D CardTexture;
        public string Name { get; }
        public string Text { get; }
        public string Art { get; }
        public RenderTarget2D Sprite { get; set; }
        public int CostA { get; set; }
        public int CostG { get; set; }
        public int CostT { get; set; }
        public int CostC { get; set; }
        public Card Above { get; set; }
        public Card Below { get; set; }

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

        public static void LoadTextures(ContentManager Content)
        {
            CardTexture = Content.Load<Texture2D>("card");
        }

        public void BuildSprite(GraphicsDevice graphics, SpriteBatch sb)
        {
            Sprite = new RenderTarget2D(graphics, CardTexture.Width, CardTexture.Height);
            graphics.SetRenderTarget(Sprite);
            graphics.Clear(Color.Transparent);

            sb.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null, null, Matrix.CreateScale(1));

            //sb.Draw(CardTexture, new Vector2(0, 0), CardColor);

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
