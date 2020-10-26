using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//modified from https://bitbucket.org/MattThorson/monocle-engine

namespace Nocturne
{
    /// <summary>
    /// note that the built in shape primatives can only be drawn at integer positions,
    /// they're mainly used for debugging so it shouldn't be a huge issue
    /// </summary>
    public static class Draw
    {
        public static SpriteBatch SpriteBatch { get; private set; }
        public static SpriteFont DefaultFont { get; private set; }
        public static Subtexture Pixel;
        private static Rectangle rect;

        internal static void Initialize( GraphicsDevice graphicsDevice )
        {
            SpriteBatch = new SpriteBatch( graphicsDevice );
            DefaultFont = Engine.ContentManager.Load<SpriteFont>( @"Nocturne\NocturneDefault" );
            UseDebugPixelTexture();
        }

        public static void UseDebugPixelTexture()
        {
            Subtexture texture = new Subtexture( 2, 2, Color.White );
            Pixel = new Subtexture( texture.Texture2D, new Rectangle( 0, 0, 1, 1 ) );
        }

        public static void Point(Vector2 at, Color color)
        {
            SpriteBatch.Draw( Pixel.Texture2D, at, Pixel.SourceRect, color, 0, Vector2.Zero, 1f, SpriteEffects.None, 0 );
        }

        #region Line Angle
        public static void LineAngle( Vector2 start, float angle, float length, Color color )
        {
            SpriteBatch.Draw( Pixel.Texture2D, start, Pixel.SourceRect, color, angle, Vector2.Zero, new Vector2( length, 1 ), SpriteEffects.None, 0 );
        }

        public static void LineAngle( Vector2 start, float angle, float length, Color color, float thickness )
        {
            SpriteBatch.Draw( Pixel.Texture2D, start, Pixel.SourceRect, color, angle, new Vector2( 0, .5f ), new Vector2( length, thickness ), SpriteEffects.None, 0 );
        }

        public static void LineAngle( float startX, float startY, float angle, float length, Color color )
        {
            LineAngle( new Vector2( startX, startY ), angle, length, color );
        }
        #endregion

        #region Line
        public static void Line( Vector2 start, Vector2 end, Color color )
        {
            LineAngle( start, Mathf.AngleBetweenVectors( start, end ), Vector2.Distance( start, end ), color );
        }

        public static void Line( Vector2 start, Vector2 end, Color color, float thickness )
        {
            LineAngle( start, Mathf.AngleBetweenVectors( start, end ), Vector2.Distance( start, end ), color, thickness );
        }

        public static void Line(float x1, float y1, float x2, float y2, Color color )
        {
            Line( new Vector2( x1, y1 ), new Vector2( x2, y2 ), color );
        }
        #endregion

        #region Circle
        public static void Circle( Vector2 position, float radius, Color color, int resolution )
        {
            Vector2 last = Vector2.UnitX * radius;
            Vector2 lastP = last.Perpendicular();
            for ( int i = 1; i <= resolution; i++ )
            {
                Vector2 at = Mathf.AngleToVector( i * MathHelper.PiOver2 / resolution, radius );
                Vector2 atP = at.Perpendicular();

                Draw.Line( position + last, position + at, color );
                Draw.Line( position - last, position - at, color );
                Draw.Line( position + lastP, position + atP, color );
                Draw.Line( position - lastP, position - atP, color );

                last = at;
                lastP = atP;
            }
        }

        public static void Circle( float x, float y, float radius, Color color, int resolution )
        {
            Circle( new Vector2( x, y ), radius, color, resolution );
        }

        public static void Circle( Vector2 position, float radius, Color color, float thickness, int resolution )
        {
            Vector2 last = Vector2.UnitX * radius;
            Vector2 lastP = last.Perpendicular();
            for ( int i = 1; i <= resolution; i++ )
            {
                Vector2 at = Mathf.AngleToVector( i * MathHelper.PiOver2 / resolution, radius );
                Vector2 atP = at.Perpendicular();

                Draw.Line( position + last, position + at, color, thickness );
                Draw.Line( position - last, position - at, color, thickness );
                Draw.Line( position + lastP, position + atP, color, thickness );
                Draw.Line( position - lastP, position - atP, color, thickness );

                last = at;
                lastP = atP;
            }
        }

