using Microsoft.Xna.Framework;

namespace Nocturne
{
    /// <summary>
    /// Interface to represents sprites that get rendered
    /// </summary>
    public interface ISprite
    {
        /// <summary>
        /// The rectangle space this sprite requires to fully render. This rectangle will treat (0, 0) as the entity's position
        /// so translate yourself to get a cull rectangle
        /// </summary>
        RectangleF Bounds { get; }

        /// <summary>
        /// The offset value this sprite will use when it draws
        /// </summary>
        Vector2 Offset { get; }

        /// <summary>
        /// Render the sprite at the given origin
        /// </summary>
        /// <param name="originPosition">Origin Position</param>
        /// <param name="color">Color</param>
        void DrawSprite(Vector2 originPosition, Color color, float layerDepth = 0f);
    }
}
