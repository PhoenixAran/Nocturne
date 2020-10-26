using Microsoft.Xna.Framework;

namespace Nocturne
{
    public class PrototypeSprite : ISprite
    {
        private RectangleF _bounds;
        private Vector2 _offset;

        public readonly Subtexture Texture;
        public RectangleF Bounds => _bounds;
        public Vector2 Offset => _offset;

        public PrototypeSprite(int width, int height, Color color, Vector2 offset )
        {
            _bounds = new RectangleF( 0, 0, width, height );
            _offset = offset;
            Texture = new Subtexture( width, height, color );
        }

        public void DrawSprite( Vector2 originPosition, Color color, float layerDepth = 1 )
        {
            Vector2 position =  originPosition -= _offset;
            Draw.SpriteBatch.Draw(
                texture: Texture,
                position: position,
                sourceRectangle: Texture.SourceRect,
                color: color,
                rotation: 0f,
                origin: new Vector2(Bounds.Width / 2, Bounds.Height / 2),
                scale: 1f,
                effects: Microsoft.Xna.Framework.Graphics.SpriteEffects.None,
                layerDepth: layerDepth
            );
        }
    }
}
