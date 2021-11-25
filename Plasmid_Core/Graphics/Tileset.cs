using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Plasmid.Graphics
{
    public struct Tileset
    {
        public Texture2D Texture { get => this.texture; }
        private Texture2D texture;

        public int TileSize { get => this.tileSize; }
        private int tileSize;

        public Tileset(ContentManager content, string filename, int tileSize)
        {
            this.texture = content.Load<Texture2D>(filename);
            if (this.texture.Width % tileSize != 0 || this.texture.Height % tileSize != 0)
                throw new ArgumentException("tileSize doesn't match image dimensions.");

            this.tileSize = tileSize;
        }

        public Rectangle GetTileRectangle(int index)
        {
            int x = (tileSize * index) % this.texture.Width;
            int y = tileSize * ((tileSize * index) / this.texture.Width);

            return new Rectangle(x, y, this.tileSize, this.tileSize);
        }


    }
}
