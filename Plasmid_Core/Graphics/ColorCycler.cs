using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Plasmid.Graphics
{
    public class ColorCycler
    {
        private static List<Color> defColors = new List<Color>() {new Color(255,0,0), new Color(255,255,0), new Color(0,255,0),
                new Color(0,255,255), new Color(0,0,255), new Color(255,0,255) };
    
        private Queue<Color> colors;
        private Color toColor;
        private Color fromColor;
        private int fadesteps;
        private int stepcount;

        public ColorCycler() : this(defColors, 50) { }
        public ColorCycler(IEnumerable<Color> colors, int fadesteps)
        {
            this.colors = new Queue<Color>(colors);

            if (this.colors.Count < 1)
                throw new ArgumentException("ColorCycler must have at least 2 colors.");

            this.fromColor = this.colors.Dequeue();
            this.toColor = this.colors.Dequeue();

            this.fadesteps = fadesteps;
            this.stepcount = 0;
        }

        public Color Next()
        {
            Color color = Color.Lerp(fromColor, toColor, stepcount * (1.0f / fadesteps));
            if (stepcount++ == fadesteps)
            {
                colors.Enqueue(fromColor);
                fromColor = toColor;
                toColor = colors.Dequeue();
                stepcount = 0;
            }

            return color;
        }

    }
}
