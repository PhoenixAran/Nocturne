
using Microsoft.Xna.Framework;

namespace Nocturne
{
    public abstract class RenderableComponent : Component
    {

        public bool Visible = true;
        public Color Color = Color.White;
        /// <summary>
        /// width of the drawable component. 
        /// </summary>
        public float Width => Bounds.Width;
        /// <summary>
        /// height of the drawable component
        /// </summary>
        public float Height => Bounds.Height;

        /// <summary>
        /// the AABB that wraps this object. Used for camera culling.
        /// </summary>
        public virtual RectangleF Bounds
        {
            get
            {
                if ( _areBoundsDirty )
                {
                    _bounds.CalculateBounds( Entity.Transform.Position, _localOffset, Vector2.Zero,
                        Entity.Transform.Scale, Entity.Transform.Rotation, Width, Height );
                    _areBoundsDirty = false;
                }
                return _bounds;
            }
        }

        /// <summary>
        /// offset from the parent entity. Useful for adding multiple drawables to an Entity that need 
        /// specific positioning
        /// </summary>
        public Vector2 LocalOffset
        {
            get => _localOffset;
            set => SetLocalOffset( value );
        }

        protected Vector2 _localOffset;
        protected RectangleF _bounds;
        protected bool _areBoundsDirty = true;

        public RenderableComponent( bool enabled = true, bool visible = true ) : base( enabled )
        {
            Visible = visible;
        }

        public override void OnEntityTransformChanged( Transform.Component comp )
        {
            _areBoundsDirty = true;
        }

        public RenderableComponent SetLocalOffset( Vector2 offset )
        {
            if ( _localOffset != offset )
            {
                _localOffset = offset;
                _areBoundsDirty = true;
            }
            return this;
        }

        public abstract void Render();

    }
}
