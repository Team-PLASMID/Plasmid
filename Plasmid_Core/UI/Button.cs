using Microsoft.Xna.Framework;
using Plasmid.Graphics;
using System.Diagnostics;
using Plasmid.Input;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Plasmid.UI
{

    public delegate void ButtonEventHandler(object sender);

    public class Button : Widget
    {
        public static int IdSource = 0;
        public Label Label { get; set; }
        public Texture2D Texture { get; set; }
        public Color DepressedColor { get; set; }
        public bool IsDepressed { get; protected set; }
        public bool IsTexture { get;}
        public int ID { get; }

        public event ButtonEventHandler Click;
        
        public Button(Vector2 dimensions, Color color, Vector2 position, Texture2D texture) : this(dimensions, color, color.ModifyL(1.2f), Alignment.None, null)
        {
            this.Position = position;
            this.Texture = texture;
        }
        public Button(Vector2 dimensions, Color color, Alignment align, string text, Color textColor)
            : this(dimensions, color, color.ModifyL(1.2f), align, new Label(text, Game.LabelFont, textColor, Alignment.Center)) { }
        public Button(Vector2 dimensions, Color color, Color depressedColor, Alignment align, Label? label) : base(dimensions, align, color)
        {
            this.IsDepressed = false;

            IsTexture = false;
            if (label == null)
                IsTexture = true;
            else
                this.Label = label;

            this.DepressedColor = depressedColor;

            Game.Touch.Pressed += OnButtonPress;
            Game.Touch.Released += OnButtonRelease;

            ID = IdSource++;
        }

        private void OnButtonPress(object sender, Vector2 position, float gametime)
        {
            //Debug.WriteLine("Press: " + ID);
            if (!this.IsDepressed && this.Area.Contains(position))
                this.IsDepressed = true;
        }

        private void OnButtonRelease(object sender, Vector2 position, float gametime)
        {
            //Debug.WriteLine("Release: " + ID);
            if (this.IsDepressed && this.Area.Contains(position))
            {
                //Debug.WriteLine("Click: " + ID);
                Click?.Invoke(this);
            }

            this.IsDepressed = false;
        }

        public override bool Align(Rectangle area, bool respectPadding=true)
        {
            bool result = base.Align(area);
            this.Label.Align(this.Area);
            return result;
        }

        public override void Draw()
        {
            if (IsTexture)
                DrawTexture();
            else
                DrawLabel();
        }

        private void DrawTexture()
        {
            Game.Sprites.Draw(Texture, Vector2.Zero, Position, Color);
        }

        private void DrawLabel()
        {
            if (!this.IsDepressed)
                base.Draw();
            else
                base.Draw(1, null, this.DepressedColor);

            Label.Draw();
        }

    }
}