        public static void Circle( float x, float y, float radius, Color color, float thickness, int resolution )
        {
            Circle( new Vector2( x, y ), radius, color, thickness, resolution );
        }
        #endregion

        #region Rect
        public static void Rect(float x, float y, float width, float height, Color color )
        {
            rect.X = (int)x;
            rect.Y = (int)y;
            rect.Width = (int)width;
            rect.Height = (int)height;

            SpriteBatch.Draw( Pixel.Texture2D, rect, Pixel.SourceRect, color );
        }

        public static void Rect(Vector2 position, float width, float height, Color color )
        {
            Rect( position.X, position.Y, width, height, color );
        }

        public static void Rect( Rectangle rect, Color color )
        {
            Draw.rect = rect;
            SpriteBatch.Draw( Pixel.Texture2D, rect, Pixel.SourceRect, color );
        }
        #endregion

        #region Hollow Rect
        public static void HollowRect( float x, float y, float width, float height, Color color )
        {
            rect.X = (int)x;
            rect.Y = (int)y;
            rect.Width = (int)width;
            rect.Height = 1;

            SpriteBatch.Draw( Pixel.Texture2D, rect, Pixel.SourceRect, color );

            rect.Y += (int)height - 1;
            SpriteBatch.Draw( Pixel.Texture2D, rect, Pixel.SourceRect, color );

            rect.Y -= (int)height - 1;
            rect.Width = 1;
            rect.Height = (int)height;

            SpriteBatch.Draw( Pixel.Texture2D, rect, Pixel.SourceRect, color );
            rect.X += (int)width - 1;

            SpriteBatch.Draw( Pixel.Texture2D, rect, Pixel.SourceRect, color );
        }

        public static void HollowRect(Vector2 position, float width, float height, Color color )
        {
            HollowRect( position.X, position.Y, width, height, color );
        }
        #endregion

        #region Polygon
        public static void Polygon(Vector2 position, Vector2[] points, Color color, bool closePoly = true )
        {
            if ( points.Length < 2 )
            {
                return;
            }

            for ( int i = 1; i < points.Length; ++i )
            {
                Line( position + points[i - 1], position + points[i], color );
            }

            if ( closePoly )
            {
                Line( position + points[points.Length - 1], position + points[0], color );
            }
        }

        #endregion

        #region Text

        public static void Text( SpriteFont font, string text, Vector2 position, Color color )
        {
            Draw.SpriteBatch.DrawString( font, text, Mathf.Floor( position ), color );
        }

        public static void Text( SpriteFont font, string text, Vector2 position, Color color, Vector2 origin, Vector2 scale, float rotation )
        {
            Draw.SpriteBatch.DrawString( font, text, Mathf.Floor( position ), color, rotation, origin, scale, SpriteEffects.None, 0 );
        }

        public static void TextJustified( SpriteFont font, string text, Vector2 position, Color color, Vector2 justify )
        {
            Vector2 origin = font.MeasureString( text );
            origin.X *= justify.X;
            origin.Y *= justify.Y;

            Draw.SpriteBatch.DrawString( font, text, Mathf.Floor( position ), color, 0, origin, 1, SpriteEffects.None, 0 );
        }

        public static void TextJustified( SpriteFont font, string text, Vector2 position, Color color, float scale, Vector2 justify )
        {
            Vector2 origin = font.MeasureString( text );
            origin.X *= justify.X;
            origin.Y *= justify.Y;
            Draw.SpriteBatch.DrawString( font, text, Mathf.Floor( position ), color, 0, origin, scale, SpriteEffects.None, 0 );
        }

        public static void TextCentered( SpriteFont font, string text, Vector2 position )
        {
            Text( font, text, position - font.MeasureString( text ) * .5f, Color.White );
        }

