using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using Plasmid.Graphics;

namespace Plasmid.UI
{
    public class Padding
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public Padding() : this(0, 0, 0, 0) { }
        public Padding(int left, int top, int right, int bottom)
        {
            this.Left = left;
            this.Top = top;
            this.Right = right;
            this.Bottom = bottom;
        }
        public bool IsEmpty()
        {
            if (Left == 0 && Top == 0 && Right == 0 && Bottom == 0)
                return true;
            else
                return false;
        }
    }
    public enum Alignment
    {
        TopLeft, TopCenter, TopRight,
        Left, Center, Right,
        BottomLeft, BottomCenter, BottomRight, None
    }
    public abstract class Widget
    {
        public static Tileset Tiles;
        public static Game1 Game;

        private static bool isInitialized = false;

        public Rectangle Area { get => new Rectangle((int)Position.X, (int)Position.Y, (int)Dimensions.X, (int)Dimensions.Y); }
        public Rectangle Footprint { get => new Rectangle(
                                        (int)(Position.X - Pad.Left),
                                        (int)(Position.Y - Pad.Bottom),
                                        (int)(Dimensions.X + Pad.Left + Pad.Right),
                                        (int)(Dimensions.Y + Pad.Bottom + Pad.Top)); }
        public Vector2 Position { get; protected set; }
        public Vector2 Dimensions { get; protected set; }
        public Padding Pad { get; protected set; }
        public Alignment Alignment { get; set; }
        public Color Color { get; set; }

        public bool FillSpace { get; protected set; }
        public bool IsAligned { get; protected set; }
        public bool IsVisible { get; set; }
        
        // Auto-fill available space
        public Widget(Alignment alignment, Color color)
        {
            Widget.CheckInit();

            this.Position = Vector2.Zero;
            this.Dimensions = Vector2.Zero;
            this.Pad = new Padding();
            this.Alignment = alignment;
            this.Color = color;
            this.FillSpace = true;
            this.IsAligned = false;
            this.IsVisible = true;
        }
        // Fixed size
        public Widget(Vector2 dimensions, Alignment alignment, Color color, Vector2? position = null)
        {
            Widget.CheckInit();

            this.Position = position ?? Vector2.Zero;
            this.Dimensions = dimensions;
            this.Pad = new Padding();
            this.Alignment = alignment;
            this.Color = color;
            this.FillSpace = false;
            this.IsAligned = false;
            this.IsVisible = true;
        }

        public static void Init(Game1 game)
        {
            Widget.Game = game;
            Widget.Tiles = new Tileset(Widget.Game.Content, "panel_parts", 8);
            Widget.isInitialized = true;
        }
        public static void CheckInit()
        {
            if (!Widget.isInitialized)
                throw new Exception("Call Widget.Init() before using Widgets.");
        }

        public virtual void Draw()
        {
            this.Draw(0, null, null);
        }
        public void Draw(int type = 0, Vector2? positionOverride=null, Color? colorOverride=null)
        {
            if (this.Alignment != Alignment.None && !this.IsAligned)
                return;
            //if (this.Alignment != Alignment.None && !this.IsAligned)
            //    throw new Exception("Widget with specified alignment must be aligned to container before drawing.");

            if (!this.IsVisible)
                return;

            Vector2 position = positionOverride ?? this.Position;
            Color color = colorOverride ?? this.Color;

            int x = (int)(this.Dimensions.X - 1);
            int y = (int)(this.Dimensions.Y - 1);
            int ts = Widget.Tiles.TileSize;

            // CORNERS:
            // Top Left
            Widget.Game.Sprites.Draw(Widget.Tiles.Texture, Widget.Tiles.GetTileRectangle(0 + (type * 3)),
                new Rectangle((int)position.X, (int)position.Y + (y - ts), ts, ts), color);
            // Top Right
            Widget.Game.Sprites.Draw(Widget.Tiles.Texture, Widget.Tiles.GetTileRectangle(2 + (type * 3)),
                new Rectangle((int)position.X + (x - ts), (int)position.Y + (y - ts), ts, ts), color);
            // Bottom Left
            Widget.Game.Sprites.Draw(Widget.Tiles.Texture, Widget.Tiles.GetTileRectangle(24 + (type * 3)),
                new Rectangle((int)position.X, (int)position.Y, ts, ts), color);
            // Bottom Right
            Widget.Game.Sprites.Draw(Widget.Tiles.Texture, Widget.Tiles.GetTileRectangle(26 + (type * 3)),
                new Rectangle((int)position.X + (x - ts), (int)position.Y, ts, ts), color);

            // EDGES:
            // Top
            Widget.Game.Sprites.Draw(Widget.Tiles.Texture, Widget.Tiles.GetTileRectangle(1 + (type * 3)),
                new Rectangle((int)position.X + ts, (int)position.Y + y - ts, x - 2 * ts, ts), color);
            // Left
            Widget.Game.Sprites.Draw(Widget.Tiles.Texture, Widget.Tiles.GetTileRectangle(12 + (type * 3)),
                new Rectangle((int)position.X, (int)position.Y + ts, ts, y - 2 * ts), color);
            // Right
            Widget.Game.Sprites.Draw(Widget.Tiles.Texture, Widget.Tiles.GetTileRectangle(14 + (type * 3)),
                new Rectangle((int)position.X + x - ts, (int)position.Y + ts, ts, y - 2 * ts), color);
            // Bottom
            Widget.Game.Sprites.Draw(Widget.Tiles.Texture, Widget.Tiles.GetTileRectangle(25 + (type * 3)),
                new Rectangle((int)position.X + ts, (int)position.Y, x - 2 * ts, ts), color);

            // CENTER
            Widget.Game.Sprites.Draw(Widget.Tiles.Texture, Widget.Tiles.GetTileRectangle(13 + (type * 3)),
                new Rectangle((int)position.X + ts, (int)position.Y + ts, x - 2 * ts, y - 2 * ts), color);
        }

        public void SetPadding(int left, int top, int right, int bottom)
        {
            this.SetPadding(new Padding(left, top, right, bottom));
        }
        public void SetPadding(Padding padding)
        {
            this.Pad = padding;
        }

        public bool ManualAlign(Rectangle alignment)
        {
            this.Position = new Vector2(alignment.X, alignment.Y);
            this.Dimensions = new Vector2(alignment.Width, alignment.Height);

            return true;
        }
        public virtual bool Align(Rectangle canvas, bool respectPadding=true)
        {
            Rectangle rect;
            if (!respectPadding || this.Pad.IsEmpty())
                rect = this.Area;
            else
                rect = this.Footprint;

            if (Util.Align(rect, canvas, this.Alignment, out Vector2 position))
            {
                if (!respectPadding || this.Pad.IsEmpty())
                    this.Position = position;
                else
                    this.Position = new Vector2(position.X + this.Pad.Left, position.Y + this.Pad.Bottom);
                this.IsAligned = true;
                if (this.Dimensions == Vector2.Zero && this.Position == Vector2.Zero)
                    throw new Exception("Widget aligned, but with zeroed out dimensions." +
                        "\nDid you forget to add an auto-filling widget to a parent?");
                return true;
            }
            else
            {
                this.IsAligned = false;
                return false;
            }
        }
        public virtual void UnAlign()
        {
            this.IsAligned = false;
        }
        
    }
}
