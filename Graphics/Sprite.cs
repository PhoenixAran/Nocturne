using Microsoft.Xna.Framework;

namespace Nocturne
{
    /// <summary>
    /// Normal sprite
    /// </summary>
    public class Sprite : ISprite
    {
        private RectangleF _bounds;
        private Vector2 _offset;
        public readonly Subtexture Texture; 

        public RectangleF Bounds
        {
            get => _bounds;
            set => _bounds = value;
        }

        public Vector2 Offset
        {
            get => _offset;
            set => _offset = value;
        }

        public Sprite( Subtexture texture, int spriteWidth, int spriteHeight, Vector2 offset )
        {
            Texture = texture;
           
            Bounds = new RectangleF( 0, 0, spriteWidth, spriteHeight );
            Offset = offset;
        }

        public Sprite( Subtexture texture, Vector2 offset ) : this( texture, texture.SourceRect.Width, texture.SourceRect.Height, offset ) { }

        public Sprite( Subtexture texture ) : this( texture, Vector2.Zero ) { }

        public Sprite( Subtexture texture, int spriteWidth, int spriteHeight ) : this(texture, spriteWidth, spriteHeight, new Vector2(0, 0)) { }

        void ISprite.DrawSprite( Vector2 originPosition, Color color, float layerDepth )
        {
            Vector2 position = originPosition += _offset;
            Draw.SpriteBatch.Draw(
                texture: Texture.Texture2D,
                position: position,
                sourceRectangle: Texture.SourceRect,
                color: color,
                rotation: 0f,
                origin: new Vector2( _bounds.Width / 2, _bounds.Height / 2),
                scale: 1f,
                effects: Microsoft.Xna.Framework.Graphics.SpriteEffects.None,
                layerDepth: layerDepth
            );
        }
    }
}
