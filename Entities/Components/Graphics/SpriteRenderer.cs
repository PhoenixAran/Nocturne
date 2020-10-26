
using Microsoft.Xna.Framework;

namespace Nocturne
{
    /// <summary>
    /// basic sprite component. Just draws a given sprite instance
    /// </summary>
    public class SpriteRenderer : RenderableComponent
    {
        protected Vector2 _origin;
        protected ISprite _sprite;

        public Vector2 Origin
        {
            get => _origin;
            set => SetOrigin( value );
        }

        public ISprite Sprite
        {
            get => _sprite;
            set => SetSprite( value );
        }

        public override RectangleF Bounds
        {
            get
            {
                if ( _areBoundsDirty )
                {
                    if ( _sprite != null )
                    {
                        _bounds.CalculateBounds( Entity.Transform.Position, _localOffset + _sprite.Offset, _origin,
                            Entity.Transform.Scale, Entity.Transform.Rotation, _sprite.Bounds.Width, _sprite.Bounds.Height );
                    }
                    _areBoundsDirty = false;
                }
                return _bounds;
            }
        }

        public SpriteRenderer( bool enabled = true, bool visible = true ) : base(enabled, visible )
        {

        }

        public SpriteRenderer( ISprite sprite, bool enabled = true, bool visible = true ) : base(enabled, visible)
        {
            _sprite = sprite;
            _origin = new Vector2(_sprite.Bounds.Width / 2, _sprite.Bounds.Height / 2);
        }

        public SpriteRenderer SetOrigin( Vector2 origin )
        {
            if ( _origin != origin )
            {
                _origin = origin;
                _areBoundsDirty = true;
            }
            return this;
        }

        public SpriteRenderer SetSprite( ISprite sprite )
        {
            _sprite = sprite;
            SetOrigin( new Vector2( _sprite.Bounds.Width / 2, _sprite.Bounds.Height / 2 ) );
            _areBoundsDirty = true;
            return this;
        }


        public override void Render()
        {
            _sprite?.DrawSprite( Entity.Transform.Position + LocalOffset, Color );
        }
        
        public override void DebugRender()
        {
            Draw.HollowRect( Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height, Color.Green);
        }
        
        
    }
}
