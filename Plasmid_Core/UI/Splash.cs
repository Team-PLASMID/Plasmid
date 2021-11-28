using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Plasmid.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Plasmid.UI
{
    public class Splash
    {
        public Rectangle Area { get; private set; }
        public List<Widget> Widgets { get; private set; }
        public List<Texture2D> BackgroundTextures { get; private set; }
        public List<Vector2> TexturePositions {  get; private set; }
        public List<Color> TextureColors {  get; private set; }

        public bool IsVisible { get; set; }

        public Splash(Rectangle region)
        {
            this.Area = region;
            this.Widgets = new List<Widget>();
            this.BackgroundTextures = new List<Texture2D>();
            this.TexturePositions = new List<Vector2>();
            this.TextureColors = new List<Color>();
            this.IsVisible = false;
        }

        public bool AddWidget(Widget widget)
        {
            Rectangle canvas = this.GetCanvasArea();

            if (!this.CheckFit(widget, canvas))
                return false;

            this.Widgets.Add(widget);

            return true;
        }

        public void AlignWidgets()
        {
            foreach (Widget widget in this.Widgets)
            {
                Rectangle canvas = this.GetCanvasArea();
                if (widget.FillSpace)
                    widget.ManualAlign(canvas);
                else
                    widget.Align(canvas);
            }
        }

        public bool AddTexture(ContentManager content, string filename, Vector2 position, Color color)
        {
            Texture2D texture = content.Load<Texture2D>(filename);
            this.BackgroundTextures.Add(texture);
            this.TexturePositions.Add(position);
            this.TextureColors.Add(color);
            return true;
        }
        public bool CheckFit(Widget widget, Rectangle canvas)
        {
            if (widget.Dimensions.X > canvas.Width || widget.Dimensions.Y > canvas.Height)
                return false;
            else
                return true;
        }

        public Rectangle GetCanvasArea()
        {
            Rectangle area = this.Area;
            foreach (Widget widget in this.Widgets)
            {
                if (widget.IsAligned)
                {
                    area = Util.RemoveIntersection(area, widget.Footprint, widget.Alignment);
                }
            }
            return area;
        }

        public void Update(double gametime)
        {
            throw new NotImplementedException();
        }

        public void DrawBG(SpriteBatcher sprites)
        {
            if (this.BackgroundTextures.Count != this.TexturePositions.Count ||
                this.BackgroundTextures.Count != this.TextureColors.Count)
                throw new Exception("Mismatched textures and positions in Splash.");

            for (int i = 0; i < this.BackgroundTextures.Count; i++)
                sprites.Draw(
                    BackgroundTextures[i],
                    Vector2.Zero,
                    TexturePositions[i],
                    TextureColors[i]);
        }

        public void DrawWidgets()
        {
            foreach (Widget widget in this.Widgets)
                widget.Draw();
        }
    }
}
