using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Plasmid.UI
{
    internal class Util
    {
        public enum Direction { Left, Top, Right, Bottom, None}
        public static string WrapString(SpriteFont font, string text, float maxLineWidth)
        {
            string[] words = text.Split(' ');
            StringBuilder sb = new StringBuilder();
            float lineWidth = 0f;
            float spaceWidth = font.MeasureString(" ").X;

            foreach (string word in words)
            {
                Vector2 size = font.MeasureString(word);

                if (lineWidth + size.X < maxLineWidth)
                {
                    sb.Append(word + " ");
                    lineWidth += size.X + spaceWidth;
                }
                else
                {
                    if (size.X > maxLineWidth)
                    {
                        if (sb.ToString() == "")
                        {
                            sb.Append(WrapString(font, word.Insert(word.Length / 2, " ") + " ", maxLineWidth));
                        }
                        else
                        {
                            sb.Append("\n" + WrapString(font, word.Insert(word.Length / 2, " ") + " ", maxLineWidth));
                        }
                    }
                    else
                    {
                        sb.Append("\n" + word + " ");
                        lineWidth = size.X + spaceWidth;
                    }
                }
            }

            return sb.ToString();
        }

        public static bool Align(Rectangle subject, Rectangle canvas, Alignment alignment, out Vector2 position)
        {
            return Align(new Vector2(subject.Width, subject.Height), canvas, alignment, out position);
        }
        public static bool Align(Vector2 subjectDimensions, Rectangle canvas, Alignment alignment, out Vector2 subjectAlignedPosition)
        {
            subjectAlignedPosition = Vector2.Zero;

            if (subjectDimensions.X > canvas.Width || subjectDimensions.Y > canvas.Height)
                return false;

            //float leftX = canvas.X;
            float rightX = canvas.X + canvas.Width - subjectDimensions.X;
            float centerX = (rightX-canvas.X) / 2 + canvas.X;
            //float bottomY = canvas.Y;
            float topY = canvas.Y + canvas.Height - subjectDimensions.Y;
            float centerY = (topY-canvas.Y) / 2 + canvas.Y;

            switch (alignment)
            {
                //case Alignment.None:
                    //break;
                case Alignment.BottomLeft:
                    subjectAlignedPosition.X = canvas.X;
                    subjectAlignedPosition.Y = canvas.Y;
                    break;
                case Alignment.BottomCenter:
                    subjectAlignedPosition.X = centerX;
                    subjectAlignedPosition.Y = canvas.Y;
                    break;
                case Alignment.BottomRight:
                    subjectAlignedPosition.X = rightX;
                    subjectAlignedPosition.Y = canvas.Y;
                    break;
                case Alignment.Left:
                    subjectAlignedPosition.X = canvas.X;
                    subjectAlignedPosition.Y = centerY;
                    break;
                case Alignment.Center:
                    subjectAlignedPosition.X = centerX;
                    subjectAlignedPosition.Y = centerY;
                    break;
                case Alignment.Right:
                    subjectAlignedPosition.X = rightX;
                    subjectAlignedPosition.Y = centerY;
                    break;
                case Alignment.TopLeft:
                    subjectAlignedPosition.X = canvas.X;
                    subjectAlignedPosition.Y = topY;
                    break;
                case Alignment.TopCenter:
                    subjectAlignedPosition.X = centerX;
                    subjectAlignedPosition.Y = topY;
                    break;
                case Alignment.TopRight:
                    subjectAlignedPosition.X = rightX;
                    subjectAlignedPosition.Y = topY;
                    break;
                default:
                    throw new Exception("Failed to align.");
            }

            return true;
        }

        public static Rectangle RemoveIntersection(Rectangle rect, Rectangle intersect, Alignment alignment)
        {
            Direction direction;
            if (alignment == Alignment.TopCenter)
                direction = Direction.Top;
            else if (alignment == Alignment.BottomCenter)
                direction = Direction.Bottom;
            else if (alignment == Alignment.Left)
                direction = Direction.Left;
            else if (alignment == Alignment.Right)
                direction = Direction.Right;
            else
                return RemoveIntersection(rect, intersect, null);

            return RemoveIntersection(rect, intersect, direction);
        }
        public static Rectangle RemoveIntersection(Rectangle rect, Rectangle intersect, Direction? approach=null)
        {
            if (intersect == null || intersect.Width == 0 || intersect.Height == 0)
                return rect;

            Direction direction = approach ?? Direction.None;

            // Auto-select approach direction
            if (direction == Direction.None)
            {
                // Left
                int maxOverlap = intersect.X + intersect.Width - rect.X;
                direction = Direction.Left;
                // Top
                int overlap = rect.Y + rect.Height - intersect.Y;
                if (overlap > maxOverlap)
                {
                    maxOverlap = overlap;
                    direction = Direction.Top;
                }
                // Right
                overlap = rect.X + rect.Width - intersect.X;
                if (overlap > maxOverlap)
                {
                    maxOverlap = overlap;
                    direction = Direction.Right;
                }
                // Bottom
                overlap = intersect.Y + intersect.Height - rect.Y;
                if (overlap > maxOverlap)
                {
                    maxOverlap = overlap;
                    direction = Direction.Bottom;
                }
            }

            switch (direction)
            {
                case Direction.Left:
                    if (intersect.X + intersect.Width > rect.X)
                    {
                        rect.Width -= intersect.X + intersect.Width - rect.X;
                        rect.X = intersect.X + intersect.Width - rect.X;
                    }
                    break;
                case Direction.Top:
                    if (intersect.Y < rect.Y + rect.Height)
                    {
                        rect.Height -= rect.Y +  rect.Height - intersect.Y;
                    }
                    break;
                case Direction.Right:
                    if (intersect.X < rect.X + rect.Width)
                    {
                        rect.Width -= rect.X + rect.Width - intersect.X;
                    }
                    break;
                case Direction.Bottom:
                    if (intersect.Y + intersect.Height > rect.Y)
                    {
                        rect.Height -= intersect.Y + intersect.Height - rect.Y;
                        rect.Y = intersect.Y + intersect.Height - rect.Y;
                    }
                    break;
                //default:
                    //break;
            }

            return rect;
        }
    }
}
