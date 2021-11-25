using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace Plasmid.Graphics
{
    public struct ColorHSL
    {
        public float H, SL, L;
        public ColorHSL(float h, float sl, float l)
        {
            this.H = h;
            this.SL = sl;
            this.L = l;
        }
        public Color ToRGB()
        {
            return ColorExtensions.HSLtoRGB(this.H, this.SL, this.L);
        }
    }

    public static class ColorExtensions
    {
        public static Color ModifyL(this Color color, float factor, bool debug=false)
        {
            if (factor < 0f)
                throw new ArgumentException("Factor must be non-negative.");

            ColorHSL hsl = color.ToHSL();
            hsl.L *= factor;
            hsl.L = GraphUtils.Clamp(hsl.L, 0.0f, 1.0f);

            if (debug)
            {
                Color newcolor = hsl.ToRGB();
                Debug.WriteLine("R: " + newcolor.R + "  G: " + newcolor.G + "  B: " + newcolor.B);
                return newcolor;
            }
            return hsl.ToRGB();


        }
        public static ColorHSL ToHSL(this Color color)
        {
            float r = color.R / 255.0f;
            float g = color.G / 255.0f;
            float b = color.B / 255.0f;

            float v, m, vm, r2, g2, b2;

            // default black
            float h = 0;
            float s = 0;
            float l = 0;
            v = (float)Math.Max(r, g);
            v = (float)Math.Max(v, b);
            m = (float)Math.Min(r, g);
            m = (float)Math.Min(m, b);
            l = (m + v) / 2.0f;

            if (l <= 0.0)
                return new ColorHSL(h, s, l);

            vm = v - m;
            s = vm;

            if (s > 0.0)
                s /= (l <= 0.5f) ? (v + m) : (2.0f - v - m);
            else
                return new ColorHSL(h, s, l);

            r2 = (v - r) / vm;
            g2 = (v - g) / vm;
            b2 = (v - b) / vm;

            if (r == v)
                h = (g == m ? 5.0f + b2 : 1.0f - g2);
            else if (g == v)
                h = (b == m ? 1.0f + r2 : 3.0f - b2);
            else
                h = (r == m ? 3.0f + g2 : 5.0f - r2);

            h /= 6.0f;

            return new ColorHSL(h, s, l);
        }

        public static Color HSLtoRGB(float h, float sl, float l)
        {
            double v;
            double r, g, b;

            // default gray
            r = l;
            g = l;
            b = l;
            v = (l <= 0.5) ? (l * (1.0 + sl)) : (l + sl - l * sl);

            if (v > 0)
            {
                double m;
                double sv;
                int sextant;
                double fract, vsf, mid1, mid2;

                m = l + l - v;
                sv = (v - m) / v;
                h *= 6.0f;
                sextant = (int)h;
                fract = h - sextant;
                vsf = v * sv * fract;
                mid1 = m + vsf;
                mid2 = v - vsf;

                switch (sextant)
                {
                    case 0:
                        r = v;
                        g = mid1;
                        b = m;
                        break;
                    case 1:
                        r = mid2;
                        g = v;
                        b = m;
                        break;
                    case 2:
                        r = m;
                        g = v;
                        b = mid1;
                        break;
                    case 3:
                        r = m;
                        g = mid2;
                        b = v;
                        break;
                    case 4:
                        r = mid1;
                        g = m;
                        b = v;
                        break;
                    case 5:
                        r = v;
                        g = m;
                        b = mid2;
                        break;
                }
            }

            return new Color((float)r, (float)g, (float)b);
        }
    }
}
