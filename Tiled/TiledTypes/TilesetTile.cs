using System.Collections.Generic;

namespace Nocturne.Tiled
{
    //you can use TilesetTile directly or make your own tile entity from this class

    /// <summary>
    /// Tile element in Tilesets. Requires wrapper class TileLayerTile for correct Tile mapping ids. 
    /// Can be reused since it does not contain any MapData specific data.
    /// </summary>
    public class TilesetTile
    {
        public int Id;

        public string Type;
        public int Width;
        public int Height;

        public Subtexture Texture;
        public bool IsAnimated
        {
            get
            {
                if ( AnimatedTextures == null )
                    return false;
                return AnimatedTextures.Count > 0;
            }
        }
        public List<Subtexture> AnimatedTextures;

        /// <summary>
        /// how long each animated texture lasts. Indices will be mapped 1 to 1 with 
        /// AnimatedTextures
        /// </summary>
        public List<float> Durations;
        public Dictionary<string, string> Properties;   


        public bool TryGetProperty( string propertyKey )
        {
            return Properties.TryGetValue( propertyKey, out string value );
        }
    }
}
