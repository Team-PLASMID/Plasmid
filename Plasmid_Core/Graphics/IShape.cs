using Microsoft.Xna.Framework;

namespace Plasmid.Graphics
{
    public interface IShape
    {
        //public void Draw(Game1 game, Transform transform, Color color, bool fill, int thickness);
        public void Draw(Game1 game);
        public void Draw(Game1 game, Vector2 position);
        public void ApplyTransform(Transform transform);
    }
}