        public static void TextCentered( SpriteFont font, string text, Vector2 position, Color color )
        {
            Text( font, text, position - font.MeasureString( text ) * .5f, color );
        }

        public static void TextCentered( SpriteFont font, string text, Vector2 position, Color color, float scale )
        {
            Text( font, text, position, color, font.MeasureString( text ) * .5f, Vector2.One * scale, 0 );
        }

        public static void TextCentered( SpriteFont font, string text, Vector2 position, Color color, float scale, float rotation )
        {
            Text( font, text, position, color, font.MeasureString( text ) * .5f, Vector2.One * scale, rotation );
        }

        public static void OutlineTextCentered( SpriteFont font, string text, Vector2 position, Color color, float scale )
        {
            Vector2 origin = font.MeasureString( text ) / 2;

            for ( int i = -1; i < 2; i++ )
                for ( int j = -1; j < 2; j++ )
                    if ( i != 0 || j != 0 )
                        Draw.SpriteBatch.DrawString( font, text, Mathf.Floor( position ) + new Vector2( i, j ), Color.Black, 0, origin, scale, SpriteEffects.None, 0 );
            Draw.SpriteBatch.DrawString( font, text, Mathf.Floor( position ), color, 0, origin, scale, SpriteEffects.None, 0 );
        }

        public static void OutlineTextCentered( SpriteFont font, string text, Vector2 position, Color color, Color outlineColor )
        {
            Vector2 origin = font.MeasureString( text ) / 2;

            for ( int i = -1; i < 2; i++ )
                for ( int j = -1; j < 2; j++ )
                    if ( i != 0 || j != 0 )
                        Draw.SpriteBatch.DrawString( font, text, Mathf.Floor( position ) + new Vector2( i, j ), outlineColor, 0, origin, 1, SpriteEffects.None, 0 );
            Draw.SpriteBatch.DrawString( font, text, Mathf.Floor( position ), color, 0, origin, 1, SpriteEffects.None, 0 );
        }

        public static void OutlineTextCentered( SpriteFont font, string text, Vector2 position, Color color, Color outlineColor, float scale )
        {
            Vector2 origin = font.MeasureString( text ) / 2;

            for ( int i = -1; i < 2; i++ )
                for ( int j = -1; j < 2; j++ )
                    if ( i != 0 || j != 0 )
                        Draw.SpriteBatch.DrawString( font, text, Mathf.Floor( position ) + new Vector2( i, j ), outlineColor, 0, origin, scale, SpriteEffects.None, 0 );
            Draw.SpriteBatch.DrawString( font, text, Mathf.Floor( position ), color, 0, origin, scale, SpriteEffects.None, 0 );
        }

        public static void OutlineTextJustify( SpriteFont font, string text, Vector2 position, Color color, Color outlineColor, Vector2 justify )
        {
            Vector2 origin = font.MeasureString( text ) * justify;

            for ( int i = -1; i < 2; i++ )
                for ( int j = -1; j < 2; j++ )
                    if ( i != 0 || j != 0 )
                        Draw.SpriteBatch.DrawString( font, text, Mathf.Floor( position ) + new Vector2( i, j ), outlineColor, 0, origin, 1, SpriteEffects.None, 0 );
            Draw.SpriteBatch.DrawString( font, text, Mathf.Floor( position ), color, 0, origin, 1, SpriteEffects.None, 0 );
        }

        public static void OutlineTextJustify( SpriteFont font, string text, Vector2 position, Color color, Color outlineColor, Vector2 justify, float scale )
        {
            Vector2 origin = font.MeasureString( text ) * justify;

            for ( int i = -1; i < 2; i++ )
                for ( int j = -1; j < 2; j++ )
                    if ( i != 0 || j != 0 )
                        Draw.SpriteBatch.DrawString( font, text, Mathf.Floor( position ) + new Vector2( i, j ), outlineColor, 0, origin, scale, SpriteEffects.None, 0 );
            Draw.SpriteBatch.DrawString( font, text, Mathf.Floor( position ), color, 0, origin, scale, SpriteEffects.None, 0 );
        }

        #endregion

    }
}
