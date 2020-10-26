using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Nocturne
{

    /// <summary>
    /// Can render multiple sprite instances
    /// </summary>
    public class CompositeSprite : ISprite
    {

        RectangleF _bounds;
        Vector2 _offset;
        public List<ISprite> Sprites;

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


        public CompositeSprite( List<ISprite> sprites, Vector2 offset )
        {
            Sprites = sprites;
            _offset = offset;
            CalculateBounds();
        }

        public CompositeSprite( List<ISprite> sprites ) : this( sprites, Vector2.Zero ) { }

        public CompositeSprite( IEnumerable<ISprite> sprites, Vector2 offset )
        {
            Sprites = new List<ISprite>();
            Sprites.AddRange( sprites );
            _offset = offset;

            CalculateBounds();
        }

        public CompositeSprite( IEnumerable<ISprite> sprites ) : this (sprites, Vector2.Zero ) { }

        /// <summary>
        /// Sets the bounds based off the space needed to render it's composite sprites
        /// </summary>
        void CalculateBounds()
        {
            if ( Sprites.Count == 0 )
                return;

            if ( Sprites.Count == 1 )
            {
                _bounds = Sprites[0].Bounds;
                return;
            }

            float top = GetMostTopBoundary( Sprites );
            float bottom = GetMostBottomBoundary( Sprites );
            float left = GetMostLeftBoundary( Sprites );
            float right = GetMostRightBoundary( Sprites );

            _bounds = new RectangleF( 0, 0, right - left, bottom - top );
        }

        float GetMostLeftBoundary( List<ISprite> sprites )
        {
            float returnVal = float.MaxValue;
            for ( int i = 0; i < sprites.Count; ++i )
            {
                ISprite sprite = sprites[i];
                RectangleF translatedRectangle = new RectangleF( sprite.Offset.X, sprite.Offset.Y, sprite.Bounds.Width, sprite.Bounds.Height );
                //get x coordinate of the left side of the sprite's rectangle
                float xValue = translatedRectangle.Left;
                if ( xValue < returnVal )
                {
                    returnVal = xValue;
                }
            }
            return returnVal;
        }

        float GetMostRightBoundary( List<ISprite> sprites )
        {
            float returnVal = float.MinValue;
            for ( int i = 0; i < sprites.Count; ++i )
            {
                ISprite sprite = sprites[i];
                RectangleF translatedRectangle = new RectangleF( sprite.Offset.X, sprite.Offset.Y, sprite.Bounds.Width, sprite.Bounds.Height );
                //get x coordinate of the right side of the sprite's rectangle
                float xValue = translatedRectangle.Right;
                if ( xValue > returnVal )
                {
                    returnVal = xValue;
                }
            }
            return returnVal;
        }

        float GetMostTopBoundary( List<ISprite> sprites )
        {
            float returnVal = float.MaxValue;
            for ( int i = 0; i < sprites.Count; ++i )
            {
                ISprite sprite = sprites[i];
                RectangleF translatedRectangle = new RectangleF( sprite.Offset.X, sprite.Offset.Y, sprite.Bounds.Width, sprite.Bounds.Height );
                //get y coordinate of the top side of the sprite's rectangle
                float yValue = translatedRectangle.Top;
                if ( yValue < returnVal )
                {
                    returnVal = yValue;
                }
            }
            return returnVal;
        }

        float GetMostBottomBoundary( List<ISprite> sprites )
        {
            float returnVal = float.MinValue;
            for ( int i = 0; i < sprites.Count; ++i )
            {
                ISprite sprite = sprites[i];
                RectangleF translatedRectangle = new RectangleF( sprite.Offset.X, sprite.Offset.Y, sprite.Bounds.Width, sprite.Bounds.Height );
                //get y coordinate of the left bottom of the sprite's rectangle
                float yValue = translatedRectangle.Bottom;
                if ( yValue > returnVal )
                {
                    returnVal = yValue;
                }
            }
            return returnVal;
        }

        public void DrawSprite( Vector2 originPosition, Color color, float layerDepth )
        {
            for( int i = 0; i < Sprites.Count; ++i )
            {
                ISprite sprite = Sprites[i];
                sprite.DrawSprite( originPosition + Offset, color );
            }
        }
    }
}
