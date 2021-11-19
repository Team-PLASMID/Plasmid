using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Plasmid.Input
{
    public interface ITouchable
    {
        public Vector2 Position { get; }
        public void TouchPress(Vector2 position);
        public void TouchMove(Vector2 position);
        public void TouchRelease(Vector2 position);
    }
}
