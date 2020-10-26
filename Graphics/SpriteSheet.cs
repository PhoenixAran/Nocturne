using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Nocturne
{
    /// <summary>
    /// Contains textures split up given a sprite sheet
    /// Assumes spritesheet has the standard 1px padding to prevent bleeding
    /// </summary>
    public class SpriteSheet : IEnumerable, IEnumerable<Subtexture>
    {
        private int _rowCount, _colCount;
        private List<Subtexture> _textures;

        public int RowCount => _rowCount;
        public int ColCount => _colCount;

        public SpriteSheet( Texture2D texture, int width, int height, int padding = 1, int margin = 0 )
        {
            _textures = new List<Subtexture>();
            bool countedCols = false;
            _rowCount = 0;
            _colCount = 0;

            for ( int y = margin; y < texture.Height - margin; y += height + padding )
            {
                for ( int x = margin; x < texture.Width - margin; x += height + padding )
                {
                    _textures.Add( new Subtexture( texture, new Rectangle(x, y, width, height ) ) );

                    if ( !countedCols )
                    {
                        _colCount += 1;
                    }
                }
                countedCols = true;
                _rowCount++;
            }

        }

        public SpriteSheet( List<Subtexture> textures, int rowCount, int colCount )
        {
            _textures = textures;
            _rowCount = rowCount;
            _colCount = colCount;
        }

        /// <summary>
        /// Gets the texture at the given coordinate
        /// </summary>
        /// <param name="x">X Cell Coordinate</param>
        /// <param name="y">Y Cell Coordinate</param>
        public Subtexture GetTexture( int x, int y )
        {
            return _textures[x * _colCount + y];
        }
            
        /// <summary>
        /// Gets the texture by accessing the list given the index
        /// </summary>
        public Subtexture GetTexture( int index )
        {
            return _textures[index];
        }

        public int Count => _textures.Count;

        public IEnumerator<Subtexture> GetEnumerator()
        {
            return _textures.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Subtexture this[int index] => GetTexture(index);

        public Subtexture this[int x, int y] => GetTexture(x, y);
    }
}
