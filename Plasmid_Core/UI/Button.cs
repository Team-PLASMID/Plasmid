using Microsoft.Xna.Framework;
using Plasmid.Graphics;
using System.Diagnostics;
using Plasmid.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Plasmid.UI
{
    class Button : Widget
    {
        public Label Label { get; set; }
        public Color DepressedColor { get; set; }
        public bool IsDepressed { get => isDepressed; }
        private bool isDepressed;

        public string Name;

        public delegate void ButtonEventHandler(object sender);
        public event ButtonEventHandler Click;
        
        public Button(Vector2 dimensions, Color color, Alignment align, string text, Color textColor)
            : this(dimensions, color, color.ModifyL(1.2f), align, new Label(text, Label.DefaultFont, textColor, Alignment.Center)) { }
        public Button(Vector2 dimensions, Color color, Color depressedColor, Alignment align, Label label) : base(dimensions, align, color)
        {
            this.isDepressed = false;
            this.Label = label;
            this.DepressedColor = depressedColor;

            Widget.Game.Touch.Pressed += new Input.TouchPressedEventHandler(OnButtonPress);
            Widget.Game.Touch.Released += new Input.TouchReleasedEventHandler(OnButtonRelease);
        }

        private void OnButtonPress(object sender, Vector2 position, double gametime)
        {
            if (!this.isDepressed && this.Area.Contains(position))
                this.isDepressed = true;
        }

        private void OnButtonRelease(object sender, Vector2 position, double gametime)
        {
            if (this.isDepressed && this.Area.Contains(position))
            {
                if (Click != null)
                    Click(this);
            }

            this.isDepressed = false;
        }

        public override bool Align(Rectangle area, bool respectPadding=true)
        {
            bool result = base.Align(area);
            this.Label.Align(this.Area);
            return result;
        }

        public override void Draw()
        {
            if (!this.isDepressed)
                base.Draw();
            else
                base.Draw(1, null, this.DepressedColor);

            Label.Draw();
        }

    }
}
