using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Nocturne.Tiled
{
    /// <summary>
    /// Contains data for a Tmx Map
    /// </summary>
    public class MapData : TmxDocument
    {
        public string Version;
        public string TiledVersion;
        public int Width;
        public int Height;
        public int TileWidth;
        public int TileHeight;
        public int WorldWidth => Width * TileWidth;
        public int WorldHeight => Height * TileHeight;
        public Color BackgroundColor;
        public int? NextObjectId;

        /// <summary>
        /// when we have an image tileset, tiles can be any size so we record the max size for culling
        /// </summary>
        public int MaxTileWidth;

        /// <summary>
        /// when we have an image tileset, tiles can be any size so we record the max size for culling
        /// </summary>
        public int MaxTileHeight;

        /// <summary>
        /// does this map have non-default tile sizes that would require special culling?
        /// </summary>
        public bool RequiresLargeTileCulling => MaxTileWidth > TileWidth || MaxTileHeight > TileHeight;


        /// <summary>
        /// the included tilesets
        /// </summary>
        public List<TileLayerTileset> Tilesets;

        /// <summary>
        /// contains all of the ITmxLayers, regardless of their specific type. 
        /// Note that layers in a TmxGroup will not be in the list. TmxGroup manages
        /// it's own layers list
        /// </summary>
        public TmxList<ITmxLayer> Layers;

        public TmxList<TileLayer> TileLayers;
        public TmxList<ObjectLayer> ObjectLayers;
        public TmxList<ImageLayer> ImageLayers;

        public Dictionary<string, string> Properties;

        #region Utils
        /// <summary>
        /// gets the Tileset for the given TileLayerTile gid
        /// </summary>
        public TileLayerTileset GetTileSetForTileLayerGid( int gid )
        {
            Insist.IsTrue( Tilesets.Count > 0 );
            if ( Tilesets.Count == 1 )
            {
                return Tilesets[0];
            }

            for ( int i = 1; i < Tilesets.Count; ++i )
            {
                TileLayerTileset setA = Tilesets[i - 1];
                TileLayerTileset setB = Tilesets[i];

                if ( setA.FirstGid <= gid && gid < setB.FirstGid )
                {
                    return setA;
                }
            }

            return Tilesets[Tilesets.Count - 1];
        }
        #endregion
    }
}
