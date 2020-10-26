using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Nocturne
{
    /// <summary>
    /// Wrapper around Texture2D 
    /// </summary>
    public class Subtexture : IDisposable
    {
        public readonly Texture2D Texture2D;
        public readonly Rectangle SourceRect;

        public Subtexture(Texture2D texture, Rectangle rect)
        {
            Texture2D = texture;
            SourceRect = rect;
        }

        public Subtexture( int width, int height, Color color )
        {
            Texture2D = new Texture2D( Engine.Instance.GraphicsDevice, width, height );
            Color[] colors = new Color[width * height];
            for ( int i = 0; i < width * height; ++i )
            {
                colors[i] = color;
            }

            Texture2D.SetData<Color>( colors );

            SourceRect = new Rectangle( 0, 0, width, height );
        }

        public void Dispose()
        {
            Texture2D.Dispose();
        }

        public static implicit operator Texture2D ( Subtexture texture )
        {
            return texture.Texture2D;
        }
    }
}
