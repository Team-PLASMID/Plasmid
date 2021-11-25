using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Plasmid.UI
{
    class Window : Panel
    {

        public Window(Vector2 dimensions, Color color, Alignment align) : base(dimensions, color, align) { }

        public override void Draw()
        {
            base.Draw(1);
            foreach (Widget widget in this.Widgets)
                widget.Draw();
        }

        public override bool Align(Rectangle area)
        {
            return base.Align(area);
        }

    }
}
