using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Plasmid.Graphics
{
    public class Palette
    {
        public Color[,] Colors { get; set; }

        public Palette (Texture2D paletteTexture)
        {
            Color[] colors1D = new Color[paletteTexture.Width * paletteTexture.Height];
            paletteTexture.GetData(colors1D);
            Colors = new Color[paletteTexture.Width, paletteTexture.Height];
            for (int x = 0; x < paletteTexture.Width; x++)
                for (int y = 0; y < paletteTexture.Height; y++)
                    Colors[x, y] = colors1D[x + y * paletteTexture.Width];
        }

        public Color GetColor(int index, int variant=0)
        {
            return Colors[index, variant];
        }
    }
}
