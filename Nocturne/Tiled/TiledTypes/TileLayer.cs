using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nocturne.Tiled
{
    public class TileLayer : ITmxLayer
    {
        public string Name { get; set; }
        public Dictionary<string, string> Properties { get; set; }

        /// <summary>
        /// Width in tiles for this layer.
        /// </summary>
        public int Width;

        /// <summary>
        /// Height in tiles for this layer
        /// </summary>
        public int Height;
        public TileLayerTile[] Tiles;

        public TileLayerTile GetTileWithGid( int gid )
        {
            for ( int i = 0; i < Tiles.Length; ++i )
            {
                if ( Tiles[i] != null && Tiles[i].Gid == gid )
                {
                    return Tiles[i];
                }
            }
            return null;
        }
    }
}
