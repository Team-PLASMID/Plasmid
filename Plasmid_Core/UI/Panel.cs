#define DEBUG_UI
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Plasmid.UI
{
    public class Panel : Widget
    {
        public List<Widget> Widgets { get; protected set; }

        public Panel(Vector2 dimensions, Color color, Alignment align) : base(dimensions, align, color)
        {
            if (dimensions.X < 24 || dimensions.Y < 24)
                throw new ArgumentException("Panel cannot be smaller than 24x24 pixels.");

            this.Widgets = new List<Widget>();
        }

        public bool CheckFit(Widget widget)
        {
            return CheckFit(widget, this.GetCanvasArea());
        }
        protected bool CheckFit(Widget widget, Rectangle area)
        {
            if (widget.Dim.X > area.Width || widget.Dim.Y > area.Height)
                return false;
            else
                return true;
        }

        public bool AddWidget(Widget widget)
        {
            Rectangle canvas = this.GetCanvasArea();

            if (!this.CheckFit(widget, canvas))
                return false;

            this.Widgets.Add(widget);
            return true;
        }

        public override bool Align(Rectangle area, bool respectPadding=true)
        {
            bool result = base.Align(area);
            this.AlignSubWidgets();
            return result;
        }
        public override void UnAlign()
        {
            base.UnAlign();
            foreach(Widget widget in this.Widgets)
                widget.UnAlign();
        }
        private void AlignSubWidgets()
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

        public Rectangle GetCanvasArea()
        {
            Rectangle area = new Rectangle(
                (int)this.Pos.X + Widget.Tiles.TileSize,
                (int)this.Pos.Y + Widget.Tiles.TileSize,
                (int)this.Dim.X - (2 * Widget.Tiles.TileSize),
                (int)this.Dim.Y - (2 * Widget.Tiles.TileSize) );

            foreach(Widget widget in this.Widgets)
            {
                if (widget.IsAligned)
                {
                    area = Util.RemoveIntersection(area, widget.Footprint, widget.Alignment);
                }
            }

            return area;
        }

        public override void Draw()
        {
#if DEBUG_UI
            Game.Shapes.Begin();
            Game.Shapes.DrawRectangleFill(this.Footprint, new Color(Color.Magenta, .5f));
            Game.Shapes.DrawRectangle(this.Footprint, 1f, Color.DarkMagenta);
            Game.Shapes.End();
#endif
            base.Draw();
            foreach (Widget widget in this.Widgets)
                widget.Draw();
        }


    }
}
